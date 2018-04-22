using Microsoft.Extensions.Logging;
using System;

namespace GameServerWebAPI.Infrastructure
{
    public static class LoggerExtensions
    {
        private static readonly Action<ILogger, int, Exception> gameServerListRequested;

        static LoggerExtensions()
        {
            gameServerListRequested = LoggerMessage.Define<int>(
                LogLevel.Information,
                new EventId(102, nameof(GameServerListRequested)),
                "Game server list requested (Limit = '{Limit}')");
        }

        public static void GameServerListRequested(this ILogger logger, int limit)
        {
            gameServerListRequested(logger, limit, null);
        }
    }
}