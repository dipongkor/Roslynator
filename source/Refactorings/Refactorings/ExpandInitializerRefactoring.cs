﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ExpandInitializerRefactoring
    {
        private const string Title = "Expand initializer";

        public static async Task ComputeRefactoringsAsync(RefactoringContext context, InitializerExpressionSyntax initializer)
        {
            if (initializer.IsKind(
                    SyntaxKind.ObjectInitializerExpression,
                    SyntaxKind.CollectionInitializerExpression)
                && initializer.Expressions.Any())
            {
                SyntaxNode parent = initializer.Parent;

                if (parent?.IsKind(SyntaxKind.ObjectCreationExpression) == true)
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    if (CanExpand(initializer, semanticModel, context.CancellationToken))
                    {
                        parent = parent.Parent;

                        switch (parent?.Kind())
                        {
                            case SyntaxKind.SimpleAssignmentExpression:
                                {
                                    var assignmentExpression = (AssignmentExpressionSyntax)parent;

                                    ExpressionSyntax left = assignmentExpression.Left;

                                    if (left != null)
                                    {
                                        parent = assignmentExpression.Parent;

                                        if (parent?.IsKind(SyntaxKind.ExpressionStatement) == true)
                                            RegisterRefactoring(context, (StatementSyntax)parent, initializer, left);
                                    }

                                    break;
                                }
                            case SyntaxKind.EqualsValueClause:
                                {
                                    var equalsValueClause = (EqualsValueClauseSyntax)parent;

                                    parent = equalsValueClause.Parent;

                                    if (parent?.IsKind(SyntaxKind.VariableDeclarator) == true)
                                    {
                                        parent = parent.Parent;

                                        if (parent?.IsKind(SyntaxKind.VariableDeclaration) == true)
                                        {
                                            parent = parent.Parent;

                                            if (parent?.IsKind(SyntaxKind.LocalDeclarationStatement) == true)
                                            {
                                                var variableDeclarator = (VariableDeclaratorSyntax)equalsValueClause.Parent;

                                                RegisterRefactoring(
                                                    context,
                                                    (StatementSyntax)parent,
                                                    initializer,
                                                    IdentifierName(variableDeclarator.Identifier.ValueText));
                                            }
                                        }
                                    }

                                    break;
                                }
                        }
                    }
                }
            }
        }

        private static void RegisterRefactoring(
            RefactoringContext context,
            StatementSyntax statement,
            InitializerExpressionSyntax initializer,
            ExpressionSyntax expression)
        {
            StatementContainer container;

            if (StatementContainer.TryCreate(statement, out container))
            {
                context.RegisterRefactoring(
                    Title,
                    cancellationToken => RefactorAsync(
                        context.Document,
                        container,
                        statement,
                        initializer,
                        expression.WithoutTrivia(),
                        cancellationToken));
            }
        }

        private static bool CanExpand(
            InitializerExpressionSyntax initializer,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            var objectCreationExpression = (ObjectCreationExpressionSyntax)initializer.Parent;

            if (objectCreationExpression.Type != null)
            {
                ExpressionSyntax expression = initializer.Expressions[0];

                if (expression.IsKind(SyntaxKind.SimpleAssignmentExpression))
                {
                    var assignment = (AssignmentExpressionSyntax)expression;

                    ExpressionSyntax left = assignment.Left;

                    if (left.IsKind(SyntaxKind.ImplicitElementAccess))
                    {
                        var implicitElementAccess = (ImplicitElementAccessSyntax)left;

                        BracketedArgumentListSyntax argumentList = implicitElementAccess.ArgumentList;

                        if (argumentList?.Arguments.Any() == true)
                        {
                            return HasPublicWritableIndexer(
                                argumentList.Arguments[0].Expression,
                                objectCreationExpression,
                                semanticModel,
                                cancellationToken);
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else if (expression.IsKind(SyntaxKind.ComplexElementInitializerExpression))
                {
                    var initializerExpression = (InitializerExpressionSyntax)expression;

                    SeparatedSyntaxList<ExpressionSyntax> expressions = initializerExpression.Expressions;

                    if (expressions.Any())
                        return HasPublicWritableIndexer(expressions[0], objectCreationExpression, semanticModel, cancellationToken);
                }
                else
                {
                    return HasPublicAddMethod(expression, objectCreationExpression, semanticModel, cancellationToken);
                }
            }

            return false;
        }

        private static bool HasPublicAddMethod(
            ExpressionSyntax expression,
            ObjectCreationExpressionSyntax objectCreationExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            var typeSymbol = semanticModel.GetSymbol(objectCreationExpression.Type, cancellationToken) as ITypeSymbol;

            if (typeSymbol != null)
            {
                foreach (IMethodSymbol methodSymbol in typeSymbol.GetMethods("Add"))
                {
                    if (methodSymbol.IsPublic()
                        && !methodSymbol.IsStatic)
                    {
                        ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                        if (parameters.Length == 1)
                        {
                            ITypeSymbol expressionSymbol = semanticModel.GetTypeInfo(expression, cancellationToken).ConvertedType;

                            return expressionSymbol?.Equals(parameters[0].Type) == true;
                        }
                    }
                }
            }

            return false;
        }

        private static bool HasPublicWritableIndexer(
            ExpressionSyntax expression,
            ObjectCreationExpressionSyntax objectCreationExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            var typeSymbol = semanticModel.GetSymbol(objectCreationExpression.Type, cancellationToken) as ITypeSymbol;

            if (typeSymbol != null)
            {
                foreach (ISymbol member in typeSymbol.GetMembers("this[]"))
                {
                    if (member.IsPublic()
                        && !member.IsStatic
                        && member.IsProperty())
                    {
                        var propertySymbol = (IPropertySymbol)member;

                        if (!propertySymbol.IsReadOnly)
                        {
                            ImmutableArray<IParameterSymbol> parameters = propertySymbol.Parameters;

                            if (parameters.Length == 1)
                            {
                                ITypeSymbol expressionSymbol = semanticModel.GetTypeInfo(expression, cancellationToken).ConvertedType;

                                return expressionSymbol?.Equals(propertySymbol.Parameters[0].Type) == true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            StatementContainer statementContainer,
            StatementSyntax statement,
            InitializerExpressionSyntax initializer,
            ExpressionSyntax expression,
            CancellationToken cancellationToken)
        {
            ExpressionStatementSyntax[] expressions = Refactor(initializer, expression).ToArray();

            expressions[expressions.Length - 1] = expressions[expressions.Length - 1]
                .WithTrailingTrivia(statement.GetTrailingTrivia());

            SyntaxList<StatementSyntax> statements = statementContainer.Statements;

            int index = statements.IndexOf(statement);

            StatementSyntax newStatement = statement.RemoveNode(initializer, SyntaxRemoveOptions.KeepNoTrivia);

            SyntaxKind statementKind = statement.Kind();

            if (statementKind == SyntaxKind.ExpressionStatement)
            {
                var expressionStatement = (ExpressionStatementSyntax)newStatement;

                newStatement = expressionStatement
                    .WithExpression(expressionStatement.Expression.WithoutTrailingTrivia());
            }
            else if (statementKind == SyntaxKind.LocalDeclarationStatement)
            {
                var localDeclaration = (LocalDeclarationStatementSyntax)newStatement;

                newStatement = localDeclaration
                    .WithDeclaration(localDeclaration.Declaration.WithoutTrailingTrivia());
            }

            SyntaxList<StatementSyntax> newStatements = statements.Replace(statement, newStatement);

            SyntaxNode newNode = statementContainer
                .NodeWithStatements(newStatements.InsertRange(index + 1, expressions))
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(statementContainer.Node, newNode, cancellationToken).ConfigureAwait(false);
        }

        private static IEnumerable<ExpressionStatementSyntax> Refactor(
            InitializerExpressionSyntax initializer,
            ExpressionSyntax initializedExpression)
        {
            foreach (ExpressionSyntax expression in initializer.Expressions)
            {
                SyntaxKind kind = expression.Kind();

                if (kind == SyntaxKind.SimpleAssignmentExpression)
                {
                    var assignment = (AssignmentExpressionSyntax)expression;
                    ExpressionSyntax left = assignment.Left;
                    ExpressionSyntax right = assignment.Right;

                    if (left.IsKind(SyntaxKind.ImplicitElementAccess))
                    {
                        yield return ExpressionStatement(
                            SimpleAssignmentExpression(
                                ElementAccessExpression(
                                    initializedExpression,
                                    ((ImplicitElementAccessSyntax)left).ArgumentList),
                                right));
                    }
                    else
                    {
                        yield return ExpressionStatement(
                            SimpleAssignmentExpression(
                                SimpleMemberAccessExpression(
                                    initializedExpression,
                                    (IdentifierNameSyntax)left),
                                right));
                    }
                }
                else if (kind == SyntaxKind.ComplexElementInitializerExpression)
                {
                    var elementInitializer = (InitializerExpressionSyntax)expression;

                    yield return ExpressionStatement(
                        SimpleAssignmentExpression(
                            ElementAccessExpression(
                                initializedExpression,
                                BracketedArgumentList(SingletonSeparatedList(Argument(elementInitializer.Expressions[0])))),
                            elementInitializer.Expressions[1]));
                }
                else
                {
                    yield return ExpressionStatement(
                        InvocationExpression(
                            SimpleMemberAccessExpression(
                                initializedExpression,
                                IdentifierName("Add")),
                            ArgumentList(Argument(expression))
                        )
                    );
                }
            }
        }
    }
}
