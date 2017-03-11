﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;

namespace Roslynator.CSharp.Refactorings.Test
{
    internal class ExpandLambdaExpressionBodyRefactoring
    {
        public void SomeMethod()
        {
            var items = Enumerable.Range(0, 10).Select(i => i + 1);
        }
    }
}
