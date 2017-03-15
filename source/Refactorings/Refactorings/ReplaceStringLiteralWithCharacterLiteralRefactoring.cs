﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceStringLiteralWithCharacterLiteralRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, LiteralExpressionSyntax literalExpression)
        {
            if (literalExpression.IsKind(SyntaxKind.StringLiteralExpression)
                && literalExpression.Token.ValueText.Length == 1)
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                if (semanticModel.ContainsDiagnostic(CSharpErrorCodes.CannotImplicitlyConvertType, literalExpression.Span, context.CancellationToken))
                {
                    context.RegisterRefactoring(
                        "Replace string literal with character literal",
                        cancellationToken => RefactorAsync(context.Document, literalExpression, cancellationToken));
                }
            }
        }

        private static Task<Document> RefactorAsync(
            Document document,
            LiteralExpressionSyntax literalExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var newNode = (LiteralExpressionSyntax)ParseExpression($"'{GetCharacterLiteralText(literalExpression)}'")
                .WithTriviaFrom(literalExpression);

            return document.ReplaceNodeAsync(literalExpression, newNode, cancellationToken);
        }

        private static string GetCharacterLiteralText(LiteralExpressionSyntax literalExpression)
        {
            string s = literalExpression.Token.ValueText;

            switch (s[0])
            {
                case '\'':
                    return @"\'";
                case '\"':
                    return @"\""";
                case '\\':
                    return @"\\";
                case '\0':
                    return @"\0";
                case '\a':
                    return @"\a";
                case '\b':
                    return @"\b";
                case '\f':
                    return @"\f";
                case '\n':
                    return @"\n";
                case '\r':
                    return @"\r";
                case '\t':
                    return @"\t";
                case '\v':
                    return @"\v";
                default:
                    return s;
            }
        }
    }
}
