﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;

namespace Roslynator.CSharp.Refactorings.Test
{
    internal static class IntroduceLocalFromExpressionStatementThatReturnsValueRefactoring
    {
        public static void Foo()
        {
            int i;
            i = 0;
            i++;

            Execute();

            var x = GetValue();

            GetValue();

            DateTime
        }

        public static async Task ExecuteAsync() => ExecuteAsync();

        private static async Task<Entity> GetValueAsync()
        {
            GetValueAsync();

            return null;
        }

        public static void Execute()
        {
        }

        public static string GetValue()
        {
            string s = null;

            return s;
        }

        private class Entity
        {
        }
    }
}
