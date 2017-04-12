﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    //TODO: AsyncMethodNameGenerator
    internal class AsyncMethodNameGenerator : NameGenerator
    {
        public async Task<string> EnsureUniqueAsyncMethodNameAsync(
            string baseName,
            IMethodSymbol methodSymbol,
            Solution solution,
            bool isCaseSensitive = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (methodSymbol == null)
                throw new ArgumentNullException(nameof(methodSymbol));

            if (solution == null)
                throw new ArgumentNullException(nameof(solution));

            HashSet<string> reservedNames = await GetReservedNamesAsync(methodSymbol, solution, isCaseSensitive, cancellationToken).ConfigureAwait(false);

            return EnsureUniqueName(baseName, reservedNames);
        }

        public override string EnsureUniqueName(string baseName, HashSet<string> reservedNames)
        {
            int suffix = 1;

            string name = baseName + "Async";

            while (!IsUniqueName(name, reservedNames))
            {
                suffix++;
                name = baseName + suffix.ToString() + "Async";
            }

            return name;
        }

        public override string EnsureUniqueName(string baseName, ImmutableArray<ISymbol> symbols, bool isCaseSensitive)
        {
            int suffix = 1;

            string name = baseName + "Async";

            while (!IsUniqueName(name, symbols, isCaseSensitive))
            {
                suffix++;
                name = baseName + suffix.ToString() + "Async";
            }

            return name;
        }
    }
}
