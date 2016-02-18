using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace FsApi
{
    public class Client : IDisposable
    {
        private readonly ICommunicator _communicator;

        /// <summary>
        /// This represents the Radio Device you need this Object to set and get data!
        /// </summary>
        /// <param name="baseUri">The Uri to the fsapi like http://192.169.0.2/fsapi/</param>
        public Client(Uri baseUri)
        {
            _communicator = new Communicator(baseUri);
        }

        /// <summary>
        /// Disposes the Client, you should close Session before
        /// </summary>
        public void Dispose()
        {
            _communicator.Dispose();
        }

        /// <summary>
        /// You have to Create a session first to get data from your device
        /// </summary>
        /// <param name="pin">the PIN which was set in the device menu (default 1234)</param>
        /// <returns></returns>
        public Task<FsResult<int>> CreateSession(int pin)
        {
            return _communicator.CreateSession(pin);
        }

        /// <summary>
        /// Close session after this you have to call CreateSession if you want to get data again
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<int>> DeleteSession()
        {
            return _communicator.DeleteSession();
        }

        /// <summary>
        /// Set the Device Volume
        /// </summary>
        /// <param name="volume">Raw Volume Value get Max value by GetVolumeSteps()</param>
        /// <returns></returns>
        public Task<FsResult<FsVoid>> SetVolume(int volume)
        {
            var args = CreateArgs("value", volume.ToString());
            return _communicator.GetResponse<FsVoid>(Command.VOLUME, args, Verb.Set);
        }

        /// <summary>
        /// Get the Max-Value to be send by SetVolume
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<byte>> GetVolumeSteps()
        {
            return _communicator.GetResponse<byte>(Command.VOLUME_STEPS);
        }

        /// <summary>
        /// Get the current device Volume
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<byte>> GetVolume()
        {
            return _communicator.GetResponse<byte>(Command.VOLUME);
        }

        /// <summary>
        /// Get a list with all available Radio Modes, like DAB,FM,AUX ...
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<IEnumerable<RadioMode>>> GetRadioModes()
        {
            return _communicator.GetResponse<IEnumerable<RadioMode>>(Command.VALID_MODES, null, Verb.ListGetNext);
        }

        /// <summary>
        /// Get current active Mode
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<IEnumerable<int>>> GetMode()
        {
            return _communicator.GetResponse<IEnumerable<int>>(Command.MODE, null, Verb.Get);
        }

        /// <summary>
        /// Set the active Mode to another source
        /// </summary>
        /// <param name="Mode">Mode ID you can get the ID from GetRadioModes() list</param>
        /// <returns></returns>
        public Task<FsResult<IEnumerable<FsVoid>>> SetMode(int Mode)
        {
            var args = CreateArgs("value", Mode.ToString());
            return _communicator.GetResponse<IEnumerable<FsVoid>>(Command.MODE, args, Verb.Set);
        }


        public Task<FsResult<IEnumerable<EqualizerPreset>>> GetEqualizerPresets()
        {
            return _communicator.GetResponse<IEnumerable<EqualizerPreset>>(Command.EQUALIZER_PRESETS, null, Verb.ListGetNext);
        }

        /// <summary>
        /// Get the User Presets stored in the Radio
        /// </summary>
        /// <param name="startItem">number of StartIndex</param>
        /// <param name="maxItems">number Items to be loaded</param>
        /// <returns></returns>
        public Task<FsResult<IEnumerable<Preset>>> GetPresets(int startItem = -1, int maxItems =20)
        {
            var args = CreateArgs("maxItems", maxItems.ToString(), "startItem", startItem.ToString());
            return _communicator.GetResponse<IEnumerable<Preset>>(Command.PRESETS, args, Verb.ListGetNext);
        }

        /// <summary>
        /// Get the current selected Preset (only 0 don't know why)
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<int>> getSelectedPreset()
        {
            return _communicator.GetResponse<int>(Command.SELECTPRESET);
        }

        /// <summary>
        /// Set Selected Preset 
        /// </summary>
        /// <param name="presetNr">preset id from GetPresets()</param>
        /// <returns></returns>
        public Task<FsResult<FsVoid>> setSelectedPreset(int presetNr)
        {
            var args = CreateArgs("value", presetNr.ToString());
            return _communicator.GetResponse<FsVoid>(Command.SELECTPRESET, args, Verb.Set);
        }

        /// <summary>
        /// Get the current Powerstatus from the Radio
        /// </summary>
        /// <returns>True: on, False: off</returns>
        public Task<FsResult<bool>> GetPowerStatus()
        {
            return _communicator.GetResponse<bool>(Command.POWER);
        }

        /// <summary>
        /// set the current Powerstatus from the Radio
        /// </summary>
        /// <param name="on">on: true, off: false</param>
        /// <returns></returns>
        public Task<FsResult<FsVoid>> setPowerStatus(bool on)
        {
            var args = CreateArgs("value", (on ? 1 : 0).ToString());
            return _communicator.GetResponse<FsVoid>(Command.POWER, args, Verb.Set);
        }

        /// <summary>
        /// get current station name 
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<string>> GetPlayInfoName()
        {
            return _communicator.GetResponse<string>(Command.PLAY_INFO_NAME);
        }

        /// <summary>
        /// get web uri, the the image from current programm
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<string>> GetPlayInfoGraphicUri()
        {
            return _communicator.GetResponse<string>(Command.PLAY_INFO_GRAPHIC);
        }

        /// <summary>
        /// get Radio Text or current song name
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<string>> GetPlayInfoText()
        {
            return _communicator.GetResponse<string>(Command.PLAY_INFO_TEXT);
        }

        /// <summary>
        /// get Menu/Channel list for current Mode
        /// </summary>
        /// <param name="startItem">number of StartIndex</param>
        /// <param name="maxItems">number Items to be loaded</param>
        /// <returns></returns>
        public Task<FsResult<IEnumerable<NavListItem>>> getMenuList(int startItem = -1, int maxItems = 50)
        {
            var args = CreateArgs("maxItems", maxItems.ToString(), "startItem", startItem.ToString());
            return _communicator.GetResponse<IEnumerable<NavListItem>>(Command.NAVLIST, args, Verb.ListGetNext);
        }

        /// <summary>
        /// select a playable Item from getMenuList()
        /// </summary>
        /// <param name="ItemNr">id form getMenuList()</param>
        /// <returns></returns>
        public Task<FsResult<FsVoid>> setSelectedItem(int ItemNr)
        {
            var args = CreateArgs("value", ItemNr.ToString());
            return _communicator.GetResponse<FsVoid>(Command.SELECTITEM, args, Verb.Set);
        }

        /// <summary>
        /// Navigate to item (folder) from getMenuList()
        /// </summary>
        /// <param name="ItemNr">id from getMenuList(), go back with -1</param>
        /// <returns></returns>
        public Task<FsResult<FsVoid>> setNavigtion(int ItemNr)
        {
            var args = CreateArgs("value", ItemNr.ToString());
            return _communicator.GetResponse<FsVoid>(Command.NAVIGATE, args, Verb.Set);
        }

        /// <summary>
        /// Enable getMenuList(), setSelectedItem(), setNavigtion() the are olny working if this is enabled Every change of the system mode, will disable the nav state to reset the current menu-position.
        /// </summary>
        /// <param name="on">true: working, false: not working</param>
        /// <returns></returns>
        public Task<FsResult<FsVoid>> setNavState(bool on)
        {
            var args = CreateArgs("value", (on ? 1 : 0).ToString());
            return _communicator.GetResponse<FsVoid>(Command.NAVSTATE, args, Verb.Set);
        }

        /// <summary>
        /// While the device prepares the menu this node is set to 0, if the menu is ready it is set to 1.
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<bool>> getMenuStatus()
        {
            return _communicator.GetResponse<bool>(Command.NAVSTATUS);
        }

        /// <summary>
        /// get the Notifications created from the device (no more informations yet)
        /// </summary>
        /// <returns></returns>
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
