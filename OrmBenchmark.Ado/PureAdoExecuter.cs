using OrmBenchmark.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;

namespace OrmBenchmark.Ado
{
    public class PureAdoExecuter : IOrmExecuter
    {
        private IDbConnection conn;

        public DatabaseProvider DatabaseProvider { get; private set; }

        public string Name
        {
            get
            {
                return "ADO (Pure)";
            }
        }

        public void Init(string connectionString, DatabaseProvider databaseType)
        {
            DatabaseProvider = databaseType;
            conn = DatabaseProvider.GetOpenedConnection(connectionString);
        }

        public IPost GetItemAsObject(int id)
        {
            var cmd = SelectFromPostsByIdCommand(id);
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                    return reader.MapToPost();
            };

            return null;
        }

        public dynamic GetItemAsDynamic(int id)
        {
            var cmd = SelectFromPostsByIdCommand(id);
            var obj = new ExpandoObject() as IDictionary<string, object>;
            using (IDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        obj.Add(reader.GetName(i), reader.GetValue(i));
                    }
                    return obj;
                }
            };

            return null;
        }

        public IEnumerable<IPost> GetAllItemsAsObject()
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = SelectAllPosts();

            using (var reader = cmd.ExecuteReader())
            {
                return reader.Select<IPost>(SqlDataReaderExtentions.MapToPost).ToList();
            }
        }

        public IEnumerable<dynamic> GetAllItemsAsDynamic()
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = SelectAllPosts();

            using var reader = cmd.ExecuteReader();
            return reader.Select<dynamic>(dataReader =>
             {
                 var obj = new ExpandoObject() as IDictionary<string, object>;
                 for (int i = 0; i < dataReader.FieldCount; i++)
                 {
                     obj.Add(reader.GetName(i), reader.GetValue(i));
                 }

                 return obj;
             }).ToList();
        }

        public void Dispose()
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Dispose();
        }

        private IDbCommand SelectFromPostsByIdCommand(int id)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = SelectFromPostsById();
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = "@Id";
            parameter.Value = id;
            parameter.DbType = DbType.Int32;
            cmd.Parameters.Add(parameter);
            return cmd;
        }

        private string SelectAllPosts()
        {
            switch (DatabaseProvider)
            {
                case DatabaseProvider.MySqlData:
                case DatabaseProvider.MySqlConnector:
                    return @"select * from Posts";

                case DatabaseProvider.Npgsql:
                    return "select * from public.\"posts\" ";

                case DatabaseProvider.SystemData:
                case DatabaseProvider.MicrosoftData:
                    return @"select * from Posts";
            }

            throw new ArgumentOutOfRangeException();
        }

        private string SelectFromPostsById()
        {
            switch (DatabaseProvider)
            {
                case DatabaseProvider.MySqlData:
                case DatabaseProvider.MySqlConnector:
                    return @"select * from Posts where Id = @Id";

                case DatabaseProvider.Npgsql:
                    return "select * from public.\"posts\" where Id = @Id";

                case DatabaseProvider.SystemData:
                case DatabaseProvider.MicrosoftData:
                    return @"select * from Posts where Id = @Id";
            }

            throw new ArgumentOutOfRangeException();
        }

        public bool IsSupported(DatabaseProvider databaseType) => true;
    }
}