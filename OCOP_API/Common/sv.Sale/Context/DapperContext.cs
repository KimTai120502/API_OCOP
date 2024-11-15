using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sv.Sale.Context
{
    public class DapperContext
    {

        private readonly IConfiguration configuration;
        private readonly string connectionString;

        public DapperContext(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.connectionString = this.configuration.GetConnectionString("sqlConnectionString");
        }

        internal IDbConnection CreateConnection()
            => new SqlConnection(this.connectionString);

        /// <summary>
        /// Convert sang ExpandoObject dạng IDictionary.
        /// Call: db.Database.Connection.Query(sqlQuery, param).Select(x => (ExpandoObject)DapperHelper.ToExpandoDynamic(x))
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal dynamic ToExpandoDynamic(object value)
        {
            IDictionary<string, object> dapperRowProperties = value as IDictionary<string, object>;

            IDictionary<string, object> expando = new ExpandoObject();

            foreach (KeyValuePair<string, object> property in dapperRowProperties)
                expando.Add(property.Key, property.Value);

            return expando as ExpandoObject;
        }
    }
}
