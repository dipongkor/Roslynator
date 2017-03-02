﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceIfStatementWithAssignmentRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, IfStatementSyntax ifStatement)
        {
            if (CanRefactor(ifStatement, context.SemanticModel, context.CancellationToken)
                && !ifStatement.SpanContainsDirectives())
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.ReplaceIfStatementWithAssignment,
                    ifStatement);
            }
        }

        public static bool CanRefactor(
            IfStatementSyntax ifStatement,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (IfElseChain.IsTopmostIf(ifStatement))
            {
                ElseClauseSyntax elseClause = ifStatement.Else;

                if (elseClause != null)
                {
                    ExpressionSyntax condition = ifStatement.Condition;

                    if (condition != null)
                    {
                        ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(condition, cancellationToken);

                        if (typeSymbol?.IsBoolean() == true)
                        {
                            AssignmentExpressionSyntax trueExpression = GetSimpleAssignmentExpression(ifStatement.Statement);

                            ExpressionSyntax trueRight = trueExpression?.Right;

                            if (trueRight?.IsBooleanLiteralExpression() == true)
                            {
                                AssignmentExpressionSyntax falseExpression = GetSimpleAssignmentExpression(elseClause.Statement);

                                ExpressionSyntax falseRight = falseExpression?.Right;

                                if (falseRight?.IsBooleanLiteralExpression() == true)
                                {
                                    var trueBooleanLiteral = (LiteralExpressionSyntax)trueRight;
                                    var falseBooleanLiteral = (LiteralExpressionSyntax)falseRight;

                                    if (trueBooleanLiteral.IsKind(SyntaxKind.TrueLiteralExpression) != falseBooleanLiteral.IsKind(SyntaxKind.TrueLiteralExpression)
                                        && trueExpression.Left?.IsEquivalentTo(falseExpression.Left, topLevel: false) == true)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        private static AssignmentExpressionSyntax GetSimpleAssignmentExpression(StatementSyntax statement)
        {
            statement = GetStatement(statement);

            if (statement?.IsKind(SyntaxKind.ExpressionStatement) == true)
            {
                var expressionStatement = (ExpressionStatementSyntax)statement;

                ExpressionSyntax expression = expressionStatement.Expression;

                if (expression?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true)
                    return (AssignmentExpressionSyntax)expression;
            }

            return null;
        }

        private static StatementSyntax GetStatement(StatementSyntax statement)
        {
            if (statement != null)
            {
                if (statement.IsKind(SyntaxKind.Block))
                {
                    var block = (BlockSyntax)statement;

                    SyntaxList<StatementSyntax> statements = block.Statements;

                    if (statements.Count == 1)
                        return statements.First();
                }
                else
                {
                    return statement;
                }
            }

            return null;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax condition = ifStatement.Condition;

            AssignmentExpressionSyntax assignment = GetSimpleAssignmentExpression(ifStatement.Statement);

            if (assignment.Right.IsKind(SyntaxKind.FalseLiteralExpression))
                condition = Negator.LogicallyNegate(condition);

            ExpressionStatementSyntax newNode = SimpleAssignmentExpressionStatement(assignment.Left, condition)
                .WithTriviaFrom(ifStatement)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(ifStatement, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
