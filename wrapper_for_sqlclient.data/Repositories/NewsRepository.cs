using System;
using System.Collections.Generic;
using wrapper_for_sqlclient.data.Models;
using System.Threading.Tasks;

namespace wrapper_for_sqlclient.data.Repositories
{
    public interface INewsRepository
    {
        Task<IEnumerable<NewsModel>> Select(int? id = null);
    }
    public class NewsRepository : INewsRepository
    {
        private readonly ISqlFactory Sql;

        public NewsRepository(ISqlFactory sql)
        {
            Sql = sql;
        }

        public async Task<IEnumerable<NewsModel>> Select(int? id = null) =>
            await Sql.New("spSelectNews")
                .AddIn("id", id)
                .Select(NewsModelAdapter);

        private async Task<NewsModel> NewsModelAdapter(IDataGetter r) =>
            new NewsModel
            {
                Id = await r.Get<int>("id"),
                Title = await r.Get<string>("title"),
                Body = await r.Get<string>("body"),
                DateCreate = await r.Get<DateTime>("date_create")
            };
    }
}
