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
        public Task<FsResult<int>> GetMode()
        {
            return _communicator.GetResponse<int>(Command.MODE, null, Verb.Get);
        }

        /// <summary>
        /// Set the active Mode to another source
        /// </summary>
        /// <param name="Mode">Mode ID you can get the ID from GetRadioModes() list</param>
        /// <returns></returns>
        public Task<FsResult<FsVoid>> SetMode(int Mode)
        {
            var args = CreateArgs("value", Mode.ToString());
            return _communicator.GetResponse<FsVoid>(Command.MODE, args, Verb.Set);
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
        /// Set current Station as Preset 
        /// </summary>
        /// <param name="Number">Preset Number 0-9</param>
        /// <returns></returns>
        public Task<FsResult<int>> getSelectedPreset(int Number)
        {

            var args = CreateArgs("value", Number.ToString());
            return _communicator.GetResponse<int>(Command.PLAY_ADDPRESET, args, Verb.Set);
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
        /// Search in current nav list for Values
        /// </summary>
        /// <param name="SearchValue"></param>
        /// <returns></returns>
        public Task<FsResult<FsVoid>> searchTermNavList(String SearchValue)
        {
            var args = CreateArgs("value", SearchValue.ToString());
            return _communicator.GetResponse<FsVoid>(Command.SEARCH, args, Verb.Set);
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


        //Play:

        /// <summary>
        /// set the Current RadioStation as preset
        /// </summary>
        /// <param name="PresetNumber">Perset Number</param>
        /// <returns></returns>
        public Task<FsResult<FsVoid>> setaddPreset(int PresetNumber)
        {
            var args = CreateArgs("value", PresetNumber.ToString());
            return _communicator.GetResponse<FsVoid>(Command.PLAY_ADDPRESET, args, Verb.Set);
        }

        /// <summary>
        /// set the current play-controll mode
        /// </summary>
        /// <param name="ControlValue">1=Play; 2=Pause; 3=Next (song/station); 4=Previous (song/station)</param>
        /// <returns></returns>
        public Task<FsResult<FsVoid>> setPlayControl(int ControlValue)
        {
            var args = CreateArgs("value", ControlValue.ToString());
            return _communicator.GetResponse<FsVoid>(Command.PLAY_CONTROL, args, Verb.Set);
        }

        /// <summary>
        /// Return the current play-controll mode true=Play; false=Pause;
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<byte>> getPlayControl()
        {
            return _communicator.GetResponse<byte>(Command.PLAY_CONTROL, null, Verb.Get);
        }

        /// <summary>
        /// set the current frequency for fm 
        /// </summary>
        /// <param name="Frequency">Frequency for Station (in herz) 96700 = 96,70 MHz</param>
        /// <returns></returns>
        public Task<FsResult<FsVoid>> setFrequency(int Frequency)
        {
            var args = CreateArgs("value", Frequency.ToString());
            return _communicator.GetResponse<FsVoid>(Command.PLAY_FREQU, args, Verb.Set);
        }

        /// <summary>
        /// get the current frequency for fm (in herz) 96700 = 96,70 MHz
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<int>> getFrequency()
        {
            return _communicator.GetResponse<int>(Command.PLAY_FREQU);
        }

        /// <summary>
        /// get the name of the album of the current song
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<string>> GetPlayAlbum()
        {
            return _communicator.GetResponse<string>(Command.PLAY_ALBUM);
        }


        /// <summary>
        /// get the name of the artist of the current song
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<string>> GetPlayArtist()
        {
            return _communicator.GetResponse<string>(Command.PLAY_ARTIST);
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
        /// get the duration for the track in milliseconds
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<int>> GetDuration()
        {
            return _communicator.GetResponse<int>(Command.PLAY_DURATION);
        }

        /// <summary>
        /// get the current position in the track in milliseconds
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<int>> GetPosition()
        {
            return _communicator.GetResponse<int>(Command.PLAY_POS);
        }

        /// <summary>
        /// set the current position in the track in milliseconds
        /// </summary>
        /// <param name="position">position in the track in milliseconds</param>
        /// <returns></returns>
        public Task<FsResult<FsVoid>> setPosition(int position)
        {
            var args = CreateArgs("value", position.ToString());
            return _communicator.GetResponse<FsVoid>(Command.PLAY_POS, args, Verb.Set);
        }


        /// <summary>
        /// get the Repeate state
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<bool>> GetRepeat()
        {
            return _communicator.GetResponse<bool>(Command.PLAY_REPEATE);
        }

        /// <summary>
        /// set  the Repeate state
        /// </summary>
        /// <param name="on">on = true, off = false</param>
        /// <returns></returns>
        public Task<FsResult<FsVoid>> setRepeat(bool on)
        {
            var args = CreateArgs("value", (on ? 1 : 0).ToString());
            return _communicator.GetResponse<FsVoid>(Command.PLAY_REPEATE, args, Verb.Set);
        }



        /// <summary>
        /// get scrobble is enabled or not
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<bool>> GetScrobble()
        {
            return _communicator.GetResponse<bool>(Command.PLAY_SCROBBLE);
        }



        /// <summary>
        /// get suffle is enabled or not
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<bool>> GetSuffle()
        {
            return _communicator.GetResponse<bool>(Command.PLAY_SUFFLE);
        }

        /// <summary>
        /// set suffle is enabled or not
        /// </summary>
        /// <param name="on">on = true, off = false</param>
        /// <returns></returns>
        public Task<FsResult<FsVoid>> setSuffle(bool on)
        {
            var args = CreateArgs("value", (on ? 1 : 0).ToString());
            return _communicator.GetResponse<FsVoid>(Command.PLAY_SUFFLE, args, Verb.Set);
        }


        /// <summary>
        /// get signal strenght
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<byte>> GetSignal()
        {
            return _communicator.GetResponse<byte>(Command.PLAY_SIGNAL);
        }

        /// <summary>
        /// get Play Status 1=buffering/loading, 2=playing, 3=paused
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<byte>> GetPlayStatus()
        {
            return _communicator.GetResponse<byte>(Command.PLAY_STATUS);
        }




        //SYS

        /// <summary>
        /// get the number of the selected eq-presets
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<byte>> GetEQPreset()
        {
            return _communicator.GetResponse<byte>(Command.EQ_PRESET);
        }

        /// <summary>
        /// set the number of the selected eq-presets
        /// </summary>
        /// <param name="presetNr">Number of the Preset (get it from presetList)</param>
        /// <returns></returns>
        public Task<FsResult<FsVoid>> setEQPreset(int presetNr)
        {
            var args = CreateArgs("value", presetNr.ToString());
            return _communicator.GetResponse<FsVoid>(Command.EQ_PRESET, args, Verb.Set);
        }

        /// <summary>
        /// get the second value for costum eq-settings (Treble)
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<short>> GetEQTreble()
        {
            return _communicator.GetResponse<short>(Command.CUSTOM_EQ_TREBLE);
        }

        /// <summary>
        /// set the second value for costum eq-settings (Treble)
        /// </summary>
        /// <param name="Volume">Treble Volume -7 to +7</param>
        /// <returns></returns>
        public Task<FsResult<FsVoid>> setEQTreble(int Volume)
        {
            var args = CreateArgs("value", Volume.ToString());
            return _communicator.GetResponse<FsVoid>(Command.CUSTOM_EQ_TREBLE, args, Verb.Set);
        }

        /// <summary>
        /// get the first value for costum eq-settings (Bass)
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<short>> GetEQBass()
        {
            return _communicator.GetResponse<short>(Command.CUSTOM_EQ_BASS);
        }

        /// <summary>
        /// set the first value for costum eq-settings (Bass)
        /// </summary>
        /// <param name="Volume">Bass  Volume -7 to +7</param>
        /// <returns></returns>
        public Task<FsResult<FsVoid>> setEQBass(int Volume)
        {
            var args = CreateArgs("value", Volume.ToString());
            return _communicator.GetResponse<FsVoid>(Command.CUSTOM_EQ_BASS, args, Verb.Set);
        }

        /// <summary>
        /// get whether or not loudness is activated This function is only available if costum eq is active
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<bool>> GetEQLoudness()
        {
            return _communicator.GetResponse<bool>(Command.CUSTOM_EQ_LOUDNESS);
        }

        /// <summary>
        /// set whether or not loudness is activated This function is only available if costum eq is active
        /// </summary>
        /// <param name="on">true=on , false=off</param>
        /// <returns></returns>
        public Task<FsResult<FsVoid>> setEQLoudness(bool on)
        {
            var args = CreateArgs("value", (on ? 1 : 0).ToString());
            return _communicator.GetResponse<FsVoid>(Command.CUSTOM_EQ_LOUDNESS, args, Verb.Set);
        }

        /// <summary>
        /// get whether or not device is muted
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<bool>> GetMute()
        {
            return _communicator.GetResponse<bool>(Command.MUTE);
        }

        /// <summary>
        /// set whether or not device is muted
        /// </summary>
        /// <param name="on">true=Mute on , false=Mute off</param>
        /// <returns></returns>
        public Task<FsResult<FsVoid>> setMute(bool on)
        {
            var args = CreateArgs("value", (on ? 1 : 0).ToString());
            return _communicator.GetResponse<FsVoid>(Command.MUTE, args, Verb.Set);
        }


        /// <summary>
        /// get the highest available fm-frequency
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<int>> GetMaxFMFreq()
        {
            return _communicator.GetResponse<int>(Command.MAX_FM_FREQ);
        }

        /// <summary>
        /// get the lowest available fm-frequency
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<int>> GetMinFMFreq()
        {
            return _communicator.GetResponse<int>(Command.MIN_FM_FREQ);
        }

        /// <summary>
        /// get the Min Step for fm-frequency
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<int>> GetFMFreqStep()
        {
            return _communicator.GetResponse<int>(Command.STEP_FM_FREQ);
        }

        /// <summary>
        /// get list of available EQ Bands (Bass+Treble) with Min/Max Values
        /// </summary>
        /// <param name="startItem">number of StartIndex</param>
        /// <param name="maxItems">number Items to be loaded</param>
        /// <returns></returns>
        public Task<FsResult<IEnumerable<EQBandListItem>>> getEQBandsList(int startItem = -1, int maxItems = 2)
        {
            var args = CreateArgs("maxItems", maxItems.ToString(), "startItem", startItem.ToString());
            return _communicator.GetResponse<IEnumerable<EQBandListItem>>(Command.CUSTOM_EQ_BANDS, args, Verb.ListGetNext);
        }

        /// <summary>
        /// get network connection is not disconnected in standby
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<bool>> GetNetworkStandby()
        {
            return _communicator.GetResponse<bool>(Command.STANDBY_NETWORK);
        }

        /// <summary>
        /// set network connection is not disconnected in standby
        /// </summary>
        /// <param name="on">true=not disconnected , false=disconnected</param>
        /// <returns></returns>
        public Task<FsResult<FsVoid>> setNetworkStandby(bool on)
        {
            var args = CreateArgs("value", (on ? 1 : 0).ToString());
            return _communicator.GetResponse<FsVoid>(Command.STANDBY_NETWORK, args, Verb.Set);
        }


        /// <summary>
        /// get Sleep time left
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<int>> GetSleep()
        {
            return _communicator.GetResponse<int>(Command.SLEEP);
        }

        /// <summary>
        /// set Sleep time
        /// </summary>
        /// <param name="Seconds">tSeconds to Sleep</param>
        /// <returns></returns>
        public Task<FsResult<FsVoid>> setSleep(int Seconds)
        {
            var args = CreateArgs("value", Seconds.ToString());
            return _communicator.GetResponse<FsVoid>(Command.SLEEP, args, Verb.Set);
        }


        /// <summary>
        /// get device name
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<string>> GetDeviceName()
        {
            return _communicator.GetResponse<string>(Command.NAME);
        }


        /// <summary>
        /// Set device name
        /// </summary>
        /// <param name="Name">New Device Name</param>
        /// <returns></returns>
        public Task<FsResult<FsVoid>> SetDeviceName(String Name)
        {
            var args = CreateArgs("value", Name);
            return _communicator.GetResponse<FsVoid>(Command.NAME, args, Verb.Set);
        }



        /// <summary>
        /// get Device version string
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<string>> GetDeviceVersion()
        {
            return _communicator.GetResponse<string>(Command.VERSION);
        }


        /// <summary>
        /// get Device WLAN Signal Strength
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<byte>> GetWLANSignal()
        {
            return _communicator.GetResponse<byte>(Command.WLAN_STREGHT);
        }

        /// <summary>
        /// get Device WLAN MAC Adress
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<String>> GetWLANMAC()
        {
            return _communicator.GetResponse<String>(Command.WIRELESS_MAC);
        }

        /// <summary>
        /// get Device LAN MAC Adress
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<String>> GetLANMAC()
        {
            return _communicator.GetResponse<String>(Command.WIRED_MAC);
        }

        /// <summary>
        /// get current date
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<string>> GetDate()
        {
            return _communicator.GetResponse<string>(Command.DATE);
        }

        /// <summary>
        /// get current time
        /// </summary>
        /// <returns></returns>
        public Task<FsResult<string>> GetTime()
        {
            return _communicator.GetResponse<string>(Command.TIME);
        }

        /// <summary>
        /// get DAB RadioDNS Name
        /// </summary>
        /// <returns></returns>
        public async Task<FsResult<string>> GetDABRadiDNSInfo()
        {
            try
            {
                var ecc = (await _communicator.GetResponse<byte>(Command.DAB_ECC)).Value.ToString("X");

                var eid = (await _communicator.GetResponse<ushort>(Command.DAB_EID)).Value.ToString("X");

                var sid = (await _communicator.GetResponse<int>(Command.DAB_SID)).Value.ToString("X");

                var scid = (await _communicator.GetResponse<byte>(Command.DAB_SCID)).Value.ToString("X");
                return new FsResult<string>(scid + "." + sid + "." + eid + "." + sid[0] + ecc + ".dab.radiodns.org");
            }
            catch
            {
                return new FsResult<string>(null);
            }

        }


        /// <summary>
        /// get FM RadioDNS Name
        /// </summary>
        /// <returns></returns>
        public async Task<FsResult<string>> GetFMRadiDNSInfo()
        {
            try
            {
                var ecc = (await _communicator.GetResponse<byte>(Command.DAB_ECC)).Value.ToString("X");

                var sid = (await _communicator.GetResponse<ushort>(Command.FM_RDSPI)).Value.ToString("X");

                var freq = (await _communicator.GetResponse<int>(Command.PLAY_FREQU)).Value.ToString();
                return new FsResult<string>(freq + "." + sid + "." + sid[0] + ecc + ".fm.radiodns.org");
            }
            catch
            {
                return new FsResult<string>(null);
            }

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
