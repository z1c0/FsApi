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
      // TODO: FS_TIMEOUT

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
        }
        throw new NotImplementedException(command);
      }
      else if (verb == Verb.Get)
      {
        return new FsResult<bool>();
      }
      throw new NotImplementedException(verb.ToString());
    }

    private static IEnumerable<EqualizerPreset> ParseEqualizerPresets(XDocument xdoc)
    {
      var presets = new List<EqualizerPreset>();
      var items = xdoc.Descendants("item");
      foreach (var i in items)
      {
        presets.Add(new EqualizerPreset
        {
          Key = int.Parse(i.Attribute("key").Value),
          Label = ParseString(GetField(i, "label")),
        });
      }
      return presets;
    }

    private static IEnumerable<RadioMode> ParseValidModes(XDocument xdoc)
    {
      var modes = new List<RadioMode>();
      var items = xdoc.Descendants("item");
      foreach (var i in items)
      {
        modes.Add(new RadioMode
        {
          Key = int.Parse(i.Attribute("key").Value ),
          Id = ParseString(GetField(i, "id")),
          Label = ParseString(GetField(i, "label")),
          IsSelectable = ParseBool(GetField(i, "selectable"))
        });
      }
      return modes;
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
