//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.12.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from SubstraitType.g4 by ANTLR 4.12.0

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using Antlr4.Runtime.Misc;
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="SubstraitTypeParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.12.0")]
[System.CLSCompliant(false)]
public interface ISubstraitTypeListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="SubstraitTypeParser.start"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStart([NotNull] SubstraitTypeParser.StartContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SubstraitTypeParser.start"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStart([NotNull] SubstraitTypeParser.StartContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Boolean</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBoolean([NotNull] SubstraitTypeParser.BooleanContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Boolean</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBoolean([NotNull] SubstraitTypeParser.BooleanContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>i8</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterI8([NotNull] SubstraitTypeParser.I8Context context);
	/// <summary>
	/// Exit a parse tree produced by the <c>i8</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitI8([NotNull] SubstraitTypeParser.I8Context context);
	/// <summary>
	/// Enter a parse tree produced by the <c>i16</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterI16([NotNull] SubstraitTypeParser.I16Context context);
	/// <summary>
	/// Exit a parse tree produced by the <c>i16</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitI16([NotNull] SubstraitTypeParser.I16Context context);
	/// <summary>
	/// Enter a parse tree produced by the <c>i32</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterI32([NotNull] SubstraitTypeParser.I32Context context);
	/// <summary>
	/// Exit a parse tree produced by the <c>i32</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitI32([NotNull] SubstraitTypeParser.I32Context context);
	/// <summary>
	/// Enter a parse tree produced by the <c>i64</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterI64([NotNull] SubstraitTypeParser.I64Context context);
	/// <summary>
	/// Exit a parse tree produced by the <c>i64</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitI64([NotNull] SubstraitTypeParser.I64Context context);
	/// <summary>
	/// Enter a parse tree produced by the <c>fp32</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFp32([NotNull] SubstraitTypeParser.Fp32Context context);
	/// <summary>
	/// Exit a parse tree produced by the <c>fp32</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFp32([NotNull] SubstraitTypeParser.Fp32Context context);
	/// <summary>
	/// Enter a parse tree produced by the <c>fp64</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFp64([NotNull] SubstraitTypeParser.Fp64Context context);
	/// <summary>
	/// Exit a parse tree produced by the <c>fp64</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFp64([NotNull] SubstraitTypeParser.Fp64Context context);
	/// <summary>
	/// Enter a parse tree produced by the <c>string</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterString([NotNull] SubstraitTypeParser.StringContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>string</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitString([NotNull] SubstraitTypeParser.StringContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>binary</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBinary([NotNull] SubstraitTypeParser.BinaryContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>binary</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBinary([NotNull] SubstraitTypeParser.BinaryContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>timestamp</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTimestamp([NotNull] SubstraitTypeParser.TimestampContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>timestamp</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTimestamp([NotNull] SubstraitTypeParser.TimestampContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>timestampTz</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTimestampTz([NotNull] SubstraitTypeParser.TimestampTzContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>timestampTz</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTimestampTz([NotNull] SubstraitTypeParser.TimestampTzContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>date</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDate([NotNull] SubstraitTypeParser.DateContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>date</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDate([NotNull] SubstraitTypeParser.DateContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>time</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTime([NotNull] SubstraitTypeParser.TimeContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>time</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTime([NotNull] SubstraitTypeParser.TimeContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>intervalDay</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIntervalDay([NotNull] SubstraitTypeParser.IntervalDayContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>intervalDay</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIntervalDay([NotNull] SubstraitTypeParser.IntervalDayContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>intervalYear</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIntervalYear([NotNull] SubstraitTypeParser.IntervalYearContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>intervalYear</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIntervalYear([NotNull] SubstraitTypeParser.IntervalYearContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>uuid</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterUuid([NotNull] SubstraitTypeParser.UuidContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>uuid</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.scalarType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitUuid([NotNull] SubstraitTypeParser.UuidContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>fixedChar</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.parameterizedType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFixedChar([NotNull] SubstraitTypeParser.FixedCharContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>fixedChar</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.parameterizedType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFixedChar([NotNull] SubstraitTypeParser.FixedCharContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>varChar</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.parameterizedType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterVarChar([NotNull] SubstraitTypeParser.VarCharContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>varChar</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.parameterizedType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitVarChar([NotNull] SubstraitTypeParser.VarCharContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>fixedBinary</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.parameterizedType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFixedBinary([NotNull] SubstraitTypeParser.FixedBinaryContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>fixedBinary</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.parameterizedType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFixedBinary([NotNull] SubstraitTypeParser.FixedBinaryContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>decimal</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.parameterizedType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDecimal([NotNull] SubstraitTypeParser.DecimalContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>decimal</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.parameterizedType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDecimal([NotNull] SubstraitTypeParser.DecimalContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>struct</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.parameterizedType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStruct([NotNull] SubstraitTypeParser.StructContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>struct</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.parameterizedType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStruct([NotNull] SubstraitTypeParser.StructContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>nStruct</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.parameterizedType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNStruct([NotNull] SubstraitTypeParser.NStructContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>nStruct</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.parameterizedType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNStruct([NotNull] SubstraitTypeParser.NStructContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>list</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.parameterizedType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterList([NotNull] SubstraitTypeParser.ListContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>list</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.parameterizedType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitList([NotNull] SubstraitTypeParser.ListContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>map</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.parameterizedType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMap([NotNull] SubstraitTypeParser.MapContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>map</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.parameterizedType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMap([NotNull] SubstraitTypeParser.MapContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>numericLiteral</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.numericParameter"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNumericLiteral([NotNull] SubstraitTypeParser.NumericLiteralContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>numericLiteral</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.numericParameter"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNumericLiteral([NotNull] SubstraitTypeParser.NumericLiteralContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>numericParameterName</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.numericParameter"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNumericParameterName([NotNull] SubstraitTypeParser.NumericParameterNameContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>numericParameterName</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.numericParameter"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNumericParameterName([NotNull] SubstraitTypeParser.NumericParameterNameContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>numericExpression</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.numericParameter"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNumericExpression([NotNull] SubstraitTypeParser.NumericExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>numericExpression</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.numericParameter"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNumericExpression([NotNull] SubstraitTypeParser.NumericExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SubstraitTypeParser.anyType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAnyType([NotNull] SubstraitTypeParser.AnyTypeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SubstraitTypeParser.anyType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAnyType([NotNull] SubstraitTypeParser.AnyTypeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SubstraitTypeParser.type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterType([NotNull] SubstraitTypeParser.TypeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SubstraitTypeParser.type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitType([NotNull] SubstraitTypeParser.TypeContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>IfExpr</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIfExpr([NotNull] SubstraitTypeParser.IfExprContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>IfExpr</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIfExpr([NotNull] SubstraitTypeParser.IfExprContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>TypeLiteral</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTypeLiteral([NotNull] SubstraitTypeParser.TypeLiteralContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>TypeLiteral</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTypeLiteral([NotNull] SubstraitTypeParser.TypeLiteralContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>StringLiteral</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStringLiteral([NotNull] SubstraitTypeParser.StringLiteralContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>StringLiteral</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStringLiteral([NotNull] SubstraitTypeParser.StringLiteralContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>MultilineDefinition</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMultilineDefinition([NotNull] SubstraitTypeParser.MultilineDefinitionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>MultilineDefinition</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMultilineDefinition([NotNull] SubstraitTypeParser.MultilineDefinitionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Ternary</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTernary([NotNull] SubstraitTypeParser.TernaryContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Ternary</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTernary([NotNull] SubstraitTypeParser.TernaryContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>BinaryExpr</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBinaryExpr([NotNull] SubstraitTypeParser.BinaryExprContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>BinaryExpr</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBinaryExpr([NotNull] SubstraitTypeParser.BinaryExprContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>TypeParam</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTypeParam([NotNull] SubstraitTypeParser.TypeParamContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>TypeParam</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTypeParam([NotNull] SubstraitTypeParser.TypeParamContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>ParenExpression</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParenExpression([NotNull] SubstraitTypeParser.ParenExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>ParenExpression</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParenExpression([NotNull] SubstraitTypeParser.ParenExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>FunctionCall</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFunctionCall([NotNull] SubstraitTypeParser.FunctionCallContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>FunctionCall</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFunctionCall([NotNull] SubstraitTypeParser.FunctionCallContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>NotExpr</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNotExpr([NotNull] SubstraitTypeParser.NotExprContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>NotExpr</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNotExpr([NotNull] SubstraitTypeParser.NotExprContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>LiteralNumber</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLiteralNumber([NotNull] SubstraitTypeParser.LiteralNumberContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>LiteralNumber</c>
	/// labeled alternative in <see cref="SubstraitTypeParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLiteralNumber([NotNull] SubstraitTypeParser.LiteralNumberContext context);
}
