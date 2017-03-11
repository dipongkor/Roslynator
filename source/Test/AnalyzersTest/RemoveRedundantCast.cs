﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analyzers.Test
{
#pragma warning disable RCS1079
    public static class RemoveRedundantCast
    {
        public static class Foo
        {
            public static void Bar()
            {
                SyntaxNode node = null;

                SyntaxNode parent = ((BlockSyntax)node).Parent;

                parent = ((BlockSyntax)node)?.Parent;

                var dic = new Dictionary<int, string>();

                string x = ((IDictionary<int, string>)dic)[0];

                x = ((IDictionary<int, string>)dic)?[0];

                IEnumerable<string> q = Enumerable.Empty<string>()
                    .AsEnumerable()
                    .Cast<string>();

                SyntaxToken openBrace = ((BlockSyntax)node).OpenBraceToken;
                Location location = ((BlockSyntax)node).GetLocation();

                var q2 = ((IEnumerable<string>)new EnumerableOfString()).GetEnumerator();
                var q3 = ((IEnumerable<string>)new EnumerableOfString2()).GetEnumerator();

                IEnumerableOfString i = null;
                var q4 = ((IEnumerable<string>)i).GetEnumerator();
            }
        }

        private interface IEnumerableOfString : IEnumerable<string>
        {
        }

        private class EnumerableOfString : IEnumerable<string>
        {
            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator<string> IEnumerable<string>.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        private class EnumerableOfString2 : EnumerableOfString
        {
        }
    }
}
