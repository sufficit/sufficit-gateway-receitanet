#if NET7_0_OR_GREATER
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;

namespace Sufficit.Gateway.ReceitaNet.MCP
{
    internal sealed class ReceitaNetMcpSessionManager
    {
        private sealed class SessionState
        {
            public string SessionId { get; set; } = string.Empty;

            public DateTimeOffset ExpiresAtUtc { get; set; }

            public string? ClientName { get; set; }

            public string? ClientVersion { get; set; }
        }

        private readonly ConcurrentDictionary<string, SessionState> _sessions = new(StringComparer.OrdinalIgnoreCase);
        private readonly IOptions<ReceitaNetMcpOptions> _options;

        public ReceitaNetMcpSessionManager(IOptions<ReceitaNetMcpOptions> options)
        {
            _options = options;
        }

        public string Initialize(string? existingSessionId, string? clientName, string? clientVersion)
        {
            var sessionId = string.IsNullOrWhiteSpace(existingSessionId)
                ? Guid.NewGuid().ToString()
                : existingSessionId.Trim();

            var now = DateTimeOffset.UtcNow;
            var state = new SessionState()
            {
                SessionId = sessionId,
                ExpiresAtUtc = now.AddMinutes(GetSessionTtlMinutes()),
                ClientName = string.IsNullOrWhiteSpace(clientName) ? null : clientName.Trim(),
                ClientVersion = string.IsNullOrWhiteSpace(clientVersion) ? null : clientVersion.Trim(),
            };

            _sessions.AddOrUpdate(sessionId, state, (_, __) => state);
            CleanupExpired(now);
            return sessionId;
        }

        public bool Validate(string? sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                return false;

            var normalized = sessionId.Trim();
            var now = DateTimeOffset.UtcNow;
            CleanupExpired(now);

            if (!_sessions.TryGetValue(normalized, out var state))
                return false;

            if (state.ExpiresAtUtc <= now)
            {
                _sessions.TryRemove(normalized, out _);
                return false;
            }

            state.ExpiresAtUtc = now.AddMinutes(GetSessionTtlMinutes());
            return true;
        }

        private int GetSessionTtlMinutes()
            => Math.Max(1, _options.Value.SessionTtlMinutes);

        private void CleanupExpired(DateTimeOffset now)
        {
            foreach (var item in _sessions)
            {
                if (item.Value.ExpiresAtUtc <= now)
                    _sessions.TryRemove(item.Key, out _);
            }
        }
    }
}
#endif