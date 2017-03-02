﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveEmptyArgumentListRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, ObjectCreationExpressionSyntax objectCreationExpression)
        {
            if (objectCreationExpression.Type?.IsMissing == false
                && objectCreationExpression.Initializer?.IsMissing == false)
            {
                ArgumentListSyntax argumentList = objectCreationExpression.ArgumentList;

                if (argumentList?.Arguments.Any() == false)
                {
                    SyntaxToken openParen = argumentList.OpenParenToken;
                    SyntaxToken closeParen = argumentList.CloseParenToken;

                    if (!openParen.IsMissing
                        && !closeParen.IsMissing
                        && openParen.TrailingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                        && closeParen.LeadingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                    {
                        context.ReportDiagnostic(DiagnosticDescriptors.RemoveEmptyArgumentList, argumentList);
                    }
                }
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ArgumentListSyntax argumentList,
            CancellationToken cancellationToken)
        {
            return await document.RemoveNodeAsync(argumentList, SyntaxRemoveOptions.KeepExteriorTrivia, cancellationToken).ConfigureAwait(false);
        }
    }
}
