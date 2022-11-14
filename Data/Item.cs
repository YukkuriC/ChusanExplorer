using System;
using System.Collections.Generic;
using System.IO;

namespace ChusanExplorer
{
    public class BaseItem : IDObject
    {
        public string name;
        public DDSImage image;

        public BaseItem(DirectoryInfo rootDir)
        {
            var root = Helpers.GetDataFromFolder(rootDir);
            id = Convert.ToInt32(root.Get("name/id"));
            name = root.Get("name/str");
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
        public Func<int> getterPlayerChoice;
        public Func<IDStorage<BaseItem>> getterStorage;
        public string name;

        public IEnumerable<BaseItem> GetItems(Pack pack)
        {
            if (pack == PackLoader.packAll) return getterStorage().Values;
            return getterPack(pack);
        }

        public override string ToString() => name;
    }

    public partial class PlayerItemProfile
    {
    }

    public static class ItemSortTypes
    {
        public const string ID = "ID";
        public const string Name = "名称";
        public static string[] AllMethods = new string[] { ID, Name };
    }
}
