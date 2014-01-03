using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace HydroData.Data
{
    public class DefaultCacheProvider : ICacheProvider
    {
        private ObjectCache Cache { get { return MemoryCache.Default; } }

        public object Get(string key)
        {
            return Cache[key];
        }

        public void Set(string key, object data, int cacheTime)
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheTime);

            Cache.Add(new CacheItem(key, data), policy);
        }

        public bool IsSet(string key)
        {
            return (Cache[key] != null);
        }

        public void Invalidate(string key)
        {
            Cache.Remove(key);
        }
        public void InvalidateByPattern(string keyPattern)
        {
            var keys = Cache.Select(kvp => kvp.Key).ToList();
            foreach (var k in keys)
            {
                if (k.StartsWith(keyPattern)) Cache.Remove(k);
            }
        }


		public string[] GetKeys()
		{
			return Cache.Select(x => x.Key).ToArray();
		}
	}



}
