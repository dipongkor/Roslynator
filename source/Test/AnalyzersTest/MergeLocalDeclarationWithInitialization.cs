﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Roslynator.CSharp.Analyzers.Test
{
    public static class MergeLocalDeclarationWithInitialization
    {
        private const bool BoolConst = false;
        private const char CharConst = '\0';
        private const int IntConst = 0;
        private const ulong ULongConst = 0;
        private const RegexOptions RegexOptionsConst = RegexOptions.None;
        private const string StringConst = null;

        private static void Foo()
        {
            bool f;
            f = true;

            bool f2 = false;
            f2 = true;

            bool f3 = BoolConst;
            f3 = true;

            char ch;
            ch = 'a';

            char ch2 = '\0';
            ch2 = 'a';

            char ch3 = CharConst;
            ch3 = 'a';

            int i;
            i = 1;

            int i2 = 0;
            i2 = 1;

            int i3 = IntConst;
            i3 = 1;

            ulong l;
            l = 1;

            ulong l2 = 0;
            l2 = 1;

            ulong l3 = ULongConst;
            l3 = 1;

            RegexOptions ro;
            ro = RegexOptions.CultureInvariant;

            RegexOptions ro2 = 0;
            ro2 = RegexOptions.CultureInvariant;

            RegexOptions ro3 = RegexOptions.None;
            ro3 = RegexOptions.CultureInvariant;

            RegexOptions ro4;
            ro4 = RegexOptions.CultureInvariant;

            string s;
            s = "";

            string s2 = null;
            s2 = "";

            string s3 = default(string);
            s3 = "";

            string s4 = StringConst;
            s4 = "";

            bool? n;
            n = true;

            bool? n2 = null;
            n2 = true;

            bool? n3 = default(bool?);
            n3 = true;

            sbyte sb = (sbyte)0;
            sb = 1;

            byte be = (byte)0;
            be = 1;

            short st = (short)0;
            st = 1;

            ushort us = (ushort)0;
            us = 1;

            int ii = (int)0;
            ii = 1;

            uint ui = (uint)0;
            ui = 1;

            long lg = (long)0;
            lg = 1;

            ulong ul = (ulong)0;
            ul = 1;

            float ft = (float)0;
            ft = 1;

            double de = (double)0;
            de = 1;

            decimal dl = (decimal)0;
            dl = 1;
        }
    }
}
