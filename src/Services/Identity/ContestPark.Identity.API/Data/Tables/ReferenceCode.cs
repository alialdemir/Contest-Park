using System.ComponentModel.DataAnnotations;

namespace ContestPark.Identity.API.Data.Tables
{
    public class ReferenceCode
    {
        public int ReferenceCodeId { get; set; }

        [MaxLength(450)]
        public string ReferenceUserId { get; set; }

        [MaxLength(450)]
        public string NewUserId { get; set; }

        public string Code { get; set; }
    }
}
