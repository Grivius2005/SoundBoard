using NAudio.Wave;

namespace SBClassLib
{
    public class SoundManager
    {
        private static string[] s_soundExt = { "mp3", "wav" };
        private static string[] s_imgExt = { "jpg", "png" };
        private DirectSoundDeviceInfo _firstOutDevice;
        private DirectSoundDeviceInfo? _secondOutDevice;
        public string? DirectoryPath { get; set; }

        public List<string?[]> Sounds { get; private set; }

        public SoundManager()
        {
            DirectoryPath = null;
            Sounds = new();
            var outDevices = SoundManager.GetOutDevices();
            _firstOutDevice = outDevices[0];
            _secondOutDevice = null;
        }
        public SoundManager(string directoryPath)
        {
            DirectoryPath = directoryPath;
            Sounds = new();
            var outDevices = SoundManager.GetOutDevices();
            _firstOutDevice = outDevices[0];
            LoadSounds();
        }

        public void ChangeDirectory(string directoryPath)
        {
            DirectoryPath = directoryPath;
            Sounds = new();
            LoadSounds();
        }

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

        public string?[] ShowCurrentDevicesDescriptions()
        {
            return [_firstOutDevice.Description, _secondOutDevice.Description];
        }


        public void PlaySound(int index)
        {
            string sfilePath = Sounds[index][0];
            using (var audioFile = new AudioFileReader(sfilePath))
            {
                using (var outDeviceFirst = new DirectSoundOut(_firstOutDevice.Guid))
                {
                    outDeviceFirst.Init(audioFile);
                    outDeviceFirst.Play();
                }
;
            }
            if(_secondOutDevice != null)
            {
                using (var audioFile = new AudioFileReader(sfilePath))
                {
                    using (var outDeviceSecond = new DirectSoundOut(_secondOutDevice.Guid))
                    {
                        outDeviceSecond.Init(audioFile);
                        outDeviceSecond.Play();
                    }
                }
            }
            
        }


        private void LoadSounds()
        {
            if(DirectoryPath != null)
            {
                List<string> allFiles = Directory.EnumerateFiles(DirectoryPath, "*.*").ToList();
                List<string> sFiles = allFiles.Where(file => SoundManager.s_soundExt.Contains(Path.GetExtension(file).TrimStart('.').ToLowerInvariant())).ToList();
                List<string> imgFiles = allFiles.Where(file => SoundManager.s_imgExt.Contains(Path.GetExtension(file).TrimStart('.').ToLowerInvariant())).ToList();
                foreach (string sfile in sFiles)
                {
                    string? imgfile = imgFiles.Where(file => Path.GetFileNameWithoutExtension(file) == Path.GetFileNameWithoutExtension(sfile)).FirstOrDefault();
                    Sounds.Add([sfile, imgfile]);
                }
            }
            else
            {
                throw new Exception("No directory selected");
            }
        }


        public static List<DirectSoundDeviceInfo> GetOutDevices()
        {
            return DirectSoundOut.Devices.ToList();
        }

    }
}
