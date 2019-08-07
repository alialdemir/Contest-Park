using ContestPark.Core.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContestPark.Core.Models
{
    public class ValidationResultModel : ValidationResult
    {
        public ValidationResultModel(string errorMessage, ErrorStatuCodes errorStatuCode) : base(errorMessage)
        {
            ErrorStatuCode = errorStatuCode;
        }

        public ValidationResultModel(string errorMessage, IEnumerable<string> memberNameserrorMessage, ErrorStatuCodes errorStatuCode) : base(errorMessage, memberNameserrorMessage)
        {
            ErrorStatuCode = errorStatuCode;
        }

        public ValidationResultModel(string errorMessage) : base(errorMessage)
        {
        }

        public ValidationResultModel(string errorMessage, IEnumerable<string> memberNames) : base(errorMessage, memberNames)
        {
        }

        public ErrorStatuCodes ErrorStatuCode { get; set; }
    }
}
