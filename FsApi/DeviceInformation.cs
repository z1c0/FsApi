using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
  }
}
