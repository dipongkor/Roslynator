﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

#pragma warning disable RCS1016, RCS1048, RCS1081, RCS1163

namespace Roslynator.CSharp.Analyzers.Test
{
    public static class MarkFieldAsReadOnly
    {
        public partial class Foo
        {
            private static string _sf;
            private string _f;
            private string _f2, _f3;
            private static string _staticAssignedInInstanceConstructor;
            private string _assigned;
            private string _inSimpleLambda;
            private string _inParenthesizedLambda;
            private string _inAnonymousMethod;
            private string _inLocalFunction;

            static Foo()
            {
                _sf = null;
            }

            public Foo()
            {
                _f = null;
                _f2 = null;
                _staticAssignedInInstanceConstructor = null;
            }

            private void Bar()
            {
                _assigned = null;
                _f3 = null;
            }
        }

        public partial class Foo
        {
            public Foo(object parameter)
            {
                var items = new List<string>();

                IEnumerable<string> q = items.Select(f =>
                {
                    _inSimpleLambda = null;
                    return f;
                });

                IEnumerable<string> q2 = items.Select((f) =>
                {
                    _inParenthesizedLambda = null;
                    return f;
                });

                IEnumerable<string> q3 = items.Select(delegate(string f)
                {
                    _inAnonymousMethod = null;
                    return f;
                });

                LocalFunction();

                void LocalFunction()
                {
                    _inLocalFunction = null;
                }
            }
        }
    }
}
