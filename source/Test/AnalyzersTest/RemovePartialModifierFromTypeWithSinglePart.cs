﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.ObjectModel;

namespace Roslynator.CSharp.Analyzers.Test
{
    public static class RemovePartialModifierFromTypeWithSinglePart
    {
        private partial class Foo
        {
        }
    }
}