namespace FsApi
{
    internal static class Command
    {
        //Sys
        public const string POWER = "netRemote.sys.power";
        public const string VOLUME_STEPS = "netRemote.sys.caps.volumeSteps";
        public const string VOLUME = "netRemote.sys.audio.volume";
        public const string MODE = "netRemote.sys.mode";
        public const string VALID_MODES = "netRemote.sys.caps.validModes";
        public const string EQUALIZER_PRESETS = "netRemote.sys.caps.eqPresets";
        public const string CUSTOM_EQ_BASS = "netRemote.sys.audio.eqCustom.param0";
        public const string CUSTOM_EQ_TREBLE = "netRemote.sys.audio.eqCustom.param1";
        public const string CUSTOM_EQ_RAW = "netRemote.sys.audio.eqCustom.param";
        public const string CUSTOM_EQ_LOUDNESS = "netRemote.sys.audio.eqLoudness";
        public const string EQ_PRESET = "netRemote.sys.audio.eqPreset";
        public const string MUTE = "netRemote.sys.audio.mute";
        public const string MIN_FM_FREQ = "netRemote.sys.caps.fmFreqRange.lower";
        public const string STEP_FM_FREQ = "netRemote.sys.caps.fmFreqRange.stepSize";
        public const string MAX_FM_FREQ = "netRemote.sys.caps.fmFreqRange.upper";
        public const string CUSTOM_EQ_BANDS = "netRemote.sys.caps.eqBands";

        public const string NAME = "netRemote.sys.info.friendlyName";
        public const string VERSION = "netRemote.sys.info.version";
        public const string STANDBY_NETWORK = "netRemote.sys.net.keepConnected";

        public const string DATE = "netRemote.sys.clock.localDate";
        public const string TIME = "netRemote.sys.clock.localTime";



        //Nav:
        public const string PRESETS = "netRemote.nav.presets";
        public const string NAVIGATE = "netRemote.nav.action.navigate";
        public const string SELECTITEM = "netRemote.nav.action.selectItem";
        public const string SELECTPRESET = "netRemote.nav.action.selectPreset";
        public const string NAVLIST = "netRemote.nav.list";
        public const string NUMENTRIES = "netRemote.nav.numItems";
        public const string SEARCH = "netRemote.nav.searchTerm";
        public const string NAVSTATE = "netRemote.nav.state";
        public const string NAVSTATUS = "netRemote.nav.status";

        //Play:
        public const string PLAY_INFO_GRAPHIC = "netRemote.play.info.graphicUri";
        public const string PLAY_INFO_NAME = "netRemote.play.info.name";
        public const string PLAY_INFO_TEXT = "netRemote.play.info.text";
        public const string PLAY_ADDPRESET = "netRemote.play.addPreset";
        public const string PLAY_CONTROL = "netRemote.play.control";
        public const string PLAY_ERROR = "netRemote.play.errorStr";
        public const string PLAY_FREQU = "netRemote.play.frequency";
        public const string PLAY_ALBUM = "netRemote.play.info.album";
        public const string PLAY_ARTIST = "netRemote.play.info.artist";
        public const string PLAY_DURATION = "netRemote.play.info.duration";
        public const string PLAY_POS = "netRemote.play.position";
        public const string PLAY_RATE = "netRemote.play.rate";
        public const string PLAY_REPEATE = "netRemote.play.repeat";
        public const string PLAY_SCROBBLE = "netRemote.play.scrobble";
        public const string PLAY_SUFFLE = "netRemote.play.shuffle";
        public const string PLAY_SIGNAL = "netRemote.play.signalStrength";
        public const string PLAY_STATUS = "netRemote.play.status";
    }
}
