namespace Zyborg.GuacBot
{
    public class FunctionSettings
    {
        public string GuacHost { get; set; }
        public string Username { get; set;}
        public string Password { get; set; }
        public string TotpSecret { get; set; }
        public string DataSource { get; set; }

        public bool SkipTls { get; set; }

        public string NsbLicenseBody { get; set; }
        public string NsbLicensePath { get; set; }
    }
}