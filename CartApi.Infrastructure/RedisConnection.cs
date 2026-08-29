using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CartApi.Infrastructure
{
	public class RedisConnection
	{
		private readonly ConnectionMultiplexer _redis;

		public RedisConnection(string connectionString)
		{
			_redis = ConnectionMultiplexer.Connect(connectionString);
		}

		public IDatabase Database => _redis.GetDatabase();
	}
}
