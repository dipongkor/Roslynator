﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analyzers.Test
{
#pragma warning disable RCS1081, CS0219
    internal static class MarkLocalAsConst
    {
        public static void Foo()
        {
            string s = "";
            string x = s;

            string s2 = s + "";
            string x2 = s2;

            var s3 = "";
            string x3 = s3;

            string s4 = "", s5 = "";
            string x4 = s4;
            string x5 = s5;

            string s6 = "", s7 = string.Empty;
            string x6 = s6;
            string x7 = s7;

            string s8 = string.Empty;
            string x8 = s8;

            string s9 = "";
            string x9 = s9;
            s9 = null;

            int i = 0;
            if (int.TryParse("", out i))
            {
            }
        }
    }
#pragma warning restore RCS1081, CS0219
}
