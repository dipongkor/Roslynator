﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConstructorDeclarationCodeFixProvider))]
    [Shared]
    public class ConstructorDeclarationCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.RemoveRedundantBaseConstructorCall,
                    DiagnosticIdentifiers.RemoveRedundantConstructor,
                    DiagnosticIdentifiers.AbstractTypeShouldNotHavePublicConstructors);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document
                .GetSyntaxRootAsync(context.CancellationToken)
                .ConfigureAwait(false);

            ConstructorDeclarationSyntax constructor = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<ConstructorDeclarationSyntax>();

            if (constructor == null)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.RemoveRedundantBaseConstructorCall:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Remove redundant base constructor call",
                                cancellationToken => RemoveRedundantBaseConstructorCallRefactoring.RefactorAsync(context.Document, constructor, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.RemoveRedundantConstructor:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Remove redundant constructor",
                                cancellationToken => RemoveRedundantConstructorRefactoring.RefactorAsync(context.Document, constructor, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.AbstractTypeShouldNotHavePublicConstructors:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Change accessibility to 'protected'",
                                cancellationToken => AbstractTypeShouldNotHavePublicConstructorsRefactoring.RefactorAsync(context.Document, constructor, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
