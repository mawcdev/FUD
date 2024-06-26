namespace FUD.Web.Utilities
{
    public class SD
    {
        public static string CouponAPIBase { get; set; }
        public static string AuthAPIBase { get; set; }
        public static string ProductAPIBase { get; set; }

        public static string RoleAdmin = "Admin";
        public static string RoleCustomer = "Customer";
        public static string TokenCookie = "JwtToken";

        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }

    }
}
