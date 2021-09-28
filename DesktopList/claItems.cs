using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopList
{
    public class claItem
    {
        public Guid MyID { get; set; }
        public string TaskText { get; set; }
        public int TaskPos { get; set; }
    }

    public static class claItems
    {
        public static List<claItem> lstItems;

        public static void LoadItems()
        {
            lstItems = new List<claItem>();
            if (System.IO.File.Exists(claSettings.strSettingsPath + "\\Items.json") == false)
            {
                // never saved yet.
                return; // >>
            }
            using (System.IO.StreamReader sr = new System.IO.StreamReader(claSettings.strSettingsPath + "\\Items.json"))
            {
                lstItems = System.Text.Json.JsonSerializer.Deserialize<List<claItem>>(sr.ReadLine());
            }
        }

        public static void SaveItems()
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(claSettings.strSettingsPath + "\\Items.json"))
            {
                sw.WriteLine(System.Text.Json.JsonSerializer.Serialize(lstItems));
            }
        }

    }
}
