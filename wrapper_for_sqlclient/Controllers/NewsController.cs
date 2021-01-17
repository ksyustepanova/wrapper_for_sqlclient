using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using wrapper_for_sqlclient.Services;
using wrapper_for_sqlclient.data.Models;

namespace wrapper_for_sqlclient.Controllers
{
    //minimal example for displaying wrapper action
    [ApiController]
    [Route("[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;
        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }
        // GET: api/News
        [HttpGet]
        public async Task<IEnumerable<NewsModel>> Get()
        {
            return await _newsService.Get(null);
        }
        // GET: api/News
        [HttpGet("{id}", Name = "Get")]
        public async Task<IEnumerable<NewsModel>> Get(int id)
        {
            return await _newsService.Get(id);
        }
    }
}
