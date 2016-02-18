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
                throw new FsException(status);
            }
            if (verb == Verb.CreateSession)
            {
                var sessionId = xdoc.Descendants("sessionId").First().Value;
                return new FsResult<int>(int.Parse(sessionId));
            }
            if (verb == Verb.DeleteSession)
            {
                return new FsResult<int>(0);
            }
            else if (verb == Verb.Get)
            {
                var value = xdoc.Descendants("value").Single();
                switch (command)
                {
                    case Command.POWER:
                    case Command.NAVSTATUS:
                        return new FsResult<bool>(ParseBool(value));

                    case Command.SELECTPRESET:
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

                    case Command.NAVLIST:
                        return new FsResult<IEnumerable<NavListItem>>(ParseNavList(xdoc));
                        

                }
                throw new NotImplementedException(command);
            }
            else if (verb == Verb.GetNotify)
            {
                return new FsResult<IEnumerable<FsNotification>>(ParseNotifications(xdoc));
            }
            else if (verb == Verb.Set)
            {
                return new FsResult<FsVoid>();
            }
            throw new NotImplementedException(verb.ToString());
        }

        private static IEnumerable<FsNotification> ParseNotifications(XContainer xdoc)
        {
            return
              from n in xdoc.Descendants("notify")
              select new FsNotification
              {
                  Name = n.Attribute("node").Value,
                  Value = ParseValue(n.Descendants("value").Single()),
              };
        }

        private static IEnumerable<Preset> ParsePresets(XContainer xdoc)
        {
            return
              from i in xdoc.Descendants("item")
              select new Preset
              {
                  Key = int.Parse(i.Attribute("key").Value),
                  Label = ParseString(GetField(i, "name")),
              };
        }

        private static IEnumerable<EqualizerPreset> ParseEqualizerPresets(XContainer xdoc)
        {
            return
              from i in xdoc.Descendants("item")
              select new EqualizerPreset
              {
                  Key = int.Parse(i.Attribute("key").Value),
                  Label = ParseString(GetField(i, "label")),
              };
        }

        private static IEnumerable<RadioMode> ParseValidModes(XContainer xdoc)
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

        private static IEnumerable<NavListItem> ParseNavList(XContainer xdoc)
        {
            var result =
              from i in xdoc.Descendants("item")
              select new NavListItem
              {
                  Key = int.Parse(i.Attribute("key").Value),
                  Label = ParseValue(GetField(i, "name")).ToString(),
                  name = ParseValue(GetField(i, "name")).ToString(),
                  ItemType = ParseByte(GetField(i, "type")),
                  subtype = ParseByte(GetField(i, "subtype"))
              };
            return result;
        }


        private static IEnumerable<NavListItem> ParsePresetList(XContainer xdoc)
        {
            var result =
              from i in xdoc.Descendants("item")
              select new NavListItem
              {
                  Key = int.Parse(i.Attribute("key").Value),
                  //name = ParseValue(GetField(i, "name")).ToString(),
                  //ItemType = ParseByte(GetField(i, "type")),
                  //subtype = ParseByte(GetField(i, "subtype"))
              };
            return result;
        }

        private static XElement GetField(XContainer e, string name)
        {
            return e.Descendants("field").Single(f => f.Attribute("name").Value == name);
        }

        private static object ParseValue(XContainer value)
        {
            var type = value.Descendants().First().Name.LocalName;
            switch (type)
            {
                case "u8":
                    return ParseByte(value);

                case "u32":
                    return ParseInt(value);

                case "c8_array":
                    return ParseString(value);
            }
            throw new NotImplementedException(type);
        }

        private static byte ParseByte(XContainer value)
        {
            return byte.Parse(value.Descendants("u8").Single().Value);
        }

        private static int ParseInt(XContainer value)
        {
            return byte.Parse(value.Descendants("u32").Single().Value);
        }

        private static bool ParseBool(XContainer value)
        {
            return ParseByte(value) != 0;
        }

        private static string ParseString(XContainer value)
        {
            return value.Descendants("c8_array").Single().Value;
        }
    }
}
