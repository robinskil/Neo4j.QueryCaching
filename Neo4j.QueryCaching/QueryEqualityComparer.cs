using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neo4j.QueryCaching
{
    internal class QueryEqualityComparer : IEqualityComparer<Query>
    {
        public bool Equals(Query x, Query y)
        {
            if(x != null && y != null)
            {
                if(x.Text == y.Text && x.Parameters.Count == y.Parameters.Count)
                {
                    foreach (var parameter in x.Parameters)
                    {
                        if (!y.Parameters.ContainsKey(parameter.Key))
                        {
                            return false;
                        }
                        if (!y.Parameters[parameter.Key].Equals(parameter.Value))
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public int GetHashCode(Query obj)
        {
            int hashCode = obj.Text.GetHashCode();
            foreach (var parameter in obj.Parameters)
            {
                hashCode = hashCode ^ parameter.Key.GetHashCode();
                hashCode = hashCode ^ parameter.Value.GetHashCode();
            }
            return hashCode;
        }
    }
}
