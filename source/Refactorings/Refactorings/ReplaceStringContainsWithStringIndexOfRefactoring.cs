﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
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
    internal static class ReplaceStringContainsWithStringIndexOfRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, InvocationExpressionSyntax invocation)
        {
            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            MethodInfo info = semanticModel.GetMethodInfo(invocation, context.CancellationToken);

            if (info.IsValid
                && info.HasName("Contains")
                && info.IsContainingType(SpecialType.System_String)
                && info.Symbol.SingleParameterOrDefault()?.Type.IsString() == true)
            {
                context.RegisterRefactoring(
                    "Replace Contains with IndexOf",
                    cancellationToken => RefactorAsync(context.Document, invocation, context.CancellationToken));
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken)
        {
            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

            InvocationExpressionSyntax newInvocationExpression = invocation
                .WithExpression(memberAccess.WithName(IdentifierName("IndexOf")))
                .AddArgumentListArguments(
                    Argument(
                        ParseName("System.StringComparison.OrdinalIgnoreCase").WithSimplifierAnnotation()));

            SyntaxNode parent = invocation.Parent;

            if (parent?.IsKind(SyntaxKind.LogicalNotExpression) == true)
            {
                BinaryExpressionSyntax equalsExpression = EqualsExpression(newInvocationExpression, NumericLiteralExpression(-1))
                    .WithTriviaFrom(parent)
                    .WithFormatterAnnotation();

                return await document.ReplaceNodeAsync(parent, equalsExpression, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                BinaryExpressionSyntax notEqualsExpression = NotEqualsExpression(newInvocationExpression, NumericLiteralExpression(-1))
                    .WithTriviaFrom(invocation)
                    .WithFormatterAnnotation();

                return await document.ReplaceNodeAsync(invocation, notEqualsExpression, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}