using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zyborg.GuacBot.DuoMFA;

namespace Zyborg.GuacBot.GuacAPI
{
    public class DuoAuthenticator : IAuthenticator
    {
        public const string AuthType = "GUAC_DUO_SIGNED_RESPONSE";

        public string ParentUrl { get; set; }
        public string Username { get; set; }
        public string TotpSecret { get; set; }


        public async Task<IReadOnlyDictionary<string, string>> Authenticate(string authType,
            IReadOnlyDictionary<string, object> args)
        {
            if (AuthType != authType)
            {
                return null;
            }
            object arg;
            if (!args.TryGetValue("apiHost", out arg)
                || !(arg is string apiHost))
            {
                throw new Exception("arguments missing or invalid `apiHost`");
            }

            if (!args.TryGetValue("signedRequest", out arg)
                || !(arg is string signedRequest))
            {
                throw new Exception("arguments missing or invalide `signedRequest`");
            }
            var signedRequestParts = signedRequest.Split(":");
            if (signedRequestParts.Length != 2)
            {
                throw new Exception("malformed `signedRequest`");
            }

            var duo = new DuoAPI((string)apiHost);
            var sigResp = await duo.Authenticate(ParentUrl, Username, TotpSecret, signedRequestParts[0]);


            return new Dictionary<string, string>
            {
                ["guac-duo-signed-response"] = $"{sigResp}:{signedRequestParts[1]}",
            };
        }
    }
}