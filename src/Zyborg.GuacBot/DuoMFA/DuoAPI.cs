using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using AngleSharp;
using OtpNet;
using Zyborg.GuacBot.DuoMFA.Model;

namespace Zyborg.GuacBot.DuoMFA
{
    public class DuoAPI
    {
        string _host;
        HttpClient _http;

        /// <summary>
        /// This API client attempts to mimic the interaction that the embedded Web
        /// authentication client performs via API calls, but decoding the HTML form
        /// and JSON responses.  It *MUST* be used with a DUO account that has a
        /// TOTP authentication token associated with it, as it only supports DUO
        /// challenge authentication via the generation of TOTP code.
        /// </summary>

        public DuoAPI(string host, HttpClient http = null)
        {
            _host = host;
            _http = _http ?? new HttpClient();
        }

        public Task<string> Authenticate(string parentUrl, string username, string totpSecret, string iKey, string sKey, string aKey)
        {
            var sigRequest = DuoWeb.SignRequestSimple(iKey, sKey, username);
            return Authenticate(parentUrl, username, totpSecret, sigRequest);
        }

        public async Task<string> Authenticate(string parentUrl, string username, string totpSecret, string sigRequest)
        {
            var parent = UrlEncoder.Default.Encode(parentUrl);
            var iframe = $"https://{_host}/frame/web/v1/auth?tx={sigRequest}&parent={parent}&v=2.3";


            var requ = new HttpRequestMessage(HttpMethod.Post, iframe);

            var resp = await _http.SendAsync(requ);
            resp.EnsureSuccessStatusCode();

            var respBody = await resp.Content.ReadAsStringAsync();

            using var asContext = BrowsingContext.New(Configuration.Default);
            using var asDoc = await asContext.OpenAsync(req => req.Content(respBody));

            var sidElement = asDoc.QuerySelector("input['name'='sid']")?.GetAttribute("value");
            var totp = new Totp(Base32Encoding.ToBytes(totpSecret));
            
            requ = new HttpRequestMessage(HttpMethod.Post, $"https://{_host}/frame/prompt?sid={sidElement}");
            requ.Content = new FormUrlEncodedContent(new[]
            {
                KeyValuePair.Create("sid", sidElement),
                KeyValuePair.Create("device", "phone1"),
                KeyValuePair.Create("factor", "Passcode"),
                KeyValuePair.Create("passcode", totp.ComputeTotp()),
            });
            resp = await _http.SendAsync(requ);
            resp.EnsureSuccessStatusCode();

            var promptResult = await JsonSerializer.DeserializeAsync<FramePromptResult>(
                await resp.Content.ReadAsStreamAsync());
            if (promptResult.Stat != "OK")
            {
                throw new Exception("Prompt did not return success: " + promptResult.Stat);
            }

            requ = new HttpRequestMessage(HttpMethod.Post, $"https://{_host}/frame/status");
            requ.Content = new FormUrlEncodedContent(new[]
            {
                KeyValuePair.Create("sid", sidElement),
                KeyValuePair.Create("txid", promptResult.Response.Txid),
            });
            resp = await _http.SendAsync(requ);
            resp.EnsureSuccessStatusCode();
            
            var statusResult = await JsonSerializer.DeserializeAsync<FrameStatusResult>(
                await resp.Content.ReadAsStreamAsync());
            if (statusResult.Stat != "OK")
            {
                throw new Exception("Pre status did not return success: " + statusResult.Stat);
            }
            if (statusResult.Response.StatusCode != "allow")
            {
                throw new Exception("Pre status did not allow passcode: " + statusResult.Response.StatusCode);
            }
            
            requ = new HttpRequestMessage(HttpMethod.Post, $"https://{_host}{statusResult.Response.ResultUrl}");
            requ.Content = new FormUrlEncodedContent(new[]
            {
                KeyValuePair.Create("sid", sidElement),
            });
            resp = await _http.SendAsync(requ);
            resp.EnsureSuccessStatusCode();

            var cookieResult = await JsonSerializer.DeserializeAsync<FrameCookieResult>(
                await resp.Content.ReadAsStreamAsync());
            if (cookieResult.Stat != "OK")
            {
                throw new Exception("Cookie result did not return success: " + cookieResult.Stat);
            }

            return cookieResult.Response.Cookie;
        }
    }
}