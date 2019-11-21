using System;
using System.Collections.Generic;
using System.Text;

namespace HackerNewsScraper.Models
{
    public class SourceItem
    {
        public string By { get; set; }
        public int Descendants { get; set; }
        public int Score { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }
}
