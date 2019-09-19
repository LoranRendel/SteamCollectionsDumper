using LevelDB;
using System;
using System.IO;
using System.Text;

namespace SteamCollectionPOC
{
    class Program
    {
        static void Main(string[] args)
        {
            var root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Steam", "htmlcache", "Local Storage", "leveldb");
            using (var db = new DB(new Options { CreateIfMissing = false }, root))
            {
                foreach (var (key, value) in db)
                {
                    var keyText = Encoding.UTF8.GetString(key).Split("\0\u0001");
                    if (!keyText[0].Equals("_https://steamloopback.host")) continue;
                    if (!keyText[1].Contains("cloud-storage-namespace")) continue;
                    var utf = Encoding.Convert(Encoding.BigEndianUnicode, Encoding.UTF8, value);
                    File.WriteAllBytes(Path.Combine(root, keyText[1]+".json"), utf);
                    Console.WriteLine(Encoding.UTF8.GetString(utf));
                }
            }
        }
    }
}
