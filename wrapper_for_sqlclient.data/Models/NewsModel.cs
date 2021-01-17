using System;

namespace wrapper_for_sqlclient.data.Models
{
    public class NewsModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime DateCreate { get; set; }
    }
}
