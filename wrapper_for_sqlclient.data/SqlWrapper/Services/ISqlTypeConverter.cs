using SimpleExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace wrapper_for_sqlclient.data.Sql
{
    public interface ISqlTypeConverter
    {
        IDataParameter GetOut<T>(string name);
        IDataParameter GetIn(string name, object value);
    }

    public class SqlEnumerableParameterConverter
    {
        public Type CType { get; set; }
        public string TypeName { get; set; }
        public Func<object, DataTable> Convert { get; set; }
    }

    public class BaseMsSqlTypeConverter : ISqlTypeConverter
    {
        private readonly IEnumerable<SqlEnumerableParameterConverter> ParameterConverter;
        private readonly IDictionary<Type, SqlDbType> TypeOutDbConverter;

        public BaseMsSqlTypeConverter(IEnumerable<SqlEnumerableParameterConverter> enumerableConverter = null, IDictionary<Type, SqlDbType> typeOutDbConverter = null)
        {
            ParameterConverter = enumerableConverter;

            TypeOutDbConverter = typeOutDbConverter ??
                new Dictionary<Type, SqlDbType> {
                    { typeof(int), SqlDbType.Int },
                    { typeof(float), SqlDbType.Float },
                    { typeof(decimal), SqlDbType.Money },
                    { typeof(double), SqlDbType.Money },
                    { typeof(DateTime), SqlDbType.DateTime },
                    { typeof(string), SqlDbType.NVarChar },
                    { typeof(Guid), SqlDbType.UniqueIdentifier },
                    { typeof(long), SqlDbType.BigInt },
                    { typeof(bool), SqlDbType.Bit }
                };
        }

        public IDataParameter GetIn(string name, object value)
        {
            var converter = value != null && typeof(System.Collections.IEnumerable).IsAssignableFrom(value.GetType())
                                ? ParameterConverter.FirstOrDefault(p => p.CType.IsInstanceOfType(value))
                                : null;
            return converter == null
                ? new SqlParameter(name, value ?? DBNull.Value)
                : new SqlParameter(name, SqlDbType.Structured) { TypeName = converter.TypeName, Value = converter.Convert(value) };
        }

        public IDataParameter GetOut<T>(string name) =>
            new SqlParameter(name, TypeOutDbConverter.TryGetValue(typeof(T), SqlDbType.Structured)) { Direction = ParameterDirection.Output };
    }
}
