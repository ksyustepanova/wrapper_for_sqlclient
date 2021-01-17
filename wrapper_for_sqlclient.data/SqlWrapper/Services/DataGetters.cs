using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace wrapper_for_sqlclient.data.Sql
{
    public class DataGet : IDataGetter
    {
        private readonly DbDataReader _reader;

        public DataGet(DbDataReader reader)
        {
            _reader = reader;
        }

        public async Task<TModel> Read<TModel>(Func<IDataGetter, Task<TModel>> create) =>
            _reader != null && await _reader.ReadAsync() ? await create.Invoke(this) : default(TModel);

        public async Task<T> Get<T>(string name) => await Get<T>(_reader.ColumnIndex(name));

        public async Task<T> Get<T>(int ordinal) => await _reader.GetAsync<T>(ordinal);
    }

    public class DataSelect : IDataGetter
    {
        private readonly DbDataReader _reader;
        private ConcurrentDictionary<string, int> _nameIndex = new ConcurrentDictionary<string, int>();

        public DataSelect(DbDataReader reader)
        {
            _reader = reader;
        }

        public async Task<IEnumerable<TModel>> Read<TModel>(Func<IDataGetter, Task<TModel>> create)
        {
            var result = new List<TModel>();

            if (_reader == null)
                return result;
            do
            {
                while (await _reader.ReadAsync())
                    result.Add(await create.Invoke(this));
            } while (await _reader.NextResultAsync());
            return result;
        }

        public async Task<T> Get<T>(string name) => await Get<T>(_nameIndex.GetOrAdd(name, _reader.ColumnIndex));

        public async Task<T> Get<T>(int ordinal) => await _reader.GetAsync<T>(ordinal);
    }
}
