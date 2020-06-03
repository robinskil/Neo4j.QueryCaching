using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Neo4j.Driver;
using Neo4j.QueryCaching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Benchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<DatabaseQueryBenchmarks>();
        }
    }
    [HtmlExporter]
    public class DatabaseQueryBenchmarks
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

        [Benchmark(Baseline = true)]
        public async Task QueryAllModelClasses()
        {
            var session = Driver.AsyncSession();
            List<IRecord> records;
            try
            {
                var result = await session.RunAsync("Match (m:Model) return m");
                records = await result.ToListAsync();
            }
            finally
            {
                await session.CloseAsync();
            }
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
        }
        [Benchmark]
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
                var result = await session.RunCachedAsync("Match (m:Model) return m",mappingFunction);
            }
            finally
            {
                await session.CloseAsync();
            }
        }
    }


}
