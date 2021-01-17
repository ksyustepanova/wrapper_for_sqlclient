using wrapper_for_sqlclient.data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace wrapper_for_sqlclient.data
{

    public interface IDataGetter
    {
        Task<T> Get<T>(string name);
        Task<T> Get<T>(int ordinal);
    }

    public interface ISqlFactory
    {
        ISqlFacade New(string sql, CommandType type = CommandType.StoredProcedure, int timeout = 30);
    }

    public class MsSqlFactory : ISqlFactory
    {
        private readonly string _connectionString;
        private readonly ISqlTypeConverter _converter;

        public MsSqlFactory(string connectionString,
            IEnumerable<SqlEnumerableParameterConverter> enumerableConverter = null,
            IDictionary<Type, SqlDbType> typeOutDbConverter = null) : this(connectionString, new BaseMsSqlTypeConverter(
                enumerableConverter
                , typeOutDbConverter
            ))
        { }

        public MsSqlFactory(string connectionString, ISqlTypeConverter adpater)
        {
            _converter = adpater;
            _connectionString = connectionString;
        }

        public ISqlFacade New(string sql,
            CommandType type = CommandType.StoredProcedure,
            int timeout = 30) =>
            new MsSqlFacade(
                new CmdSettings
                {
                    ConnectionString = _connectionString,
                    Query = sql,
                    Type = type,
                    Timeout = timeout
                }, _converter);

    }
}