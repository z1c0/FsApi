using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FsApi
{
  internal interface ICommunicator : IDisposable
  {
        Task<FsResult<int>> CreateSession(int pin);
        Task<FsResult<int>> DeleteSession();
        Task<FsResult<T>> GetResponse<T>(string command, Dictionary<string, string> args = null, Verb verb = Verb.Get);
  }
}
