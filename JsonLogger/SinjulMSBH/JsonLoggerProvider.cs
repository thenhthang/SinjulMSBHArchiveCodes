using System;
using System.Collections.Concurrent;

using Microsoft.Extensions.Logging;

namespace JsonLogger.SinjulMSBH
{
    public sealed class JsonLoggerProvider : ILoggerProvider
    {
        private readonly LoggerExternalScopeProvider _scopeProvider = new LoggerExternalScopeProvider();

        private readonly ConcurrentDictionary<string, JsonLogger> _loggers =
            new ConcurrentDictionary<string, JsonLogger>(StringComparer.Ordinal);

        public ILogger CreateLogger(string categoryName) =>
            _loggers.GetOrAdd(categoryName, category => new JsonLogger(Console.Out, category, _scopeProvider));

        public void Dispose()
        {
        }
    }
}
