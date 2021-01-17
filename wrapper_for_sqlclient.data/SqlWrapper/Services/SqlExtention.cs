using SimpleExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace wrapper_for_sqlclient.data
{
    public static class SqlExtention
    {

        private readonly static IReadOnlyDictionary<Type, Func<IDataReader, int, object>> TypeConverter =
            new Dictionary<Type, Func<IDataReader, int, object>> {
                {typeof(int), (reader, ord) => reader.GetInt32(ord) },
                {typeof(long), (reader, ord) => reader.GetInt64(ord) },
                {typeof(DateTime), (reader, ord) => reader.GetDateTime(ord) },
                {typeof(decimal), (reader, ord) => reader.GetDecimal(ord) },
                {typeof(float),(reader, ord) => reader.GetFloat(ord) },
                {typeof(double), (reader, ord) => reader.GetDouble(ord) },
                {typeof(bool), (reader, ord) => reader[ord].ToString().ToBool() },
                {typeof(string), (reader, ord) => reader.GetString(ord) },
                {typeof(Guid), (reader, ord) => reader.GetGuid(ord) }
            }.ToReadOnlyDictionary();


        private static Type getBaseType<T>()
        {
            var t = typeof(T);
            return Nullable.GetUnderlyingType(t) ?? t;
        }

        /// <summary>
        /// Получить типизированные данные и ячейки по имени
        /// </summary>
        /// <typeparam name="T">Результирующий тип данных</typeparam>
        /// <param name="reader">поставщик данных</param>
        /// <param name="name">имя ячейки</param>        
        public static T Get<T>(this IDataReader reader, string name) => reader.Get<T>(reader.ColumnIndex(name));

        public static T Get<T>(this IDataReader reader, int ordinal) =>
            ordinal < 0 || ordinal >= reader.FieldCount ? default(T) : reader.GetConvert<T>(ordinal, TypeConverter.TryGetValue(getBaseType<T>()));

        private static T GetConvert<T>(this IDataReader reader, int ordinal, Func<IDataReader, int, object> convert) =>
            convert == null ? (T)reader[ordinal] : (reader.IsDBNull(ordinal) ? default(T) : (T)convert.Invoke(reader, ordinal));

        public static int ColumnIndex(this IDataReader reader, string columnName)
        {
            for (var i = 0; i < reader.FieldCount; i++)
                if (reader.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return i;
            return -1;
        }

        public static async Task<T> GetAsync<T>(this DbDataReader reader, string name) =>
            await reader.GetAsync<T>(reader.ColumnIndex(name));

        public static async Task<T> GetAsync<T>(this DbDataReader reader, int ordinal) =>
             ordinal < 0 || ordinal >= reader.FieldCount || await reader.IsDBNullAsync(ordinal)
                ? default(T)
                : (getBaseType<T>() == typeof(bool)
                    ? (T)(reader[ordinal].ToString().ToBool() as object)
                    : await reader.GetFieldValueAsync<T>(ordinal));


        public static DbCommand ParametersFill(this DbCommand cmd, IEnumerable<IDataParameter> _parameters)
        {
            _parameters.ForEach(param => cmd.Parameters.Add(param));
            return cmd;
        }
    }
}
