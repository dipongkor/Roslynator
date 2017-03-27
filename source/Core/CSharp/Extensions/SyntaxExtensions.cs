﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Extensions
{
    public static class SyntaxExtensions
    {
        public static bool IsAutoGetter(this AccessorDeclarationSyntax accessorDeclaration)
        {
            return IsAutoAccessor(accessorDeclaration, SyntaxKind.GetAccessorDeclaration);
        }

        public static bool IsAutoSetter(this AccessorDeclarationSyntax accessorDeclaration)
        {
            return IsAutoAccessor(accessorDeclaration, SyntaxKind.SetAccessorDeclaration);
        }

        private static bool IsAutoAccessor(this AccessorDeclarationSyntax accessorDeclaration, SyntaxKind kind)
        {
            if (accessorDeclaration == null)
                throw new ArgumentNullException(nameof(accessorDeclaration));

            return accessorDeclaration.IsKind(kind)
                && IsAutoAccessor(accessorDeclaration);
        }

        private static bool IsAutoAccessor(this AccessorDeclarationSyntax accessorDeclaration)
        {
            return accessorDeclaration.SemicolonToken.IsKind(SyntaxKind.SemicolonToken)
                && accessorDeclaration.BodyOrExpressionBody() == null;
        }

        internal static AccessorDeclarationSyntax WithoutSemicolonToken(
            this AccessorDeclarationSyntax accessorDeclaration)
        {
            return accessorDeclaration.WithSemicolonToken(default(SyntaxToken));
        }

        public static CSharpSyntaxNode BodyOrExpressionBody(this AccessorDeclarationSyntax accessorDeclaration)
        {
            if (accessorDeclaration == null)
                throw new ArgumentNullException(nameof(accessorDeclaration));

            BlockSyntax body = accessorDeclaration.Body;

            return body ?? (CSharpSyntaxNode)accessorDeclaration.ExpressionBody;
        }

        public static AccessorDeclarationSyntax Getter(this AccessorListSyntax accessorList)
        {
            return Accessor(accessorList, SyntaxKind.GetAccessorDeclaration);
        }

        public static AccessorDeclarationSyntax Setter(this AccessorListSyntax accessorList)
        {
            return Accessor(accessorList, SyntaxKind.SetAccessorDeclaration);
        }

        private static AccessorDeclarationSyntax Accessor(this AccessorListSyntax accessorList, SyntaxKind kind)
        {
            if (accessorList == null)
                throw new ArgumentNullException(nameof(accessorList));

            return accessorList
                .Accessors
                .FirstOrDefault(accessor => accessor.IsKind(kind));
        }

        //TODO: SingleStatementOrDefault
        public static StatementSyntax SingleStatementOrDefault(this BlockSyntax body)
        {
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            SyntaxList<StatementSyntax> statements = body.Statements;

            return (statements.Count == 1)
                ? statements[0]
                : null;
        }

        public static TextSpan ParenthesesSpan(this CastExpressionSyntax castExpression)
        {
            if (castExpression == null)
                throw new ArgumentNullException(nameof(castExpression));

            return TextSpan.FromBounds(
                castExpression.OpenParenToken.Span.Start,
                castExpression.CloseParenToken.Span.End);
        }

        internal static ClassDeclarationSyntax WithMembers(
            this ClassDeclarationSyntax classDeclaration,
            MemberDeclarationSyntax memberDeclaration)
        {
            return classDeclaration.WithMembers(SingletonList(memberDeclaration));
        }

        internal static ClassDeclarationSyntax WithMembers(
            this ClassDeclarationSyntax classDeclaration,
            IEnumerable<MemberDeclarationSyntax> memberDeclarations)
        {
            return classDeclaration.WithMembers(List(memberDeclarations));
        }

        public static TextSpan HeaderSpan(this ClassDeclarationSyntax classDeclaration)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return TextSpan.FromBounds(
                classDeclaration.Span.Start,
                classDeclaration.Identifier.Span.End);
        }

        public static bool IsStatic(this ClassDeclarationSyntax classDeclaration)
        {
            return classDeclaration?.Modifiers.Contains(SyntaxKind.StaticKeyword) == true;
        }

        public static TextSpan BracesSpan(this ClassDeclarationSyntax classDeclaration)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return TextSpan.FromBounds(
                classDeclaration.OpenBraceToken.Span.Start,
                classDeclaration.CloseBraceToken.Span.End);
        }

        public static MemberDeclarationSyntax RemoveMemberAt(this ClassDeclarationSyntax classDeclaration, int index)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return RemoveMember(classDeclaration, classDeclaration.Members[index], index);
        }

        public static MemberDeclarationSyntax RemoveMember(this ClassDeclarationSyntax classDeclaration, MemberDeclarationSyntax member)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return RemoveMember(classDeclaration, member, classDeclaration.Members.IndexOf(member));
        }

        private static MemberDeclarationSyntax RemoveMember(
            ClassDeclarationSyntax classDeclaration,
            MemberDeclarationSyntax member,
            int index)
        {
            MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

            classDeclaration = classDeclaration
                .WithMembers(classDeclaration.Members.Replace(member, newMember));

            return classDeclaration
                .RemoveNode(classDeclaration.Members[index], Remover.GetRemoveOptions(newMember));
        }

        internal static CompilationUnitSyntax WithMembers(
            this CompilationUnitSyntax compilationUnit,
            MemberDeclarationSyntax memberDeclaration)
        {
            return compilationUnit.WithMembers(SingletonList(memberDeclaration));
        }

        internal static CompilationUnitSyntax WithMembers(
            this CompilationUnitSyntax compilationUnit,
            IEnumerable<MemberDeclarationSyntax> memberDeclarations)
        {
            return compilationUnit.WithMembers(List(memberDeclarations));
        }

        public static CompilationUnitSyntax AddUsings(this CompilationUnitSyntax compilationUnit, bool keepSingleLineCommentsOnTop, params UsingDirectiveSyntax[] usings)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            if (usings == null)
                throw new ArgumentNullException(nameof(usings));

            if (keepSingleLineCommentsOnTop
                && usings.Length > 0
                && !compilationUnit.Usings.Any())
            {
                SyntaxTriviaList leadingTrivia = compilationUnit.GetLeadingTrivia();

                SyntaxTrivia[] topTrivia = GetTopSingleLineComments(leadingTrivia).ToArray();

                if (topTrivia.Length > 0)
                {
                    compilationUnit = compilationUnit.WithoutLeadingTrivia();

                    usings[0] = usings[0].WithLeadingTrivia(topTrivia);

                    usings[usings.Length - 1] = usings[usings.Length - 1].WithTrailingTrivia(leadingTrivia.Skip(topTrivia.Length));
                }
            }

            return compilationUnit.AddUsings(usings);
        }

        private static IEnumerable<SyntaxTrivia> GetTopSingleLineComments(SyntaxTriviaList triviaList)
        {
            SyntaxTriviaList.Enumerator en = triviaList.GetEnumerator();

            while (en.MoveNext())
            {
                if (en.Current.IsKind(SyntaxKind.SingleLineCommentTrivia))
                {
                    SyntaxTrivia trivia = en.Current;

                    if (en.MoveNext() && en.Current.IsEndOfLineTrivia())
                    {
                        yield return trivia;
                        yield return en.Current;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        public static CompilationUnitSyntax RemoveMemberAt(this CompilationUnitSyntax compilationUnit, int index)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            return RemoveMember(compilationUnit, compilationUnit.Members[index], index);
        }

        public static CompilationUnitSyntax RemoveMember(this CompilationUnitSyntax compilationUnit, MemberDeclarationSyntax member)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return RemoveMember(compilationUnit, member, compilationUnit.Members.IndexOf(member));
        }

        private static CompilationUnitSyntax RemoveMember(
            CompilationUnitSyntax compilationUnit,
            MemberDeclarationSyntax member,
            int index)
        {
            MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

            compilationUnit = compilationUnit
                .WithMembers(compilationUnit.Members.Replace(member, newMember));

            return compilationUnit
                .RemoveNode(compilationUnit.Members[index], Remover.GetRemoveOptions(newMember));
        }

        public static TextSpan HeaderSpan(this ConstructorDeclarationSyntax constructorDeclaration)
        {
            if (constructorDeclaration == null)
                throw new ArgumentNullException(nameof(constructorDeclaration));

            return TextSpan.FromBounds(
                constructorDeclaration.Span.Start,
                constructorDeclaration.ParameterList?.Span.End ?? constructorDeclaration.Identifier.Span.End);
        }

        public static TextSpan HeaderSpanIncludingInitializer(this ConstructorDeclarationSyntax constructorDeclaration)
        {
            if (constructorDeclaration == null)
                throw new ArgumentNullException(nameof(constructorDeclaration));

            return TextSpan.FromBounds(
                constructorDeclaration.Span.Start,
                constructorDeclaration.Initializer?.Span.End
                    ?? constructorDeclaration.ParameterList?.Span.End
                    ?? constructorDeclaration.Identifier.Span.End);
        }

        public static bool IsStatic(this ConstructorDeclarationSyntax constructorDeclaration)
        {
            return constructorDeclaration?.Modifiers.Contains(SyntaxKind.StaticKeyword) == true;
        }

        public static CSharpSyntaxNode BodyOrExpressionBody(this ConstructorDeclarationSyntax constructorDeclaration)
        {
            if (constructorDeclaration == null)
                throw new ArgumentNullException(nameof(constructorDeclaration));

            BlockSyntax body = constructorDeclaration.Body;

            return body ?? (CSharpSyntaxNode)constructorDeclaration.ExpressionBody;
        }

        public static TextSpan HeaderSpan(this ConversionOperatorDeclarationSyntax operatorDeclaration)
        {
            if (operatorDeclaration == null)
                throw new ArgumentNullException(nameof(operatorDeclaration));

            return TextSpan.FromBounds(
                operatorDeclaration.Span.Start,
                operatorDeclaration.ParameterList?.Span.End
                    ?? operatorDeclaration.Type?.Span.End
                    ?? operatorDeclaration.OperatorKeyword.Span.End);
        }

        public static CSharpSyntaxNode BodyOrExpressionBody(this ConversionOperatorDeclarationSyntax conversionOperatorDeclaration)
        {
            if (conversionOperatorDeclaration == null)
                throw new ArgumentNullException(nameof(conversionOperatorDeclaration));

            BlockSyntax body = conversionOperatorDeclaration.Body;

            return body ?? (CSharpSyntaxNode)conversionOperatorDeclaration.ExpressionBody;
        }

        public static CSharpSyntaxNode BodyOrExpressionBody(this DestructorDeclarationSyntax destructorDeclaration)
        {
            if (destructorDeclaration == null)
                throw new ArgumentNullException(nameof(destructorDeclaration));

            BlockSyntax body = destructorDeclaration.Body;

            return body ?? (CSharpSyntaxNode)destructorDeclaration.ExpressionBody;
        }

        public static XmlElementSyntax SummaryElement(this DocumentationCommentTriviaSyntax documentationComment)
        {
            if (documentationComment == null)
                throw new ArgumentNullException(nameof(documentationComment));

            foreach (XmlNodeSyntax node in documentationComment.Content)
            {
                if (node.IsKind(SyntaxKind.XmlElement))
                {
                    var element = (XmlElementSyntax)node;

                    string name = element.StartTag?.Name?.LocalName.ValueText;

                    if (string.Equals(name, "summary", StringComparison.Ordinal))
                        return element;
                }
            }

            return null;
        }

        public static IEnumerable<XmlElementSyntax> Elements(this DocumentationCommentTriviaSyntax documentationComment, string localName)
        {
            if (documentationComment == null)
                throw new ArgumentNullException(nameof(documentationComment));

            foreach (XmlNodeSyntax node in documentationComment.Content)
            {
                if (node.IsKind(SyntaxKind.XmlElement))
                {
                    var xmlElement = (XmlElementSyntax)node;

                    XmlNameSyntax xmlName = xmlElement.StartTag?.Name;

                    if (xmlName != null
                        && string.Equals(xmlName.LocalName.ValueText, localName, StringComparison.Ordinal))
                    {
                        yield return xmlElement;
                    }
                }
            }
        }

        public static TextSpan BracesSpan(this EnumDeclarationSyntax enumDeclaration)
        {
            if (enumDeclaration == null)
                throw new ArgumentNullException(nameof(enumDeclaration));

            return TextSpan.FromBounds(
                enumDeclaration.OpenBraceToken.Span.Start,
                enumDeclaration.CloseBraceToken.Span.End);
        }

        public static TextSpan HeaderSpan(this EventDeclarationSyntax eventDeclaration)
        {
            if (eventDeclaration == null)
                throw new ArgumentNullException(nameof(eventDeclaration));

            return TextSpan.FromBounds(
                eventDeclaration.Span.Start,
                eventDeclaration.Identifier.Span.End);
        }

        public static bool IsStatic(this EventDeclarationSyntax eventDeclaration)
        {
            return eventDeclaration?.Modifiers.Contains(SyntaxKind.StaticKeyword) == true;
        }

        public static bool IsStatic(this EventFieldDeclarationSyntax eventFieldDeclaration)
        {
            return eventFieldDeclaration?.Modifiers.Contains(SyntaxKind.StaticKeyword) == true;
        }

        public static ParenthesizedExpressionSyntax Parenthesize(this ExpressionSyntax expression, bool moveTrivia = false)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            if (moveTrivia)
            {
                return ParenthesizedExpression(expression.WithoutTrivia())
                    .WithTriviaFrom(expression);
            }
            else
            {
                return ParenthesizedExpression(expression);
            }
        }

        public static ExpressionSyntax WalkDownParentheses(this ExpressionSyntax expression)
        {
            while (expression?.IsKind(SyntaxKind.ParenthesizedExpression) == true)
                expression = ((ParenthesizedExpressionSyntax)expression).Expression;

            return expression;
        }

        public static bool IsIncrementOrDecrementExpression(this ExpressionSyntax expression)
        {
            return expression?.IsKind(
                SyntaxKind.PreIncrementExpression,
                SyntaxKind.PreDecrementExpression,
                SyntaxKind.PostIncrementExpression,
                SyntaxKind.PostDecrementExpression) == true;
        }

        internal static bool SupportsCompoundAssignment(this ExpressionSyntax expression)
        {
            switch (expression?.Kind())
            {
                case SyntaxKind.AddExpression:
                case SyntaxKind.SubtractExpression:
                case SyntaxKind.MultiplyExpression:
                case SyntaxKind.DivideExpression:
                case SyntaxKind.ModuloExpression:
                case SyntaxKind.BitwiseAndExpression:
                case SyntaxKind.ExclusiveOrExpression:
                case SyntaxKind.BitwiseOrExpression:
                case SyntaxKind.LeftShiftExpression:
                case SyntaxKind.RightShiftExpression:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsConst(this FieldDeclarationSyntax fieldDeclaration)
        {
            return fieldDeclaration?.Modifiers.Contains(SyntaxKind.ConstKeyword) == true;
        }

        public static bool IsStatic(this FieldDeclarationSyntax fieldDeclaration)
        {
            return fieldDeclaration?.Modifiers.Contains(SyntaxKind.StaticKeyword) == true;
        }

        public static TextSpan ParenthesesSpan(this CommonForEachStatementSyntax forEachStatement)
        {
            if (forEachStatement == null)
                throw new ArgumentNullException(nameof(forEachStatement));

            return TextSpan.FromBounds(forEachStatement.OpenParenToken.Span.Start, forEachStatement.CloseParenToken.Span.End);
        }

        public static TextSpan ParenthesesSpan(this ForStatementSyntax forStatement)
        {
            if (forStatement == null)
                throw new ArgumentNullException(nameof(forStatement));

            return TextSpan.FromBounds(forStatement.OpenParenToken.Span.Start, forStatement.CloseParenToken.Span.End);
        }

        internal static StatementSyntax GetSingleStatementOrDefault(this IfStatementSyntax ifStatement)
        {
            return GetSingleStatementOrDefault(ifStatement.Statement);
        }

        public static bool IsSimpleIf(this IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            return !ifStatement.IsParentKind(SyntaxKind.ElseClause)
                && ifStatement.Else == null;
        }

        public static bool IsSimpleIfElse(this IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            return !ifStatement.IsParentKind(SyntaxKind.ElseClause)
                && ifStatement.Else?.Statement?.IsKind(SyntaxKind.IfStatement) == false;
        }

        internal static StatementSyntax GetSingleStatementOrDefault(this ElseClauseSyntax elseClause)
        {
            return GetSingleStatementOrDefault(elseClause.Statement);
        }

        private static StatementSyntax GetSingleStatementOrDefault(StatementSyntax statement)
        {
            if (statement?.IsKind(SyntaxKind.Block) == true)
            {
                return ((BlockSyntax)statement).SingleStatementOrDefault();
            }
            else
            {
                return statement;
            }
        }

        public static TextSpan HeaderSpan(this IndexerDeclarationSyntax indexerDeclaration)
        {
            if (indexerDeclaration == null)
                throw new ArgumentNullException(nameof(indexerDeclaration));

            return TextSpan.FromBounds(
                indexerDeclaration.Span.Start,
                indexerDeclaration.ParameterList?.Span.End ?? indexerDeclaration.ThisKeyword.Span.End);
        }

        public static AccessorDeclarationSyntax Getter(this IndexerDeclarationSyntax indexerDeclaration)
        {
            if (indexerDeclaration == null)
                throw new ArgumentNullException(nameof(indexerDeclaration));

            return indexerDeclaration
                .AccessorList?
                .Getter();
        }

        public static AccessorDeclarationSyntax Setter(this IndexerDeclarationSyntax indexerDeclaration)
        {
            if (indexerDeclaration == null)
                throw new ArgumentNullException(nameof(indexerDeclaration));

            return indexerDeclaration
                .AccessorList?
                .Setter();
        }

        public static TextSpan HeaderSpan(this InterfaceDeclarationSyntax interfaceDeclaration)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            return TextSpan.FromBounds(
                interfaceDeclaration.Span.Start,
                interfaceDeclaration.Identifier.Span.End);
        }

        public static TextSpan BracesSpan(this InterfaceDeclarationSyntax interfaceDeclaration)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            return TextSpan.FromBounds(
                interfaceDeclaration.OpenBraceToken.Span.Start,
                interfaceDeclaration.CloseBraceToken.Span.End);
        }

        public static MemberDeclarationSyntax RemoveMemberAt(this InterfaceDeclarationSyntax interfaceDeclaration, int index)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            return RemoveMember(interfaceDeclaration, interfaceDeclaration.Members[index], index);
        }

        public static MemberDeclarationSyntax RemoveMember(this InterfaceDeclarationSyntax interfaceDeclaration, MemberDeclarationSyntax member)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return RemoveMember(interfaceDeclaration, member, interfaceDeclaration.Members.IndexOf(member));
        }

        private static MemberDeclarationSyntax RemoveMember(
            InterfaceDeclarationSyntax interfaceDeclaration,
            MemberDeclarationSyntax member,
            int index)
        {
            MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

            interfaceDeclaration = interfaceDeclaration
                .WithMembers(interfaceDeclaration.Members.Replace(member, newMember));

            return interfaceDeclaration
                .RemoveNode(interfaceDeclaration.Members[index], Remover.GetRemoveOptions(newMember));
        }

        internal static InterfaceDeclarationSyntax WithMembers(
            this InterfaceDeclarationSyntax interfaceDeclaration,
            MemberDeclarationSyntax memberDeclaration)
        {
            return interfaceDeclaration.WithMembers(SingletonList(memberDeclaration));
        }

        internal static InterfaceDeclarationSyntax WithMembers(
            this InterfaceDeclarationSyntax interfaceDeclaration,
            IEnumerable<MemberDeclarationSyntax> memberDeclarations)
        {
            return interfaceDeclaration.WithMembers(List(memberDeclarations));
        }

        public static bool IsVerbatim(this InterpolatedStringExpressionSyntax interpolatedString)
        {
            if (interpolatedString == null)
                throw new ArgumentNullException(nameof(interpolatedString));

            return interpolatedString.StringStartToken.ValueText.Contains("@");
        }

        public static bool IsVerbatimStringLiteral(this LiteralExpressionSyntax literalExpression)
        {
            if (literalExpression == null)
                throw new ArgumentNullException(nameof(literalExpression));

            return literalExpression.IsKind(SyntaxKind.StringLiteralExpression)
                && literalExpression.Token.Text.StartsWith("@", StringComparison.Ordinal);
        }

        public static bool IsZeroNumericLiteral(this LiteralExpressionSyntax literalExpression)
        {
            if (literalExpression == null)
                throw new ArgumentNullException(nameof(literalExpression));

            return literalExpression.IsKind(SyntaxKind.NumericLiteralExpression)
                && string.Equals(literalExpression.Token.ValueText, "0", StringComparison.Ordinal);
        }

        internal static string GetStringLiteralInnerText(this LiteralExpressionSyntax literalExpression)
        {
            if (literalExpression == null)
                throw new ArgumentNullException(nameof(literalExpression));

            string s = literalExpression.Token.Text;

            if (s.StartsWith("@", StringComparison.Ordinal))
            {
                if (s.StartsWith("@\"", StringComparison.Ordinal))
                    s = s.Substring(2);

                if (s.EndsWith("\"", StringComparison.Ordinal))
                    s = s.Remove(s.Length - 1);
            }
            else
            {
                if (s.StartsWith("\"", StringComparison.Ordinal))
                    s = s.Substring(1);

                if (s.EndsWith("\"", StringComparison.Ordinal))
                    s = s.Remove(s.Length - 1);
            }

            return s;
        }

        public static bool IsHexadecimalNumericLiteral(this LiteralExpressionSyntax literalExpression)
        {
            if (literalExpression == null)
                throw new ArgumentNullException(nameof(literalExpression));

            return literalExpression.IsKind(SyntaxKind.NumericLiteralExpression)
                && literalExpression.Token.Text.StartsWith("0x", StringComparison.OrdinalIgnoreCase);
        }

        public static CSharpSyntaxNode BodyOrExpressionBody(this LocalFunctionStatementSyntax localFunctionStatement)
        {
            if (localFunctionStatement == null)
                throw new ArgumentNullException(nameof(localFunctionStatement));

            BlockSyntax body = localFunctionStatement.Body;

            return body ?? (CSharpSyntaxNode)localFunctionStatement.ExpressionBody;
        }

        public static SyntaxTrivia GetSingleLineDocumentationCommentTrivia(this MemberDeclarationSyntax memberDeclaration)
        {
            if (memberDeclaration == null)
                throw new ArgumentNullException(nameof(memberDeclaration));

            foreach (SyntaxTrivia trivia in memberDeclaration.GetLeadingTrivia())
            {
                if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                    return trivia;
            }

            return default(SyntaxTrivia);
        }

        public static DocumentationCommentTriviaSyntax GetSingleLineDocumentationComment(this MemberDeclarationSyntax memberDeclaration)
        {
            if (memberDeclaration == null)
                throw new ArgumentNullException(nameof(memberDeclaration));

            SyntaxTrivia trivia = memberDeclaration.GetSingleLineDocumentationCommentTrivia();

            if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
            {
                var comment = trivia.GetStructure() as DocumentationCommentTriviaSyntax;

                if (comment?.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) == true)
                    return comment;
            }

            return null;
        }

        public static bool HasSingleLineDocumentationComment(this MemberDeclarationSyntax memberDeclaration)
        {
            if (memberDeclaration == null)
                throw new ArgumentNullException(nameof(memberDeclaration));

            return memberDeclaration
                .GetLeadingTrivia()
                .Any(f => f.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia));
        }

        public static MemberDeclarationSyntax RemoveMemberAt(this MemberDeclarationSyntax containingMember, int index)
        {
            if (containingMember == null)
                throw new ArgumentNullException(nameof(containingMember));

            switch (containingMember.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    return ((NamespaceDeclarationSyntax)containingMember).RemoveMemberAt(index);
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)containingMember).RemoveMemberAt(index);
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)containingMember).RemoveMemberAt(index);
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)containingMember).RemoveMemberAt(index);
            }

            return containingMember;
        }

        public static MemberDeclarationSyntax RemoveMember(this MemberDeclarationSyntax containingMember, MemberDeclarationSyntax member)
        {
            if (containingMember == null)
                throw new ArgumentNullException(nameof(containingMember));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            switch (containingMember.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    return ((NamespaceDeclarationSyntax)containingMember).RemoveMember(member);
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)containingMember).RemoveMember(member);
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)containingMember).RemoveMember(member);
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)containingMember).RemoveMember(member);
            }

            return containingMember;
        }

        public static SyntaxTokenList GetModifiers(this SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)node).Modifiers;
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)node).Modifiers;
                case SyntaxKind.ConversionOperatorDeclaration:
                    return ((ConversionOperatorDeclarationSyntax)node).Modifiers;
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)node).Modifiers;
                case SyntaxKind.DestructorDeclaration:
                    return ((DestructorDeclarationSyntax)node).Modifiers;
                case SyntaxKind.EnumDeclaration:
                    return ((EnumDeclarationSyntax)node).Modifiers;
                case SyntaxKind.EventDeclaration:
                    return ((EventDeclarationSyntax)node).Modifiers;
                case SyntaxKind.EventFieldDeclaration:
                    return ((EventFieldDeclarationSyntax)node).Modifiers;
                case SyntaxKind.FieldDeclaration:
                    return ((FieldDeclarationSyntax)node).Modifiers;
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)node).Modifiers;
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)node).Modifiers;
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)node).Modifiers;
                case SyntaxKind.OperatorDeclaration:
                    return ((OperatorDeclarationSyntax)node).Modifiers;
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)node).Modifiers;
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)node).Modifiers;
                case SyntaxKind.IncompleteMember:
                    return ((IncompleteMemberSyntax)node).Modifiers;
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                    return ((AccessorDeclarationSyntax)node).Modifiers;
                case SyntaxKind.LocalDeclarationStatement:
                    return ((LocalDeclarationStatementSyntax)node).Modifiers;
                case SyntaxKind.LocalFunctionStatement:
                    return ((LocalFunctionStatementSyntax)node).Modifiers;
                case SyntaxKind.Parameter:
                    return ((ParameterSyntax)node).Modifiers;
                default:
                    {
                        Debug.Assert(false, node.Kind().ToString());
                        return default(SyntaxTokenList);
                    }
            }
        }

        public static SyntaxTokenList GetModifiers(this MemberDeclarationSyntax declaration)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            switch (declaration.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)declaration).Modifiers;
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)declaration).Modifiers;
                case SyntaxKind.ConversionOperatorDeclaration:
                    return ((ConversionOperatorDeclarationSyntax)declaration).Modifiers;
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)declaration).Modifiers;
                case SyntaxKind.DestructorDeclaration:
                    return ((DestructorDeclarationSyntax)declaration).Modifiers;
                case SyntaxKind.EnumDeclaration:
                    return ((EnumDeclarationSyntax)declaration).Modifiers;
                case SyntaxKind.EventDeclaration:
                    return ((EventDeclarationSyntax)declaration).Modifiers;
                case SyntaxKind.EventFieldDeclaration:
                    return ((EventFieldDeclarationSyntax)declaration).Modifiers;
                case SyntaxKind.FieldDeclaration:
                    return ((FieldDeclarationSyntax)declaration).Modifiers;
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)declaration).Modifiers;
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)declaration).Modifiers;
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)declaration).Modifiers;
                case SyntaxKind.OperatorDeclaration:
                    return ((OperatorDeclarationSyntax)declaration).Modifiers;
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)declaration).Modifiers;
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)declaration).Modifiers;
                case SyntaxKind.IncompleteMember:
                    return ((IncompleteMemberSyntax)declaration).Modifiers;
                default:
                    {
                        Debug.Assert(false, declaration.Kind().ToString());
                        return default(SyntaxTokenList);
                    }
            }
        }

        public static SyntaxNode SetModifiers(this SyntaxNode node, SyntaxTokenList modifiers)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.OperatorDeclaration:
                    return ((OperatorDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return ((ConversionOperatorDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.DestructorDeclaration:
                    return ((DestructorDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.EnumDeclaration:
                    return ((EnumDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.EventDeclaration:
                    return ((EventDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.EventFieldDeclaration:
                    return ((EventFieldDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.FieldDeclaration:
                    return ((FieldDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.IncompleteMember:
                    return ((IncompleteMemberSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                    return ((AccessorDeclarationSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.LocalDeclarationStatement:
                    return ((LocalDeclarationStatementSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.LocalFunctionStatement:
                    return ((LocalFunctionStatementSyntax)node).WithModifiers(modifiers);
                case SyntaxKind.Parameter:
                    return ((ParameterSyntax)node).WithModifiers(modifiers);
                default:
                    {
                        Debug.Assert(false, node.Kind().ToString());
                        return node;
                    }
            }
        }

        public static MemberDeclarationSyntax SetModifiers(this MemberDeclarationSyntax declaration, SyntaxTokenList modifiers)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            switch (declaration.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.OperatorDeclaration:
                    return ((OperatorDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return ((ConversionOperatorDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.DestructorDeclaration:
                    return ((DestructorDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.EnumDeclaration:
                    return ((EnumDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.EventDeclaration:
                    return ((EventDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.EventFieldDeclaration:
                    return ((EventFieldDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.FieldDeclaration:
                    return ((FieldDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)declaration).WithModifiers(modifiers);
                case SyntaxKind.IncompleteMember:
                    return ((IncompleteMemberSyntax)declaration).WithModifiers(modifiers);
                default:
                    {
                        Debug.Assert(false, declaration.Kind().ToString());
                        return declaration;
                    }
            }
        }

        public static MemberDeclarationSyntax GetMemberAt(this MemberDeclarationSyntax declaration, int index)
        {
            SyntaxList<MemberDeclarationSyntax> members = GetMembers(declaration);

            return members[index];
        }

        public static SyntaxList<MemberDeclarationSyntax> GetMembers(this MemberDeclarationSyntax declaration)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            switch (declaration.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    return ((NamespaceDeclarationSyntax)declaration).Members;
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)declaration).Members;
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)declaration).Members;
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)declaration).Members;
                default:
                    {
                        Debug.Assert(false, declaration.Kind().ToString());
                        return default(SyntaxList<MemberDeclarationSyntax>);
                    }
            }
        }

        public static MemberDeclarationSyntax SetMembers(this MemberDeclarationSyntax declaration, SyntaxList<MemberDeclarationSyntax> newMembers)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            switch (declaration.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    return ((NamespaceDeclarationSyntax)declaration).WithMembers(newMembers);
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)declaration).WithMembers(newMembers);
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)declaration).WithMembers(newMembers);
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)declaration).WithMembers(newMembers);
                default:
                    {
                        Debug.Assert(false, declaration.Kind().ToString());
                        return declaration;
                    }
            }
        }

        public static bool SupportsExpressionBody(this SyntaxNode node)
        {
            switch (node?.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                case SyntaxKind.PropertyDeclaration:
                case SyntaxKind.IndexerDeclaration:
                case SyntaxKind.OperatorDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                case SyntaxKind.ConstructorDeclaration:
                case SyntaxKind.DestructorDeclaration:
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                    return true;
                default:
                    return false;
            }
        }

        public static Accessibility GetDefaultExplicitAccessibility(this MemberDeclarationSyntax memberDeclaration)
        {
            if (memberDeclaration == null)
                throw new ArgumentNullException(nameof(memberDeclaration));

            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.ConstructorDeclaration:
                    {
                        if (((ConstructorDeclarationSyntax)memberDeclaration).IsStatic())
                        {
                            return Accessibility.NotApplicable;
                        }
                        else
                        {
                            return Accessibility.Private;
                        }
                    }
                case SyntaxKind.DestructorDeclaration:
                    {
                        return Accessibility.NotApplicable;
                    }
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)memberDeclaration;

                        if (methodDeclaration.Modifiers.Contains(SyntaxKind.PartialKeyword)
                            || methodDeclaration.ExplicitInterfaceSpecifier != null
                            || methodDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration))
                        {
                            return Accessibility.NotApplicable;
                        }
                        else
                        {
                            return Accessibility.Private;
                        }
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        var propertyDeclaration = (PropertyDeclarationSyntax)memberDeclaration;

                        if (propertyDeclaration.ExplicitInterfaceSpecifier != null
                            || propertyDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration))
                        {
                            return Accessibility.NotApplicable;
                        }
                        else
                        {
                            return Accessibility.Private;
                        }
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        var indexerDeclaration = (IndexerDeclarationSyntax)memberDeclaration;

                        if (indexerDeclaration.ExplicitInterfaceSpecifier != null
                            || indexerDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration))
                        {
                            return Accessibility.NotApplicable;
                        }
                        else
                        {
                            return Accessibility.Private;
                        }
                    }
                case SyntaxKind.EventDeclaration:
                    {
                        var eventDeclaration = (EventDeclarationSyntax)memberDeclaration;

                        if (eventDeclaration.ExplicitInterfaceSpecifier != null)
                        {
                            return Accessibility.NotApplicable;
                        }
                        else
                        {
                            return Accessibility.Private;
                        }
                    }
                case SyntaxKind.EventFieldDeclaration:
                    {
                        if (memberDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration))
                        {
                            return Accessibility.NotApplicable;
                        }
                        else
                        {
                            return Accessibility.Private;
                        }
                    }
                case SyntaxKind.FieldDeclaration:
                    {
                        return Accessibility.Private;
                    }
                case SyntaxKind.OperatorDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        return Accessibility.Public;
                    }
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.StructDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.EnumDeclaration:
                case SyntaxKind.DelegateDeclaration:
                    {
                        if (memberDeclaration.IsParentKind(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration))
                        {
                            return Accessibility.Private;
                        }
                        else
                        {
                            return Accessibility.Internal;
                        }
                    }
            }

            return Accessibility.NotApplicable;
        }

        public static Accessibility GetDeclaredAccessibility(this MemberDeclarationSyntax memberDeclaration)
        {
            if (memberDeclaration == null)
                throw new ArgumentNullException(nameof(memberDeclaration));

            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.ConstructorDeclaration:
                    {
                        var constructorDeclaration = (ConstructorDeclarationSyntax)memberDeclaration;

                        if (constructorDeclaration.IsStatic())
                        {
                            return Accessibility.Private;
                        }
                        else
                        {
                            return AccessibilityOrDefault(constructorDeclaration, constructorDeclaration.Modifiers);
                        }
                    }
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)memberDeclaration;

                        SyntaxTokenList modifiers = methodDeclaration.Modifiers;

                        if (modifiers.Contains(SyntaxKind.PartialKeyword))
                        {
                            return Accessibility.Private;
                        }
                        else if (methodDeclaration.ExplicitInterfaceSpecifier != null
                            || methodDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration))
                        {
                            return Accessibility.Public;
                        }
                        else
                        {
                            return AccessibilityOrDefault(methodDeclaration, modifiers);
                        }
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        var propertyDeclaration = (PropertyDeclarationSyntax)memberDeclaration;

                        if (propertyDeclaration.ExplicitInterfaceSpecifier != null
                            || propertyDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration))
                        {
                            return Accessibility.Public;
                        }
                        else
                        {
                            return AccessibilityOrDefault(propertyDeclaration, propertyDeclaration.Modifiers);
                        }
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        var indexerDeclaration = (IndexerDeclarationSyntax)memberDeclaration;

                        if (indexerDeclaration.ExplicitInterfaceSpecifier != null
                            || indexerDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration))
                        {
                            return Accessibility.Public;
                        }
                        else
                        {
                            return AccessibilityOrDefault(indexerDeclaration, indexerDeclaration.Modifiers);
                        }
                    }
                case SyntaxKind.EventDeclaration:
                    {
                        var eventDeclaration = (EventDeclarationSyntax)memberDeclaration;

                        if (eventDeclaration.ExplicitInterfaceSpecifier != null)
                        {
                            return Accessibility.Public;
                        }
                        else
                        {
                            return AccessibilityOrDefault(eventDeclaration, eventDeclaration.Modifiers);
                        }
                    }
                case SyntaxKind.EventFieldDeclaration:
                    {
                        if (memberDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration))
                        {
                            return Accessibility.Public;
                        }
                        else
                        {
                            var eventFieldDeclaration = (EventFieldDeclarationSyntax)memberDeclaration;

                            return AccessibilityOrDefault(eventFieldDeclaration, eventFieldDeclaration.Modifiers);
                        }
                    }
                case SyntaxKind.FieldDeclaration:
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.StructDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.EnumDeclaration:
                case SyntaxKind.DelegateDeclaration:
                    {
                        return AccessibilityOrDefault(memberDeclaration, memberDeclaration.GetModifiers());
                    }
                case SyntaxKind.DestructorDeclaration:
                case SyntaxKind.OperatorDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                case SyntaxKind.EnumMemberDeclaration:
                case SyntaxKind.NamespaceDeclaration:
                    {
                        return Accessibility.Public;
                    }
            }

            return Accessibility.NotApplicable;
        }

        private static Accessibility AccessibilityOrDefault(MemberDeclarationSyntax memberDeclaration, SyntaxTokenList modifiers)
        {
            Accessibility accessibility = modifiers.GetAccessibility();

            if (accessibility != Accessibility.NotApplicable)
            {
                return accessibility;
            }
            else
            {
                return GetDefaultExplicitAccessibility(memberDeclaration);
            }
        }

        public static bool IsPubliclyVisible(this MemberDeclarationSyntax memberDeclaration)
        {
            if (memberDeclaration == null)
                throw new ArgumentNullException(nameof(memberDeclaration));

            do
            {
                if (memberDeclaration.IsKind(SyntaxKind.NamespaceDeclaration))
                    return true;

                Accessibility accessibility = memberDeclaration.GetDeclaredAccessibility();

                if (accessibility == Accessibility.Public
                    || accessibility == Accessibility.Protected
                    || accessibility == Accessibility.ProtectedOrInternal)
                {
                    SyntaxNode parent = memberDeclaration.Parent;

                    if (parent != null)
                    {
                        if (parent.IsKind(SyntaxKind.CompilationUnit))
                            return true;

                        memberDeclaration = parent as MemberDeclarationSyntax;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            } while (memberDeclaration != null);

            return false;
        }

        public static bool IsIterator(this MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return methodDeclaration
                    .DescendantNodes(node => !node.IsNestedMethod())
                    .Any(f => f.IsKind(SyntaxKind.YieldReturnStatement, SyntaxKind.YieldBreakStatement));
        }

        public static bool ReturnsVoid(this MethodDeclarationSyntax methodDeclaration)
        {
            return methodDeclaration?.ReturnType?.IsVoid() == true;
        }

        public static TextSpan HeaderSpan(this MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return TextSpan.FromBounds(
                methodDeclaration.Span.Start,
                methodDeclaration.ParameterList?.Span.End ?? methodDeclaration.Identifier.Span.End);
        }

        public static bool IsStatic(this MethodDeclarationSyntax methodDeclaration)
        {
            return methodDeclaration?.Modifiers.Contains(SyntaxKind.StaticKeyword) == true;
        }

        public static MemberDeclarationSyntax RemoveMemberAt(this NamespaceDeclarationSyntax namespaceDeclaration, int index)
        {
            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            return RemoveMember(namespaceDeclaration, namespaceDeclaration.Members[index], index);
        }

        public static MemberDeclarationSyntax RemoveMember(this NamespaceDeclarationSyntax namespaceDeclaration, MemberDeclarationSyntax member)
        {
            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return RemoveMember(namespaceDeclaration, member, namespaceDeclaration.Members.IndexOf(member));
        }

        private static MemberDeclarationSyntax RemoveMember(
            NamespaceDeclarationSyntax namespaceDeclaration,
            MemberDeclarationSyntax member,
            int index)
        {
            MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

            namespaceDeclaration = namespaceDeclaration
                .WithMembers(namespaceDeclaration.Members.Replace(member, newMember));

            return namespaceDeclaration
                .RemoveNode(namespaceDeclaration.Members[index], Remover.GetRemoveOptions(newMember));
        }

        internal static NamespaceDeclarationSyntax WithMembers(
            this NamespaceDeclarationSyntax namespaceDeclaration,
            MemberDeclarationSyntax memberDeclaration)
        {
            return namespaceDeclaration.WithMembers(SingletonList(memberDeclaration));
        }

        internal static NamespaceDeclarationSyntax WithMembers(
            this NamespaceDeclarationSyntax namespaceDeclaration,
            IEnumerable<MemberDeclarationSyntax> memberDeclarations)
        {
            return namespaceDeclaration.WithMembers(List(memberDeclarations));
        }

        internal static bool ContainsAwait(this MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return methodDeclaration
                .DescendantNodes(node => !node.IsNestedMethod())
                .Any(f => f.IsKind(SyntaxKind.AwaitExpression));
        }

        public static CSharpSyntaxNode BodyOrExpressionBody(this MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            BlockSyntax body = methodDeclaration.Body;

            return body ?? (CSharpSyntaxNode)methodDeclaration.ExpressionBody;
        }

        public static TextSpan HeaderSpan(this NamespaceDeclarationSyntax namespaceDeclaration)
        {
            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            return TextSpan.FromBounds(
                namespaceDeclaration.Span.Start,
                namespaceDeclaration.Name?.Span.End ?? namespaceDeclaration.NamespaceKeyword.Span.End);
        }

        public static TextSpan BracesSpan(this NamespaceDeclarationSyntax namespaceDeclaration)
        {
            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            return TextSpan.FromBounds(
                namespaceDeclaration.OpenBraceToken.Span.Start,
                namespaceDeclaration.CloseBraceToken.Span.End);
        }

        public static TextSpan HeaderSpan(this OperatorDeclarationSyntax operatorDeclaration)
        {
            if (operatorDeclaration == null)
                throw new ArgumentNullException(nameof(operatorDeclaration));

            return TextSpan.FromBounds(
                operatorDeclaration.Span.Start,
                operatorDeclaration.ParameterList?.Span.End ?? operatorDeclaration.OperatorToken.Span.End);
        }

        public static CSharpSyntaxNode BodyOrExpressionBody(this OperatorDeclarationSyntax operatorDeclaration)
        {
            if (operatorDeclaration == null)
                throw new ArgumentNullException(nameof(operatorDeclaration));

            BlockSyntax body = operatorDeclaration.Body;

            return body ?? (CSharpSyntaxNode)operatorDeclaration.ExpressionBody;
        }

        public static bool IsThis(this ParameterSyntax parameter)
        {
            return parameter?.Modifiers.Contains(SyntaxKind.ThisKeyword) == true;
        }

        internal static PropertyDeclarationSyntax WithAttributeLists(
            this PropertyDeclarationSyntax propertyDeclaration,
            params AttributeListSyntax[] attributeLists)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration.WithAttributeLists(List(attributeLists));
        }

        internal static PropertyDeclarationSyntax WithoutSemicolonToken(
            this PropertyDeclarationSyntax propertyDeclaration)
        {
            return propertyDeclaration.WithSemicolonToken(default(SyntaxToken));
        }

        public static TextSpan HeaderSpan(this PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return TextSpan.FromBounds(
                propertyDeclaration.Span.Start,
                propertyDeclaration.Identifier.Span.End);
        }

        public static AccessorDeclarationSyntax Getter(this PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration.AccessorList?.Getter();
        }

        public static AccessorDeclarationSyntax Setter(this PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration.AccessorList?.Setter();
        }

        public static bool IsStatic(this PropertyDeclarationSyntax propertyDeclaration)
        {
            return propertyDeclaration?.Modifiers.Contains(SyntaxKind.StaticKeyword) == true;
        }

        public static StatementSyntax PreviousStatement(this StatementSyntax statement)
        {
            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            SyntaxList<StatementSyntax> statements;
            if (statement.TryGetContainingList(out statements))
            {
                int index = statements.IndexOf(statement);

                if (index > 0)
                {
                    return statements[index - 1];
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        public static StatementSyntax NextStatement(this StatementSyntax statement)
        {
            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            SyntaxList<StatementSyntax> statements;
            if (statement.TryGetContainingList(out statements))
            {
                int index = statements.IndexOf(statement);

                if (index < statements.Count - 1)
                {
                    return statements[index + 1];
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        public static bool TryGetContainingList(this StatementSyntax statement, out SyntaxList<StatementSyntax> statements)
        {
            SyntaxNode parent = statement?.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.Block:
                    {
                        statements = ((BlockSyntax)parent).Statements;
                        return true;
                    }
                case SyntaxKind.SwitchSection:
                    {
                        statements = ((SwitchSectionSyntax)parent).Statements;
                        return true;
                    }
                default:
                    {
                        Debug.Assert(parent == null || EmbeddedStatement.IsEmbeddedStatement(statement), parent.Kind().ToString());
                        statements = default(SyntaxList<StatementSyntax>);
                        return false;
                    }
            }
        }

        internal static StructDeclarationSyntax WithMembers(
            this StructDeclarationSyntax structDeclaration,
            MemberDeclarationSyntax memberDeclaration)
        {
            return structDeclaration.WithMembers(SingletonList(memberDeclaration));
        }

        internal static StructDeclarationSyntax WithMembers(
            this StructDeclarationSyntax structDeclaration,
            IEnumerable<MemberDeclarationSyntax> memberDeclarations)
        {
            return structDeclaration.WithMembers(List(memberDeclarations));
        }

        public static TextSpan HeaderSpan(this StructDeclarationSyntax structDeclaration)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            return TextSpan.FromBounds(
                structDeclaration.Span.Start,
                structDeclaration.Identifier.Span.End);
        }

        public static TextSpan BracesSpan(this StructDeclarationSyntax structDeclaration)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            return TextSpan.FromBounds(
                structDeclaration.OpenBraceToken.Span.Start,
                structDeclaration.CloseBraceToken.Span.End);
        }

        public static MemberDeclarationSyntax RemoveMemberAt(this StructDeclarationSyntax structDeclaration, int index)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            return RemoveMember(structDeclaration, structDeclaration.Members[index], index);
        }

        public static MemberDeclarationSyntax RemoveMember(this StructDeclarationSyntax structDeclaration, MemberDeclarationSyntax member)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return RemoveMember(structDeclaration, member, structDeclaration.Members.IndexOf(member));
        }

        private static MemberDeclarationSyntax RemoveMember(
            StructDeclarationSyntax structDeclaration,
            MemberDeclarationSyntax member,
            int index)
        {
            MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

            structDeclaration = structDeclaration
                .WithMembers(structDeclaration.Members.Replace(member, newMember));

            return structDeclaration
                .RemoveNode(structDeclaration.Members[index], Remover.GetRemoveOptions(newMember));
        }

        public static StatementSyntax SingleStatementOrDefault(this SwitchSectionSyntax switchSection)
        {
            if (switchSection == null)
                throw new ArgumentNullException(nameof(switchSection));

            SyntaxList<StatementSyntax> statements = switchSection.Statements;

            return (statements.Count == 1)
                ? statements[0]
                : null;
        }

        public static bool IsDefault(this SwitchSectionSyntax switchSection)
        {
            return switchSection?.Labels.Any(f => f.IsKind(SyntaxKind.DefaultSwitchLabel)) == true;
        }

        public static int LastIndexOf<TNode>(this SyntaxList<TNode> list, SyntaxKind kind) where TNode : SyntaxNode
        {
            return list.LastIndexOf(f => f.IsKind(kind));
        }

        public static int LastIndexOf<TNode>(this SeparatedSyntaxList<TNode> list, SyntaxKind kind) where TNode : SyntaxNode
        {
            return list.LastIndexOf(f => f.IsKind(kind));
        }

        public static bool Contains<TNode>(this SyntaxList<TNode> list, SyntaxKind kind) where TNode : SyntaxNode
        {
            return list.IndexOf(kind) != -1;
        }

        public static bool Contains<TNode>(this SeparatedSyntaxList<TNode> list, SyntaxKind kind) where TNode : SyntaxNode
        {
            return list.IndexOf(kind) != -1;
        }

        public static SyntaxList<TNode> ReplaceAt<TNode>(this SyntaxList<TNode> list, int index, TNode newNode) where TNode : SyntaxNode
        {
            return list.Replace(list[index], newNode);
        }

        public static bool IsFirst<TNode>(this SyntaxList<TNode> list, TNode node) where TNode : SyntaxNode
        {
            return list.IndexOf(node) == 0;
        }

        public static bool IsLast<TNode>(this SyntaxList<TNode> list, TNode node) where TNode : SyntaxNode
        {
            return list.Any()
                && list.IndexOf(node) == list.Count - 1;
        }

        public static bool IsFirst<TNode>(this SeparatedSyntaxList<TNode> list, TNode node) where TNode : SyntaxNode
        {
            return list.IndexOf(node) == 0;
        }

        public static bool IsLast<TNode>(this SeparatedSyntaxList<TNode> list, TNode node) where TNode : SyntaxNode
        {
            return list.Any()
                && list.IndexOf(node) == list.Count - 1;
        }

        public static SeparatedSyntaxList<TNode> ReplaceAt<TNode>(this SeparatedSyntaxList<TNode> list, int index, TNode newNode) where TNode : SyntaxNode
        {
            return list.Replace(list[index], newNode);
        }

        public static bool IsVoid(this TypeSyntax type)
        {
            return type?.IsKind(SyntaxKind.PredefinedType) == true
                && ((PredefinedTypeSyntax)type).Keyword.IsKind(SyntaxKind.VoidKeyword);
        }

        public static CSharpSyntaxNode DeclarationOrExpression(this UsingStatementSyntax usingStatement)
        {
            if (usingStatement == null)
                throw new ArgumentNullException(nameof(usingStatement));

            CSharpSyntaxNode declaration = usingStatement.Declaration;

            return declaration ?? usingStatement.Expression;
        }

        //TODO: SingleVariableOrDefault
        public static VariableDeclaratorSyntax SingleVariableOrDefault(this VariableDeclarationSyntax declaration)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            SeparatedSyntaxList<VariableDeclaratorSyntax> variables = declaration.Variables;

            return (variables.Count == 1)
                ? variables.First()
                : null;
        }

        public static bool IsYieldReturn(this YieldStatementSyntax yieldStatement)
        {
            return yieldStatement?.ReturnOrBreakKeyword.IsKind(SyntaxKind.ReturnKeyword) == true;
        }

        public static bool IsYieldBreak(this YieldStatementSyntax yieldStatement)
        {
            return yieldStatement?.ReturnOrBreakKeyword.IsKind(SyntaxKind.BreakKeyword) == true;
        }

        private static MemberDeclarationSyntax RemoveSingleLineDocumentationComment(MemberDeclarationSyntax member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            SyntaxTriviaList leadingTrivia = member.GetLeadingTrivia();

            SyntaxTriviaList.Reversed.Enumerator en = leadingTrivia.Reverse().GetEnumerator();

            int i = 0;
            while (en.MoveNext())
            {
                if (en.Current.IsWhitespaceOrEndOfLineTrivia())
                {
                    i++;
                }
                else if (en.Current.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                {
                    return member.WithLeadingTrivia(leadingTrivia.Take(leadingTrivia.Count - (i + 1)));
                }
                else
                {
                    return member;
                }
            }

            return member;
        }
    }
}
