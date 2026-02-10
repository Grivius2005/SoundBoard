namespace SBClassLib
{
    public class SoundManager
    {
        private static string[] _soundExt = { "mp3", "wav" };
        private static string[] _imgExt = { "jpg", "png" };
        public string? DirectoryPath { get; set; }
        public List<string?[]> Sounds { get; private set; }

        public SoundManager()
        {
            DirectoryPath = null;
            Sounds = new();
        }
        public SoundManager(string directoryPath)
        {
            DirectoryPath = directoryPath;
            Sounds = new();
            LoadSounds();
        }

        public void ChangeDirectory(string directoryPath)
        {
            DirectoryPath = directoryPath;
            Sounds = new();
            LoadSounds();
        }

        public void PlaySound(int index)
        {

        }

        private void LoadSounds()
        {
            if(DirectoryPath != null)
            {
                List<string> allFiles = Directory.EnumerateFiles(DirectoryPath, "*.*").ToList();
                List<string> sFiles = allFiles.Where(file => SoundManager._soundExt.Contains(Path.GetExtension(file).TrimStart('.').ToLowerInvariant())).ToList();
                List<string> imgFiles = allFiles.Where(file => SoundManager._imgExt.Contains(Path.GetExtension(file).TrimStart('.').ToLowerInvariant())).ToList();
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

    }
}
