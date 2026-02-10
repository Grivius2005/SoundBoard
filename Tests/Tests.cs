
using SBClassLib;

namespace Tests
{
    internal class Tests
    {
        private static string CURRENTDIR = Directory.GetCurrentDirectory();
        private static string FILEDIR = $@"{CURRENTDIR}\TestDir";
        static void Main(string[] args)
        {
            SoundManager manager = new(FILEDIR);


            foreach(string[] files in manager.Sounds)
            {
                Console.WriteLine($"{files[0]}\n{files[1]}");
                Console.WriteLine();
            }



        }
    }
}
