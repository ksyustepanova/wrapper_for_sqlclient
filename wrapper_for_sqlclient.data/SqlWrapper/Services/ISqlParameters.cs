using SimpleExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace wrapper_for_sqlclient.data.Sql
{

    public interface ISqlParameters
    {
        void AddIn<T>(string name, T value);
        void AddOut<T>(string name);
        T GetOut<T>(string key);
        Task<TModel> Exect<TModel>(CmdSettings settings, Func<DbCommand, Task<TModel>> exect);
    }

    public class MsSqlParameters : ISqlParameters
    {
        private readonly ICollection<IDataParameter> _parameters = new List<IDataParameter>();
        private IDictionary<string, object> _output;
        private readonly ISqlTypeConverter _adpater;

        public MsSqlParameters(ISqlTypeConverter adpater)
        {
            _adpater = adpater;
        }

        public void AddIn<T>(string name, T value) => _parameters.Add(_adpater.GetIn(name, value));

        public void AddOut<T>(string name) => _parameters.Add(_adpater.GetOut<T>(name));

        /// <summary>
        /// Возвращает output параметр
        /// </summary>
        /// <param name="key">название параметра</param>        
        public T GetOut<T>(string key) => (T)_output.TryGetValue(key, default(T));

        public async Task<TModel> Exect<TModel>(CmdSettings settings, Func<DbCommand, Task<TModel>> exect)
        {
            if (exect == null || string.IsNullOrWhiteSpace(settings.Query))
                return default(TModel);

            using (var conn = new SqlConnection(settings.ConnectionString))
            {
                using (var cmd = new SqlCommand(settings.Query, conn)
                {
                    CommandType = settings.Type,
                    CommandTimeout = settings.Timeout,
                    Connection = conn
                }.ParametersFill(_parameters))
                {
                    await conn.OpenAsync(System.Threading.CancellationToken.None);
                    var result = await exect.Invoke(cmd);

                    _output = _parameters
                        .Where(param => param.Direction == ParameterDirection.Output)
                        .ToDictionaryTry(p => p.ParameterName, p => (cmd.Parameters[p.ParameterName] as IDataParameter)?.Value);

                    return result;
                }
            }
        }
    }
}
