using System.ComponentModel.DataAnnotations;

namespace FUD.Web.Models
{
    public class CouponDto
    {
        public int Id { get; set; }
        
        [Required]
        public string Code { get; set; }
		
        [Required]
		[Range(0, 100.00, ErrorMessage = "Please enter valid Discount Amount")]
		public double DiscountAmount { get; set; }

		[Required]
		[Range(0, int.MaxValue, ErrorMessage = "Please enter valid Min amount")]
		public int MinAmount { get; set; }
    }
}
