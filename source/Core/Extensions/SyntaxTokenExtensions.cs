﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;

namespace Roslynator.Extensions
{
    public static class SyntaxTokenExtensions
    {
        public static SyntaxToken PrependToLeadingTrivia(this SyntaxToken token, IEnumerable<SyntaxTrivia> trivia)
        {
            if (trivia == null)
                throw new ArgumentNullException(nameof(trivia));

            return token.WithLeadingTrivia(trivia.Concat(token.LeadingTrivia));
        }

        public static SyntaxToken PrependToLeadingTrivia(this SyntaxToken token, SyntaxTrivia trivia)
        {
            return token.WithLeadingTrivia(token.LeadingTrivia.Insert(0, trivia));
        }

        public static SyntaxToken AppendToTrailingTrivia(this SyntaxToken token, IEnumerable<SyntaxTrivia> trivia)
        {
            if (trivia == null)
                throw new ArgumentNullException(nameof(trivia));

            return token.WithTrailingTrivia(token.TrailingTrivia.AddRange(trivia));
        }

        public static SyntaxToken AppendToTrailingTrivia(this SyntaxToken token, SyntaxTrivia trivia)
        {
            return token.WithTrailingTrivia(token.TrailingTrivia.Add(trivia));
        }

        public static IEnumerable<SyntaxTrivia> GetLeadingAndTrailingTrivia(this SyntaxToken token)
        {
            return token.LeadingTrivia.Concat(token.TrailingTrivia);
        }

        public static int GetSpanStartLine(this SyntaxToken token, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (token.SyntaxTree != null)
            {
                return token.SyntaxTree.GetLineSpan(token.Span, cancellationToken).StartLine();
            }
            else
            {
                return -1;
            }
        }

        public static int GetFullSpanStartLine(this SyntaxToken token, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (token.SyntaxTree != null)
            {
                return token.SyntaxTree.GetLineSpan(token.FullSpan, cancellationToken).StartLine();
            }
            else
            {
                return -1;
            }
        }

        public static int GetSpanEndLine(this SyntaxToken token, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (token.SyntaxTree != null)
            {
                return token.SyntaxTree.GetLineSpan(token.Span, cancellationToken).EndLine();
            }
            else
            {
                return -1;
            }
        }

        public static int GetFullSpanEndLine(this SyntaxToken token, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (token.SyntaxTree != null)
            {
                return token.SyntaxTree.GetLineSpan(token.FullSpan, cancellationToken).EndLine();
            }
            else
            {
                return -1;
            }
        }

        public static SyntaxToken WithoutLeadingTrivia(this SyntaxToken token)
        {
            return token.WithLeadingTrivia(default(SyntaxTriviaList));
        }

        public static SyntaxToken WithoutTrailingTrivia(this SyntaxToken token)
        {
            return token.WithTrailingTrivia(default(SyntaxTriviaList));
        }

        public static SyntaxToken WithFormatterAnnotation(this SyntaxToken token)
        {
            return token.WithAdditionalAnnotations(Formatter.Annotation);
        }

        public static SyntaxToken WithSimplifierAnnotation(this SyntaxToken token)
        {
            return token.WithAdditionalAnnotations(Simplifier.Annotation);
        }

        public static SyntaxToken WithRenameAnnotation(this SyntaxToken token)
        {
            return token.WithAdditionalAnnotations(RenameAnnotation.Create());
        }
    }
}
