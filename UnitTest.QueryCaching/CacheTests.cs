using Neo4j.Driver;
using Neo4j.QueryCaching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.QueryCaching
{
    public class CacheTests
    {
        IDriver Driver { get; set; } = GraphDatabase.Driver("URL", AuthTokens.Basic("neo4j", "password"));

        private T TransformValue<T>(object obj)
        {
            if (obj != null)
            {
                return (T)obj;
            }
            return default;
        }

        [Fact]
        public async Task QueryAllModelClassesUsingCaching()
        {
            Func<IResultCursor, Task<List<Model>>> mappingFunction = async (cursor) =>
            {
                var records = await cursor.ToListAsync();
                List<Model> allModels = new List<Model>();
                foreach (var record in records)
                {
                    var node = record.Values.First().Value as INode;
                    allModels.Add(new Model()
                    {
                        IsDefault = TransformValue<bool>(node["IsDefault"]),
                        Name = node["Name"] as string,
                        Order = TransformValue<long>(node["Order"]),
                        UniqueId = node["UniqueId"] as string
                    });
                }
                return allModels;
            };
            var session = Driver.AsyncSession();
            try
            {
                var result = await session.RunCachedAsync<List<Model>>("Match (m:Model) return m", mappingFunction);
                var result2 = await session.RunCachedAsync<List<Model>>("Match (m:Model) return m", mappingFunction);
            }
            finally
            {
                await session.CloseAsync();
            }
        }
    }
}
