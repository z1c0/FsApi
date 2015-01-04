using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace FsApi
{
  public class Client : IDisposable
  {
    private HttpClient _httpClient;
    private int _pin;
    private int _sessionId;

    public Client(int pin)
    {
      _pin = pin;
      var filter = new HttpBaseProtocolFilter();
      filter.CacheControl.ReadBehavior = HttpCacheReadBehavior.MostRecent;
      filter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
      _httpClient = new HttpClient(filter);
      _httpClient.DefaultRequestHeaders.Clear();
    }

    public void Dispose()
    {
      _httpClient.Dispose();
    }

    public async Task<FsResult<int>> CreateSession()
    {
      var result = new FsResult<int>();
      try
      {
        string uri = GetBaseUri();
        uri += "CREATE_SESSION?pin=" + _pin;
        var response = await _httpClient.GetAsync(new Uri(uri));
        await ReadResponse(response);
        result.Value = _sessionId;
      }
      catch (Exception e)
      {
        result.Error = e;
      }
      return result;
    }

    private static string GetBaseUri()
    {
      // TODO
      return "http://" + "192.168.1.144" + ":" + "80" + "/fsapi/";
    }

    public async Task<FsResult> SetVolume(int volume)
    {
      var result = new FsResult<int>();
      try
      {
        var uri = GetBaseUri();
        var remote = "netRemote.sys.audio.volume"; // TODO
        uri += "SET/" + remote + "?pin=" + _pin + "&sid=" + _sessionId + "&value=" + volume;
        var response = await _httpClient.GetAsync(new Uri(uri));
      }
      catch (Exception e)
      {
        result.Error = e;
      }
      return result;
    }

    public async Task<FsResult> Power(bool on)
    {
      var result = new FsResult<int>();
      try
      {
        var uri = GetBaseUri();
        var remote = "netRemote.sys.power"; // TODO
        uri += "SET/" + remote + "?pin=" + _pin + "&sid=" + _sessionId + "&value=" + (on ? 1 : 0);
        var response = await _httpClient.GetAsync(new Uri(uri));
      }
      catch (Exception e)
      {
        result.Error = e;
      }
      return result;
    }

    public async Task<FsResult> GetNotification()
    {
      var result = new FsResult();
      try
      {
        var uri = GetBaseUri();
        uri += "GET_NOTIFIES?pin=" + _pin + "&sid=" + _sessionId;
        var response = await _httpClient.GetAsync(new Uri(uri));
        await ParseResponse(response);
      }
      catch (Exception e)
      {
        result.Error = e;
      }
      return result;
    }

    private async Task ParseResponse(HttpResponseMessage response)
    {
      var s = await response.Content.ReadAsStringAsync();
      var xdoc = XDocument.Parse(s);
      xdoc.ToString();
      // TODO: FS_TIMEOUT
    }

    private async Task ReadResponse(HttpResponseMessage response)
    {
      var s = await response.Content.ReadAsStringAsync();
      var xdoc = XDocument.Parse(s);
      var session = xdoc.Descendants("sessionId").First().Value;
      _sessionId =  Convert.ToInt32(session);
    }
  }
}
