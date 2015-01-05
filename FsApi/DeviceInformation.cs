using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace FsApi
{
  public class DeviceInformation
  {
    public string FriendlyName { get; internal set; }
    public string HostName { get; internal set; }
    public string Version { get; internal set; }
    public string WebApiUrl { get; internal set; }

    public override string ToString()
    {
      return FriendlyName;
    }

    public static async Task<DeviceInformation> FromLocationUri(Uri uri)
    {
      var di = new DeviceInformation();
      using (var client = new HttpClient())
      {
        var s = await client.GetStringAsync(uri);
        var xdoc = XDocument.Parse(s);
        di.HostName = uri.Host;
        di.FriendlyName = xdoc.Descendants("friendlyName").First().Value;
        di.Version = xdoc.Descendants("version").First().Value;
        di.WebApiUrl = xdoc.Descendants("webfsapi").First().Value;
      }
      return di;
    }
  }
}

