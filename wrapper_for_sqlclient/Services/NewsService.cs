using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using wrapper_for_sqlclient.data.Models;
using wrapper_for_sqlclient.data.Repositories;

namespace wrapper_for_sqlclient.Services
{
    public interface INewsService
    {
        Task<IEnumerable<NewsModel>> Get(int? id = null);
    }

    public class NewsService : INewsService
    {
        private readonly INewsRepository _newsRepository;

        public NewsService(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        public async Task<IEnumerable<NewsModel>> Get(int? id = null) =>
            await _newsRepository.Select(id);
    }
}
