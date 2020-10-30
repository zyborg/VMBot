using System.Collections.Generic;
using System.Threading.Tasks;
using Zyborg.GuacBot.GuacAPI.Model;

namespace Zyborg.GuacBot.GuacAPI
{
    public partial class GuacClient
    {
        public Task<Dictionary<string, Connection>> GetConnections(string dataSource = null) =>
            GetDataSourceEntity<Dictionary<string, Connection>>("connections", dataSource);

        public async Task<Connection> GetConnection(string connId, string dataSource = null, ConnectionDetails? details = null)
        {
            var result = await GetDataSourceEntity<Connection>("connections", dataSource, connId);

            if (details != null)
            {
                if (details.Value.HasFlag(ConnectionDetails.Parameters))
                    result.Parameters = await GetDataSourceEntity<Dictionary<string, string>>(
                        "connections", dataSource, $"{connId}/parameters");
                if (details.Value.HasFlag(ConnectionDetails.History))
                    result.History = await GetDataSourceEntity<List<ConnectionHistory>>(
                        "connections", dataSource, $"{connId}/history");
            }

            return result;
        }

        public async Task<Connection> CreateConnectionGroup(Connection conn, string dataSource = null)
        {
            AssertAuthWithDS(ref dataSource);

            var uri = $"{_server}/api/session/data/{dataSource}/connections/?token={AuthToken}";
            using var body = CreateHttpContent(conn);
            using var resp = await _http.PostAsync(uri, body);
            resp.EnsureSuccessStatusCode();

            var result = await DeserializeAsync<Connection>(resp.Content);
            return result;
        }

        // public async Task UpdateConnectionGroup(ConnectionGroup group, string dataSource = null)
        // {
        //     if (dataSource == null)
        //     {
        //         dataSource = _authResult.DataSource;
        //     }

        //     var uri = $"{_server}/api/session/data/{dataSource}/connectionGroups/{group.Identifier}?token={AuthToken}";
        //     using var body = CreateHttpContent(group);
        //     using var resp = await _http.PutAsync(uri, body);
        //     resp.EnsureSuccessStatusCode();
        // }


        // public async Task RemoveConnectionGroup(string groupId, string dataSource = null)
        // {
        //     if (dataSource == null)
        //     {
        //         dataSource = _authResult.DataSource;
        //     }

        //     var uri = $"{_server}/api/session/data/{dataSource}/connectionGroups/{groupId}?token={AuthToken}";
        //     using var resp = await _http.DeleteAsync(uri);
        //     resp.EnsureSuccessStatusCode();
        // }
    }
}