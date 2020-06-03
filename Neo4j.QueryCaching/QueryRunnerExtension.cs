using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Neo4j.QueryCaching
{
    public static class QueryRunnerExtension
    {
        private static Cache cache = new Cache();
        public static async Task<T> RunCachedAsync<T>(this IAsyncQueryRunner queryRunner, Query query, Func<IResultCursor, Task<T>> mapping, TimeSpan expirationTime = default, bool forceRefresh = false)
        {
            //Check if the query's result already has been cached.
            //If it is, it will grab the result from cache and return, 
            //otherwise it will fetch the data from our database
            if (!cache.TryGet<T>(query, out T result) || forceRefresh)
            {
                //Execute the query against the database.
                var cursor = await queryRunner.RunAsync(query);
                //Map the binary result to result objects.
                result = await mapping(cursor);
                //Add the result objects to cache so next time
                //we can return the data from cache
                cache.AddToCache(query, result, expirationTime);
            }
            return result;
        }
        public static async Task<T> RunCachedAsync<T>(this IAsyncQueryRunner queryRunner, string query, Func<IResultCursor, Task<T>> mapping, TimeSpan expirationTime = default, bool forceRefresh = false)
        {
            return await queryRunner.RunCachedAsync<T>(new Query(query), mapping, expirationTime, forceRefresh);
        }
        public static async Task<T> RunCachedAsync<T>(this IAsyncQueryRunner queryRunner, string query, object parameters, Func<IResultCursor, Task<T>> mapping, TimeSpan expirationTime = default, bool forceRefresh = false)
        {
            return await queryRunner.RunCachedAsync<T>(new Query(query,parameters), mapping, expirationTime, forceRefresh);
        }
        public static async Task<T> RunCachedAsync<T>(this IAsyncQueryRunner queryRunner, string query,IDictionary<string,object> parameters, Func<IResultCursor, Task<T>> mapping, TimeSpan expirationTime = default, bool forceRefresh = false)
        {
            return await queryRunner.RunCachedAsync<T>(new Query(query, parameters), mapping, expirationTime, forceRefresh);
        }
    }
}
