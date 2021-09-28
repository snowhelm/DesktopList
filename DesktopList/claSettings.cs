using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopList
{
    public class claSettingsStore
    {
        public int ListTop { get; set; }
        public int ListLeft { get; set; }
        public int ListWidth { get; set; }
    }

    public static class claSettings
    {
        public static claSettingsStore iSS;
        public static MainWindow iMW; // reference to the main window.
        public static string strSettingsPath;
        public static bool booLoading = false;
        public static string strError = "Sorry, there has been a problem. Sorry for the inconvenience."
                                        + Environment.NewLine + Environment.NewLine;

        public static void LoadSettings()
        {
            strSettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            strSettingsPath += "\\DesktopList";
            if (System.IO.Directory.Exists(strSettingsPath) == false)
            {
                System.IO.Directory.CreateDirectory(strSettingsPath);
            }

            if (System.IO.File.Exists(strSettingsPath + "\\Settings.json"))
            {
                string strTemp;
                using (System.IO.StreamReader sr = new System.IO.StreamReader(strSettingsPath + "\\Settings.json"))
                {
                    strTemp = sr.ReadLine();
                }
                iSS = System.Text.Json.JsonSerializer.Deserialize<claSettingsStore>(strTemp);
            } else
            {
                iSS = new claSettingsStore();
                iSS.ListLeft = 400;
                iSS.ListTop = 400;
                iSS.ListWidth = 150;
                string strTemp = System.Text.Json.JsonSerializer.Serialize(iSS);
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(strSettingsPath + "\\Settings.json", false))
                {
                    sw.WriteLine(strTemp);
                }
            }
        }

        public static void SaveSettings()
        {
            iSS.ListTop = (int)iMW.Top;
            iSS.ListLeft = (int)iMW.Left;
            iSS.ListWidth = (int)iMW.Width;

            string strTemp = System.Text.Json.JsonSerializer.Serialize(iSS);
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(strSettingsPath + "\\Settings.json", false))
            {
                sw.WriteLine(strTemp);
            }
        }
    }
}
