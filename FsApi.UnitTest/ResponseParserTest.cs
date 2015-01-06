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
    public void ParseEqualizerPresets()
    {
      var r = (FsResult<IEnumerable<EqualizerPreset>>)ResponseParser.Parse(Verb.ListGetNext, Command.EQUALIZER_PRESETS, Xml.EqualizerPresets);
      Assert.IsTrue(r.Succeeded);
      Assert.AreEqual(9, r.Value.Count());
      Assert.IsNotNull(r.Value.First(m => m.Label == "Nachrichten"));
    }
  }
}
