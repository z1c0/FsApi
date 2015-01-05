using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FsApi
{
  public class Client : IDisposable
  {
    private HttpClient _httpClient;
    private int _pin;
    private int _sessionId;

    public Client(Uri baseUri, int pin)
    {
      _pin = pin;
      _httpClient = new HttpClient();
      _httpClient.BaseAddress = baseUri;
      _httpClient.DefaultRequestHeaders.Clear();
      _httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true };
    }

    public void Dispose()
    {
      _httpClient.Dispose();
    }

    public async Task<FsResult<int>> CreateSession()
    {
      // TODO
      var uri = "CREATE_SESSION?pin=" + _pin;
      var s = await _httpClient.GetStringAsync(uri);
      var xdoc = XDocument.Parse(s);
      var session = xdoc.Descendants("sessionId").First().Value;
      _sessionId = Convert.ToInt32(session);
      return new FsResult<int>() { Value = _sessionId };
    }

    public async Task<FsResult> SetVolume(int volume)
    {
      var args = CreateArgs("value", volume.ToString());
      return await GetResponse(Command.VOLUME, args, Verb.Set);
    }

    public async Task<FsResult<byte>> GetVolume()
    {
      return await GetResponse<byte>(Command.VOLUME);
    }

    public async Task<FsResult<byte>> GetVolumeSteps()
    {
      return await GetResponse<byte>(Command.VOLUME_STEPS);
    }

    private Dictionary<string, string> CreateArgs(params string[] args)
    {
      var d = new Dictionary<string, string>();
      d["pin"] = _pin.ToString();
      d["session"] = _sessionId.ToString();
      for (var i = 0; i < args.Length; i += 2)
      {
        d[args[i]] = args[i + 1];
      }
      return d;
    }

    private async Task<FsResult> GetResponse(string command, Dictionary<string, string> args = null, Verb verb = Verb.Get)
    {
      return await GetResponse<bool>(command, args, verb);
    }

    private async Task<FsResult<T>> GetResponse<T>(string command, Dictionary<string, string> args = null, Verb verb = Verb.Get)
    {
      FsResult<T> result;
      try
      {
        if (args == null)
        {
          args = CreateArgs();
        }
        var uri = BuildUrl(verb, command, args);
        var response = await _httpClient.GetAsync(uri);
        var r = await ResponseParser.Parse(verb, command, response);
        result = (FsResult<T>)r;
      }
      catch (Exception e)
      {
        result = new FsResult<T> { Error = e };
      }
      return result;
    }

    private string BuildUrl(Verb verb, string command, Dictionary<string, string> args)
    {
      var sb = new StringBuilder();
      switch (verb)
      {
        case Verb.Get:
          sb.Append("GET");
          break;

        case Verb.Set:
          sb.Append("SET");
          break;

        default:
          throw new InvalidOperationException("verb");
      }
      sb.Append('/');
      sb.Append(command);
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

    public async Task<FsResult<bool>> GetPowerStatus()
    {
      return await GetResponse<bool>(Command.POWER);
    }

    public async Task<FsResult<string>> GetPlayInfoName()
    {
      return await GetResponse<string>(Command.PLAY_INFO_NAME);
    }

    public async Task<FsResult<string>> GetPlayInfoGraphicUri()
    {
      return await GetResponse<string>(Command.PLAY_INFO_GRAPHIC);
    }

    public async Task<FsResult<string>> GetPlayInfoText()
    {
      return await GetResponse<string>(Command.PLAY_INFO_TEXT);
    }

    public async Task<FsResult> Power(bool on)
    {
      var args = CreateArgs("value", (on ? 1 : 0).ToString());
      return await GetResponse(Command.POWER, args, Verb.Set);
    }

    public async Task<FsResult> GetNotification()
    {
      var result = new FsResult();
      try
      {
        var uri = "GET_NOTIFIES?pin=" + _pin + "&sid=" + _sessionId;
        var response = await _httpClient.GetAsync(uri); // TODO
        //await ResponseParser.Parse(response);
      }
      catch (Exception e)
      {
        result.Error = e;
      }
      return result;
    }
  }
}
