﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Roslynator.CSharp.Analyzers.Test
{
#pragma warning disable RCS1023
    public static class NullCheckOfUnconstrainedTypeParameter
    {
        private static void Foo<T1, T2, T3>()
            where T2 : class
            where T3 : T2
            where T1 : new()
        {
            var x1 = default(T1);
            var x2 = default(T2);
            var x3 = default(T3);

            if (x1 == null) { }

            if (x1 != null) { }

            if (x2 == null) { }

            if (x3 == null) { }
        }
    }
}
