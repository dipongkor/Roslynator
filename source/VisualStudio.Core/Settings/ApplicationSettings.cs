﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Roslynator.VisualStudio.Settings
{
    public sealed class ApplicationSettings
    {
        public const string FileName = "roslynator.config";

        private static ApplicationSettings _current = new ApplicationSettings();

        public static ApplicationSettings Current
        {
            get { return _current; }
            set
            {
                if (value == null)
                    throw new ArgumentException(nameof(value));

                _current = value;
            }
        }

        public bool PrefixFieldIdentifierWithUnderscore { get; set; } = true;

        public Dictionary<string, bool> Refactorings { get; set; } = new Dictionary<string, bool>(StringComparer.Ordinal);

        public static ApplicationSettings Load(string uri)
        {
            var settings = new ApplicationSettings();

            XDocument doc = XDocument.Load(uri);

            XElement root = doc.Element("Roslynator");

            if (root != null)
            {
                foreach (XElement element in root.Elements())
                {
                    XName name = element.Name;

                    if (name == "Settings")
                        LoadSettingsElement(element, settings);
                }
            }

            return settings;
        }

        private static void LoadSettingsElement(XElement element, ApplicationSettings settings)
        {
            foreach (XElement child in element.Elements())
            {
                XName name = child.Name;

                if (name == "General")
                {
                    LoadPrefixFieldIdentifierWithUnderscore(child, settings);
                }
                else if (name == "Refactorings")
                {
                    LoadRefactorings(child, settings);
                }
            }
        }

        private static void LoadPrefixFieldIdentifierWithUnderscore(XElement parent, ApplicationSettings settings)
        {
            XElement element = parent.Element("PrefixFieldIdentifierWithUnderscore");

            if (element != null)
            {
                bool isEnabled;
                if (element.TryGetAttributeValueAsBoolean("IsEnabled", out isEnabled))
                    settings.PrefixFieldIdentifierWithUnderscore = isEnabled;
            }
        }

        private static void LoadRefactorings(XElement element, ApplicationSettings settings)
        {
            foreach (XElement child in element.Elements("Refactoring"))
                LoadRefactoring(child, settings);
        }

        private static void LoadRefactoring(XElement element, ApplicationSettings settings)
        {
            string id;
            if (element.TryGetAttributeValueAsString("Id", out id))
            {
                bool isEnabled;
                if (element.TryGetAttributeValueAsBoolean("IsEnabled", out isEnabled))
                    settings.Refactorings[id] = isEnabled;
            }
        }
    }
}
