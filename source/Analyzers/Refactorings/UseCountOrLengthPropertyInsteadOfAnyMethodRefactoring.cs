﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseCountOrLengthPropertyInsteadOfAnyMethodRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, MemberAccessExpressionSyntax memberAccess)
        {
            if (!invocation.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                SemanticModel semanticModel = context.SemanticModel;
                CancellationToken cancellationToken = context.CancellationToken;

                if (semanticModel
                    .GetExtensionMethodInfo(invocation, ExtensionMethodKind.Reduced, cancellationToken)
                    .MethodInfo
                    .IsLinqExtensionOfIEnumerableOfTWithoutParameters("Any"))
                {
                    string propertyName = GetCountOrLengthPropertyName(memberAccess.Expression, semanticModel, cancellationToken);

                    if (propertyName != null)
                    {
                        bool success = false;

                        TextSpan span = TextSpan.FromBounds(memberAccess.Name.Span.Start, invocation.Span.End);

                        if (invocation.DescendantTrivia(span).All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                        {
                            if (invocation.IsParentKind(SyntaxKind.LogicalNotExpression))
                            {
                                var logicalNot = (PrefixUnaryExpressionSyntax)invocation.Parent;

                                if (logicalNot.OperatorToken.TrailingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                                    && logicalNot.Operand.GetLeadingTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                                {
                                    success = true;
                                }
                            }
                            else
                            {
                                success = true;
                            }
                        }

                        if (success)
                        {
                            Diagnostic diagnostic = Diagnostic.Create(
                                DiagnosticDescriptors.UseCountOrLengthPropertyInsteadOfAnyMethod,
                                Location.Create(context.Node.SyntaxTree, span),
                                ImmutableDictionary.CreateRange(new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("PropertyName", propertyName) }),
                                propertyName);

                            context.ReportDiagnostic(diagnostic);
                        }
                    }
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
                if (typeSymbol.IsArrayType())
                    return "Length";

                if (typeSymbol.ImplementsICollectionOfT())
                    return "Count";
            }

            return null;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            string propertyName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

            memberAccess = memberAccess
                .WithName(IdentifierName(propertyName).WithTriviaFrom(memberAccess.Name));

            SyntaxNode newRoot = null;

            if (invocation.IsParentKind(SyntaxKind.LogicalNotExpression))
            {
                BinaryExpressionSyntax binaryExpression = EqualsExpression(
                    memberAccess,
                    NumericLiteralExpression(0));

                newRoot = root.ReplaceNode(
                    invocation.Parent,
                    binaryExpression.WithTriviaFrom(invocation.Parent));
            }
            else
            {
                BinaryExpressionSyntax binaryExpression = GreaterThanExpression(
                    memberAccess,
                    NumericLiteralExpression(0));

                newRoot = root.ReplaceNode(
                    invocation,
                    binaryExpression.WithTriviaFrom(invocation));
            }

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
