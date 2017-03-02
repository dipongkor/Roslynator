﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RedundantEmptyLineDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.RemoveRedundantEmptyLine); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(f => AnalyzeClassDeclaration(f), SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeStructDeclaration(f), SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeInterfaceDeclaration(f), SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeNamespaceDeclaration(f), SyntaxKind.NamespaceDeclaration);
            context.RegisterSyntaxNodeAction(f => AnalyzeSwitchStatement(f), SyntaxKind.SwitchStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeTryStatement(f), SyntaxKind.TryStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeElseClause(f), SyntaxKind.ElseClause);

            context.RegisterSyntaxNodeAction(f => AnalyzeIfStatement(f), SyntaxKind.IfStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeCommonForEachStatement(f), SyntaxKind.ForEachStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeCommonForEachStatement(f), SyntaxKind.ForEachVariableStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeForStatement(f), SyntaxKind.ForStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeUsingStatement(f), SyntaxKind.UsingStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeWhileStatement(f), SyntaxKind.WhileStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeDoStatement(f), SyntaxKind.DoStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeLockStatement(f), SyntaxKind.LockStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeFixedStatement(f), SyntaxKind.FixedStatement);
        }

        public void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantEmptyLineRefactoring.Analyze(context, (ClassDeclarationSyntax)context.Node);
        }

        private void AnalyzeStructDeclaration(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantEmptyLineRefactoring.Analyze(context, (StructDeclarationSyntax)context.Node);
        }

        private void AnalyzeInterfaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantEmptyLineRefactoring.Analyze(context, (InterfaceDeclarationSyntax)context.Node);
        }

        private void AnalyzeNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantEmptyLineRefactoring.Analyze(context, (NamespaceDeclarationSyntax)context.Node);
        }

        private void AnalyzeSwitchStatement(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantEmptyLineRefactoring.Analyze(context, (SwitchStatementSyntax)context.Node);
        }

        private void AnalyzeTryStatement(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantEmptyLineRefactoring.Analyze(context, (TryStatementSyntax)context.Node);
        }

        private void AnalyzeElseClause(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantEmptyLineRefactoring.Analyze(context, (ElseClauseSyntax)context.Node);
        }

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantEmptyLineRefactoring.Analyze(context, (IfStatementSyntax)context.Node);
        }

        private void AnalyzeCommonForEachStatement(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantEmptyLineRefactoring.Analyze(context, (CommonForEachStatementSyntax)context.Node);
        }

        private void AnalyzeForStatement(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantEmptyLineRefactoring.Analyze(context, (ForStatementSyntax)context.Node);
        }

        private void AnalyzeUsingStatement(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantEmptyLineRefactoring.Analyze(context, (UsingStatementSyntax)context.Node);
        }

        private void AnalyzeWhileStatement(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantEmptyLineRefactoring.Analyze(context, (WhileStatementSyntax)context.Node);
        }

        private void AnalyzeDoStatement(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantEmptyLineRefactoring.Analyze(context, (DoStatementSyntax)context.Node);
        }

        private void AnalyzeLockStatement(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantEmptyLineRefactoring.Analyze(context, (LockStatementSyntax)context.Node);
        }

        private void AnalyzeFixedStatement(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantEmptyLineRefactoring.Analyze(context, (FixedStatementSyntax)context.Node);
        }
    }
}
