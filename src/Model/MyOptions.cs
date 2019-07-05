namespace webapi_authorize.Model
{
    public class MyOptions
    {
        public JwtOptions JwtOptions { get; set; }
    }
    public class JwtOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpireHours { get; set; }
        public string Key { get; set; }
    }
}