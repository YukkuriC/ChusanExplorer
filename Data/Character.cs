using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ChusanExplorer
{
    public class Character : IDObject, ISortableInList
    {
        public string name, type, version, pack, illustrator;

        public List<CharaImageGroup> images;

        public string SortKey
        {
            get
            {
                if (Selected.player != null)
                {
                    var p = GetProfile();
                    if (p == null)
                        switch (CharacterListLoader.sort)
                        {
                            case CharaSortType.Level:
                            case CharaSortType.PlayCount:
                                return "未持有";
                        }
                    switch (CharacterListLoader.sort)
                    {
                        case CharaSortType.Level:
                            return $"{p.level}/{p.maxLevel}";
                        case CharaSortType.PlayCount:
                            return $"{p.playCount}PC";
                    }
                }
                switch (CharacterListLoader.sort)
                {
                    default:
                        return id.PadID();
                    case CharaSortType.Type:
                        return type;
                    case CharaSortType.Name:
                        return name;
                    case CharaSortType.Version:
                        return version;
                }

            }
        }
        public dynamic SortKeyInner
        {
            get
            {
                if (Selected.player != null)
                {
                    var p = GetProfile();
                    if (p == null)
                        switch (CharacterListLoader.sort)
                        {
                            case CharaSortType.Level:
                            case CharaSortType.PlayCount:
                                return float.MaxValue;
                        }
                    switch (CharacterListLoader.sort)
                    {
                        case CharaSortType.Level:
                            return -p.level - p.maxLevel * 0.001f;
                        case CharaSortType.PlayCount:
                            return -p.playCount;
                    }
                }
                switch (CharacterListLoader.sort)
                {
                    default:
                        return id;
                    case CharaSortType.Type:
                        return type;
                    case CharaSortType.Name:
                        return name;
                    case CharaSortType.Version:
                        return version;
                }

            }
        }

        public PlayerCharaProfile GetProfile()
        {
            if (Selected.player == null) return null;
            Selected.player.charaProfiles.TryGetValue(id, out var res);
            return res;
        }

        public string DisplayText
        {
            get
            {
                if (SortKey == name) return name;
                return $"[{SortKey}] {name}";
            }
        }

        public Character(DirectoryInfo rootDir, string packName)
        {
            pack = packName;
            var root = Helpers.GetDataFromFolder(rootDir);
            id = root.GetId();
            name = root.Get("name/str");
            type = root.Get("works/str");
            version = root.Get("releaseTagName/str");
            illustrator = root.Get("illustratorName/str");

            #region images
            images = new List<CharaImageGroup>();
            // defaultImages
            var idDef = Convert.ToInt32(root.Get("defaultImages/id"));
            Storage.DDSChara.TryGetValue(idDef, out var imgDef);
            if (imgDef?.Valid ?? false)
            {
                images.Add(imgDef);
                imgDef.name = name;
            }
            // addImages1 - addImages9
            for (int i = 1; i <= 9; i++)
            {
                var imgNode = root.SelectSingleNode($"addImages{i}");
                if (imgNode?.SelectSingleNode("changeImg")?.InnerText != "true") break;
                var altName = imgNode.Get("charaName/str") ?? $"立绘{i + 1}";
                var imgId = Convert.ToInt32(imgNode.Get("image/id"));
                Storage.DDSChara.TryGetValue(imgId, out var img);
                if (imgDef?.Valid ?? false)
                {
                    images.Add(img);
                    img.name = altName;
                }
            }
            // validation
            if (images.Count == 0) throw new Exception($"角色无可用立绘 ({id})");
            #endregion
        }
    }

    public static class CharacterListLoader
    {
        public static Action Loaded;
        public static List<Character> charaAll, charaShow;
        public static Dictionary<string, List<Character>> charaByType;
        public static Pack lastLoadedPack;
        public static string type, sort, search = "";
        public static bool onlyShowOwned;

        public static void Init()
        {
            UIEvents.PackChoiceChanged += Load;
        }

        static void Load(Pack pack)
        {
            if (pack == lastLoadedPack) return;
            lastLoadedPack = pack;
            if (pack == PackLoader.packAll) charaAll = new List<Character>(Storage.Characters.Values);
            else charaAll = pack.characters;
            charaByType = new Dictionary<string, List<Character>>();
            foreach (var c in charaAll)
            {
                if (!charaByType.ContainsKey(c.type)) charaByType[c.type] = new List<Character>();
                charaByType[c.type].Add(c);
            }

            Loaded.Invoke();
        }

        public static List<Character> GetDisplayList()
        {
            List<Character> lst;
            charaByType.TryGetValue(type, out lst);
            charaShow = new List<Character>(lst ?? charaAll);
            if (Selected.player != null && onlyShowOwned)
                charaShow = charaShow.FindAll(c => Selected.player.charaProfiles.ContainsKey(c.id));
            //sort
            charaShow.Sort(Helpers.CompareSortable);
            //search
            var listToShow = charaShow.FindAll(
                c => c.name.Contains(search) || c.id.ToString().Contains(search)
            );
            return listToShow;
        }
    }

    public static class CharaSortType
    {
        public const string ID = "ID";
        public const string Name = "角色名称";
        public const string Type = "分类名称";
        public const string Version = "加入版本";
        public const string Level = "角色等级";
        public const string PlayCount = "游玩次数";

        public static string[] AllMethods = new string[] { ID, Name, Type, Version };
        public static string[] AllMethodsPlayer = new string[] { ID, Name, Type, Version, Level, PlayCount };
    }
}
