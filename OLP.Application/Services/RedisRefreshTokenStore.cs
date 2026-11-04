using OLP.Application.Contracts.Services;
using StackExchange.Redis;
using System.Security.Cryptography;
using System.Text;

namespace OLP.Application.Services
{
    public class RedisRefreshTokenStore : IRefreshTokenStore
    {
        private readonly IDatabase _redis;
        private static readonly TimeSpan LifeTime = TimeSpan.FromDays(7);
        private const string RotateScript = """
        local current = redis.call('GET', KEYS[1])
        if not current or current ~= ARGV[1] then return 0 end
        redis.call('SET', KEYS[1], ARGV[2], 'EX', ARGV[3])
        return 1
        """;

        public RedisRefreshTokenStore(IConnectionMultiplexer connection)
            => _redis = connection.GetDatabase();

        public async Task<string> CreateAsync(Guid applicationUserId)
        {
            var token = Generate(applicationUserId);
            await _redis.StringSetAsync(Key(applicationUserId), Hash(token), LifeTime);
            return token;
        }

        public async Task RevokeAllAsync(Guid applicationUserId)
            => await _redis.KeyDumpAsync(Key(applicationUserId));

        public async Task<string?> RotateAsync(string oldToken)
        {
            if (!TryGetUserId(oldToken, out var userId)) return null;
            var newToken = Generate(userId);
            var rotated = (int)(long)await _redis.ScriptEvaluateAsync(
                RotateScript,
                new RedisKey[] { Key(userId) },
                new RedisValue[] { Hash(oldToken), Hash(newToken), (long)LifeTime.TotalSeconds });
            return rotated == 1 ? newToken : null;
        }

        public static bool TryGetUserId(string token, out Guid userId)
        {
            userId = default;
            if (string.IsNullOrWhiteSpace(token)) return false;
            var separator = token.IndexOf('.');
            if (separator < 1 || token.Length - separator - 1 != 86) return false;
            // A 64-byte Base64Url random secret has 86 characters without padding.
            return Guid.TryParseExact(token[..separator], "N", out userId);
        }

        private static string Generate(Guid userId)
        {
            var secret = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
                .TrimEnd('=').Replace('+', '-').Replace('/', '_');
            return $"{userId:N}.{secret}";
        }

        private static string Hash(string token)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

        private static RedisKey Key(Guid userId) => $"olp:auth:refresh:{userId:N}";
    }
}
