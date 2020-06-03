# Neo4j.QueryCaching

This is the repository for the Neo4j.QueryCaching library which is an extension to the neo4j driver. It allows developers to cache query results and retrieve them automatically from cache without the hassle of creating a caching mechanism themselves.

## Usage
```cs
//Define a method that takes the result cursor and maps it to POCO's.
//The caching mechanism will cache these POCO's to the created query object.
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
//Create a session as you would normally.
var session = Driver.AsyncSession();
try
{
  //Call the RunCachedAsync method to run the query as a cacheable query.
  //Time span tells the cache how long to keep the result in cache.
  //You can force a refresh by using the refresh parameter.
  var result = await session.RunCachedAsync<List<Model>>("Match (m:Model) return m",mappingFunction,TimeSpan.FromMinutes(5));
}
finally
{
  await session.CloseAsync();
}
```
