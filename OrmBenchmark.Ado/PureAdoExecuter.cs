using System;
using System.Collections.Generic;
using OrmBenchmark.Core;
using System.Dynamic;
using System.Data;

namespace OrmBenchmark.Ado
{
    public class PureAdoExecuter : IOrmExecuter
    {
        IDbConnection conn;

        public DatabaseType DatabaseType { get; private set; }

        public string Name
        {
            get
            {
                return "ADO (Pure)";
            }
        }

        public void Init(string connectionString, DatabaseType databaseType)
        {
            DatabaseType = databaseType;
            conn = DatabaseType.GetOpenedConnection(connectionString);
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
            dynamic obj = new ExpandoObject();
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        AddProperty(obj, reader.GetName(i), reader.GetValue(i));
                    }
                    return obj;
                }
            };

            return null;
        }

        public static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }

        public IEnumerable<IPost> GetAllItemsAsObject()
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = SelectAllPosts();

            List<IPost> list = new List<IPost>();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(reader.MapToPost());
                };
            }

            return list;
        }

        public IEnumerable<dynamic> GetAllItemsAsDynamic()
        {

            var cmd = conn.CreateCommand();
            cmd.CommandText = SelectAllPosts();

            List<dynamic> list = new List<dynamic>();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    dynamic obj = new ExpandoObject();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        AddProperty(obj, reader.GetName(i), reader.GetValue(i));
                    }
                    list.Add(obj);
                }
            }

            return list;
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
            switch (DatabaseType)
            {
                case DatabaseType.MySql:
                case DatabaseType.MySqlConnector:
                    return @"select * from Posts";
                case DatabaseType.PostgreSql:
                    return "select * from public.\"posts\" ";
                case DatabaseType.SqlServer:
                    return @"select * from Posts";
            }

            throw new ArgumentOutOfRangeException();
        }

        private string SelectFromPostsById()
        {
            switch (DatabaseType)
            {
                case DatabaseType.MySql:
                case DatabaseType.MySqlConnector:
                    return @"select * from Posts where Id = @Id";
                case DatabaseType.PostgreSql:
                    return "select * from public.\"posts\" where Id = @Id";
                case DatabaseType.SqlServer:
                    return @"select * from Posts where Id = @Id";
            }

            throw new ArgumentOutOfRangeException();
        }

        public bool IsSupported(DatabaseType databaseType) => true;
    }
}
