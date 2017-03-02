﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddBracesToSwitchSectionRefactoring
    {
        public const string Title = "Add braces to section";

        public static bool CanAddBraces(SwitchSectionSyntax section)
        {
            SyntaxList<StatementSyntax> statements = section.Statements;

            if (statements.Count > 1)
            {
                return true;
            }
            else if (statements.Count == 1 && !statements[0].IsKind(SyntaxKind.Block))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            SwitchSectionSyntax switchSection,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SwitchSectionSyntax newNode = switchSection
                .WithStatements(
                    List<StatementSyntax>(
                        SingletonList(
                            Block(switchSection.Statements))))
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(switchSection, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
