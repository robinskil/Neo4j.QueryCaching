using Neo4j.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Neo4j.QueryCaching
{
    public class Cache
    {
        private readonly ConcurrentDictionary<Query, CacheItem> _cacheContainer;
        public Cache()
        {
            _cacheContainer = new ConcurrentDictionary<Query, CacheItem>(new QueryEqualityComparer());
        }

        public bool TryInvalidateCachedQuery(Query query)
        {
            return _cacheContainer.TryRemove(query, out _);
        }

        public void ClearCache()
        {
            _cacheContainer.Clear();
        }

        public bool ContainsCachedResult(Query query)
        {
            return _cacheContainer.ContainsKey(query);
        }

        public bool TryGet<T>(Query query, out T result)
        {
            result = default;
            if (_cacheContainer.ContainsKey(query))
            {
                var cacheItem = _cacheContainer[query];
                if (cacheItem.TimeAdded.Add(cacheItem.TimeToLive) > DateTime.Now)
                {
                    result = (T)_cacheContainer[query].CachedObject;
                    return true;
                }
            }
            return false;
        }

        public void AddToCache(Query query, object obj, TimeSpan timeToLive = default)
        {
            if(timeToLive == default)
            {
                timeToLive = TimeSpan.FromDays(90);
            }
            var cacheItem = new CacheItem(timeToLive, DateTime.Now, obj);
            _cacheContainer.AddOrUpdate(query,cacheItem, (queryKey, prevCacheItem) => cacheItem);
        }
    }
}
