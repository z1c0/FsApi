using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FsApi
{
  internal static class ResponseParser
  {
    internal static FsResult Parse(Verb verb, string command, string xml)
    {
      var xdoc = XDocument.Parse(xml);
      var status = xdoc.Descendants("status").First().Value;
      if (status != "FS_OK")
      {
        throw new InvalidOperationException(status);
      }
      if (verb == Verb.Get)
      {
        var value = xdoc.Descendants("value").Single();
        switch (command)
        {
          case Command.POWER:
            return new FsResult<bool>(ParseBool(value));

          case Command.MODE:
            return new FsResult<int>(ParseInt(value));

          case Command.VOLUME:
          case Command.VOLUME_STEPS:
            return new FsResult<byte>(ParseByte(value));

          case Command.PLAY_INFO_NAME:
          case Command.PLAY_INFO_TEXT:
          case Command.PLAY_INFO_GRAPHIC:
            return new FsResult<string>(ParseString(value));
        }
        throw new NotImplementedException(command);
      }
      else if (verb == Verb.ListGetNext)
      {
        switch (command)
        {
          case Command.VALID_MODES:
            return new FsResult<IEnumerable<RadioMode>>(ParseValidModes(xdoc));

          case Command.EQUALIZER_PRESETS:
            return new FsResult<IEnumerable<EqualizerPreset>>(ParseEqualizerPresets(xdoc));

          case Command.PRESETS:
            return new FsResult<IEnumerable<Preset>>(ParsePresets(xdoc));
        }
        throw new NotImplementedException(command);
      }
      else if (verb == Verb.Set)
      {
        return new FsResult<bool>();
      }
      throw new NotImplementedException(verb.ToString());
    }

    private static IEnumerable<Preset> ParsePresets(XDocument xdoc)
    {
      return
        from i in xdoc.Descendants("item")
        select new Preset
        {
          Key = int.Parse(i.Attribute("key").Value),
          Label = ParseString(GetField(i, "name")),
        };
    }

    private static IEnumerable<EqualizerPreset> ParseEqualizerPresets(XDocument xdoc)
    {
      return
        from i in xdoc.Descendants("item")
        select new EqualizerPreset
        {
          Key = int.Parse(i.Attribute("key").Value),
          Label = ParseString(GetField(i, "label")),
        };
    }

    private static IEnumerable<RadioMode> ParseValidModes(XDocument xdoc)
    {
      return
        from i in xdoc.Descendants("item")
        select new RadioMode
        {
          Key = int.Parse(i.Attribute("key").Value),
          Id = ParseString(GetField(i, "id")),
          Label = ParseString(GetField(i, "label")),
          IsSelectable = ParseBool(GetField(i, "selectable"))
        };
    }

    private static XElement GetField(XElement e, string name)
    {
      return e.Descendants("field").Where(f => f.Attribute("name").Value == name).Single();
    }

    private static byte ParseByte(XElement value)
    {
      return byte.Parse(value.Descendants("u8").Single().Value);
    }

    private static int ParseInt(XElement value)
    {
      return byte.Parse(value.Descendants("u32").Single().Value);
    }

    private static bool ParseBool(XElement value)
    {
      return ParseByte(value) != 0;
    }

    private static string ParseString(XElement value)
    {
      return value.Descendants("c8_array").Single().Value;
    }
  }
}
