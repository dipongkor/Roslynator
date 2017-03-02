﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Extensions
{
    public static class DocumentExtensions
    {
        public static Task<IEnumerable<ReferencedSymbol>> FindSymbolReferencesAsync(
            this Document document,
            ISymbol symbol,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            return SymbolFinder.FindReferencesAsync(
                symbol,
                document.Project.Solution,
                ImmutableHashSet.Create(document),
                cancellationToken);
        }

        public static async Task<ImmutableArray<SyntaxNode>> FindSymbolNodesAsync(
            this Document document,
            ISymbol symbol,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            List<SyntaxNode> nodes = null;

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            IEnumerable<ReferencedSymbol> referencedSymbols = await document.FindSymbolReferencesAsync(symbol, cancellationToken).ConfigureAwait(false);

            foreach (Location location in referencedSymbols
                .SelectMany(f => f.Locations)
                .Select(f => f.Location)
                .Where(f => f.IsInSource))
            {
                SyntaxNode node = root.FindNode(location.SourceSpan, findInsideTrivia: true, getInnermostNodeForTie: true);

                Debug.Assert(node != null);

                if (node != null)
                {
                    if (nodes == null)
                        nodes = new List<SyntaxNode>();

                    nodes.Add(node);
                }
            }

            if (nodes != null)
            {
                return ImmutableArray.CreateRange(nodes);
            }
            else
            {
                return ImmutableArray<SyntaxNode>.Empty;
            }
        }

        public static async Task<Document> WithTextChangeAsync(
            this Document document,
            TextChange textChange,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            SourceText newSourceText = sourceText.WithChanges(new TextChange[] { textChange });

            return document.WithText(newSourceText);
        }

        public static async Task<Document> WithTextChangesAsync(
            this Document document,
            TextChange[] textChanges,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (textChanges == null)
                throw new ArgumentNullException(nameof(textChanges));

            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            SourceText newSourceText = sourceText.WithChanges(textChanges);

            return document.WithText(newSourceText);
        }

        public static async Task<Document> WithTextChangesAsync(
            this Document document,
            IEnumerable<TextChange> textChanges,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (textChanges == null)
                throw new ArgumentNullException(nameof(textChanges));

            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            SourceText newSourceText = sourceText.WithChanges(textChanges);

            return document.WithText(newSourceText);
        }

        public static Task<Solution> RemoveFromSolutionAsync(this Document document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            var tcs = new TaskCompletionSource<Solution>();

            Solution newSolution = document.Project.Solution.RemoveDocument(document.Id);

            tcs.SetResult(newSolution);

            return tcs.Task;
        }

        public static async Task<Document> ReplaceNodeAsync(
            this Document document,
            SyntaxNode oldNode,
            SyntaxNode newNode,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (oldNode == null)
                throw new ArgumentNullException(nameof(oldNode));

            if (newNode == null)
                throw new ArgumentNullException(nameof(newNode));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = root.ReplaceNode(oldNode, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> ReplaceNodeAsync(
            this Document document,
            SyntaxNode oldNode,
            IEnumerable<SyntaxNode> newNodes,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (oldNode == null)
                throw new ArgumentNullException(nameof(oldNode));

            if (newNodes == null)
                throw new ArgumentNullException(nameof(newNodes));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = root.ReplaceNode(oldNode, newNodes);

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> ReplaceTokenAsync(
            this Document document,
            SyntaxToken oldToken,
            SyntaxToken newToken,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = root.ReplaceToken(oldToken, newToken);

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> ReplaceTokenAsync(
            this Document document,
            SyntaxToken oldToken,
            IEnumerable<SyntaxToken> newTokens,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (newTokens == null)
                throw new ArgumentNullException(nameof(newTokens));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = root.ReplaceToken(oldToken, newTokens);

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> ReplaceTriviaAsync(
            this Document document,
            SyntaxTrivia oldTrivia,
            SyntaxTrivia newTrivia,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = root.ReplaceTrivia(oldTrivia, newTrivia);

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> ReplaceTriviaAsync(
            this Document document,
            SyntaxTrivia oldTrivia,
            IEnumerable<SyntaxTrivia> newTrivia,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (newTrivia == null)
                throw new ArgumentNullException(nameof(newTrivia));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = root.ReplaceTrivia(oldTrivia, newTrivia);

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> InsertNodeBeforeAsync(
            this Document document,
            SyntaxNode nodeInList,
            SyntaxNode newNode,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (newNode == null)
                throw new ArgumentNullException(nameof(newNode));

            return await InsertNodesBeforeAsync(document, nodeInList, new SyntaxNode[] { newNode }, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<Document> InsertNodesBeforeAsync(
            this Document document,
            SyntaxNode nodeInList,
            IEnumerable<SyntaxNode> newNodes,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (nodeInList == null)
                throw new ArgumentNullException(nameof(nodeInList));

            if (newNodes == null)
                throw new ArgumentNullException(nameof(newNodes));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = root.InsertNodesBefore(nodeInList, newNodes);

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> InsertNodeAfterAsync(
            this Document document,
            SyntaxNode nodeInList,
            SyntaxNode newNode,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (newNode == null)
                throw new ArgumentNullException(nameof(newNode));

            return await InsertNodesAfterAsync(document, nodeInList, new SyntaxNode[] { newNode }, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<Document> InsertNodesAfterAsync(
            this Document document,
            SyntaxNode nodeInList,
            IEnumerable<SyntaxNode> newNodes,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (nodeInList == null)
                throw new ArgumentNullException(nameof(nodeInList));

            if (newNodes == null)
                throw new ArgumentNullException(nameof(newNodes));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = root.InsertNodesAfter(nodeInList, newNodes);

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> RemoveNodeAsync(
            this Document document,
            SyntaxNode node,
            SyntaxRemoveOptions options,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (node == null)
                throw new ArgumentNullException(nameof(node));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = root.RemoveNode(node, options);

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> RemoveNodesAsync(
            this Document document,
            IEnumerable<SyntaxNode> nodes,
            SyntaxRemoveOptions options,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (nodes == null)
                throw new ArgumentNullException(nameof(nodes));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = root.RemoveNodes(nodes, options);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
