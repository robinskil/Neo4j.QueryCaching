using System;

namespace Neo4j.QueryCaching
{
    public class CacheItem
    {
        public TimeSpan TimeToLive { get; }
        public DateTime TimeAdded { get; }
        public object CachedObject { get; }
        public CacheItem(TimeSpan timeToLive, DateTime timeAdded, object cachedObject)
        {
            TimeToLive = timeToLive;
            TimeAdded = timeAdded;
            CachedObject = cachedObject;
        }
    }
}
