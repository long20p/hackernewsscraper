using System;
using System.Collections.Generic;
using System.Text;

namespace HackerNewsScraper.Models
{
    public class Story
    {
        public string Title { get; set; }
        public string Uri { get; set; }
        public string Author { get; set; }
        public int Points { get; set; }
        public int Comments { get; set; }
        public int Rank { get; set; }
    }
}
