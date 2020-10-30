using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zyborg.GuacBot.GuacAPI
{
    public interface IAuthenticator
    {
        Task<IReadOnlyDictionary<string, string>> Authenticate(string authType, IReadOnlyDictionary<string, object> args);
    }
}