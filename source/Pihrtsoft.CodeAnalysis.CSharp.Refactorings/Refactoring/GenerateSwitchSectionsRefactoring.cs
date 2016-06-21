﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class GenerateSwitchSectionsRefactoring
    {
        public static async Task<bool> CanRefactorAsync(
            RefactoringContext context,
            SwitchStatementSyntax switchStatement)
        {
            if (switchStatement.Expression != null
                && IsEmptyOrContainsOnlyDefaultSection(switchStatement))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync();

                var namedTypeSymbol = semanticModel
                    .GetTypeInfo(switchStatement.Expression, context.CancellationToken)
                    .ConvertedType as INamedTypeSymbol;

                if (namedTypeSymbol?.TypeKind == TypeKind.Enum)
                {
                    foreach (ISymbol memberSymbol in namedTypeSymbol.GetMembers())
                    {
                        if (memberSymbol.Kind == SymbolKind.Field)
                            return true;
                    }
                }
            }

            return false;
        }

        private static bool IsEmptyOrContainsOnlyDefaultSection(SwitchStatementSyntax switchStatement)
        {
            if (switchStatement.Sections.Count == 0)
                return true;

            if (switchStatement.Sections.Count == 1)
            {
                SwitchSectionSyntax section = switchStatement.Sections[0];

                return section.Labels.Count == 1
                    && section.Labels[0].IsKind(SyntaxKind.DefaultSwitchLabel);
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            SwitchStatementSyntax switchStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken);

            var enumTypeSymbol = semanticModel
                .GetTypeInfo(switchStatement.Expression, cancellationToken)
                .ConvertedType as INamedTypeSymbol;

            SwitchStatementSyntax newNode = switchStatement
                .WithSections(List(CreateSwitchSections(enumTypeSymbol)))
                .WithAdditionalAnnotations(Formatter.Annotation);

            root = root.ReplaceNode(switchStatement, newNode);

            return document.WithSyntaxRoot(root);
        }

        private static IEnumerable<SwitchSectionSyntax> CreateSwitchSections(INamedTypeSymbol enumTypeSymbol)
        {
            foreach (ISymbol memberSymbol in enumTypeSymbol.GetMembers())
            {
                if (memberSymbol.Kind == SymbolKind.Field)
                {
                    yield return SwitchSection(
                        SingletonList<SwitchLabelSyntax>(
                            CaseSwitchLabel(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    TypeSyntaxRefactoring.CreateTypeSyntax(enumTypeSymbol)
                                        .WithAdditionalAnnotations(Simplifier.Annotation),
                                    (SimpleNameSyntax)ParseName(memberSymbol.Name)))),
                        SingletonList<StatementSyntax>(BreakStatement()));
                }
            }

            yield return SwitchSection(
                SingletonList<SwitchLabelSyntax>(
                    DefaultSwitchLabel()),
                SingletonList<StatementSyntax>(BreakStatement()));
        }
    }
}
