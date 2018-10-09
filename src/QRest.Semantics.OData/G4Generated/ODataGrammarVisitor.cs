//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.7.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from ODataGrammar.g4 by ANTLR 4.7.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="ODataGrammarParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.1")]
[System.CLSCompliant(false)]
public interface IODataGrammarVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="ODataGrammarParser.parse"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitParse([NotNull] ODataGrammarParser.ParseContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ODataGrammarParser.queryOptions"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitQueryOptions([NotNull] ODataGrammarParser.QueryOptionsContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ODataGrammarParser.queryOption"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitQueryOption([NotNull] ODataGrammarParser.QueryOptionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ODataGrammarParser.filter"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFilter([NotNull] ODataGrammarParser.FilterContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ODataGrammarParser.select"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSelect([NotNull] ODataGrammarParser.SelectContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ODataGrammarParser.selectItem"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSelectItem([NotNull] ODataGrammarParser.SelectItemContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ODataGrammarParser.count"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCount([NotNull] ODataGrammarParser.CountContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ODataGrammarParser.orderby"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitOrderby([NotNull] ODataGrammarParser.OrderbyContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ODataGrammarParser.orderbyItem"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitOrderbyItem([NotNull] ODataGrammarParser.OrderbyItemContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ODataGrammarParser.order"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitOrder([NotNull] ODataGrammarParser.OrderContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ODataGrammarParser.top"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTop([NotNull] ODataGrammarParser.TopContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ODataGrammarParser.skip"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSkip([NotNull] ODataGrammarParser.SkipContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>binaryExpression</c>
	/// labeled alternative in <see cref="ODataGrammarParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBinaryExpression([NotNull] ODataGrammarParser.BinaryExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>decimalExpression</c>
	/// labeled alternative in <see cref="ODataGrammarParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDecimalExpression([NotNull] ODataGrammarParser.DecimalExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>dateTimeOffsetExpression</c>
	/// labeled alternative in <see cref="ODataGrammarParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDateTimeOffsetExpression([NotNull] ODataGrammarParser.DateTimeOffsetExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>stringExpression</c>
	/// labeled alternative in <see cref="ODataGrammarParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStringExpression([NotNull] ODataGrammarParser.StringExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>boolExpression</c>
	/// labeled alternative in <see cref="ODataGrammarParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBoolExpression([NotNull] ODataGrammarParser.BoolExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>intExpression</c>
	/// labeled alternative in <see cref="ODataGrammarParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIntExpression([NotNull] ODataGrammarParser.IntExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>identifierExpression</c>
	/// labeled alternative in <see cref="ODataGrammarParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIdentifierExpression([NotNull] ODataGrammarParser.IdentifierExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>notExpression</c>
	/// labeled alternative in <see cref="ODataGrammarParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNotExpression([NotNull] ODataGrammarParser.NotExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>parenExpression</c>
	/// labeled alternative in <see cref="ODataGrammarParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitParenExpression([NotNull] ODataGrammarParser.ParenExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>comparatorExpression</c>
	/// labeled alternative in <see cref="ODataGrammarParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitComparatorExpression([NotNull] ODataGrammarParser.ComparatorExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>funcCallExpression</c>
	/// labeled alternative in <see cref="ODataGrammarParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFuncCallExpression([NotNull] ODataGrammarParser.FuncCallExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ODataGrammarParser.comparator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitComparator([NotNull] ODataGrammarParser.ComparatorContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ODataGrammarParser.binary"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBinary([NotNull] ODataGrammarParser.BinaryContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ODataGrammarParser.bool"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBool([NotNull] ODataGrammarParser.BoolContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ODataGrammarParser.dateTimeOffset"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDateTimeOffset([NotNull] ODataGrammarParser.DateTimeOffsetContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ODataGrammarParser.year"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitYear([NotNull] ODataGrammarParser.YearContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ODataGrammarParser.month"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMonth([NotNull] ODataGrammarParser.MonthContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ODataGrammarParser.day"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDay([NotNull] ODataGrammarParser.DayContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ODataGrammarParser.hour"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitHour([NotNull] ODataGrammarParser.HourContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ODataGrammarParser.minute"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMinute([NotNull] ODataGrammarParser.MinuteContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ODataGrammarParser.second"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSecond([NotNull] ODataGrammarParser.SecondContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ODataGrammarParser.fractionalSeconds"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFractionalSeconds([NotNull] ODataGrammarParser.FractionalSecondsContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ODataGrammarParser.functionParams"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFunctionParams([NotNull] ODataGrammarParser.FunctionParamsContext context);
}
