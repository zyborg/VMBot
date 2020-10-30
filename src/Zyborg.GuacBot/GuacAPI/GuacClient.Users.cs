using System.Collections.Generic;
using System.Threading.Tasks;
using Zyborg.GuacBot.GuacAPI.Model;

namespace Zyborg.GuacBot.GuacAPI
{
    public partial class GuacClient
    {
        public async Task<Dictionary<string, User>> GetUsers(string dataSource = null)
        {
            AssertAuthWithDS(ref dataSource);

            var uri = $"{_server}/api/session/data/{dataSource}/users/?token={AuthToken}";
            using var resp = await _http.GetAsync(uri);
            resp.EnsureSuccessStatusCode();

            var result = await DeserializeAsync<Dictionary<string, User>>(resp.Content);
            return result;
        }

        public async Task<User> GetUser(string username, string dataSource = null, UserDetails? details = null)
        {
            AssertAuthWithDS(ref dataSource);

            var baseUri = $"{_server}/api/session/data/{dataSource}/users/{username}";
            var uri = baseUri;

            uri += $"?token={AuthToken}";
            using var resp = await _http.GetAsync(uri);
            resp.EnsureSuccessStatusCode();

            var result = await DeserializeAsync<User>(resp.Content);

            if (details != null)
            {
                if (details.Value.HasFlag(UserDetails.History))
                    result.History = await GetUserDetail<List<object>>(
                        $"{baseUri}/history?token={AuthToken}");
                if (details.Value.HasFlag(UserDetails.UserGroups))
                    result.UserGroups = await GetUserDetail<List<object>>(
                        $"{baseUri}/userGroups?token={AuthToken}");
                if (details.Value.HasFlag(UserDetails.Permissions))
                    result.Permissions = await GetUserDetail<Dictionary<string, object>>(
                        $"{baseUri}/permissions?token={AuthToken}");
                if (details.Value.HasFlag(UserDetails.EffectivePermissions))
                    result.EffectivePermissions = await GetUserDetail<Dictionary<string, object>>(
                        $"{baseUri}/effectivePermissions?token={AuthToken}");
            }

            return result;
        }

        internal async Task<T> GetUserDetail<T>(string uri)
        {
            using var resp = await _http.GetAsync(uri);
            resp.EnsureSuccessStatusCode();
            return await DeserializeAsync<T>(resp.Content);
        }        
    }
}