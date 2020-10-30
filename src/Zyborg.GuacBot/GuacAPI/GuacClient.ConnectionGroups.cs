using System.Collections.Generic;
using System.Threading.Tasks;
using Zyborg.GuacBot.GuacAPI.Model;

namespace Zyborg.GuacBot.GuacAPI
{
    public partial class GuacClient
    {
        public async Task<Dictionary<string, ConnectionGroup>> GetConnectionGroups(string dataSource = null)
        {
            AssertAuthWithDS(ref dataSource);

            var uri = $"{_server}/api/session/data/{dataSource}/connectionGroups/?token={AuthToken}";
            using var resp = await _http.GetAsync(uri);
            resp.EnsureSuccessStatusCode();

            var result = await DeserializeAsync<Dictionary<string, ConnectionGroup>>(resp.Content);
            return result;
        }

        public async Task<ConnectionGroup> GetConnectionGroup(string groupId, string dataSource = null, bool tree = false)
        {
            AssertAuthWithDS(ref dataSource);

            var uri = $"{_server}/api/session/data/{dataSource}/connectionGroups/{groupId}";
            if (tree)
            {
                uri += "/tree";
            }
            uri += $"/?token={AuthToken}";
            using var resp = await _http.GetAsync(uri);
            resp.EnsureSuccessStatusCode();

            var result = await DeserializeAsync<ConnectionGroup>(resp.Content);
            return result;
        }

        public async Task<ConnectionGroup> CreateConnectionGroup(ConnectionGroup group, string dataSource = null)
        {
            AssertAuthWithDS(ref dataSource);

            var uri = $"{_server}/api/session/data/{dataSource}/connectionGroups/?token={AuthToken}";
            using var body = CreateHttpContent(group);
            using var resp = await _http.PostAsync(uri, body);
            resp.EnsureSuccessStatusCode();

            var result = await DeserializeAsync<ConnectionGroup>(resp.Content);
            return result;
        }

        public async Task UpdateConnectionGroup(ConnectionGroup group, string dataSource = null)
        {
            AssertAuthWithDS(ref dataSource);

            var uri = $"{_server}/api/session/data/{dataSource}/connectionGroups/{group.Identifier}?token={AuthToken}";
            using var body = CreateHttpContent(group);
            using var resp = await _http.PutAsync(uri, body);
            resp.EnsureSuccessStatusCode();
        }


        public async Task RemoveConnectionGroup(string groupId, string dataSource = null)
        {
            AssertAuthWithDS(ref dataSource);

            var uri = $"{_server}/api/session/data/{dataSource}/connectionGroups/{groupId}?token={AuthToken}";
            using var resp = await _http.DeleteAsync(uri);
            resp.EnsureSuccessStatusCode();
        }
    }
}