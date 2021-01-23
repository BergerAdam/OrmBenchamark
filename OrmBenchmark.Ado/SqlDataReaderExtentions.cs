using System;
using System.Data;

namespace OrmBenchmark.Ado
{
    public static class SqlDataReaderExtentions
    {
        public static Post MapToPost(this IDataReader reader)
        {
           return new Post
            {
                Id = reader.GetInt32(0),
                Text = reader.GetString(1),
                CreationDate = reader.GetDateTime(2),
                LastChangeDate = reader.GetDateTime(3),
                Counter1 = reader.GetNullableValue<int>(4),
                Counter2 = reader.GetNullableValue<int>(5),
                Counter3 = reader.GetNullableValue<int>(6),
                Counter4 = reader.GetNullableValue<int>(7),
                Counter5 = reader.GetNullableValue<int>(8),
                Counter6 = reader.GetNullableValue<int>(9),
                Counter7 = reader.GetNullableValue<int>(10),
                Counter8 = reader.GetNullableValue<int>(11),
                Counter9 = reader.GetNullableValue<int>(12),
            };
        }

        public static T? GetNullableValue<T>(this IDataReader reader, int index) where T : struct
        {
            object tmp = reader.GetValue(index);
            if (tmp != DBNull.Value)
            {
                return (T)tmp;
            }
            return null;
        }
    }
}
