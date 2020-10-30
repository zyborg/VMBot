using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Common;
using Microsoft.AspNetCore.JsonPatch;
using Zyborg.GuacBot.GuacAPI.Model;

namespace Zyborg.GuacBot.GuacAPI
{
    public partial class GuacClient
    {
        private HttpClient _http;
        private string _server;
        private GuacAuthenticationState _state;

        private static readonly JsonSerializerOptions ModelSerOptions = new JsonSerializerOptions()
        {
            Converters = {
                //new JsonStringEnumConverter(allowIntegerValues: false),
                new EnumCustomConverterFactory(),
            },
            IgnoreNullValues = true,
        };

        public GuacClient(string server, HttpClient http = null,
            string authToken = null,
            string defaultDataSource = null)
        {
            _server = server;
            _http = http ?? new HttpClient();

            if (authToken != null)
            {
                _state = new GuacAuthenticationState
                {
                    AuthToken = authToken,
                    DataSource = defaultDataSource,
                };
            }
        }

        public void Reset(HttpClient http = null, string authToken = null, string dataSource = null)
        {
            if (http != null)
                _http = http;

            _state = new GuacAuthenticationState
            {
                AuthToken = authToken,
                DataSource = dataSource,
            };
        }

        public string AuthToken => _state?.AuthToken;
        public string DataSource => _state?.DataSource;
        public string AuthType => _state?.Type;
        public string Username => _state?.Username;
        public IEnumerable<string> AvailableDataSources => _state?.AvailableDataSources;

        public bool IsAuthenticated => !string.IsNullOrEmpty(_state?.AuthToken);

        public string GetAuthenticationState() => _state == null ? null
            : JsonSerializer.Serialize(_state);

        public void SetAuthenticationState(string state) => _state = state != null
            ? JsonSerializer.Deserialize<GuacAuthenticationState>(state)
            : throw new ArgumentNullException(nameof(state));

        public async Task<bool> CheckAuthenticated(bool refreshState = true)
        {
            if (string.IsNullOrEmpty(AuthToken))
            {
                return false;
            }

            var uri = $"{_server}/api/tokens";
            var requArgs = new Dictionary<string, string>
            {
                ["token"] = AuthToken,
            };

            if (refreshState)
            {
                _state = null;
            }

            using var body = new FormUrlEncodedContent(requArgs);
            using var resp = await _http.PostAsync(uri, body);
            if (!resp.IsSuccessStatusCode)
            {
                return false;
            }

            var result = await DeserializeAsync<AuthenticateResult>(resp.Content);
            if (string.IsNullOrEmpty(result.AuthToken))
            {
                return false;
            }

            if (refreshState)
            {
                _state = new GuacAuthenticationState(result);
            }
            return true;
        }

        public async Task Unauthenticate()
        {
            var uri = $"{_server}/api/tokens/{AuthToken}";
            var resp = await _http.DeleteAsync(uri);
            _state = null;
            resp.EnsureSuccessStatusCode();
        }

        public async Task Authenticate(string username, string password,
            params IAuthenticator[] auths)
        {
            var uri = $"{_server}/api/tokens";
            var requArgs = new Dictionary<string, string>
            {
                [nameof(username)] = username,
                [nameof(password)] = password,
            };

            _state = null;

            var authResult = await Authenticate(uri, requArgs, auths);
            if (authResult.Type == "INSUFFICIENT_CREDENTIALS")
            {
                throw new Exception("insufficient credentials");
            }
            if (string.IsNullOrWhiteSpace(authResult.AuthToken))
            {
                throw new Exception("Missing authentication token");
            }

            _state = new GuacAuthenticationState(authResult);
        }

        public Task<AuthenticateResult> TryAuthenticate(string username, string password,
            bool saveAuthState = false, params IAuthenticator[] auths) => TryAuthenticate(
                new Dictionary<string, string>
                {
                    [nameof(username)] = username,
                    [nameof(password)] = password,
                }, saveAuthState, auths);

        public async Task<AuthenticateResult> TryAuthenticate(Dictionary<string, string> requArgs,
            bool saveAuthState = false, params IAuthenticator[] auths)
        {
            var uri = $"{_server}/api/tokens";

            if (saveAuthState)
                _state = null;

            var authResult = await Authenticate(uri, requArgs, auths);

            if (saveAuthState)
                _state = new GuacAuthenticationState(authResult);

            return authResult;
        }

        private async Task<AuthenticateResult> Authenticate(string uri, Dictionary<string, string> requArgs,
            params IAuthenticator[] auths)
        {
            using var requ1Body = new FormUrlEncodedContent(requArgs);
            using var resp1 = await _http.PostAsync(uri, requ1Body);
            var authResult = await DeserializeAsync<AuthenticateResult>(resp1.Content);

            if (!resp1.IsSuccessStatusCode
                && authResult.Type == "INSUFFICIENT_CREDENTIALS"
                && authResult.Expected?.Length > 0)
            {
                if (auths?.Length > 0)
                {
                    foreach (var expected in authResult.Expected)
                    {
                        var authArgs = expected.GetAuthArgsAsStrings();
                        foreach (var auth in auths)
                        {
                            var moreArgs = await auth.Authenticate(expected.Type, authArgs);
                            if (moreArgs != null)
                            {
                                using var requ2Body = new FormUrlEncodedContent(requArgs.Concat(moreArgs));
                                using var resp2 = await _http.PostAsync(uri, requ2Body);
                                authResult = await DeserializeAsync<AuthenticateResult>(resp2.Content);
                                resp2.EnsureSuccessStatusCode();
                                return authResult;
                            }
                        }
                    }
                }
            }
            else
            {
                resp1.EnsureSuccessStatusCode();
            }
            return authResult;
        }

        public Task<Dictionary<string, object>> GetDataSourceObject(
            string type, string dataSource = null, string child = null) =>
            GetDataSourceEntity<Dictionary<string, object>>(type, dataSource, child);
        public Task<List<object>> GetDataSourceArray(
            string type, string dataSource = null, string child = null) =>
            GetDataSourceEntity<List<object>>(type, dataSource, child);

        public async Task<T> GetDataSourceEntity<T>(string path,
            string dataSource = null, string subpath = null)
        {
            AssertAuthWithDS(ref dataSource);

            var uri = $"{_server}/api/session/data/{dataSource}/{path}";

            if (!string.IsNullOrEmpty(subpath))
            {
                uri += $"/{subpath}";
            }

            uri += $"/?token={AuthToken}";
            using var resp = await _http.GetAsync(uri);
            resp.EnsureSuccessStatusCode();

            var result = await DeserializeAsync<T>(resp.Content);
            return result;
        }

        public async Task<T> PostDataSourceEntity<T>(T entity, string path,
            string dataSource = null)
        {
            AssertAuthWithDS(ref dataSource);

            var uri = $"{_server}/api/session/data/{dataSource}/{path}/?token={AuthToken}";
            using var body = CreateHttpContent(entity);
            using var resp = await _http.PostAsync(uri, body);
            resp.EnsureSuccessStatusCode();

            var result = await DeserializeAsync<T>(resp.Content);
            return result;
        }

        public async Task PatchDataSourceEntity<TPatch>(
            IEnumerable<JsonPatchValue<TPatch>> patches,
            string path, string dataSource = null, string subpath = null)
        {
            AssertAuthWithDS(ref dataSource);

            var uri = $"{_server}/api/session/data/{dataSource}/{path}";
            if (!string.IsNullOrEmpty(subpath))
            {
                uri += $"/{subpath}";
            }
            uri += $"/?token={AuthToken}";

            using var body = CreateHttpContent(patches);
            using var resp = await _http.PatchAsync(uri, body);
            resp.EnsureSuccessStatusCode();
        }

        public async Task<TResult> PatchDataSourceEntity<TPatch, TResult>(
            IEnumerable<JsonPatchValue<TPatch>> patches,
            string path, string dataSource = null, string subpath = null)
        {
            AssertAuthWithDS(ref dataSource);

            var uri = $"{_server}/api/session/data/{dataSource}/{path}";
            if (!string.IsNullOrEmpty(subpath))
            {
                uri += $"/{subpath}";
            }
            uri += $"/?token={AuthToken}";

            using var body = CreateHttpContent(patches);
            using var resp = await _http.PatchAsync(uri, body);
            resp.EnsureSuccessStatusCode();

            var result = await DeserializeAsync<TResult>(resp.Content);
            return result;
        }

        private void AssertAuthWithDS(ref string dataSource)
        {
            if (_state == null)
            {
                throw new Exception("client is not in authenticated state");
            }
            if (dataSource == null)
            {
                dataSource = _state.DataSource;
            }
        }

        private static ValueTask<T> DeserializeAsync<T>(Stream stream) =>
            JsonSerializer.DeserializeAsync<T>(stream, ModelSerOptions);

        private static async ValueTask<T> DeserializeAsync<T>(HttpContent content) =>
            await DeserializeAsync<T>(await content.ReadAsStreamAsync());

        private static Task SerializeAsync<T>(T value, Stream stream) =>
            JsonSerializer.SerializeAsync(stream, ModelSerOptions);

        private static string Serialize<T>(T value) =>
            JsonSerializer.Serialize(value, ModelSerOptions);

        private static HttpContent CreateHttpContent<T>(T value) =>
            new StringContent(Serialize(value), Encoding.UTF8, "application/json");
    }
}