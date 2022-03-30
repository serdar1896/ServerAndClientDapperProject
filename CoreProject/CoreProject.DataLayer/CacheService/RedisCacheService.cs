using CoreProject.DataLayer.CacheService.Redis;
using CoreProject.Entities.VMModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreProject.DataLayer.CacheService
{
    public class RedisCacheService : ICacheService
    {
        private readonly RedisServer _redisServer;
        public RedisCacheService(RedisServer redisServer)
        {
            _redisServer = redisServer;
        }
        public async Task AddAsync(string key, object data)
        {
            string jsonData = JsonConvert.SerializeObject(data);
            await _redisServer.Database.StringSetAsync(key,jsonData);
        }
        public async Task<bool> AnyAsync(string key)
        {
            return  await _redisServer.Database.KeyExistsAsync(key);
        }
        public void Clear()
        {
            _redisServer.FlushDatabase();
        }
        public async Task<ServiceResponse<T>> GetAsync<T>(string key)
        {
            var response = new ServiceResponse<T>();
            if (await AnyAsync(key))
            {
                string jsonData = await _redisServer.Database.StringGetAsync(key);
                response.Entity=JsonConvert.DeserializeObject<T>(jsonData);
                return response;
            }
            return default;
        }
        public async Task RemoveAsync(string key)
        {
            await _redisServer.Database.KeyDeleteAsync(key);
        }
    }
}
