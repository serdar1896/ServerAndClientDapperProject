using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace CoreProject.DataLayer.CacheService.Redis
{
    public class RedisServer
    {
        private ConnectionMultiplexer _connectionMultiplexer;
        private IDatabase _database;
        private string configurationString;
        private int _currentDatabaseId = 0;
        public RedisServer(IConfiguration configuration)
        {
            CreateRedisConfigurationString(configuration);
            _connectionMultiplexer = ConnectionMultiplexer.Connect(configurationString);

            _database = _connectionMultiplexer.GetDatabase(_currentDatabaseId);
        }

        public IDatabase Database => _database;
        public void FlushDatabase()
        {
            _connectionMultiplexer.GetServer(configurationString).FlushDatabase(_currentDatabaseId);
        }
        private void CreateRedisConfigurationString(IConfiguration configuration)
        {
            string host = configuration.GetSection("RedisConfiguration:Host").Value;
            string port = configuration.GetSection("RedisConfiguration:Port").Value;

            configurationString = $"{host}:{port}";
        }

    }
}