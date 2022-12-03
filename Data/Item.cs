using System;
using System.Collections.Generic;
using System.IO;

namespace ChusanExplorer
{
    public class BaseItem : IDObject
    {
        public string name, descrip;
        public DDSImage image;
        // 我弃疗，所有属性都堆基类里得了
        public int rarity;

        public BaseItem(DirectoryInfo rootDir)
        {
            var root = Helpers.GetDataFromFolder(rootDir);
            id = Convert.ToInt32(root.Get("name/id"));
            name = root.Get("name/str");
            rarity = Convert.ToInt32(root.Get("rareType") ?? "-1");
            descrip = root.Get("explainText") ?? Config.NO_DESCRIP;
            var imageNode = root.SelectSingleNode("image/path");
            if (imageNode != null) image = new DDSImage(Path.Combine(rootDir.FullName, imageNode.InnerText));
        }

        public dynamic SortKeyInner
        {
            get
            {
                switch (Main.instance.ItemSortType)
                {
                    default:
                        return id;
                    case ItemSortTypes.Name:
                        return name;
                }
            }
        }
    }

    public partial class ItemDescriptor : IDObject
    {
        public Func<Pack, IEnumerable<BaseItem>> getterPack;
        public Func<ItemGroup> getterPlayerSet;
        public Func<IDStorage<BaseItem>> getterStorage;
        public string nameVerbose, nameField, nameSQL;

        public IEnumerable<BaseItem> GetItems(Pack pack)
        {
            if (pack == PackLoader.packAll) return getterStorage().Values;
            return getterPack(pack);
        }

        public override string ToString() => nameVerbose;
    }

    public partial class PlayerItemProfile
    {
        public Dictionary<string, int> fields;
        public PlayerItemProfile()
        {
            fields = new Dictionary<string, int>();
        }

        public PlayerItemProfile Clone() => new PlayerItemProfile { fields = new Dictionary<string, int>(fields) };

        public static bool operator ==(PlayerItemProfile a, PlayerItemProfile b)
        {
            bool anull = (object)a == null,
                bnull = (object)b == null;
            if (anull) return bnull;
            if (bnull) return false;
            foreach (var pair in a.fields)
            {
                b.fields.TryGetValue(pair.Key, out var v);
                if (v != pair.Value) return false;
            }
            return true;
        }
        public static bool operator !=(PlayerItemProfile a, PlayerItemProfile b) => !(a == b);
    }

    public static class ItemSortTypes
    {
        public const string ID = "ID";
        public const string Name = "名称";
        public static string[] AllMethods = new string[] { ID, Name };
    }
}
