using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FsApi
{
  internal static class ResponseParser
  {
    internal static async Task<FsResult> Parse(string command, HttpResponseMessage response)
    {
      response.EnsureSuccessStatusCode();
      var s = await response.Content.ReadAsStringAsync();
      var xdoc = XDocument.Parse(s);
      // TODO: FS_TIMEOUT

      var value = xdoc.Descendants("value").Single();
      switch (command)
      {
        case Commands.POWER:
          return ParseBool(value);

        case Commands.PLAY_INFO_NAME:
        case Commands.PLAY_INFO_TEXT:
          return ParseString(value);
      }
      throw new NotImplementedException(command);
    }

    private static FsResult<bool> ParseBool(XElement value)
    {
      var v = value.Descendants("u8").Single().Value;
      return new FsResult<bool>()
      {
        Value = v != "0"
      };
    }

    private static FsResult<string> ParseString(XElement value)
    {
      var v = value.Descendants("c8_array").Single().Value;
      return new FsResult<string>()
      {
        Value = v
      };
    }
  }
}
