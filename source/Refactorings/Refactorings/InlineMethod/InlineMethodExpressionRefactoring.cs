﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings.InlineMethod
{
    internal class InlineMethodExpressionRefactoring : AbstractInlineMethodRefactoring
    {
        public InlineMethodExpressionRefactoring(
            Document document,
            InvocationExpressionSyntax invocationExpression,
            INamedTypeSymbol invocationEnclosingType,
            IMethodSymbol methodSymbol,
            MethodDeclarationSyntax methodDeclaration,
            ParameterInfo[] parameterInfos,
            SemanticModel invocationSemanticModel,
            SemanticModel declarationSemanticModel,
            CancellationToken cancellationToken) : base(document, invocationExpression, invocationEnclosingType, methodSymbol, methodDeclaration, parameterInfos, invocationSemanticModel, declarationSemanticModel, cancellationToken)
        {
        }

        public Task<Document> InlineMethodAsync(InvocationExpressionSyntax invocation, ExpressionSyntax expression)
        {
            ExpressionSyntax newExpression = RewriteExpression(invocation, expression);

            return Document.ReplaceNodeAsync(invocation, newExpression, CancellationToken);
        }

        public async Task<Solution> InlineAndRemoveMethodAsync(InvocationExpressionSyntax invocation, ExpressionSyntax expression)
        {
            if (invocation.SyntaxTree == MethodDeclaration.SyntaxTree)
            {
                DocumentEditor editor = await DocumentEditor.CreateAsync(Document, CancellationToken).ConfigureAwait(false);

                ExpressionSyntax newExpression = RewriteExpression(invocation, expression);

                editor.ReplaceNode(invocation, newExpression);

                editor.RemoveNode(MethodDeclaration);

                return editor.GetChangedDocument().Solution();
            }
            else
            {
                Document newDocument = await InlineMethodAsync(invocation, expression).ConfigureAwait(false);

                DocumentId documentId = Document.Solution().GetDocumentId(MethodDeclaration.SyntaxTree);

                newDocument = await newDocument.Solution().GetDocument(documentId).RemoveMemberAsync(MethodDeclaration, CancellationToken).ConfigureAwait(false);

                return newDocument.Solution();
            }
        }

        private ExpressionSyntax RewriteExpression(InvocationExpressionSyntax invocation, ExpressionSyntax expression)
        {
            return RewriteNode(expression)
                .WithoutTrivia()
                .Parenthesize()
                .WithTriviaFrom(invocation)
                .WithSimplifierAnnotation()
                .WithFormatterAnnotation();
        }
    }
}
