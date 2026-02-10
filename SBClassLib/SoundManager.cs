using NAudio.Wave;

namespace SBClassLib
{
    public class SoundManager
    {
        private static string[] s_soundExt = { "mp3", "wav" };
        private static string[] s_imgExt = { "jpg", "png" };
        private DirectSoundDeviceInfo _firstOutDevice;
        private DirectSoundDeviceInfo _secondOutDevice;
        private DirectSoundOut _firstSoundOut;
        private DirectSoundOut _secondSoundOut;
        public string DirectoryPath { get; set; }
        public List<string[]> Sounds { get; private set; }
        //Constructors
        public SoundManager()
        {
            DirectoryPath = null;
            Sounds = new();
            var outDevices = SoundManager.GetOutDevices();
            _firstOutDevice = outDevices[0];
            _firstSoundOut = null;
            _secondOutDevice = null;
            _secondSoundOut = null;
        }
        public SoundManager(string directoryPath)
        {
            DirectoryPath = directoryPath;
            Sounds = new();
            var outDevices = SoundManager.GetOutDevices();
            _firstOutDevice = outDevices[0];
            _firstSoundOut = null;
            _secondOutDevice = null;
            _secondSoundOut = null;
            LoadSounds();
        }
        //---------------------------

        //Directory managing
        public void ChangeDirectory(string directoryPath)
        {
            DirectoryPath = directoryPath;
            Sounds = new();
            LoadSounds();
        }
        private void LoadSounds()
        {
            if (DirectoryPath != null && Directory.Exists(DirectoryPath))
            {
                List<string> allFiles = Directory.EnumerateFiles(DirectoryPath, "*.*").ToList();
                List<string> sFiles = allFiles.Where(file => SoundManager.s_soundExt.Contains(Path.GetExtension(file).TrimStart('.').ToLowerInvariant())).ToList();
                List<string> imgFiles = allFiles.Where(file => SoundManager.s_imgExt.Contains(Path.GetExtension(file).TrimStart('.').ToLowerInvariant())).ToList();
                foreach (string sfile in sFiles)
                {
                    string imgfile = imgFiles.Where(file => Path.GetFileNameWithoutExtension(file) == Path.GetFileNameWithoutExtension(sfile)).FirstOrDefault();
                    Sounds.Add([sfile, imgfile]);
                }
            }
            else
            {
                throw new Exception("No directory selected");
            }
        }

        //-------------------------------

        //Out Device managing
        public void ChangeDevice(string deviceDescription, bool secondDevice = false)
        {
            var outDevice = SoundManager.GetOutDevices().Find(device => device.Description == deviceDescription);
            if (outDevice == null)
            {
                throw new Exception("No such device exist");
            }
            if (secondDevice)
            {
                _secondOutDevice = outDevice;
            }
            else
            {
                _firstOutDevice = outDevice;
            }
        }
        public void ChangeDevice(Guid deviceGuid, bool secondDevice = false)
        {
            var outDevice = SoundManager.GetOutDevices().Find(device => device.Guid == deviceGuid);
            if (outDevice == null)
            {
                throw new Exception("No such device exist");
            }
            if (secondDevice)
            {
                _secondOutDevice = outDevice;
            }
            else
            {
                _firstOutDevice = outDevice;
            }
        }

        public string[] ShowCurrentDevicesDescriptions()
        {
            return [_firstOutDevice.Description, _secondOutDevice != null ? _secondOutDevice.Description : null];
        }

        public DirectSoundDeviceInfo[] GetCurrentDevices()
        {
            return [_firstOutDevice, _secondOutDevice];
        }

        //--------------------
        //Sound managing
        public void PlaySound(int index)
        {
            if (DirectoryPath == null)
            {
                throw new Exception("No directory selected");
            }
            if (index >= Sounds.Count)
            {
                throw new Exception("Sound index out of range");
            }
            string sfilePath = Sounds[index][0];
            using (var audioFile = new AudioFileReader(sfilePath))
            {
                if (_firstSoundOut != null)
                {
                    _firstSoundOut.Dispose();
                }
                _firstSoundOut = new DirectSoundOut(_firstOutDevice.Guid);
                _firstSoundOut.Init(audioFile);
                _firstSoundOut.Play();
;
            }
            if (_secondOutDevice != null && _secondOutDevice.Guid != _firstOutDevice.Guid)
            {
                using (var audioFile = new AudioFileReader(sfilePath))
                {
                    if (_secondSoundOut != null)
                    {
                        _secondSoundOut.Dispose();
                    }
                    _secondSoundOut = new DirectSoundOut(_secondOutDevice.Guid);
                    _secondSoundOut.Init(audioFile);
                    _secondSoundOut.Play();
                }

                _secondSoundOut.Dispose();
            }



            
        }

        public void StopAll()
        { 
            if(_firstSoundOut != null)
            {
                _firstSoundOut.Stop();
            }
            if(_secondSoundOut != null)
            {
                _secondSoundOut.Stop();
            }
        }


        //-----------------------
        //Static methods

        public static List<DirectSoundDeviceInfo> GetOutDevices()
        {
            return DirectSoundOut.Devices.ToList();
        }
        //------------------------
        ~SoundManager()
        {
            if (_firstSoundOut != null)
            {
                _firstSoundOut.Dispose();
            }
            if (_secondSoundOut != null)
            {
                _secondSoundOut.Dispose();
            }
        }

    }
}
