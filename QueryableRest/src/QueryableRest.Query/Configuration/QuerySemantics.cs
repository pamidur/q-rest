//using QueryableRest.Query.Semantics;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;

//namespace QueryableRest.Query.Configuration
//{
//    public class QuerySemantics
//    {
//        public static Expression FieldValue = Expression.Constant(new object());
//        public static Expression Value = Expression.Constant(new object());

//        private static readonly Regex _filterMatch = new Regex(@"^([a-zA-Z\d_\.]+)(\=\=|\!\=|>=|<=|>%|>\*|<|%|\*|>)(*)$", RegexOptions.Compiled);
//        private static readonly Regex _sortMatch = new Regex(@"^(|\+|-)([a-zA-Z\d_\.]+)$", RegexOptions.Compiled);

//        private static char[] _defaultStatementSeparators = new[] { ';' };

//        protected QuerySemantics()
//        {
//        }

//        public virtual string[] SplitStatements(string entry)
//        {
//            return entry?.Split(_defaultStatementSeparators, StringSplitOptions.RemoveEmptyEntries) ?? new string[] { };
//        }

//        public virtual IEnumerable<Operation> ParseSelectQuery(string[] entries)
//        {
//            return entries
//                .SelectMany(SplitStatements)
//                .Select(e => new GetFieldOperation() { FieldName = e });
//        }

//        public virtual Func<string[], IOrderedEnumerable<SortStatement>> SortQueryParser { get; set; }
//        public virtual Tuple<string, Expression, string> FilterStatementParser { get; set; }

//        public virtual List<FilterOperation> FilterOperationsRegistry { get; set; } = new List<FilterOperation>
//        {
//            FilterOperation.Equals,
//        };

//        public virtual List<SortOperation> SortOperationsRegistry { get; set; } = new List<SortOperation>
//        {
//            SortOperation.Ascending,
//            SortOperation.Descending
//        };

//        public static QuerySemantics Default { get; } = new QuerySemantics();
//    }
//}