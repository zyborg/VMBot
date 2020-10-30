using Zyborg.GuacBot.GuacAPI.Model;

namespace Zyborg.GuacBot.GuacAPI
{
    internal class GuacAuthenticationState
    {
        public GuacAuthenticationState()
        { 
            
        }

        public GuacAuthenticationState(AuthenticateResult result)
        {
            LastResult = result;
            AuthToken = result.AuthToken;
            DataSource = result.DataSource;
            AvailableDataSources = result.AvailableDataSources;
            Username = result.Username;
            Type = result.Type;
        }

        public string AuthToken { get; set; }

        public string DataSource { get; set; }

        public string[] AvailableDataSources { get; set; }

        public string Username { get; set; }
        
        public string Type { get; set; }

        public AuthenticateResult LastResult { get; set; }
    }
}