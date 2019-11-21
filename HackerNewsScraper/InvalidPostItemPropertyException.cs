using System;
using System.Collections.Generic;
using System.Text;

namespace HackerNewsScraper
{
    public class InvalidPostItemPropertyException : Exception
    {
        public InvalidPostItemPropertyException(string message) : base(message)
        {
        }
    }
}
