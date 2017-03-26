﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings.WrapStatements
{
    internal abstract class WrapStatementsRefactoring<TStatement> where TStatement : StatementSyntax
    {
        public abstract TStatement CreateStatement(ImmutableArray<StatementSyntax> statements);

        public Task<Document> RefactorAsync(
            Document document,
            StatementContainerSlice slice,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            IStatementContainer container = slice.Container;

            StatementSyntax[] statements = slice.ToArray();

            int index = slice.StartIndex;

            SyntaxTriviaList leadingTrivia = statements[0].GetLeadingTrivia();
            SyntaxTriviaList trailingTrivia = statements[statements.Length - 1].GetTrailingTrivia();

            statements[0] = statements[0].WithLeadingTrivia();
            statements[statements.Length - 1] = statements[statements.Length - 1].WithTrailingTrivia();

            SyntaxList<StatementSyntax> newStatements = container.Statements;

            int cnt = statements.Length;

            while (cnt > 0)
            {
                newStatements = newStatements.RemoveAt(index);
                cnt--;
            }

            TStatement statement = CreateStatement(statements.ToImmutableArray());

            statement = statement
                .WithLeadingTrivia(leadingTrivia)
                .WithTrailingTrivia(trailingTrivia)
                .WithFormatterAnnotation();

            newStatements = newStatements.Insert(index, statement);

            return document.ReplaceNodeAsync(container.Node, container.NodeWithStatements(newStatements), cancellationToken);
        }
    }
}
