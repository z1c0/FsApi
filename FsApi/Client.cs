using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
      var remote = "netRemote.sys.audio.volume"; // TODO
      var uri = "SET/" + remote + "?pin=" + _pin + "&sid=" + _sessionId + "&value=" + volume;
      return await GetResponse(remote, uri);
    }

    private async Task<FsResult> GetResponse(string command, string uri)
    {
      return await GetResponse<bool>(command, uri);
    }

    private async Task<FsResult<T>> GetResponse<T>(string command, string uri)
    {
      FsResult<T> result;
      try
      {
        var response = await _httpClient.GetAsync(uri);
        var r = await ResponseParser.Parse(command, response);
        result = (FsResult<T>)r;
      }
      catch (Exception e)
      {
        result = new FsResult<T> { Error = e };
      }
      return result;
    }

    private string GetUrl(string command)
    {
      // TODO: "verb"
      return "GET/" + command + "?pin=" + _pin + "&sid=" + _sessionId;
    }

    public async Task<FsResult<bool>> GetPowerStatus()
    {
      return await GetResponse<bool>(Commands.POWER, GetUrl(Commands.POWER));
    }
    public async Task<FsResult<string>> GetPlayInfoText()
    {
      return await GetResponse<string>(Commands.PLAY_INFO_TEXT, GetUrl(Commands.PLAY_INFO_TEXT));
    }

    public async Task<FsResult> Power(bool on)
    {
      var remote = "netRemote.sys.power"; // TODO
      var uri = "SET/" + remote + "?pin=" + _pin + "&sid=" + _sessionId + "&value=" + (on ? 1 : 0);
      return await GetResponse(remote, uri);
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
