using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace FsApi.UnitTest
{
  [TestClass]
  public class ResponseParserTest
  {
    [TestMethod]
    public void ParseValidModes()
    {      
      var r = (FsResult<IEnumerable<RadioMode>>)ResponseParser.Parse(Verb.ListGetNext, Command.VALID_MODES, Xml.RadioModes);
      Assert.IsTrue(r.Succeeded);
      Assert.AreEqual(8, r.Value.Count());
      Assert.IsNotNull(r.Value.First(m => m.Label == "AUX-Modus"));
    }

    [TestMethod]
    public void ParsePresets()
    {
      var r = (FsResult<IEnumerable<Preset>>)ResponseParser.Parse(Verb.ListGetNext, Command.PRESETS, Xml.Presets);
      Assert.IsTrue(r.Succeeded);
      Assert.AreEqual(10, r.Value.Count());
    }

    [TestMethod]
    public void ParseEqualizerPresets()
    {
      var r = (FsResult<IEnumerable<EqualizerPreset>>)ResponseParser.Parse(Verb.ListGetNext, Command.EQUALIZER_PRESETS, Xml.EqualizerPresets);
      Assert.IsTrue(r.Succeeded);
      Assert.AreEqual(9, r.Value.Count());
      Assert.IsNotNull(r.Value.First(m => m.Label == "Nachrichten"));
    }

    [TestMethod]
    public void ParseNotificationSingle()
    {
      var r = (FsResult<IEnumerable<FsNotification>>)ResponseParser.Parse(Verb.GetNotify, null, Xml.NotificationSingle);
      Assert.IsTrue(r.Succeeded);
      Assert.AreEqual(1, r.Value.Count());
      Assert.AreEqual("netremote.play.status", r.Value.ElementAt(0).Name);
      Assert.AreEqual(2, r.Value.ElementAt(0).GetValue<byte>());
    }

    [TestMethod]
    public void ParseNotificationMultiple()
    {
      var r = (FsResult<IEnumerable<FsNotification>>)ResponseParser.Parse(Verb.GetNotify, null, Xml.NotificationMultiple);
      Assert.IsTrue(r.Succeeded);
      Assert.AreEqual(2, r.Value.Count());
      Assert.AreEqual("netremote.play.info.name", r.Value.ElementAt(0).Name);
      Assert.AreEqual("Lounge FM Vienna", r.Value.ElementAt(0).GetValue<string>());
      Assert.AreEqual("netremote.play.info.text", r.Value.ElementAt(1).Name);
      Assert.AreEqual("Coralie Clément - L'Ombre Et La Lumiere", r.Value.ElementAt(1).GetValue<string>());
    }

    [TestMethod]
    [ExpectedException(typeof(FsException))]
    public void ParseTimeout()
    {
      ResponseParser.Parse(Verb.GetNotify, null, Xml.Timeout);
    }
  }
}
