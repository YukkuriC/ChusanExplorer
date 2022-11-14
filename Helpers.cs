using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;

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

        public static Comparison<PlayerLevelResult> levelRatingDec = (PlayerLevelResult a, PlayerLevelResult b) => b.Rating > a.Rating ? 1 : b.Rating == a.Rating ? 0 : -1;

        public static IDStorage<T> IDListToDict<T>(IEnumerable<T> raw) where T : IDObject
        {
            var res = new IDStorage<T>();
            foreach (var i in raw) res.Push(i);
            return res;
        }

        #region xml
        public static XmlNode GetDataFromFolder(DirectoryInfo dir)
        {
            var xmlFiles = dir.GetFiles("*.xml");
            if (xmlFiles.Length != 1)
                throw new Exception("目录下xml数量异常");
            var doc = new XmlDocument();
            doc.Load(xmlFiles[0].FullName);
            return doc.DocumentElement;
        }

        public static string Get(this XmlNode rootNode, string path) => rootNode.SelectSingleNode(path)?.InnerText;
        public static int GetId(this XmlNode rootNode) => Convert.ToInt32(rootNode.Get("name/id"));
        public static string GetName(this XmlNode rootNode) => rootNode.Get("name/str");
        #endregion
    }
}
