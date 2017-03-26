﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;

namespace Roslynator
{
    internal static class StringUtility
    {
        public static string FirstCharToLower(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value.Length > 0)
            {
                return char.ToLower(value[0]) + value.Substring(1);
            }
            else
            {
                return value;
            }
        }

        public static string FirstCharToLowerInvariant(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value.Length > 0)
            {
                return char.ToLowerInvariant(value[0]) + value.Substring(1);
            }
            else
            {
                return value;
            }
        }

        public static string FirstCharToUpper(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value.Length > 0)
            {
                return char.ToUpper(value[0]) + value.Substring(1);
            }
            else
            {
                return value;
            }
        }

        public static string FirstCharToUpperInvariant(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value.Length > 0)
            {
                return char.ToUpperInvariant(value[0]) + value.Substring(1);
            }
            else
            {
                return value;
            }
        }

        public static bool StartsWithLowerLetter(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.Length > 0
                && char.IsLetter(value[0])
                && char.IsLower(value[0]);
        }

        public static bool StartsWithUpperLetter(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.Length > 0
                && char.IsLetter(value[0])
                && char.IsUpper(value[0]);
        }

        public static bool IsWhitespace(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            for (int i = 0; i < value.Length; i++)
            {
                if (!char.IsWhiteSpace(value[i]))
                    return false;
            }

            return true;
        }

        public static string GetIndent(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value.Length == 0)
                return string.Empty;

            var sb = new StringBuilder();

            foreach (char ch in value)
            {
                if (ch == '\n'
                    || ch == '\r'
                    || !char.IsWhiteSpace(ch))
                {
                    break;
                }

                sb.Append(ch);
            }

            return sb.ToString();
        }

        public static string DoubleBraces(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value
                .Replace("{", "{{")
                .Replace("}", "}}");
        }

        public static string EscapeQuote(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.Replace("\"", @"\" + "\"");
        }

        public static string DoubleQuote(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.Replace("\"", "\"\"");
        }
    }
}
