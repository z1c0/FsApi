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
    internal static async Task<FsResult> Parse(Verb verb, string command, HttpResponseMessage response)
    {
      response.EnsureSuccessStatusCode();
      var s = await response.Content.ReadAsStringAsync();
      var xdoc = XDocument.Parse(s);
      // TODO: FS_TIMEOUT
      if (verb == Verb.Get)
      {
        var value = xdoc.Descendants("value").Single();
        switch (command)
        {
          case Command.POWER:
            return ParseBool(value);

          case Command.VOLUME:
          case Command.VOLUME_STEPS:
            return ParseByte(value);

          case Command.PLAY_INFO_NAME:
          case Command.PLAY_INFO_TEXT:
          case Command.PLAY_INFO_GRAPHIC:
            return ParseString(value);
        }
        throw new NotImplementedException(command);
      }
      return new FsResult<bool>();
    }

    private static FsResult<byte> ParseByte(XElement value)
    {
      var v = byte.Parse(value.Descendants("u8").Single().Value);
      return new FsResult<byte>() { Value = v };
    }

    private static FsResult<bool> ParseBool(XElement value)
    {
      var v = value.Descendants("u8").Single().Value;
      return new FsResult<bool>() { Value = v != "0" };
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
