﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ParenthesizeConditionInConditionalExpressionRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, ConditionalExpressionSyntax conditionalExpression)
        {
            ExpressionSyntax condition = conditionalExpression.Condition;

            if (condition?.IsKind(SyntaxKind.ParenthesizedExpression) == false)
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.ParenthesizeConditionInConditionalExpression,
                    condition);
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ConditionalExpressionSyntax conditionalExpression,
            CancellationToken cancellationToken)
        {
            ConditionalExpressionSyntax newNode = conditionalExpression
                .WithCondition(
                    SyntaxFactory.ParenthesizedExpression(
                        conditionalExpression.Condition.WithoutTrivia()
                    ).WithTriviaFrom(conditionalExpression.Condition)
                ).WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(conditionalExpression, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
