﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UncommentRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            SyntaxTrivia comment,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxToken token = comment.Token;

            var triviaList = default(SyntaxTriviaList);

            int index = token.LeadingTrivia.IndexOf(comment);

            if (index != -1)
            {
                triviaList = token.LeadingTrivia;
            }
            else
            {
                index = token.TrailingTrivia.IndexOf(comment);
                triviaList = token.TrailingTrivia;
            }

            IEnumerable<TextChange> textChanges = GetTextChanges(triviaList, index);

            return document.WithTextChangesAsync(textChanges, cancellationToken);
        }

        private static IEnumerable<TextChange> GetTextChanges(SyntaxTriviaList triviaList, int index)
        {
            int i = index;
            while (i >= 0)
            {
                SyntaxTrivia trivia = triviaList[i];

                if (IsAllowedTrivia(trivia))
                {
                    if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
                        yield return new TextChange(trivia.Span, trivia.ToString().Substring(2));

                    i--;
                }
                else
                {
                    break;
                }
            }

            i = index + 1;
            while (i < triviaList.Count)
            {
                SyntaxTrivia trivia = triviaList[i];

                if (IsAllowedTrivia(trivia))
                {
                    if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
                        yield return new TextChange(trivia.Span, trivia.ToString().Substring(2));

                    i++;
                }
                else
                {
                    break;
                }
            }
        }

        private static bool IsAllowedTrivia(SyntaxTrivia trivia)
        {
            switch (trivia.Kind())
            {
                case SyntaxKind.WhitespaceTrivia:
                case SyntaxKind.EndOfLineTrivia:
                case SyntaxKind.SingleLineCommentTrivia:
                    return true;
                default:
                    return false;
            }
        }
    }
}
