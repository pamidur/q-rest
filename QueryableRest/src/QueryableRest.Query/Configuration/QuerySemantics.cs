using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QueryableRest.Query.Configuration
{
    public class QuerySemantics
    {
        private static readonly Regex _filterMatch = new Regex(@"^([a-zA-Z\d_\.]+)(\=\=|\!\=|>=|<=|>%|>\*|<|%|\*|>)(*)$", RegexOptions.Compiled);
        private static readonly Regex _sortMatch = new Regex(@"^(|\+|-)([a-zA-Z\d_\.]+)$", RegexOptions.Compiled);

        private static char[] _defaultStatementSeparators = new[] { ';' };

        protected QuerySemantics()
        {
        }

        public virtual Func<string, string[]> StatementSplitter { get; set; } = entry =>
        {
            return entry?.Split(_defaultStatementSeparators, StringSplitOptions.RemoveEmptyEntries) ?? new string[] { };
        };

        public virtual Tuple<bool, string> SortStatementParser { get; set; }

        public virtual Tuple<string, Expression, string> FilterStatementParser { get; set; }

        public virtual Dictionary<string, ExpressionType> FilterOperationsMap { get; set; } = new Dictionary<string, ExpressionType>  {
            {"==", ExpressionType.Equal },
            {"!=", ExpressionType.NotEqual }
        };

        public static QuerySemantics Default { get; } = new QuerySemantics();
    }
}