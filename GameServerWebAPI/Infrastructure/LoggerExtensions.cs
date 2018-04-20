using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServerWebAPI.Infrastructure
{
    public static class LoggerExtensions
    {
        private static readonly Action<ILogger, int, Exception> gameServerListRequested;

        static LoggerExtensions()
        {
            gameServerListRequested = LoggerMessage.Define<int>(
                LogLevel.Information,
                new EventId(2, nameof(GameServerListRequested)),
                "Game server list requested (Limit = '{Limit}')");
        }

        public static void GameServerListRequested(this ILogger logger, int limit)
        {
            gameServerListRequested(logger, limit, null);
        }
    }
}