using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Redis_Test.Extensions
{
    public static class DistributedCacheExtensions
    {
        //default configuration for expire time
        public static TimeSpan Default_AbsoluteExpireTime = TimeSpan.FromSeconds(300);

        //to store data in cache
        public static async Task SetRecordAsync<T>(this IDistributedCache cache, string recordId, T data,
            TimeSpan? AbsoluteExpireTime = null, TimeSpan? UnusedExpireTime = null)
        {
            var options = new DistributedCacheEntryOptions();
            //if the AbsoluteExpireTime = null the ?? redirect to Default_AbsoluteExpireTime
            options.AbsoluteExpirationRelativeToNow = AbsoluteExpireTime ?? Default_AbsoluteExpireTime;
            //if this time reach and data not used even the absolute time not reached it will remove
            options.SlidingExpiration = UnusedExpireTime;

            //convert data to json to store in Redis Cache
            var jsonData = JsonSerializer.Serialize(data);

            await cache.SetStringAsync(recordId, jsonData, options);
        }

        //to retrive data from cache
        public static async Task<T> GetRecordAsync<T> (this IDistributedCache cache, string recordId)
        {
            var jsonData = await cache.GetStringAsync(recordId);

            if(jsonData is null)
            {
                return default(T);
            }

            return JsonSerializer.Deserialize<T>(jsonData);
        }
    }
}
