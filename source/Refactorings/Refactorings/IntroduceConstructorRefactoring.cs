﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
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
    internal static class IntroduceConstructorRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, MemberDeclarationSyntax declaration)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.IntroduceConstructor))
            {
                List<MemberDeclarationSyntax> members = await GetAssignableMembersAsync(context, declaration).ConfigureAwait(false);

                if (members?.Count > 0)
                {
                    context.RegisterRefactoring(
                        "Introduce constructor",
                        cancellationToken => RefactorAsync(context.Document, declaration, members, cancellationToken));
                }
            }
        }

        private static async Task<List<MemberDeclarationSyntax>> GetAssignableMembersAsync(
            RefactoringContext context,
            MemberDeclarationSyntax declaration)
        {
            if (declaration.IsKind(SyntaxKind.PropertyDeclaration, SyntaxKind.FieldDeclaration))
            {
                if (await CanBeAssignedFromConstructorAsync(context, declaration).ConfigureAwait(false))
                {
                    return new List<MemberDeclarationSyntax>()
                    {
                        declaration
                    };
                }
            }
            else if (declaration.IsKind(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration))
            {
                List<MemberDeclarationSyntax> list = null;

                foreach (MemberDeclarationSyntax member in declaration.GetMembers())
                {
                    if (await CanBeAssignedFromConstructorAsync(context, member).ConfigureAwait(false))
                    {
                        if (list == null)
                            list = new List<MemberDeclarationSyntax>();

                        list.Add(member);
                    }
                }

                return list;
            }

            return null;
        }

        private static async Task<bool> CanBeAssignedFromConstructorAsync(
            RefactoringContext context,
            MemberDeclarationSyntax declaration)
        {
            if (context.Span.Contains(declaration.Span))
            {
                switch (declaration.Kind())
                {
                    case SyntaxKind.PropertyDeclaration:
                        return await CanPropertyBeAssignedFromConstructorAsync(context, (PropertyDeclarationSyntax)declaration).ConfigureAwait(false);
                    case SyntaxKind.FieldDeclaration:
                        return await CanFieldBeAssignedFromConstructorAsync(context, (FieldDeclarationSyntax)declaration).ConfigureAwait(false);
                }
            }

            return false;
        }

        private static async Task<bool> CanPropertyBeAssignedFromConstructorAsync(
            RefactoringContext context,
            PropertyDeclarationSyntax propertyDeclaration)
        {
            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            ISymbol symbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, context.CancellationToken);

            if (symbol != null
                && !symbol.IsStatic
                && propertyDeclaration.Parent != null
                && propertyDeclaration.Parent.IsKind(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration))
            {
                if (propertyDeclaration.ExpressionBody != null)
                {
                    ExpressionSyntax expression = propertyDeclaration.ExpressionBody?.Expression;

                    if (expression != null)
                        return GetBackingFieldSymbol(expression, semanticModel, context.CancellationToken) != null;
                }
                else
                {
                    AccessorDeclarationSyntax getter = propertyDeclaration.Getter();

                    if (getter != null)
                    {
                        if (getter.Body == null)
                            return true;

                        if (getter.Body.Statements.Count == 1)
                            return GetBackingFieldSymbol(getter.Body.Statements[0], semanticModel, context.CancellationToken) != null;
                    }
                }
            }

            return false;
        }

        private static async Task<bool> CanFieldBeAssignedFromConstructorAsync(
            RefactoringContext context,
            FieldDeclarationSyntax fieldDeclaration)
        {
            VariableDeclaratorSyntax variable = fieldDeclaration.Declaration?.SingleVariableOrDefault();

            if (variable != null)
            {
                MemberDeclarationSyntax parentMember = GetContainingMember(fieldDeclaration);

                if (parentMember != null)
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    ISymbol symbol = semanticModel.GetDeclaredSymbol(variable, context.CancellationToken);

                    return symbol != null
                        && !symbol.IsStatic
                        && !parentMember
                            .GetMembers()
                            .Any(member => IsBackingField(member, symbol, context, semanticModel));
                }
            }

            return false;
        }

        private static bool IsBackingField(
            MemberDeclarationSyntax member,
            ISymbol symbol,
            RefactoringContext context,
            SemanticModel semanticModel)
        {
            if (member.IsKind(SyntaxKind.PropertyDeclaration)
                && context.Span.Contains(member.Span))
            {
                var propertyDeclaration = (PropertyDeclarationSyntax)member;

                if (propertyDeclaration.ExpressionBody?.Expression != null)
                {
                    ISymbol symbol2 = GetBackingFieldSymbol(propertyDeclaration.ExpressionBody.Expression, semanticModel, context.CancellationToken);

                    if (symbol.Equals(symbol2))
                        return true;
                }
                else
                {
                    AccessorDeclarationSyntax getter = propertyDeclaration.Getter();

                    if (getter?.Body?.Statements.Count == 1)
                    {
                        ISymbol symbol2 = GetBackingFieldSymbol(getter.Body.Statements[0], semanticModel, context.CancellationToken);

                        if (symbol.Equals(symbol2))
                            return true;
                    }
                }
            }

            return false;
        }

        private static ISymbol GetBackingFieldSymbol(
            StatementSyntax statement,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (statement.IsKind(SyntaxKind.ReturnStatement))
            {
                var returnStatement = (ReturnStatementSyntax)statement;

                if (returnStatement.Expression != null)
                    return GetBackingFieldSymbol(returnStatement.Expression, semanticModel, cancellationToken);
            }

            return null;
        }

        private static ISymbol GetBackingFieldSymbol(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ISymbol symbol = semanticModel.GetSymbol(expression, cancellationToken);

            if (symbol?.IsStatic == false
                && symbol.IsField())
            {
                return symbol;
            }
            else
            {
                return null;
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax declaration,
            List<MemberDeclarationSyntax> assignableMembers,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            MemberDeclarationSyntax parentMember = GetContainingMember(declaration);

            SyntaxList<MemberDeclarationSyntax> members = parentMember.GetMembers();

            SyntaxList<MemberDeclarationSyntax> newMembers = Inserter.InsertMember(
                members,
                CreateConstructor(GetConstructorIdentifierText(parentMember), assignableMembers));

            MemberDeclarationSyntax newNode = parentMember.SetMembers(newMembers)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(parentMember, newNode, cancellationToken).ConfigureAwait(false);
        }

        private static string GetConstructorIdentifierText(MemberDeclarationSyntax declaration)
        {
            switch (declaration.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)declaration).Identifier.Text;
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)declaration).Identifier.Text;
            }

            return null;
        }

        private static MemberDeclarationSyntax GetContainingMember(MemberDeclarationSyntax declaration)
        {
            switch (declaration.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.StructDeclaration:
                    return declaration;
                default:
                    {
                        Debug.Assert(declaration.Parent is MemberDeclarationSyntax, "Parent is not MemberDeclarationSyntax");
                        return declaration.Parent as MemberDeclarationSyntax;
                    }
            }
        }

        private static ConstructorDeclarationSyntax CreateConstructor(string identifierText, IEnumerable<MemberDeclarationSyntax> members)
        {
            var parameters = new List<ParameterSyntax>();
            var statements = new List<ExpressionStatementSyntax>();

            foreach (MemberDeclarationSyntax member in members)
            {
                string name = GetIdentifier(member).ValueText;
                string parameterName = Identifier.ToCamelCase(name);

                statements.Add(ExpressionStatement(
                    SimpleAssignmentExpression(
                        IdentifierName(name),
                        IdentifierName(parameterName))));

                parameters.Add(Parameter(
                    default(SyntaxList<AttributeListSyntax>),
                    default(SyntaxTokenList),
                    GetType(member),
                    Identifier(parameterName),
                    default(EqualsValueClauseSyntax)));
            }

            return ConstructorDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                ModifierFactory.Public(),
                Identifier(identifierText),
                ParameterList(SeparatedList(parameters)),
                default(ConstructorInitializerSyntax),
                Block(statements));
        }

        private static TypeSyntax GetType(MemberDeclarationSyntax memberDeclaration)
        {
            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)memberDeclaration).Type;
                case SyntaxKind.FieldDeclaration:
                    return ((FieldDeclarationSyntax)memberDeclaration).Declaration.Type;
            }

            return null;
        }

        private static SyntaxToken GetIdentifier(MemberDeclarationSyntax memberDeclaration)
        {
            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.PropertyDeclaration:
                    return GetPropertyIdentifier((PropertyDeclarationSyntax)memberDeclaration);
                case SyntaxKind.FieldDeclaration:
                    return ((FieldDeclarationSyntax)memberDeclaration).Declaration.Variables[0].Identifier;
            }

            return default(SyntaxToken);
        }

        private static SyntaxToken GetPropertyIdentifier(PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration.ExpressionBody != null)
            {
                ExpressionSyntax expression = propertyDeclaration.ExpressionBody?.Expression;

                if (expression?.IsKind(SyntaxKind.IdentifierName) == true)
                    return ((IdentifierNameSyntax)expression).Identifier;
            }
            else
            {
                AccessorDeclarationSyntax getter = propertyDeclaration.Getter();

                if (getter?.Body != null)
                {
                    var returnStatement = (ReturnStatementSyntax)getter.Body.Statements[0];

                    return ((IdentifierNameSyntax)returnStatement.Expression).Identifier;
                }
            }

            return propertyDeclaration.Identifier;
        }
    }
}
