using System;

namespace ContestPark.Category.API.Model
{
    public class IndexResponseModel
    {
        public bool IsValid { get; set; }
        public string StatusMessage { get; set; }
        public Exception Exception { get; set; }
    }
}