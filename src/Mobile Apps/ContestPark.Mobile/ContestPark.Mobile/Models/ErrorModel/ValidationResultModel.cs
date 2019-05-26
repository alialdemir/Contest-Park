using System;
using System.Collections.Generic;
using System.Text;

namespace ContestPark.Mobile.Models.ErrorModel
{
    public class ValidationResultModel
    {
        public string ErrorMessage { get; set; }
        public IEnumerable<string> MemberNames { get; }
    }
}