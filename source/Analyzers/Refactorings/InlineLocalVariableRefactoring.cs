﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class InlineLocalVariableRefactoring
    {
        public static DiagnosticDescriptor FadeOutDescriptor
        {
            get { return DiagnosticDescriptors.InlineLocalVariableFadeOut; }
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, LocalDeclarationStatementSyntax localDeclaration)
        {
            VariableDeclarationSyntax declaration = localDeclaration.Declaration;

            if (declaration != null)
            {
                SeparatedSyntaxList<VariableDeclaratorSyntax> variables = declaration.Variables;

                if (variables.Count == 1)
                {
                    VariableDeclaratorSyntax declarator = variables[0];
                    EqualsValueClauseSyntax initializer = declarator.Initializer;

                    if (initializer != null)
                    {
                        ExpressionSyntax value = initializer.Value;

                        if (value != null)
                        {
                            SyntaxToken identifier = declarator.Identifier;

                            SyntaxList<StatementSyntax> statements = StatementContainer.GetStatements(localDeclaration);

                            if (statements.Any())
                            {
                                int index = statements.IndexOf(localDeclaration);

                                if (index < statements.Count - 1)
                                {
                                    StatementSyntax nextStatement = statements[index + 1];

                                    ExpressionSyntax right = GetAssignedValue(nextStatement);

                                    if (right?.IsKind(SyntaxKind.IdentifierName) == true)
                                    {
                                        var identifierName = (IdentifierNameSyntax)right;

                                        if (identifier.ValueText == identifierName.Identifier.ValueText)
                                        {
                                            bool isReferenced = false;

                                            if (index < statements.Count - 2)
                                            {
                                                TextSpan span = TextSpan.FromBounds(statements[index + 2].SpanStart, statements.Last().Span.End);

                                                isReferenced = IsLocalVariableReferenced(context, localDeclaration.Parent, span, declarator, identifier);
                                            }

                                            if (!isReferenced
                                                && !localDeclaration.Parent.ContainsDirectives(TextSpan.FromBounds(localDeclaration.SpanStart, nextStatement.Span.End)))
                                            {
                                                ReportDiagnostic(context, localDeclaration, declaration, identifier, initializer, right);
                                            }
                                        }
                                    }
                                    else if (nextStatement.IsKind(SyntaxKind.ForEachStatement))
                                    {
                                        var forEachStatement = (ForEachStatementSyntax)nextStatement;

                                        ExpressionSyntax expression = forEachStatement.Expression;

                                        if (expression?.IsKind(SyntaxKind.IdentifierName) == true)
                                        {
                                            var identifierName = (IdentifierNameSyntax)expression;

                                            if (identifier.ValueText == identifierName.Identifier.ValueText)
                                            {
                                                TextSpan span = TextSpan.FromBounds(expression.Span.End, statements.Last().Span.End);

                                                if (!IsLocalVariableReferenced(context, localDeclaration.Parent, span, declarator, identifier)
                                                    && !localDeclaration.Parent.ContainsDirectives(TextSpan.FromBounds(localDeclaration.SpanStart, expression.Span.End)))
                                                {
                                                    ReportDiagnostic(context, localDeclaration, declaration, identifier, initializer, expression);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void ReportDiagnostic(
            SyntaxNodeAnalysisContext context,
            LocalDeclarationStatementSyntax localDeclaration,
            VariableDeclarationSyntax declaration,
            SyntaxToken identifier,
            EqualsValueClauseSyntax initializer,
            ExpressionSyntax expression)
        {
            context.ReportDiagnostic(DiagnosticDescriptors.InlineLocalVariable, localDeclaration);

            foreach (SyntaxToken modifier in localDeclaration.Modifiers)
                context.ReportToken(FadeOutDescriptor, modifier);

            context.ReportNode(FadeOutDescriptor, declaration.Type);
            context.ReportToken(FadeOutDescriptor, identifier);
            context.ReportToken(FadeOutDescriptor, initializer.EqualsToken);
            context.ReportToken(FadeOutDescriptor, localDeclaration.SemicolonToken);
            context.ReportNode(FadeOutDescriptor, expression);
        }

        private static ExpressionSyntax GetAssignedValue(StatementSyntax statement)
        {
            switch (statement.Kind())
            {
                case SyntaxKind.ExpressionStatement:
                    {
                        var expressionStatement = (ExpressionStatementSyntax)statement;

                        ExpressionSyntax expression = expressionStatement.Expression;

                        if (expression?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true)
                        {
                            var assignment = (AssignmentExpressionSyntax)expression;

                            return assignment.Right;
                        }

                        break;
                    }
                case SyntaxKind.LocalDeclarationStatement:
                    {
                        var localDeclaration = (LocalDeclarationStatementSyntax)statement;

                        return localDeclaration
                            .Declaration?
                            .SingleVariableOrDefault()?
                            .Initializer?
                            .Value;
                    }
            }

            return null;
        }

        private static bool IsLocalVariableReferenced(
            SyntaxNodeAnalysisContext context,
            SyntaxNode node,
            TextSpan span,
            VariableDeclaratorSyntax declarator,
            SyntaxToken identifier)
        {
            foreach (SyntaxNode descendant in node.DescendantNodes(span))
            {
                if (descendant.IsKind(SyntaxKind.IdentifierName))
                {
                    var identifierName = (IdentifierNameSyntax)descendant;

                    if (identifier.ValueText == identifierName.Identifier.ValueText)
                    {
                        ISymbol symbol = context.SemanticModel.GetSymbol(identifierName, context.CancellationToken);

                        if (symbol?.IsLocal() == true
                            && symbol.Equals(context.SemanticModel.GetDeclaredSymbol(declarator, context.CancellationToken)))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            LocalDeclarationStatementSyntax localDeclaration,
            CancellationToken cancellationToken)
        {
            StatementContainer container;

            if (StatementContainer.TryCreate(localDeclaration, out container))
            {
                SyntaxList<StatementSyntax> statements = container.Statements;

                int index = statements.IndexOf(localDeclaration);

                ExpressionSyntax value = localDeclaration
                    .Declaration
                    .Variables
                    .First()
                    .Initializer
                    .Value;

                StatementSyntax nextStatement = statements[index + 1];

                StatementSyntax newStatement = GetStatementWithReplacedValue(nextStatement, value);

                SyntaxTriviaList leadingTrivia = localDeclaration.GetLeadingTrivia();

                IEnumerable<SyntaxTrivia> trivia = container
                    .Node
                    .DescendantTrivia(TextSpan.FromBounds(localDeclaration.SpanStart, nextStatement.Span.Start));

                if (!trivia.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                {
                    newStatement = newStatement.WithLeadingTrivia(leadingTrivia.Concat(trivia));
                }
                else
                {
                    newStatement = newStatement.WithLeadingTrivia(leadingTrivia);
                }

                SyntaxList<StatementSyntax> newStatements = statements
                    .Replace(nextStatement, newStatement)
                    .RemoveAt(index);

                return await document.ReplaceNodeAsync(container.Node, container.NodeWithStatements(newStatements), cancellationToken).ConfigureAwait(false);
            }

            Debug.Assert(false, "");

            return document;
        }

        private static StatementSyntax GetStatementWithReplacedValue(StatementSyntax statement, ExpressionSyntax newValue)
        {
            switch (statement.Kind())
            {
                case SyntaxKind.ExpressionStatement:
                    {
                        var expressionStatement = (ExpressionStatementSyntax)statement;

                        var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;

                        AssignmentExpressionSyntax newAssignment = assignment.WithRight(newValue.WithTriviaFrom(assignment.Right));

                        return expressionStatement.WithExpression(newAssignment);
                    }
                case SyntaxKind.LocalDeclarationStatement:
                    {
                        var localDeclaration = (LocalDeclarationStatementSyntax)statement;

                        ExpressionSyntax value = localDeclaration
                            .Declaration
                            .Variables[0]
                            .Initializer
                            .Value;

                        return statement.ReplaceNode(value, newValue.WithTriviaFrom(value));
                    }
                case SyntaxKind.ForEachStatement:
                    {
                        var forEachStatement = (ForEachStatementSyntax)statement;

                        return forEachStatement.WithExpression(newValue.WithTriviaFrom(forEachStatement.Expression));
                    }
                default:
                    {
                        Debug.Assert(false, "");
                        return statement;
                    }
            }
        }
    }
}
