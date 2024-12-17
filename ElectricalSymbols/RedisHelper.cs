using StackExchange.Redis;

namespace ElectricalSymbols
{
	public static class RedisHelper
	{
		private static IDatabase? _redisDb;

		// Method to establish connection to Redis server
		public static void Conn()
		{
			Task.Run(() =>
			{
				try
				{
					string redisConnectionString = "172.26.172.124:6379";
					ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisConnectionString);
					_redisDb = redis.GetDatabase();
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
			});
		}

		// Method to get a boolean value from Redis
		public static bool? GetBoolValue(string key)
		{
			if (_redisDb!=null)
			{
				var value = _redisDb.StringGet(key).ToString();
				bool.TryParse(value, out bool result);
				return result;
			}

			return null;
		}

		// Method to set a string value in Redis
		public static void SetStringValue(string key, string value)
		{
			_redisDb.StringSet(key, value);
		}

		// Method to get a string value from Redis
		public static string GetStringValue(string key)
		{
			return _redisDb.StringGet(key);
		}
	}
}