﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseAutoPropertyRefactoring
    {
        private static readonly SyntaxAnnotation _removeAnnotation = new SyntaxAnnotation();

        private static SymbolDisplayFormat _symbolDisplayFormat { get; } = new SymbolDisplayFormat(
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers);

        public static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var property = (PropertyDeclarationSyntax)context.Node;

            if (!property.ContainsDiagnostics)
            {
                IFieldSymbol fieldSymbol = GetBackingFieldSymbol(property, context.SemanticModel, context.CancellationToken);

                if (fieldSymbol != null)
                {
                    var variableDeclarator = (VariableDeclaratorSyntax)fieldSymbol.GetSyntax(context.CancellationToken);

                    if (variableDeclarator.SyntaxTree == property.SyntaxTree)
                    {
                        IPropertySymbol propertySymbol = context.SemanticModel.GetDeclaredSymbol(property, context.CancellationToken);

                        if (propertySymbol?.ExplicitInterfaceImplementations.IsDefaultOrEmpty == true
                            && propertySymbol.IsStatic == fieldSymbol.IsStatic
                            && propertySymbol.Type.Equals(fieldSymbol.Type)
                            && propertySymbol.ContainingType?.Equals(fieldSymbol.ContainingType) == true
                            && !HasStructLayoutAttributeWithExplicitKind(propertySymbol.ContainingType, context.Compilation)
                            && CheckPreprocessorDirectives(property, variableDeclarator))
                        {
                            context.ReportDiagnostic(DiagnosticDescriptors.UseAutoProperty, property.Identifier);

                            if (property.ExpressionBody != null)
                            {
                                context.ReportNode(DiagnosticDescriptors.UseAutoPropertyFadeOut, property.ExpressionBody);
                            }
                            else
                            {
                                AccessorDeclarationSyntax getter = property.Getter();

                                if (getter != null)
                                    FadeOutBodyOrExpressionBody(context, getter);

                                AccessorDeclarationSyntax setter = property.Setter();

                                if (setter != null)
                                    FadeOutBodyOrExpressionBody(context, setter);
                            }
                        }
                    }
                }
            }
        }

        private static bool HasStructLayoutAttributeWithExplicitKind(INamedTypeSymbol typeSymbol, Compilation compilation)
        {
            AttributeData attribute = typeSymbol.GetAttributeByMetadataName(MetadataNames.System_Runtime_InteropServices_StructLayoutAttribute, compilation);

            if (attribute != null)
            {
                ImmutableArray<TypedConstant> constructorArguments = attribute.ConstructorArguments;

                if (constructorArguments.Length == 1)
                {
                    TypedConstant typedConstant = constructorArguments[0];

                    return typedConstant.Type?.Equals(compilation.GetTypeByMetadataName(MetadataNames.System_Runtime_InteropServices_LayoutKind)) == true
                        && (((LayoutKind)typedConstant.Value) == LayoutKind.Explicit);
                }
            }

            return false;
        }

        private static void FadeOutBodyOrExpressionBody(SyntaxNodeAnalysisContext context, AccessorDeclarationSyntax accessor)
        {
            BlockSyntax body = accessor.Body;

            if (body != null)
            {
                StatementSyntax statement = body.Statements.First();

                switch (statement.Kind())
                {
                    case SyntaxKind.ReturnStatement:
                        {
                            context.ReportNode(DiagnosticDescriptors.UseAutoPropertyFadeOut, ((ReturnStatementSyntax)statement).Expression);
                            break;
                        }
                    case SyntaxKind.ExpressionStatement:
                        {
                            context.ReportNode(DiagnosticDescriptors.UseAutoPropertyFadeOut, ((ExpressionStatementSyntax)statement).Expression);
                            break;
                        }
                }

                context.ReportBraces(DiagnosticDescriptors.UseAutoPropertyFadeOut, body);
            }
            else
            {
                context.ReportNode(DiagnosticDescriptors.UseAutoPropertyFadeOut, accessor.ExpressionBody);
            }
        }

        private static IFieldSymbol GetBackingFieldSymbol(
            PropertyDeclarationSyntax property,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ArrowExpressionClauseSyntax expressionBody = property.ExpressionBody;

            if (expressionBody != null)
            {
                NameSyntax identifier = GetIdentifierFromExpression(expressionBody.Expression);

                if (identifier != null)
                    return GetBackingFieldSymbol(identifier, semanticModel, cancellationToken);
            }
            else
            {
                AccessorDeclarationSyntax getter = property.Getter();

                if (getter != null)
                {
                    AccessorDeclarationSyntax setter = property.Setter();

                    if (setter != null)
                    {
                        return GetBackingFieldSymbol(getter, setter, semanticModel, cancellationToken);
                    }
                    else
                    {
                        NameSyntax identifier = GetIdentifierFromGetter(getter);

                        if (identifier != null)
                            return GetBackingFieldSymbol(identifier, semanticModel, cancellationToken);
                    }
                }
            }

            return null;
        }

        private static IFieldSymbol GetBackingFieldSymbol(
            NameSyntax identifier,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ISymbol symbol = semanticModel.GetSymbol(identifier, cancellationToken);

            if (symbol.IsPrivate()
                && symbol.IsField())
            {
                var fieldSymbol = (IFieldSymbol)symbol;

                if (fieldSymbol.IsReadOnly
                    && !fieldSymbol.IsVolatile)
                {
                    return fieldSymbol;
                }
            }

            return null;
        }

        private static IFieldSymbol GetBackingFieldSymbol(
            AccessorDeclarationSyntax getter,
            AccessorDeclarationSyntax setter,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            NameSyntax getterIdentifier = GetIdentifierFromGetter(getter);

            if (getterIdentifier != null)
            {
                NameSyntax setterIdentifier = GetIdentifierFromSetter(setter);

                if (setterIdentifier != null)
                {
                    ISymbol symbol = semanticModel.GetSymbol(getterIdentifier, cancellationToken);

                    if (symbol?.IsPrivate() == true
                        && symbol.IsField())
                    {
                        var fieldSymbol = (IFieldSymbol)symbol;

                        if (!fieldSymbol.IsVolatile)
                        {
                            ISymbol symbol2 = semanticModel.GetSymbol(setterIdentifier, cancellationToken);

                            if (fieldSymbol.Equals(symbol2))
                                return fieldSymbol;
                        }
                    }
                }
            }

            return null;
        }

        private static NameSyntax GetIdentifierFromGetter(AccessorDeclarationSyntax getter)
        {
            if (getter != null)
            {
                BlockSyntax body = getter.Body;

                if (body != null)
                {
                    SyntaxList<StatementSyntax> statements = body.Statements;

                    if (statements.Count == 1
                        && statements[0].IsKind(SyntaxKind.ReturnStatement))
                    {
                        var returnStatement = (ReturnStatementSyntax)statements[0];

                        return GetIdentifierFromExpression(returnStatement.Expression);
                    }
                }
                else
                {
                    return GetIdentifierFromExpression(getter.ExpressionBody?.Expression);
                }
            }

            return null;
        }

        private static NameSyntax GetIdentifierFromSetter(AccessorDeclarationSyntax setter)
        {
            if (setter != null)
            {
                BlockSyntax body = setter.Body;

                if (body != null)
                {
                    SyntaxList<StatementSyntax> statements = body.Statements;

                    if (statements.Count == 1
                        && statements[0].IsKind(SyntaxKind.ExpressionStatement))
                    {
                        var statement = (ExpressionStatementSyntax)statements[0];
                        ExpressionSyntax expression = statement.Expression;

                        return GetIdentifierFromSetterExpression(expression);
                    }
                }
                else
                {
                    return GetIdentifierFromSetterExpression(setter.ExpressionBody.Expression);
                }
            }

            return null;
        }

        private static NameSyntax GetIdentifierFromSetterExpression(ExpressionSyntax expression)
        {
            if (expression?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true)
            {
                var assignment = (AssignmentExpressionSyntax)expression;
                ExpressionSyntax right = assignment.Right;

                if (right?.IsKind(SyntaxKind.IdentifierName) == true
                    && ((IdentifierNameSyntax)right).Identifier.ValueText == "value")
                {
                    return GetIdentifierFromExpression(assignment.Left);
                }
            }

            return null;
        }

        private static SimpleNameSyntax GetIdentifierFromExpression(ExpressionSyntax expression)
        {
            switch (expression?.Kind())
            {
                case SyntaxKind.IdentifierName:
                    {
                        return (IdentifierNameSyntax)expression;
                    }
                case SyntaxKind.SimpleMemberAccessExpression:
                    {
                        var memberAccess = (MemberAccessExpressionSyntax)expression;

                        if (memberAccess.Expression?.IsKind(SyntaxKind.ThisExpression) == true)
                            return memberAccess.Name;

                        break;
                    }
            }

            return null;
        }

        private static bool CheckPreprocessorDirectives(
            PropertyDeclarationSyntax property,
            VariableDeclaratorSyntax declarator)
        {
            ArrowExpressionClauseSyntax expressionBody = property.ExpressionBody;

            if (expressionBody != null)
            {
                if (expressionBody.SpanContainsDirectives())
                    return false;
            }
            else if (property.AccessorList.Accessors.Any(f => f.SpanContainsDirectives()))
            {
                return false;
            }

            var variableDeclaration = (VariableDeclarationSyntax)declarator.Parent;

            if (variableDeclaration.Variables.Count == 1)
            {
                if (variableDeclaration.Parent.SpanContainsDirectives())
                    return false;
            }
            else if (declarator.SpanContainsDirectives())
            {
                return false;
            }

            return true;
        }

        public static async Task<Solution> RefactorAsync(
            Document document,
            PropertyDeclarationSyntax propertyDeclaration,
            CancellationToken cancellationToken)
        {
            Solution solution = document.Solution();

            SyntaxToken propertyIdentifier = propertyDeclaration.Identifier.WithoutTrivia();

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, cancellationToken);

            ISymbol fieldSymbol = GetFieldSymbol(propertyDeclaration, semanticModel, cancellationToken);

            var variableDeclarator = (VariableDeclaratorSyntax)await fieldSymbol.DeclaringSyntaxReferences[0].GetSyntaxAsync(cancellationToken).ConfigureAwait(false);

            var variableDeclaration = (VariableDeclarationSyntax)variableDeclarator.Parent;

            var fieldDeclaration = (FieldDeclarationSyntax)variableDeclaration.Parent;

            bool isSingleDeclarator = variableDeclaration.Variables.Count == 1;

            var newDocuments = new List<Document>();

            foreach (SyntaxTree syntaxTree in propertySymbol
                .ContainingType
                .DeclaringSyntaxReferences
                .Select(f => f.GetSyntax(cancellationToken).SyntaxTree)
                .Distinct())
            {
                document = solution.GetDocument(syntaxTree);

                semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

                ImmutableArray<SyntaxNode> nodes = await document.FindNodesAsync(fieldSymbol, cancellationToken: cancellationToken).ConfigureAwait(false);

                if (propertyDeclaration.SyntaxTree == semanticModel.SyntaxTree)
                {
                    nodes = nodes.Add(propertyDeclaration);

                    if (isSingleDeclarator)
                    {
                        nodes = nodes.Add(fieldDeclaration);
                    }
                    else
                    {
                        nodes = nodes.Add(variableDeclarator);
                    }
                }

                SyntaxNode newRoot = root.ReplaceNodes(nodes, (node, rewrittenNode) =>
                {
                    switch (node.Kind())
                    {
                        case SyntaxKind.IdentifierName:
                            {
                                return CreateNewExpression(node, propertyIdentifier, propertySymbol)
                                    .WithTriviaFrom(node)
                                    .WithFormatterAnnotation();
                            }
                        case SyntaxKind.PropertyDeclaration:
                            {
                                return CreateAutoProperty(propertyDeclaration, variableDeclarator.Initializer);
                            }
                        case SyntaxKind.VariableDeclarator:
                        case SyntaxKind.FieldDeclaration:
                            {
                                return node.WithAdditionalAnnotations(_removeAnnotation);
                            }
                        default:
                            {
                                Debug.Fail(node.ToString());
                                return node;
                            }
                    }
                });

                SyntaxNode nodeToRemove = newRoot.GetAnnotatedNodes(_removeAnnotation).FirstOrDefault();

                if (nodeToRemove != null)
                    newRoot = newRoot.RemoveNode(nodeToRemove, SyntaxRemoveOptions.KeepUnbalancedDirectives);

                newDocuments.Add(document.WithSyntaxRoot(newRoot));
            }

            foreach (Document newDocument in newDocuments)
                solution = solution.WithDocumentSyntaxRoot(newDocument.Id, await newDocument.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false));

            return solution;
        }

        private static ISymbol GetFieldSymbol(PropertyDeclarationSyntax property, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            ArrowExpressionClauseSyntax expressionBody = property.ExpressionBody;

            if (expressionBody != null)
            {
                return semanticModel.GetSymbol(expressionBody.Expression, cancellationToken);
            }
            else
            {
                AccessorDeclarationSyntax getter = property.Getter();

                BlockSyntax body = getter.Body;

                if (body != null)
                {
                    var returnStatement = (ReturnStatementSyntax)body.Statements[0];

                    return semanticModel.GetSymbol(returnStatement.Expression, cancellationToken);
                }
                else
                {
                    return semanticModel.GetSymbol(getter.ExpressionBody.Expression, cancellationToken);
                }
            }
        }

        public static SyntaxNode CreateNewExpression(SyntaxNode node, SyntaxToken identifier, IPropertySymbol propertySymbol)
        {
            if (node.IsParentKind(SyntaxKind.SimpleMemberAccessExpression)
                && ((MemberAccessExpressionSyntax)node.Parent).Name == node)
            {
                return IdentifierName(identifier);
            }
            else if (propertySymbol.IsStatic)
            {
                return ParseName($"{propertySymbol.ContainingType.ToTypeSyntax()}.{propertySymbol.ToDisplayString(_symbolDisplayFormat)}")
                    .WithSimplifierAnnotation();
            }
            else
            {
                return SimpleMemberAccessExpression(ThisExpression(), IdentifierName(identifier))
                    .WithSimplifierAnnotation();
            }
        }

        public static PropertyDeclarationSyntax CreateAutoProperty(PropertyDeclarationSyntax propertyDeclaration, EqualsValueClauseSyntax initializer)
        {
            AccessorListSyntax accessorList = CreateAccessorList(propertyDeclaration);

            if (accessorList
                .DescendantTrivia()
                .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                accessorList = accessorList.RemoveWhitespaceOrEndOfLineTrivia();
            }

            PropertyDeclarationSyntax newProperty = propertyDeclaration
                .WithIdentifier(propertyDeclaration.Identifier.WithTrailingTrivia(Space))
                .WithExpressionBody(null)
                .WithAccessorList(accessorList);

            if (initializer != null)
            {
                newProperty = newProperty
                    .WithInitializer(initializer)
                    .WithSemicolonToken(SemicolonToken());
            }
            else
            {
                newProperty = newProperty.WithoutSemicolonToken();
            }

            return newProperty
                .WithTriviaFrom(propertyDeclaration)
                .WithFormatterAnnotation();
        }

        private static AccessorListSyntax CreateAccessorList(PropertyDeclarationSyntax property)
        {
            if (property.ExpressionBody != null)
            {
                return AccessorList(AutoGetAccessorDeclaration())
                    .WithTriviaFrom(property.ExpressionBody);
            }
            else
            {
                AccessorListSyntax accessorList = property.AccessorList;

                IEnumerable<AccessorDeclarationSyntax> newAccessors = accessorList
                    .Accessors
                    .Select(accessor =>
                    {
                        return accessor
                            .WithBody(null)
                            .WithExpressionBody(null)
                            .WithSemicolonToken(SemicolonToken())
                            .WithTriviaFrom(accessor);
                    });

                return accessorList.WithAccessors(List(newAccessors));
            }
        }
    }
}
