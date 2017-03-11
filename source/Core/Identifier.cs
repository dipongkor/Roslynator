﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using Roslynator.Extensions;
using Roslynator.Internal;

namespace Roslynator
{
    public static class Identifier
    {
        public const string DefaultVariableName = "x";
        public const string DefaultForVariableName = "i";
        public const string DefaultForEachVariableName = "item";
        public const string DefaultEventArgsVariableName = "e";
        public const string DefaultEventHandlerVariableName = "handler";
        public const string DefaultNamespaceName = "Namespace";
        public const string DefaultEnumMemberName = "EnumMember";
        public const string DefaultTypeParameterName = "T";

        private static StringComparer OrdinalComparer { get; } = StringComparer.Ordinal;

        public static string EnsureUniqueMemberName(
            string baseName,
            int position,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            INamedTypeSymbol containingType = semanticModel.GetEnclosingNamedType(position, cancellationToken);

            if (containingType != null)
            {
                return EnsureUniqueMemberName(baseName, containingType);
            }
            else
            {
                return EnsureUniqueName(baseName, semanticModel.LookupSymbols(position));
            }
        }

        public static string EnsureUniqueMemberName(
            string baseName,
            INamedTypeSymbol containingType)
        {
            if (containingType == null)
                throw new ArgumentNullException(nameof(containingType));

            return EnsureUniqueName(baseName, containingType.GetMembers());
        }

        public static string EnsureUniqueLocalName(
            string baseName,
            int position,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            SyntaxNode container = FindContainer(position, semanticModel, cancellationToken);

            HashSet<ISymbol> containerSymbols = GetContainerSymbols(container, semanticModel, cancellationToken);

            IEnumerable<string> reservedNames = semanticModel
                .LookupSymbols(position)
                .Select(f => f.Name)
                .Concat(containerSymbols.Select(f => f.Name));

            return EnsureUniqueName(baseName, reservedNames);
        }

        private static SyntaxNode FindContainer(int position, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            ISymbol enclosingSymbol = semanticModel.GetEnclosingSymbol(position, cancellationToken);

            ImmutableArray<SyntaxReference> syntaxReferences = enclosingSymbol.DeclaringSyntaxReferences;

            if (syntaxReferences.Length == 1)
            {
                return syntaxReferences[0].GetSyntax(cancellationToken);
            }
            else
            {
                foreach (SyntaxReference syntaxReference in syntaxReferences)
                {
                    SyntaxNode syntax = syntaxReference.GetSyntax(cancellationToken);

                    if (syntax.SyntaxTree == semanticModel.SyntaxTree)
                        return syntax;
                }
            }

            return null;
        }

        private static HashSet<ISymbol> GetContainerSymbols(
            SyntaxNode container,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            var symbols = new HashSet<ISymbol>();

            foreach (SyntaxNode node in container.DescendantNodesAndSelf())
            {
                ISymbol symbol = semanticModel.GetDeclaredSymbol(node, cancellationToken);

                if (symbol?.IsAnonymousTypeProperty() == false)
                    symbols.Add(symbol);
            }

            return symbols;
        }

        public static async Task<string> EnsureUniqueParameterNameAsync(
            IParameterSymbol parameterSymbol,
            string baseName,
            Solution solution,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (parameterSymbol == null)
                throw new ArgumentNullException(nameof(parameterSymbol));

            if (solution == null)
                throw new ArgumentNullException(nameof(solution));

            HashSet<string> reservedNames = GetParameterNames(parameterSymbol);

            foreach (ReferencedSymbol referencedSymbol in await SymbolFinder.FindReferencesAsync(parameterSymbol, solution, cancellationToken).ConfigureAwait(false))
            {
                foreach (ReferenceLocation referenceLocation in referencedSymbol.Locations)
                {
                    if (!referenceLocation.IsImplicit
                        && !referenceLocation.IsCandidateLocation)
                    {
                        SemanticModel semanticModel = await referenceLocation.Document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                        foreach (ISymbol symbol in semanticModel.LookupSymbols(referenceLocation.Location.SourceSpan.Start))
                        {
                            if (!parameterSymbol.Equals(symbol))
                                reservedNames.Add(symbol.Name);
                        }
                    }
                }
            }

            return EnsureUniqueName(baseName, reservedNames);
        }

        private static HashSet<string> GetParameterNames(IParameterSymbol parameterSymbol)
        {
            var reservedNames = new HashSet<string>(OrdinalComparer);

            ISymbol containingSymbol = parameterSymbol.ContainingSymbol;

            if (containingSymbol != null)
            {
                SymbolKind kind = containingSymbol.Kind;

                Debug.Assert(kind == SymbolKind.Method || kind == SymbolKind.Property, kind.ToString());

                if (kind == SymbolKind.Method)
                {
                    var methodSymbol = (IMethodSymbol)containingSymbol;

                    foreach (IParameterSymbol symbol in methodSymbol.Parameters)
                    {
                        if (!parameterSymbol.Equals(symbol))
                            reservedNames.Add(symbol.Name);
                    }
                }
                else if (kind == SymbolKind.Property)
                {
                    var propertySymbol = (IPropertySymbol)containingSymbol;

                    if (propertySymbol.IsIndexer)
                    {
                        foreach (IParameterSymbol symbol in propertySymbol.Parameters)
                        {
                            if (!parameterSymbol.Equals(symbol))
                                reservedNames.Add(symbol.Name);
                        }
                    }
                }
            }

            return reservedNames;
        }

        public static async Task<string> EnsureUniqueAsyncMethodNameAsync(
            IMethodSymbol methodSymbol,
            string baseName,
            Solution solution,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (methodSymbol == null)
                throw new ArgumentNullException(nameof(methodSymbol));

            if (solution == null)
                throw new ArgumentNullException(nameof(solution));

            HashSet<string> reservedNames = await GetReservedNamesAsync(methodSymbol, solution, cancellationToken).ConfigureAwait(false);

            var generator = new AsyncMethodNameGenerator();
            return generator.EnsureUniqueName(baseName, reservedNames);
        }

        public static async Task<string> EnsureUniqueMemberNameAsync(
            ISymbol memberSymbol,
            string baseName,
            Solution solution,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (memberSymbol == null)
                throw new ArgumentNullException(nameof(memberSymbol));

            if (solution == null)
                throw new ArgumentNullException(nameof(solution));

            HashSet<string> reservedNames = await GetReservedNamesAsync(memberSymbol, solution, cancellationToken).ConfigureAwait(false);

            return EnsureUniqueName(baseName, reservedNames);
        }

        public static string EnsureUniqueEnumMemberName(INamedTypeSymbol enumSymbol, string baseName)
        {
            if (enumSymbol == null)
                throw new ArgumentNullException(nameof(enumSymbol));

            return EnsureUniqueName(baseName, enumSymbol.MemberNames);
        }

        public static bool IsUniqueMemberName(
            string name,
            int position,
            SemanticModel semanticModel,
            bool isCaseSensitive = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            INamedTypeSymbol containingType = semanticModel.GetEnclosingNamedType(position, cancellationToken);

            return IsUniqueMemberName(name, containingType, isCaseSensitive);
        }

        public static bool IsUniqueMemberName(
            string name,
            INamedTypeSymbol containingType,
            bool isCaseSensitive = true)
        {
            if (containingType == null)
                throw new ArgumentNullException(nameof(containingType));

            return IsUniqueName(name, containingType.GetMembers(), GetStringComparison(isCaseSensitive));
        }

        public static async Task<bool> IsUniqueMemberNameAsync(
            ISymbol memberSymbol,
            string name,
            Solution solution,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (memberSymbol == null)
                throw new ArgumentNullException(nameof(memberSymbol));

            if (solution == null)
                throw new ArgumentNullException(nameof(solution));

            HashSet<string> reservedNames = await GetReservedNamesAsync(memberSymbol, solution, cancellationToken).ConfigureAwait(false);

            return !reservedNames.Contains(name);
        }

        private static async Task<HashSet<string>> GetReservedNamesAsync(
            ISymbol memberSymbol,
            Solution solution,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            HashSet<string> reservedNames = GetMemberNames(memberSymbol);

            foreach (ReferencedSymbol referencedSymbol in await SymbolFinder.FindReferencesAsync(memberSymbol, solution, cancellationToken).ConfigureAwait(false))
            {
                foreach (ReferenceLocation referenceLocation in referencedSymbol.Locations)
                {
                    if (!referenceLocation.IsImplicit
                        && !referenceLocation.IsCandidateLocation)
                    {
                        SemanticModel semanticModel = await referenceLocation.Document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                        foreach (ISymbol symbol in semanticModel.LookupSymbols(referenceLocation.Location.SourceSpan.Start))
                        {
                            if (!memberSymbol.Equals(symbol))
                                reservedNames.Add(symbol.Name);
                        }
                    }
                }
            }

            return reservedNames;
        }

        private static HashSet<string> GetMemberNames(ISymbol memberSymbol)
        {
            INamedTypeSymbol containingType = memberSymbol.ContainingType;

            Debug.Assert(containingType != null);

            if (containingType != null)
            {
                IEnumerable<string> memberNames = containingType
                    .GetMembers()
                    .Where(f => !memberSymbol.Equals(f))
                    .Select(f => f.Name);

                return new HashSet<string>(memberNames, OrdinalComparer);
            }
            else
            {
                return new HashSet<string>(OrdinalComparer);
            }
        }

        private static string EnsureUniqueName(string baseName, ImmutableArray<ISymbol> symbols, bool isCaseSensitive = true)
        {
            return EnsureUniqueName(baseName, symbols, GetStringComparison(isCaseSensitive));
        }

        public static string EnsureUniqueName(string baseName, IEnumerable<string> reservedNames, bool isCaseSensitive = true)
        {
            return EnsureUniqueName(baseName, reservedNames, GetStringComparison(isCaseSensitive));
        }

        private static string EnsureUniqueName(string baseName, IList<ISymbol> symbols, StringComparison stringComparison)
        {
            int suffix = 2;

            string name = baseName;

            while (!IsUniqueName(name, symbols, stringComparison))
            {
                name = baseName + suffix.ToString();
                suffix++;
            }

            return name;
        }

        public static string EnsureUniqueName(string baseName, IEnumerable<string> reservedNames, StringComparison stringComparison)
        {
            if (reservedNames == null)
                throw new ArgumentNullException(nameof(reservedNames));

            int suffix = 2;

            string name = baseName;

            while (!IsUniqueName(name, reservedNames, stringComparison))
            {
                name = baseName + suffix.ToString();
                suffix++;
            }

            return name;
        }

        private static bool IsUniqueName(string name, IList<ISymbol> symbols, StringComparison stringComparison)
        {
            return !symbols.Any(symbol => string.Equals(symbol.Name, name, stringComparison));
        }

        private static bool IsUniqueName(string name, IEnumerable<string> reservedNames, StringComparison stringComparison)
        {
            return !reservedNames.Any(f => string.Equals(f, name, stringComparison));
        }

        public static string CreateName(ITypeSymbol typeSymbol, bool firstCharToLower = false)
        {
            return CreateNameFromTypeSymbol.CreateName(typeSymbol, firstCharToLower);
        }

        private static StringComparison GetStringComparison(bool isCaseSensitive)
        {
            if (isCaseSensitive)
            {
                return StringComparison.Ordinal;
            }
            else
            {
                return StringComparison.OrdinalIgnoreCase;
            }
        }

        public static string ToCamelCase(string value, bool prefixWithUnderscore = false)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            string prefix = (prefixWithUnderscore) ? "_" : "";

            if (value.Length > 0)
            {
                return ToCamelCase(value, prefix);
            }
            else
            {
                return prefix;
            }
        }

        private static string ToCamelCase(string value, string prefix)
        {
            var sb = new StringBuilder(prefix, value.Length + prefix.Length);

            int i = 0;

            while (i < value.Length && value[i] == '_')
                i++;

            if (char.IsUpper(value[i]))
            {
                sb.Append(char.ToLower(value[i]));
            }
            else
            {
                sb.Append(value[i]);
            }

            i++;

            sb.Append(value, i, value.Length - i);

            return sb.ToString();
        }

        public static bool IsCamelCasePrefixedWithUnderscore(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value[0] == '_')
            {
                if (value.Length > 1)
                {
                    return value[1] != '_'
                        && !char.IsUpper(value[1]);
                }

                return true;
            }

            return false;
        }

        public static bool IsCamelCaseNotPrefixedWithUnderscore(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.Length > 0
                && value[0] != '_'
                && char.IsLower(value[0]);
        }

        public static bool HasPrefix(string value, string prefix, StringComparison comparison = StringComparison.Ordinal)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (prefix == null)
                throw new ArgumentNullException(nameof(prefix));

            return prefix.Length > 0
                && value.Length > prefix.Length
                && value.StartsWith(prefix, comparison)
                && IsBoundary(value[prefix.Length - 1], value[prefix.Length]);
        }

        public static bool HasSuffix(string value, string suffix, StringComparison comparison = StringComparison.Ordinal)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (suffix == null)
                throw new ArgumentNullException(nameof(suffix));

            return suffix.Length > 0
                && value.Length > suffix.Length
                && value.EndsWith(suffix, comparison)
                && IsBoundary(value[value.Length - suffix.Length - 1], value[value.Length - suffix.Length]);
        }

        private static bool IsBoundary(char ch1, char ch2)
        {
            if (IsHyphenOrUnderscore(ch1))
            {
                return !IsHyphenOrUnderscore(ch2);
            }
            else if (char.IsDigit(ch1))
            {
                return IsHyphenOrUnderscore(ch2);
            }
            else if (char.IsLower(ch1))
            {
                return IsHyphenOrUnderscore(ch2) || char.IsUpper(ch2);
            }
            else
            {
                return IsHyphenOrUnderscore(ch2);
            }
        }

        private static bool IsHyphenOrUnderscore(char ch)
        {
            return ch == '-' || ch == '_';
        }

        public static string RemovePrefix(string value, string prefix, StringComparison comparison = StringComparison.Ordinal)
        {
            if (HasPrefix(value, prefix, comparison))
            {
                return value.Substring(prefix.Length);
            }
            else
            {
                return value;
            }
        }

        public static string RemoveSuffix(string value, string suffix, StringComparison comparison = StringComparison.Ordinal)
        {
            if (HasSuffix(value, suffix, comparison))
            {
                return value.Remove(value.Length - suffix.Length);
            }
            else
            {
                return value;
            }
        }
    }
}
