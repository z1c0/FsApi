using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FsApi
{
  internal class Communicator : ICommunicator
  {
    private readonly HttpClient _httpClient;
    private int _pin;
    private int _sessionId;


    internal Communicator(Uri baseUri)
    {
      _httpClient = new HttpClient { BaseAddress = baseUri };
      _httpClient.DefaultRequestHeaders.Clear();
      _httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true };
    }

    public void Dispose()
    {
      _httpClient.Dispose();
    }

    public async Task<FsResult<int>> CreateSession(int pin)
    {
      _pin = pin;
      var result = await GetResponse<int>(null, null, Verb.CreateSession);
      _sessionId = result.Value;
      return result;
    }

    public async Task<FsResult<T>> GetResponse<T>(string command, Dictionary<string, string> args, Verb verb)
    {
      FsResult<T> result;
      try
      {
        args = EnsureArgs(args, verb);
        var uri = BuildUrl(verb, command, args);
        var response = await _httpClient.GetAsync(uri);
        response.EnsureSuccessStatusCode();
        var s = await response.Content.ReadAsStringAsync();
        result = (FsResult<T>)ResponseParser.Parse(verb, command, s);
      }
      catch (Exception e)
      {
        result = new FsResult<T> { Error = e };
      }
      result.Command = command;
      return result;
    }

    private Dictionary<string, string> EnsureArgs(Dictionary<string, string> args, Verb verb)
    {
      if (args == null)
      {
        args = new Dictionary<string, string>();
      }
      args["pin"] = _pin.ToString();
      if (verb != Verb.CreateSession)
      {
        args["session"] = _sessionId.ToString();
      }
      if (verb == Verb.ListGetNext && !args.ContainsKey("maxItems"))
      {
        args["maxItems"] = ushort.MaxValue.ToString();
      }
      return args;
    }

    private static string BuildUrl(Verb verb, string command, Dictionary<string, string> args)
    {
      var sb = new StringBuilder();
      switch (verb)
      {
        case Verb.CreateSession:
          sb.Append("CREATE_SESSION");
          break;

        case Verb.Get:
          sb.Append("GET");
          break;

        case Verb.Set:
          sb.Append("SET");
          break;

        case Verb.ListGetNext:
          sb.Append("LIST_GET_NEXT");
          break;

        default:
          throw new InvalidOperationException("verb");
      }
      if (command != null)
      {
        sb.Append('/');
        sb.Append(command);
      }
      if (verb == Verb.ListGetNext)
      {
        sb.Append("/-1"); // TODO
      }
      var delim = '?';
      foreach (var e in args)
      {
        sb.Append(delim);
        sb.Append(e.Key);
        sb.Append('=');
        sb.Append(WebUtility.UrlEncode(e.Value));
        delim = '&';
      }
      return sb.ToString();
    }
  }
}
