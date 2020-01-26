namespace szedarserver.Infrastructure.Setting
{
    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public int ExpireDays { get; set; }
        public string Site { get; set; }
    }
}