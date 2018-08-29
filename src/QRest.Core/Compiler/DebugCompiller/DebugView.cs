using QRest.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Compiler.Debug
{
    public class DebugTermView
    {
        public ITerm Term { get; internal set; }
        public string DebugInfo { get; private set; }

        public T SetDebugResult<T>(T data)
        {
            try
            {
                DebugInfo = data.ToString();
            }
            catch (Exception e)
            {
                DebugInfo = $"EX: {e.Message}";
            }
            return data;
        }

        public override string ToString()
        {
            return $"{Term.DebugView} => {DebugInfo}";
        }
    }

    public class DebugResult
    {
        public ITerm Root { get; internal set; }
        public Expression Expression { get; internal set; }
        public IReadOnlyList<DebugTermView> DebugView { get; internal set; }
    }

    public class DebugResult<T> : DebugResult
    {
        public Expression<Func<T, object>> Func { get; internal set; }
    }
}
