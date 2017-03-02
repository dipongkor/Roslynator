﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceReturnStatementWithExpressionStatementRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, ReturnStatementSyntax returnStatement)
        {
            if (CanRefactor(returnStatement, context.SemanticModel, context.CancellationToken))
            {
                context.ReportDiagnostic(DiagnosticDescriptors.ReplaceReturnStatementWithExpressionStatement, returnStatement.ReturnKeyword, "return");
            }
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, YieldStatementSyntax yieldStatement)
        {
            if (CanRefactor(yieldStatement, context.SemanticModel, context.CancellationToken)
                && !yieldStatement.ContainsDirectives(TextSpan.FromBounds(yieldStatement.YieldKeyword.Span.End, yieldStatement.Expression.Span.Start)))
            {
                TextSpan span = TextSpan.FromBounds(yieldStatement.YieldKeyword.SpanStart, yieldStatement.ReturnOrBreakKeyword.Span.End);

                Location location = Location.Create(yieldStatement.SyntaxTree, span);

                context.ReportDiagnostic(DiagnosticDescriptors.ReplaceReturnStatementWithExpressionStatement, location, "yield");
            }
        }

        public static bool CanRefactor(ReturnStatementSyntax returnStatement, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            ExpressionSyntax expression = returnStatement.Expression;

            return expression?.IsMissing == false
                && semanticModel
                    .GetTypeSymbol(expression, cancellationToken)?
                    .IsVoid() == true;
        }

        public static bool CanRefactor(YieldStatementSyntax yieldStatement, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (yieldStatement.IsYieldReturn())
            {
                ExpressionSyntax expression = yieldStatement.Expression;

                return expression?.IsMissing == false
                    && semanticModel
                        .GetTypeSymbol(expression, cancellationToken)?
                        .IsVoid() == true;
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ReturnStatementSyntax returnStatement,
            CancellationToken cancellationToken)
        {
            ExpressionStatementSyntax newNode = ExpressionStatement(returnStatement.Expression)
                .WithTriviaFrom(returnStatement)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(returnStatement, newNode, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            YieldStatementSyntax yieldStatement,
            CancellationToken cancellationToken)
        {
            ExpressionStatementSyntax newNode = ExpressionStatement(yieldStatement.Expression)
                .WithTriviaFrom(yieldStatement)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(yieldStatement, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}