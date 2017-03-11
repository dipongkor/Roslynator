﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// xxx

using System;

/// <summary>
/// 
/// </summary>
namespace Roslynator.CSharp.Analyzers.Test
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal static class DeclareUsingDirectiveOnTopLevel
    {
        public static void Foo()
        {
            var items = new List<string>();
        }

        private static Task<object> GetValueAsync()
        {
            return Task.FromResult(new object());
        }
    }
}
