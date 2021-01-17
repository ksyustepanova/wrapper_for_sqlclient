using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace wrapper_for_sqlclient.data.Sql
{
    public interface ISqlFacade
    {
        ISqlFacade AddIn(string name, object value);

        ISqlFacade AddOut<T>(string name);

        T GetOut<T>(string key);

        Task<T> Get<T>();

        Task<TModel> Get<TModel>(Func<IDataGetter, Task<TModel>> create);

        Task Execute();

        Task<IEnumerable<TModel>> Select<TModel>();

        Task<IEnumerable<TModel>> Select<TModel>(Func<IDataGetter, Task<TModel>> create);
    }

    public class MsSqlFacade : SqlFacade
    {
        public MsSqlFacade(CmdSettings settings, ISqlTypeConverter adpater) : base(settings, new MsSqlParameters(adpater)) { }
    }

    /// <summary>
    /// Класс для удобной работы с sql
    /// </summary>
    public abstract class SqlFacade : ISqlFacade
    {
        protected readonly CmdSettings _settings;
        protected readonly ISqlParameters _parameters;

        public SqlFacade(CmdSettings settings, ISqlParameters parameters)
        {
            _settings = settings;
            _parameters = parameters;
        }

        public ISqlFacade AddIn(string name, object value)
        {
            _parameters.AddIn(name, value);
            return this;
        }

        public ISqlFacade AddOut<T>(string name)
        {
            _parameters.AddOut<T>(name);
            return this;
        }

        /// <summary>
        /// Возвращает output параметр
        /// </summary>
        /// <param name="key">название параметра</param>        
        public T GetOut<T>(string key) => _parameters.GetOut<T>(key);

        public async Task<T> Get<T>() => await Get(async r => await r.Get<T>(0));

        /// <summary>
        /// Возвращает модель данных
        /// </summary>
        /// <typeparam name="TModel">тип сущности</typeparam>
        /// <param name="readerAdapter">функция получения данных из reader</param>
        public async Task<TModel> Get<TModel>(Func<IDataGetter, Task<TModel>> create) =>
            create == null
                ? default(TModel)
                : await _parameters.Exect(
                    _settings
                    , async cmd => await new DataGet(await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.SingleRow)).Read(create));


        /// <summary>
        /// Выполняет хранимую процедуру
        /// </summary>
        public async Task Execute() => await _parameters.Exect(_settings, async cmd => await cmd.ExecuteNonQueryAsync());

        public async Task<IEnumerable<TModel>> Select<TModel>() => await Select(async r => await r.Get<TModel>(0));

        /// <summary>
        /// Выполняет хранимую процедуру, которая возвращает список моделей
        /// </summary>
        /// <typeparam name="TModel">тип модели</typeparam>
        /// <param name="readerAdapter">функция получения данных из reader</param>
        public async Task<IEnumerable<TModel>> Select<TModel>(Func<IDataGetter, Task<TModel>> create) =>
            create == null
                ? new List<TModel>()
                : await _parameters.Exect(_settings, async cmd => await new DataSelect(await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.Default)).Read(create));

    }
}
