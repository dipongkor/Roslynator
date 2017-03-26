﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveRedundantCallRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken)
        {
            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

            ExpressionSyntax newExpression = memberAccess.Expression
                .AppendToTrailingTrivia(
                    memberAccess.OperatorToken.GetLeadingTrailingTrivia()
                        .Concat(memberAccess.Name.GetLeadingTrailingTrivia())
                        .Concat(invocation.ArgumentList.OpenParenToken.GetLeadingTrailingTrivia())
                        .Concat(invocation.ArgumentList.CloseParenToken.GetLeadingTrailingTrivia()));

            newExpression = newExpression.WithFormatterAnnotation();

            return document.ReplaceNodeAsync(invocation, newExpression, cancellationToken);
        }
    }
}