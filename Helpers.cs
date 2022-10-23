using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChusanExplorer
{
    public static class Helpers
    {
        public static string PadID(this int id, int digit = 6) => id.ToString().PadLeft(digit, '0');

        public static void ShowError(this Exception e, string title = null)
        {
            if (title == null) title = "出错辣!";
            if (!Config.MuteErrors)
                MessageBox.Show($"{e.Message}:\n{e.StackTrace}", title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static int CompareSortable(ISortableInList a, ISortableInList b)
        {
            var ka = a.SortKeyInner;
            var kb = b.SortKeyInner;
            if (ka.GetType() == typeof(string)) return string.Compare(ka, kb);
            return ka > kb ? 1 : ka == kb ? 0 : -1;
        }
    }
}
