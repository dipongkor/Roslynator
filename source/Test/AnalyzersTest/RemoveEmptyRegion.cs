﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analyzers.Test
{
    internal static class RemoveEmptyRegion
    {
        #region
        public static void Foo()
        #endregion
        {
            #region
            
            #endregion
        }
        #region


        #endregion
    }
}
