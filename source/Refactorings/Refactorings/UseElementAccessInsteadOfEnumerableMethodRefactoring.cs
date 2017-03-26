﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
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
    internal static class UseElementAccessInsteadOfEnumerableMethodRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, InvocationExpressionSyntax invocation)
        {
            if (invocation.Expression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true
                && invocation.ArgumentList != null)
            {
                var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

                string methodName = memberAccess.Name?.Identifier.ValueText;

                switch (methodName)
                {
                    case "First":
                    case "Last":
                        {
                            await ProcessFirstOrLastAsync(context, invocation, methodName).ConfigureAwait(false);
                            break;
                        }
                    case "ElementAt":
                        {
                            await ProcessElementAtAsync(context, invocation).ConfigureAwait(false);
                            break;
                        }
                }
            }
        }

        private static async Task ProcessFirstOrLastAsync(RefactoringContext context, InvocationExpressionSyntax invocation, string methodName)
        {
            if (invocation.ArgumentList.Arguments.Count == 0)
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                if (semanticModel
                    .GetExtensionMethodInfo(invocation, ExtensionMethodKind.Reduced, context.CancellationToken)
                    .IsLinqExtensionOfIEnumerableOfTWithoutParameters(methodName, allowImmutableArrayExtension: true))
                {
                    var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

                    ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(memberAccess.Expression, context.CancellationToken);

                    if (typeSymbol != null
                        && (typeSymbol.IsArrayType() || SymbolUtility.FindGetItemMethodWithInt32Parameter(typeSymbol)?.IsAccessible(semanticModel, invocation.SpanStart) == true))
                    {
                        string propertyName = GetCountOrLengthPropertyName(memberAccess.Expression, semanticModel, context.CancellationToken);

                        if (propertyName != null)
                        {
                            context.RegisterRefactoring(
                                $"Use [] instead of calling '{methodName}'",
                                cancellationToken =>
                                {
                                    return RefactorAsync(
                                        context.Document,
                                        invocation,
                                        propertyName,
                                        cancellationToken);
                                });
                        }
                    }
                }
            }
        }

        private static async Task ProcessElementAtAsync(RefactoringContext context, InvocationExpressionSyntax invocation)
        {
            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            if (invocation.ArgumentList?.Arguments.Count == 1
                && semanticModel
                    .GetExtensionMethodInfo(invocation, ExtensionMethodKind.Reduced, context.CancellationToken)
                    .IsLinqElementAt(allowImmutableArrayExtension: true))
            {
                var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(memberAccess.Expression, context.CancellationToken);

                if (typeSymbol != null
                    && (typeSymbol.IsArrayType() || SymbolUtility.FindGetItemMethodWithInt32Parameter(typeSymbol)?.IsAccessible(semanticModel, invocation.SpanStart) == true))
                {
                    context.RegisterRefactoring(
                        "Use [] instead of calling 'ElementAt'",
                        cancellationToken => RefactorAsync(context.Document, invocation, null, cancellationToken));
                }
            }
        }

        private static string GetCountOrLengthPropertyName(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, cancellationToken);

            if (typeSymbol?.IsErrorType() == false
                && !typeSymbol.IsConstructedFromIEnumerableOfT())
            {
                if (typeSymbol.IsArrayType()
                    || typeSymbol.IsConstructedFromImmutableArrayOfT(semanticModel))
                {
                    return "Length";
                }

                if (typeSymbol.ImplementsICollectionOfT())
                    return "Count";
            }

            return null;
        }

        private static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            string propertyName = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

            ElementAccessExpressionSyntax elementAccess = ElementAccessExpression(
                memberAccess.Expression.WithoutTrailingTrivia(),
                BracketedArgumentList(
                    SingletonSeparatedList(
                        Argument(CreateArgumentExpression(invocation, memberAccess, propertyName)))));

            return document.ReplaceNodeAsync(
                invocation,
                elementAccess.WithTriviaFrom(invocation),
                cancellationToken);
        }

        private static ExpressionSyntax CreateArgumentExpression(
            InvocationExpressionSyntax invocation,
            MemberAccessExpressionSyntax memberAccess,
            string propertyName)
        {
            switch (memberAccess.Name.Identifier.ValueText)
            {
                case "First":
                    {
                        return NumericLiteralExpression(0);
                    }
                case "Last":
                    {
                        return SubtractExpression(
                            SimpleMemberAccessExpression(
                                ProcessExpression(memberAccess.Expression),
                                IdentifierName(propertyName)),
                            NumericLiteralExpression(1));
                    }
                case "ElementAt":
                    {
                        return ProcessExpression(invocation.ArgumentList.Arguments[0].Expression);
                    }
            }

            return default(ExpressionSyntax);
        }

        private static ExpressionSyntax ProcessExpression(ExpressionSyntax expression)
        {
            if (expression
                .DescendantTrivia(expression.Span)
                .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                expression = Remover.RemoveWhitespaceOrEndOfLineTrivia(expression)
                    .WithFormatterAnnotation();
            }

            return expression;
        }
    }
}
