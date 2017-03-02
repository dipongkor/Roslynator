﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceInterpolatedStringWithStringLiteralRefactoring
    {
        public static bool CanRefactor(InterpolatedStringExpressionSyntax interpolatedString)
        {
            if (interpolatedString == null)
                throw new ArgumentNullException(nameof(interpolatedString));

            SyntaxList<InterpolatedStringContentSyntax> contents = interpolatedString.Contents;

            return contents.Count == 0
                || (contents.Count == 1 && contents[0].IsKind(SyntaxKind.InterpolatedStringText));
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            InterpolatedStringExpressionSyntax interpolatedString,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (interpolatedString == null)
                throw new ArgumentNullException(nameof(interpolatedString));

            string s = UnescapeBraces(interpolatedString.ToString().Substring(1));

            var newNode = (LiteralExpressionSyntax)SyntaxFactory.ParseExpression(s)
                .WithTriviaFrom(interpolatedString);

            return await document.ReplaceNodeAsync(interpolatedString, newNode, cancellationToken).ConfigureAwait(false);
        }

        private static string UnescapeBraces(string s)
        {
            var sb = new StringBuilder(s.Length);

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '{')
                {
                    if (i < s.Length - 1 && s[i + 1] == '{')
                    {
                        sb.Append('{');
                        i++;
                        continue;
                    }
                }
                else if (s[i] == '}')
                {
                    if (i < s.Length - 1 && s[i + 1] == '}')
                    {
                        sb.Append('}');
                        i++;
                        continue;
                    }
                }

                sb.Append(s[i]);
            }

            return sb.ToString();
        }
    }
}
