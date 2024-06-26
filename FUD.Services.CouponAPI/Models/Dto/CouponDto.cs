namespace FUD.Services.CouponAPI.Models.Dto
{
    public class CouponDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public double DiscountAmount { get; set; }
        public int MinAmount { get; set; }
    }
}
