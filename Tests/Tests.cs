
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
            foreach(var device in SoundManager.GetOutDevices())
            {
                Console.WriteLine(device.Description);
            }
            manager.ChangeDevice("CABLE In 16ch (VB-Audio Virtual Cable)", true);
            while (true)
            {
                Console.Write("Wybierz indeks dźwięku: ");
                string? inp = Console.ReadLine();
                if(int.TryParse(inp, out int id))
                {
                    manager.PlaySound(id);
                }
                Console.Clear();
            }


        }
    }
}
