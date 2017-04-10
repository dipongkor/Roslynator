﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Syntax
{
    public struct SimpleAssignmentExpression : IEquatable<SimpleAssignmentExpression>
    {
        private SimpleAssignmentExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            Left = left;
            Right = right;
        }

        public ExpressionSyntax Left { get; }
        public ExpressionSyntax Right { get; }

        public AssignmentExpressionSyntax AssignmentExpression
        {
            get { return (AssignmentExpressionSyntax)Parent; }
        }

        private SyntaxNode Parent
        {
            get { return Left?.Parent; }
        }

        public static SimpleAssignmentExpression Create(AssignmentExpressionSyntax assignmentExpression)
        {
            if (assignmentExpression == null)
                throw new ArgumentNullException(nameof(assignmentExpression));

            if (!assignmentExpression.IsKind(SyntaxKind.SimpleAssignmentExpression))
                throw new ArgumentException("", nameof(assignmentExpression));

            return new SimpleAssignmentExpression(assignmentExpression.Left, assignmentExpression.Right);
        }

        public static bool TryCreate(SyntaxNode assignmentExpression, out SimpleAssignmentExpression result)
        {
            if (assignmentExpression?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true)
                return TryCreateCore((AssignmentExpressionSyntax)assignmentExpression, out result);

            result = default(SimpleAssignmentExpression);
            return false;
        }

        public static bool TryCreate(AssignmentExpressionSyntax assignmentExpression, out SimpleAssignmentExpression result)
        {
            if (assignmentExpression?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true)
                return TryCreateCore(assignmentExpression, out result);

            result = default(SimpleAssignmentExpression);
            return false;
        }

        private static bool TryCreateCore(AssignmentExpressionSyntax assignmentExpression, out SimpleAssignmentExpression simpleAssignment)
        {
            ExpressionSyntax left = assignmentExpression.Left;

            ExpressionSyntax right = assignmentExpression.Right;

            simpleAssignment = new SimpleAssignmentExpression(left, right);
            return true;
        }

        public bool Equals(SimpleAssignmentExpression other)
        {
            return Parent == other.Parent;
        }

        public override bool Equals(object obj)
        {
            return obj is SimpleAssignmentExpression
                && Equals((SimpleAssignmentExpression)obj);
        }

        public override int GetHashCode()
        {
            return Parent?.GetHashCode() ?? 0;
        }

        public static bool operator ==(SimpleAssignmentExpression left, SimpleAssignmentExpression right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SimpleAssignmentExpression left, SimpleAssignmentExpression right)
        {
            return !left.Equals(right);
        }
    }
}
