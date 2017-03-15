﻿## Roslynator Refactorings

#### Add boolean comparison

* **Syntax**: boolean? expression in place where must be boolean expression

![Add boolean comparison](../../images/refactorings/AddBooleanComparison.png)

#### Add braces

* **Syntax**: do statement, else clause, fixed statement, for statement, foreach statement, if statement, lock statement, using statement, while statement
* **Scope**: embedded statement

![Add braces](../../images/refactorings/AddBraces.png)

#### Add braces to if\-else

* **Syntax**: if\-else chain
* **Scope**: embedded statement

![Add braces to if\-else](../../images/refactorings/AddBracesToIfElse.png)

#### Add braces to switch section

* **Syntax**: switch section
* **Scope**: statements

![Add braces to switch section](../../images/refactorings/AddBracesToSwitchSection.png)

#### Add braces to switch sections

* **Syntax**: switch statement
* **Scope**: switch keyword

![Add braces to switch sections](../../images/refactorings/AddBracesToSwitchSections.png)

#### Add cast expression

* **Syntax**: argument, assignment expression, return statement, variable declaration

![Add cast expression](../../images/refactorings/AddCastExpressionToArgument.png)

![Add cast expression](../../images/refactorings/AddCastExpressionToAssignmentExpression.png)

![Add cast expression](../../images/refactorings/AddCastExpressionToReturnStatement.png)

![Add cast expression](../../images/refactorings/AddCastExpressionToVariableDeclaration.png)

#### Add default value to parameter

* **Syntax**: parameter without default value
* **Scope**: identifier

![Add default value to parameter](../../images/refactorings/AddDefaultValueToParameter.png)

#### Add default value to return statement

* **Syntax**: return statement without expression

![Add default value to return statement](../../images/refactorings/AddDefaultValueToReturnStatement.png)

#### Add exception to documentation comment

* **Syntax**: throw statement

![Add exception to documentation comment](../../images/refactorings/AddExceptionToDocumentationComment.png)

#### Add identifier to variable declaration

* **Syntax**: variable declaration

![Add identifier to variable declaration](../../images/refactorings/AddIdentifierToVariableDeclaration.png)

#### Add parameter name to argument

* **Syntax**: argument list

![Add parameter name to argument](../../images/refactorings/AddParameterNameToArgument.png)

#### Add parameter name to parameter

* **Syntax**: parameter
* **Scope**: missing identifier

![Add parameter name to parameter](../../images/refactorings/AddParameterNameToParameter.png)

#### Add type parameter

* **Syntax**: class declaration, struct declaration, interface declaration, delegate declaration, method declaration, local function

![Add type parameter](../../images/refactorings/AddTypeParameter.png)

#### Add using directive

* **Syntax**: qualified name
* **Scope**: selected namespace

![Add using directive](../../images/refactorings/AddUsingDirective.png)

#### Add using static directive

* **Syntax**: member access expression \(public or internal static class\)
* **Scope**: selected class name

![Add using static directive](../../images/refactorings/AddUsingStaticDirective.png)

#### Call 'ConfigureAwait\(false\)'

* **Syntax**: awaitable method invocation
* **Scope**: method name

![Call 'ConfigureAwait\(false\)'](../../images/refactorings/CallConfigureAwait.png)

#### Call extension method as instance method

* **Syntax**: method invocation

![Call extension method as instance method](../../images/refactorings/CallExtensionMethodAsInstanceMethod.png)

#### Call 'To\.\.\.' method \(ToString, ToArray, ToList\)

* **Syntax**: argument, assignment expression, return statement, variable declaration

![Call 'To\.\.\.' method \(ToString, ToArray, ToList\)](../../images/refactorings/CallToMethod.png)

#### Change explicit type to 'var'

* **Syntax**: variable declaration, foreach statement
* **Scope**: type

![Change explicit type to 'var'](../../images/refactorings/ChangeExplicitTypeToVar.png)

#### Change method return type to 'void'

* **Syntax**: method

![Change method return type to 'void'](../../images/refactorings/ChangeMethodReturnTypeToVoid.png)

#### Change method/property/indexer type according to return expression

* **Syntax**: return statement in method/property/indexer

![Change method/property/indexer type according to return expression](../../images/refactorings/ChangeMemberTypeAccordingToReturnExpression.png)

#### Change method/property/indexer type according to yield return expression

* **Syntax**: yield return statement in method/property/indexer

![Change method/property/indexer type according to yield return expression](../../images/refactorings/ChangeMemberTypeAccordingToYieldReturnExpression.png)

#### Change type according to expression

* **Syntax**: variable declaration, foreach statement
* **Scope**: type

![Change type according to expression](../../images/refactorings/ChangeTypeAccordingToExpression.png)

![Change type according to expression](../../images/refactorings/ChangeForEachTypeAccordingToExpression.png)

#### Change 'var' to explicit type

* **Syntax**: variable declaration, foreach statetement
* **Scope**: type

![Change 'var' to explicit type](../../images/refactorings/ChangeVarToExplicitType.png)

#### Check expression for null

* **Syntax**: local declaration \(identifier\), assignment expression \(left\)

![Check expression for null](../../images/refactorings/CheckExpressionForNull.png)

#### Check parameter for null

* **Syntax**: parameter
* **Scope**: parameter identifier

![Check parameter for null](../../images/refactorings/CheckParameterForNull.png)

#### Collapse to initalizer

* **Syntax**: object creation followed with assignment\(s\)

![Collapse to initalizer](../../images/refactorings/CollapseToInitializer.png)

#### Comment out member

* **Syntax**: method, constructor, property, indexer, operator, event, namespace, class, struct, interface
* **Scope**: opening or closing brace

![Comment out member](../../images/refactorings/CommentOutMember.png)

#### Comment out statement

* **Syntax**: do statement, fixed statement, for statement, foreach statement, checked statement, if statement, lock statement, switch statement, try statement, unchecked statement, unsafe statement, using statement, while statement
* **Scope**: opening or closing brace

![Comment out statement](../../images/refactorings/CommentOutStatement.png)

#### Copy documentation comment from base member

* **Syntax**: constructor, method, property, indexer, event

![Copy documentation comment from base member](../../images/refactorings/CopyDocumentationCommentFromBaseMember.png)

![Copy documentation comment from base member](../../images/refactorings/CopyDocumentationCommentFromImplementedMember.png)

#### Duplicate argument

* **Syntax**: missing argument

![Duplicate argument](../../images/refactorings/DuplicateArgument.png)

#### Duplicate member

* **Syntax**: method, constructor, property, indexer, operator, event, namespace, class, struct, interface
* **Scope**: opening or closing brace

![Duplicate member](../../images/refactorings/DuplicateMember.png)

#### Duplicate parameter

* **Syntax**: missing parameter

![Duplicate parameter](../../images/refactorings/DuplicateParameter.png)

#### Duplicate statement

* **Syntax**: do statement, fixed statement, for statement, foreach statement, checked statement, if statement, lock statement, switch statement, try statement, unchecked statement, unsafe statement, using statement, while statement
* **Scope**: opening or closing brace

![Duplicate statement](../../images/refactorings/DuplicateStatement.png)

#### Expand assignment expression

* **Syntax**: assignment expression
* **Scope**: operator

![Expand assignment expression](../../images/refactorings/ExpandAssignmentExpression.png)

#### Expand coalesce expression

* **Syntax**: coalesce expression
* **Scope**: ?? operator

![Expand coalesce expression](../../images/refactorings/ExpandCoalesceExpression.png)

#### Expand event

* **Syntax**: event field declaration

![Expand event](../../images/refactorings/ExpandEvent.png)

#### Expand expression body

* **Syntax**: expression body

![Expand expression body](../../images/refactorings/ExpandExpressionBody.png)

#### Expand initializer

* **Syntax**: initializer

![Expand initializer](../../images/refactorings/ExpandInitializer.png)

#### Expand lambda expression body

* **Syntax**: lambda expression
* **Scope**: body

![Expand lambda expression body](../../images/refactorings/ExpandLambdaExpressionBody.png)

#### Expand property

* **Syntax**: auto\-property

![Expand property](../../images/refactorings/ExpandProperty.png)

#### Expand property and add backing field

* **Syntax**: auto\-property

![Expand property and add backing field](../../images/refactorings/ExpandPropertyAndAddBackingField.png)

#### Extract declaration from using statement

* **Syntax**: using statement
* **Scope**: declaration

![Extract declaration from using statement](../../images/refactorings/ExtractDeclarationFromUsingStatement.png)

#### Extract expression from condition

* **Syntax**: if statement, while statement
* **Scope**: condition

![Extract expression from condition](../../images/refactorings/ExtractExpressionFromCondition.png)

#### Extract generic type

* **Syntax**: generic name with single type argument
* **Scope**: type argument

![Extract generic type](../../images/refactorings/ExtractGenericType.png)

#### Extract statement\(s\)

* **Syntax**: else clause, fixed statement, for statement, foreach statement, checked statement, if statement, lock statement, try statement, unsafe statement, using statement, while statement

![Extract statement\(s\)](../../images/refactorings/ExtractStatement.png)

#### Extract type declaration to a new file

* **Syntax**: class declaration, struct declaration, interface declaration, enum declaration, delegate declaration
* **Scope**: identifier

![Extract type declaration to a new file](../../images/refactorings/ExtractTypeDeclarationToNewFile.png)

#### Format accessor braces

* **Syntax**: get accessor, set accessor, add accessor, remove accessor
* **Scope**: block

![Format accessor braces](../../images/refactorings/FormatAccessorBracesOnMultipleLines.png)

![Format accessor braces](../../images/refactorings/FormatAccessorBracesOnSingleLine.png)

#### Format argument list

* **Syntax**: argument list

![Format argument list](../../images/refactorings/FormatEachArgumentOnSeparateLine.png)

![Format argument list](../../images/refactorings/FormatAllArgumentsOnSingleLine.png)

#### Format binary expression

* **Syntax**: logical and/or expression, bitwise and/or expression

![Format binary expression](../../images/refactorings/FormatBinaryExpression.png)

#### Format conditional expression

* **Syntax**: conditional expression

![Format conditional expression](../../images/refactorings/FormatConditionalExpressionOnMultipleLines.png)

![Format conditional expression](../../images/refactorings/FormatConditionalExpressionOnSingleLine.png)

#### Format expression chain

* **Syntax**: expression chain

![Format expression chain](../../images/refactorings/FormatExpressionChainOnMultipleLines.png)

![Format expression chain](../../images/refactorings/FormatExpressionChainOnSingleLine.png)

#### Format initializer

* **Syntax**: initializer

![Format initializer](../../images/refactorings/FormatInitializerOnMultipleLines.png)

![Format initializer](../../images/refactorings/FormatInitializerOnSingleLine.png)

#### Format parameter list

* **Syntax**: parameter list

![Format parameter list](../../images/refactorings/FormatEachParameterOnSeparateLine.png)

![Format parameter list](../../images/refactorings/FormatAllParametersOnSingleLine.png)

#### Generate base constructors

* **Syntax**: class declaration
* **Scope**: identifier

![Generate base constructors](../../images/refactorings/GenerateBaseConstructors.png)

#### Generate combined enum member

* **Syntax**: enum declaration \(with FlagsAttribute\)

![Generate combined enum member](../../images/refactorings/GenerateCombinedEnumMember.png)

#### Generate enum member

* **Syntax**: enum declaration \(with FlagsAttribute\)

![Generate enum member](../../images/refactorings/GenerateEnumMember.png)

#### Generate enum values

* **Syntax**: enum declaration \(with FlagsAttribute\)

![Generate enum values](../../images/refactorings/GenerateEnumValues.png)

#### Generate event invoking method

* **Syntax**: event
* **Scope**: identifier

![Generate event invoking method](../../images/refactorings/GenerateEventInvokingMethod.png)

#### Generate switch sections

* **Syntax**: switch statement \(that is empty or contains only default section\)

![Generate switch sections](../../images/refactorings/GenerateSwitchSections.png)

#### Implement IEquatable\<T\>

* **Syntax**: class declaration, struct declaration, interface declaration
* **Scope**: base list

![Implement IEquatable\<T\>](../../images/refactorings/ImplementIEquatableOfT.png)

#### Initialize local with default value

* **Syntax**: local declaration without initializer
* **Scope**: identifier

![Initialize local with default value](../../images/refactorings/InitializeLocalWithDefaultValue.png)

#### Inline alias expression

* **Syntax**: using alias directive
* **Scope**: identifier

![Inline alias expression](../../images/refactorings/InlineAliasExpression.png)

#### Inline method

* **Syntax**: static/extension method invocation

![Inline method](../../images/refactorings/InlineMethod.png)

#### Insert string interpolation

* **Syntax**: string literal, interpolated string

![Insert string interpolation](../../images/refactorings/InsertInterpolationIntoStringLiteral.png)

![Insert string interpolation](../../images/refactorings/InsertInterpolationIntoInterpolatedString.png)

#### Introduce and initialize field

* **Syntax**: constructor parameter

![Introduce and initialize field](../../images/refactorings/IntroduceAndInitializeField.png)

#### Introduce and initialize property

* **Syntax**: constructor parameter

![Introduce and initialize property](../../images/refactorings/IntroduceAndInitializeProperty.png)

#### Introduce constructor

* **Syntax**: field, property

![Introduce constructor](../../images/refactorings/IntroduceConstructor.png)

#### Introduce field to lock on

* **Syntax**: lock statement
* **Scope**: missing expression

![Introduce field to lock on](../../images/refactorings/IntroduceFieldToLockOn.png)

#### Introduce local from statement that returns value

* **Syntax**: expression statement

![Introduce local from statement that returns value](../../images/refactorings/IntroduceLocalFromStatementThatReturnsValue.png)

#### Make member abstract

* **Syntax**: non\-abstract indexer/method/property in abstract class
* **Scope**: indexer/method/property header

![Make member abstract](../../images/refactorings/MakeMemberAbstract.png)

#### Make member virtual

* **Syntax**: method declaration, indexer declaration

![Make member virtual](../../images/refactorings/MakeMemberVirtual.png)

#### Mark containing class as abstract

* **Syntax**: method declaration, property declaration, indexer declaration, event declaration

![Mark containing class as abstract](../../images/refactorings/MarkContainingClassAsAbstract.png)

#### Mark member as static

* **Syntax**: non\-static field/method/property/event in static class

![Mark member as static](../../images/refactorings/MarkMemberAsStatic.png)

#### Merge assignment expression with return statement

* **Syntax**: assignment expression followed with return statement

![Merge assignment expression with return statement](../../images/refactorings/MergeAssignmentExpressionWithReturnStatement.png)

#### Merge attributes

* **Syntax**: selected attribute lists

![Merge attributes](../../images/refactorings/MergeAttributes.png)

#### Merge if statements

* **Syntax**: selected if statements

![Merge if statements](../../images/refactorings/MergeIfStatements.png)

#### Merge interpolation into interpolated string

* **Syntax**: interpolation

![Merge interpolation into interpolated string](../../images/refactorings/MergeInterpolationIntoInterpolatedString.png)

#### Merge local declarations

* **Syntax**: local declarations with same type

![Merge local declarations](../../images/refactorings/MergeLocalDeclarations.png)

#### Merge string expressions

* **Syntax**: concatenated string expressions

![Merge string expressions](../../images/refactorings/MergeStringExpressions.png)

![Merge string expressions](../../images/refactorings/MergeStringLiterals.png)

![Merge string expressions](../../images/refactorings/MergeStringLiteralsIntoMultilineStringLiteral.png)

#### Negate binary expression

* **Syntax**: logical and/or expression

![Negate binary expression](../../images/refactorings/NegateBinaryExpression.png)

#### Negate boolean literal

* **Syntax**: boolean literal

![Negate boolean literal](../../images/refactorings/NegateBooleanLiteral.png)

#### Negate is expression

* **Syntax**: is expression
* **Scope**: operator

![Negate is expression](../../images/refactorings/NegateIsExpression.png)

#### Negate operator

* **Syntax**: \!=, &&, ||, \<, \<=, ==, \>, \>=

![Negate operator](../../images/refactorings/NegateOperator.png)

#### Notify property changed

* **Syntax**: property in class/struct that implements INotifyPropertyChanged
* **Scope**: setter

![Notify property changed](../../images/refactorings/NotifyPropertyChanged.png)

#### Parenthesize expression

* **Syntax**: selected expression

![Parenthesize expression](../../images/refactorings/ParenthesizeExpression.png)

#### Promote local to parameter

* **Syntax**: local declaration in method

![Promote local to parameter](../../images/refactorings/PromoteLocalToParameter.png)

#### Remove all comments

* **Syntax**: singleline/multiline comment, singleline/multiline documentation documentation comment

![Remove all comments](../../images/refactorings/RemoveAllComments.png)

#### Remove all comments \(except documentation comments\)

* **Syntax**: singleline/multiline comment

![Remove all comments \(except documentation comments\)](../../images/refactorings/RemoveAllCommentsExceptDocumentationComments.png)

#### Remove all documentation comments

* **Syntax**: singleline/multiline documentation comment

![Remove all documentation comments](../../images/refactorings/RemoveAllDocumentationComments.png)

#### Remove all member declarations

* **Syntax**: namespace, class, struct, interface
* **Scope**: opening or closing brace

![Remove all member declarations](../../images/refactorings/RemoveAllMemberDeclarations.png)

#### Remove all preprocessor directives

* **Syntax**: preprocessor directive

![Remove all preprocessor directives](../../images/refactorings/RemoveAllPreprocessorDirectives.png)

#### Remove all region directives

* **Syntax**: region directive

![Remove all region directives](../../images/refactorings/RemoveAllRegionDirectives.png)

#### Remove all statements

* **Syntax**: method, constructor, operator
* **Scope**: opening or closing brace

![Remove all statements](../../images/refactorings/RemoveAllStatements.png)

#### Remove all switch sections

* **Syntax**: switch statement
* **Scope**: opening or closing brace

![Remove all switch sections](../../images/refactorings/RemoveAllSwitchSections.png)

#### Remove braces

* **Syntax**: do statement, else clause, fixed statement, for statement, foreach statement, if statement, lock statement, using statement, while statement
* **Scope**: block with a single statement

![Remove braces](../../images/refactorings/RemoveBraces.png)

#### Remove braces from if\-else

* **Syntax**: if\-else chain
* **Scope**: embedded statement

![Remove braces from if\-else](../../images/refactorings/RemoveBracesFromIfElse.png)

#### Remove braces from switch section

* **Syntax**: switch section
* **Scope**: block

![Remove braces from switch section](../../images/refactorings/RemoveBracesFromSwitchSection.png)

#### Remove braces from switch sections

* **Syntax**: switch statement
* **Scope**: switch keyword

![Remove braces from switch sections](../../images/refactorings/RemoveBracesFromSwitchSections.png)

#### Remove comment

* **Syntax**: singleline/multiline comment, singleline/multiline xml documentation comment

![Remove comment](../../images/refactorings/RemoveComment.png)

#### Remove condition from last else clause

* **Syntax**: else clause
* **Scope**: else keyword

![Remove condition from last else clause](../../images/refactorings/RemoveConditionFromLastElse.png)

#### Remove directive and related directives

* **Syntax**: preprocessor directive, region directive

![Remove directive and related directives](../../images/refactorings/RemoveDirectiveAndRelatedDirectives.png)

#### Remove empty lines

* **Syntax**: selected lines

![Remove empty lines](../../images/refactorings/RemoveEmptyLines.png)

#### Remove interpolation

* **Syntax**: string interpolation
* **Scope**: opening or closing brace

![Remove interpolation](../../images/refactorings/RemoveInterpolation.png)

#### Remove member

* **Syntax**: method, constructor, property, indexer, operator, event, namespace, class, struct, interface
* **Scope**: opening or closing brace

![Remove member](../../images/refactorings/RemoveMember.png)

#### Remove member declarations above/below

* **Syntax**: empty line between member declarations

![Remove member declarations above/below](../../images/refactorings/RemoveMemberDeclarations.png)

#### Remove parameter name from argument

* **Syntax**: selected argument\(s\)

![Remove parameter name from argument](../../images/refactorings/RemoveParameterNameFromArgument.png)

#### Remove parentheses

* **Syntax**: parenthesized expression
* **Scope**: opening or closing parenthesis

![Remove parentheses](../../images/refactorings/RemoveParentheses.png)

#### Remove property initializer

* **Syntax**: property initializer

![Remove property initializer](../../images/refactorings/RemovePropertyInitializer.png)

#### Remove region

* **Syntax**: region directive

![Remove region](../../images/refactorings/RemoveRegion.png)

#### Remove statement

* **Syntax**: do statement, fixed statement, for statement, foreach statement, checked statement, if statement, lock statement, switch statement, try statement, unchecked statement, unsafe statement, using statement, while statement
* **Scope**: open/close brace

![Remove statement](../../images/refactorings/RemoveStatement.png)

#### Remove statements from switch sections

* **Syntax**: selected switch sections

![Remove statements from switch sections](../../images/refactorings/RemoveStatementsFromSwitchSections.png)

#### Rename backing field according to property name

* **Syntax**: field identifier inside property declaration

![Rename backing field according to property name](../../images/refactorings/RenameBackingFieldAccordingToPropertyName.png)

#### Rename identifier according to type name

* **Syntax**: foreach statement, local/field/constant declaration
* **Scope**: identifier

![Rename identifier according to type name](../../images/refactorings/RenameForEachIdentifierAccordingToTypeName.png)

![Rename identifier according to type name](../../images/refactorings/RenameFieldIdentifierAccordingToTypeName.png)

#### Rename method according to type name

* **Syntax**: method

![Rename method according to type name](../../images/refactorings/RenameMethodAccordingToTypeName.png)

#### Rename parameter according to its type name

* **Syntax**: parameter
* **Scope**: parameter identifier

![Rename parameter according to its type name](../../images/refactorings/RenameParameterAccordingToTypeName.png)

#### Rename property according to type name

* **Syntax**: property identifier

![Rename property according to type name](../../images/refactorings/RenamePropertyAccordingToTypeName.png)

#### Replace Any with All \(or All with Any\)

* **Syntax**: Any\(Func\<T, bool\> or All\(Func\<T, bool\> from System\.Linq\.Enumerable namespace
* **Scope**: method name

![Replace Any with All \(or All with Any\)](../../images/refactorings/ReplaceAnyWithAllOrAllWithAny.png)

#### Replace as expression with cast expression

* **Syntax**: as expression

![Replace as expression with cast expression](../../images/refactorings/ReplaceAsWithCast.png)

#### Replace cast expression with as expression

* **Syntax**: cast expression

![Replace cast expression with as expression](../../images/refactorings/ReplaceCastWithAs.png)

#### Replace conditional expression with expression

* **Syntax**: conditional expression
* **Scope**: selected true/false expression

![Replace conditional expression with expression](../../images/refactorings/ReplaceConditionalExpressionWithExpression.png)

#### Replace conditional expression with if\-else

* **Syntax**: conditional expression

![Replace conditional expression with if\-else](../../images/refactorings/ReplaceConditionalExpressionWithIfElse.png)

#### Replace constant with field

* **Syntax**: constant declaration

![Replace constant with field](../../images/refactorings/ReplaceConstantWithField.png)

#### Replace Count property with Length property \(or Length with Count\)

* **Syntax**: member access expression
* **Scope**: name

![Replace Count property with Length property \(or Length with Count\)](../../images/refactorings/ReplaceCountWithLengthOrLengthWithCount.png)

#### Replace do statement with while statement

* **Syntax**: do statement
* **Scope**: do keyword

![Replace do statement with while statement](../../images/refactorings/ReplaceDoStatementWithWhileStatement.png)

#### Replace equals expression with String\.Equals

* **Syntax**: equals expression, not equals expression
* **Scope**: operator

![Replace equals expression with String\.Equals](../../images/refactorings/ReplaceEqualsExpressionWithStringEquals.png)

#### Replace equals expression with String\.IsNullOrEmpty

* **Syntax**: equals expression, not equals expression
* **Scope**: operator

![Replace equals expression with String\.IsNullOrEmpty](../../images/refactorings/ReplaceEqualsExpressionWithStringIsNullOrEmpty.png)

#### Replace equals expression with String\.IsNullOrWhiteSpace

* **Syntax**: equals expression, not equals expression
* **Scope**: operator

![Replace equals expression with String\.IsNullOrWhiteSpace](../../images/refactorings/ReplaceEqualsExpressionWithStringIsNullOrWhiteSpace.png)

#### Replace expression with constant value

* **Syntax**: expression that has constant value

![Replace expression with constant value](../../images/refactorings/ReplaceExpressionWithConstantValue.png)

#### Replace field with constant

* **Syntax**: read\-only field

![Replace field with constant](../../images/refactorings/ReplaceFieldWithConstant.png)

#### Replace for statement with foreach statement

* **Syntax**: for statement

![Replace for statement with foreach statement](../../images/refactorings/ReplaceForWithForEach.png)

#### Replace for statement with while statement

* **Syntax**: for statement

![Replace for statement with while statement](../../images/refactorings/ReplaceForWithWhile.png)

#### Replace foreach statement with for statement

* **Syntax**: foreach statement

![Replace foreach statement with for statement](../../images/refactorings/ReplaceForEachWithFor.png)

#### Replace hexadecimal literal with decimal literal

* **Syntax**: hexadecimal literal

![Replace hexadecimal literal with decimal literal](../../images/refactorings/ReplaceHexadecimalLiteralWithDecimalLiteral.png)

#### Replace if\-else with switch statement

* **Syntax**: if statement

![Replace if\-else with switch statement](../../images/refactorings/ReplaceIfElseWithSwitch.png)

#### Replace increment operator with decrement operator

* **Syntax**: prefix/postfix unary expression

![Replace increment operator with decrement operator](../../images/refactorings/ReplaceIncrementOperatorWithDecrementOperator.png)

#### Replace interpolated string with interpolation expression

* **Syntax**: interpolated string with single interpolation and no text
* **Scope**: interpolation

![Replace interpolated string with interpolation expression](../../images/refactorings/ReplaceInterpolatedStringWithInterpolationExpression.png)

#### Replace interpolated string with string literal

* **Syntax**: Interpolated string without any interpolation

![Replace interpolated string with string literal](../../images/refactorings/ReplaceInterpolatedStringWithStringLiteral.png)

#### Replace method group with lambda

* **Syntax**: method group

![Replace method group with lambda](../../images/refactorings/ReplaceMethodGroupWithLambda.png)

#### Replace method with property

* **Syntax**: method
* **Scope**: method header

![Replace method with property](../../images/refactorings/ReplaceMethodWithProperty.png)

#### Replace null literal expression with default expression

* **Syntax**: argument

![Replace null literal expression with default expression](../../images/refactorings/ReplaceNullLiteralExpressionWithDefaultExpression.png)

#### Replace prefix operator to postfix operator

* **Syntax**: prefix/postfix unary expression

![Replace prefix operator to postfix operator](../../images/refactorings/ReplacePrefixOperatorWithPostfixOperator.png)

#### Replace property with method

* **Syntax**: read\-only property
* **Scope**: property header

![Replace property with method](../../images/refactorings/ReplacePropertyWithMethod.png)

#### Replace regular string literal with verbatim string literal

* **Syntax**: regular string literal

![Replace regular string literal with verbatim string literal](../../images/refactorings/ReplaceRegularStringLiteralWithVerbatimStringLiteral.png)

#### Replace statement with if statement

* **Syntax**: return statement, yield return statement, assignment statement, local declaration statement

![Replace statement with if statement](../../images/refactorings/ReplaceReturnStatementWithIfStatement.png)

#### Replace string literal with character literal

* **Syntax**: string literal

![Replace string literal with character literal](../../images/refactorings/ReplaceStringLiteralWithCharacterLiteral.png)

#### Replace String\.Contains with String\.IndexOf

* **Syntax**: method invocation
* **Scope**: method name

![Replace String\.Contains with String\.IndexOf](../../images/refactorings/ReplaceStringContainsWithStringIndexOf.png)

#### Replace String\.Format with interpolated string

* **Syntax**: String\.Format method

![Replace String\.Format with interpolated string](../../images/refactorings/ReplaceStringFormatWithInterpolatedString.png)

#### Replace switch statement with if\-else

* **Syntax**: switch statement
* **Scope**: switch keyword

![Replace switch statement with if\-else](../../images/refactorings/ReplaceSwitchWithIfElse.png)

#### Replace verbatim string literal with regular string literal

* **Syntax**: verbatim string literal

![Replace verbatim string literal with regular string literal](../../images/refactorings/ReplaceVerbatimStringLiteralWithRegularStringLiteral.png)

#### Replace verbatim string literal with regular string literals

* **Syntax**: multiline verbatim string literal

![Replace verbatim string literal with regular string literals](../../images/refactorings/ReplaceVerbatimStringLiteralWithRegularStringLiterals.png)

#### Replace while statement with do statement

* **Syntax**: while statement
* **Scope**: while keyword

![Replace while statement with do statement](../../images/refactorings/ReplaceWhileStatementWithDoStatement.png)

#### Replace while statement with for statement

* **Syntax**: while statement

![Replace while statement with for statement](../../images/refactorings/ReplaceWhileWithFor.png)

#### Reverse for loop

* **Syntax**: for statement

![Reverse for loop](../../images/refactorings/ReverseForLoop.png)

#### Simplify if

* **Syntax**: if statement

![Simplify if](../../images/refactorings/SimplifyIf.png)

#### Simplify lambda expression

* **Syntax**: lambda expression with block with single single\-line statement
* **Scope**: body

![Simplify lambda expression](../../images/refactorings/SimplifyLambdaExpression.png)

#### Sort member declarations

* **Syntax**: namespace declarations, class declarations, struct declarations, interface declarations, enum declarations
* **Scope**: selected member declarations

![Sort member declarations](../../images/refactorings/SortMembersByKind.png)

![Sort member declarations](../../images/refactorings/SortMembersByName.png)

![Sort member declarations](../../images/refactorings/SortEnumMembersByName.png)

![Sort member declarations](../../images/refactorings/SortEnumMembersByValue.png)

#### Split attributes

* **Syntax**: selected attribute list

![Split attributes](../../images/refactorings/SplitAttributes.png)

#### Split switch labels

* **Syntax**: selected switch labels

![Split switch labels](../../images/refactorings/SplitSwitchLabels.png)

#### Split variable declaration

* **Syntax**: local declaration, field declaration, event field declaration

![Split variable declaration](../../images/refactorings/SplitLocalDeclaration.png)

#### Swap expressions in binary expression

* **Syntax**: logical and/or expression
* **Scope**: binary operator

![Swap expressions in binary expression](../../images/refactorings/SwapExpressionsInBinaryExpression.png)

#### Swap expressions in conditional expression

* **Syntax**: conditional expression
* **Scope**: condition

![Swap expressions in conditional expression](../../images/refactorings/SwapExpressionsInConditionalExpression.png)

#### Swap member declarations

* **Syntax**: empty line between member declarations

![Swap member declarations](../../images/refactorings/SwapMemberDeclarations.png)

#### Swap statements in if\-else

* **Syntax**: if statement
* **Scope**: if keyword

![Swap statements in if\-else](../../images/refactorings/SwapStatementsInIfElse.png)

#### Uncomment

* **Syntax**: single\-line comment\(s\)

![Uncomment](../../images/refactorings/Uncomment.png)

#### Use "" instead of String\.Empty

* **Syntax**: String\.Empty field

![Use "" instead of String\.Empty](../../images/refactorings/UseEmptyStringLiteralInsteadOfStringEmpty.png)

#### Use bitwise operation instead of calling 'HasFlag'

* **Syntax**: Enum\.HasFlag method invocation

![Use bitwise operation instead of calling 'HasFlag'](../../images/refactorings/UseBitwiseOperationInsteadOfCallingHasFlag.png)

#### Use coalesce expression instead of if

* **Syntax**: if statement

![Use coalesce expression instead of if](../../images/refactorings/UseCoalesceExpressionInsteadOfIf.png)

#### Use conditional expression instead of if

* **Syntax**: if statement

![Use conditional expression instead of if](../../images/refactorings/UseConditionalExpressionInsteadOfIf.png)

#### Use element access instead of 'First/Last'ElementAt' method

* **Syntax**: First/Last/ElementAt method invocation
* **Scope**: method name

![Use element access instead of 'First/Last'ElementAt' method](../../images/refactorings/UseElementAccessInsteadOfEnumerableMethod.png)

#### Use expression\-bodied member

* **Syntax**: method, property, indexer, operator
* **Scope**: body or accessor list

![Use expression\-bodied member](../../images/refactorings/UseExpressionBodiedMember.png)

#### Use lambda expression instead of anonymous method

* **Syntax**: anonymous method
* **Scope**: delegate keyword

![Use lambda expression instead of anonymous method](../../images/refactorings/UseLambdaExpressionInsteadOfAnonymousMethod.png)

#### Use String\.Empty instead of ""

* **Syntax**: empty string literal

![Use String\.Empty instead of ""](../../images/refactorings/UseStringEmptyInsteadOfEmptyStringLiteral.png)

#### Wrap in \#if directive

* **Syntax**: selected lines

![Wrap in \#if directive](../../images/refactorings/WrapInIfDirective.png)

#### Wrap in condition

* **Syntax**: selected statements

![Wrap in condition](../../images/refactorings/WrapInCondition.png)

#### Wrap in else clause

* **Syntax**: statement

![Wrap in else clause](../../images/refactorings/WrapInElseClause.png)

#### Wrap in region

* **Syntax**: selected lines

![Wrap in region](../../images/refactorings/WrapInRegion.png)

#### Wrap in try\-catch

* **Syntax**: selected statements

![Wrap in try\-catch](../../images/refactorings/WrapInTryCatch.png)

#### Wrap in using statement

* **Syntax**: local declaration of type that implements IDisposable

![Wrap in using statement](../../images/refactorings/WrapInUsingStatement.png)
