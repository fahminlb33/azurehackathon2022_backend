using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evangelion01.BackendApp.Infrastructure.Helpers
{
    public static class ContainerExtensionMethods
    {
        public static async Task<T?> GetFirstOrDefault<T>(this Container container, string sql)
        {
            var query = new QueryDefinition(sql);
            var itemIterator = container.GetItemQueryIterator<T>(query);
            if (!itemIterator.HasMoreResults)
            {
                throw new Exception("No document with the specified query is found");
            }

            var resultSet = await itemIterator.ReadNextAsync();
            return resultSet.FirstOrDefault();
        }

        public static async Task<IList<T>> RunSql<T>(this Container container, string sql)
        {
            var query = new QueryDefinition(sql);
            var itemIterator = container.GetItemQueryIterator<T>(query);
            var resultSet = new List<T>();

            while (itemIterator.HasMoreResults)
            {
                var currentBatch = await itemIterator.ReadNextAsync();
                resultSet.AddRange(currentBatch);
            }

            return resultSet;
        }
    }
}
