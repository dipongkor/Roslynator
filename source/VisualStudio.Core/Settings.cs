﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.VisualStudio
{
    public class Settings
    {
        public bool PrefixFieldIdentifierWithUnderscore { get; set; } = true;

        public Dictionary<string, bool> Refactorings { get; set; } = new Dictionary<string, bool>(StringComparer.Ordinal);

        public virtual void Update(Settings settings)
        {
            PrefixFieldIdentifierWithUnderscore = settings.PrefixFieldIdentifierWithUnderscore;

            Refactorings.Clear();

            foreach (KeyValuePair<string, bool> kvp in settings.Refactorings)
                Refactorings[kvp.Key] = kvp.Value;
        }

        public void ApplyTo(RefactoringSettings settings)
        {
            settings.PrefixFieldIdentifierWithUnderscore = PrefixFieldIdentifierWithUnderscore;

            foreach (KeyValuePair<string, bool> kvp in Refactorings)
                settings.SetRefactoring(kvp.Key, kvp.Value);
        }
    }
}
