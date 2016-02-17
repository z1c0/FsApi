using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace FsApi
{
    public class Client : IDisposable
    {
        private readonly ICommunicator _communicator;

        public Client(Uri baseUri)
        {
            _communicator = new Communicator(baseUri);
        }

        public void Dispose()
        {
            _communicator.Dispose();
        }

        public Task<FsResult<int>> CreateSession(int pin)
        {
            return _communicator.CreateSession(pin);
        }

        public Task<FsResult<int>> DeleteSession()
        {
            return _communicator.DeleteSession();
        }

        public Task<FsResult<FsVoid>> SetVolume(int volume)
        {
            var args = CreateArgs("value", volume.ToString());
            return _communicator.GetResponse<FsVoid>(Command.VOLUME, args, Verb.Set);
        }

        public Task<FsResult<byte>> GetVolume()
        {
            return _communicator.GetResponse<byte>(Command.VOLUME);
        }

        public Task<FsResult<IEnumerable<RadioMode>>> GetRadioModes()
        {
            return _communicator.GetResponse<IEnumerable<RadioMode>>(Command.VALID_MODES, null, Verb.ListGetNext);
        }

        public Task<FsResult<IEnumerable<int>>> GetMode()
        {
            return _communicator.GetResponse<IEnumerable<int>>(Command.MODE, null, Verb.Get);
        }

        public Task<FsResult<IEnumerable<FsVoid>>> SetMode(int Mode)
        {
            var args = CreateArgs("value", Mode.ToString());
            return _communicator.GetResponse<IEnumerable<FsVoid>>(Command.MODE, args, Verb.Set);
        }

        public Task<FsResult<IEnumerable<EqualizerPreset>>> GetEqualizerPresets()
        {
            return _communicator.GetResponse<IEnumerable<EqualizerPreset>>(Command.EQUALIZER_PRESETS, null, Verb.ListGetNext);
        }

        public Task<FsResult<IEnumerable<Preset>>> GetPresets()
        {
            var args = CreateArgs("maxItems", "20");
            return _communicator.GetResponse<IEnumerable<Preset>>(Command.PRESETS, args, Verb.ListGetNext);
        }

        public Task<FsResult<byte>> GetVolumeSteps()
        {
            return _communicator.GetResponse<byte>(Command.VOLUME_STEPS);
        }

        public Task<FsResult<bool>> GetPowerStatus()
        {
            return _communicator.GetResponse<bool>(Command.POWER);
        }

        public Task<FsResult<string>> GetPlayInfoName()
        {
            return _communicator.GetResponse<string>(Command.PLAY_INFO_NAME);
        }

        public Task<FsResult<string>> GetPlayInfoGraphicUri()
        {
            return _communicator.GetResponse<string>(Command.PLAY_INFO_GRAPHIC);
        }

        public Task<FsResult<string>> GetPlayInfoText()
        {
            return _communicator.GetResponse<string>(Command.PLAY_INFO_TEXT);
        }

        public Task<FsResult<IEnumerable<NavListItem>>> getMenuList()
        {
            var args = CreateArgs("maxItems", "200");
            return _communicator.GetResponse<IEnumerable<NavListItem>>(Command.NAVLIST, args, Verb.ListGetNext);
        }

        public Task<FsResult<FsVoid>> setSelectedItem(int ItemNr)
        {
            var args = CreateArgs("value", ItemNr.ToString());
            return _communicator.GetResponse<FsVoid>(Command.SELECTITEM, args, Verb.Set);
        }

        public Task<FsResult<FsVoid>> setNavigtion(int ItemNr)
        {
            var args = CreateArgs("value", ItemNr.ToString());
            return _communicator.GetResponse<FsVoid>(Command.NAVIGATE, args, Verb.Set);
        }

        public Task<FsResult<FsVoid>> Power(bool on)
        {
            var args = CreateArgs("value", (on ? 1 : 0).ToString());
            return _communicator.GetResponse<FsVoid>(Command.POWER, args, Verb.Set);
        }

        public Task<FsResult<int>> getSelectedPreset()
        {
            return _communicator.GetResponse<int>(Command.SELECTPRESET);
        }
        public Task<FsResult<FsVoid>> setSelectedPreset(int presetNr)
        {
            var args = CreateArgs("value", presetNr.ToString());
            return _communicator.GetResponse<FsVoid>(Command.SELECTPRESET, args, Verb.Set);
        }

        public Task<FsResult<FsVoid>> setNavState(bool on)
        {
            var args = CreateArgs("value", (on ? 1 : 0).ToString());
            return _communicator.GetResponse<FsVoid>(Command.NAVSTATE, args, Verb.Set);
        }

        public Task<FsResult<IEnumerable<FsNotification>>> GetNotification()
        {
            return _communicator.GetResponse<IEnumerable<FsNotification>>(null, null, Verb.GetNotify);
        }

        private static Dictionary<string, string> CreateArgs(params string[] args)
        {
            var d = new Dictionary<string, string>();
            for (var i = 0; i < args.Length; i += 2)
            {
                d[args[i]] = args[i + 1];
            }
            return d;
        }
    }
}
