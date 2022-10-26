using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ChusanExplorer
{
    public class Music : IDObject
    {
        public const int LEVEL_COUNT = 6; // B A E M U / WE
        public const string WENull = "Invalid";
        public int musicId;
        public string name, pack, version, author, genre, WEType;
        public MusicLevel[] levels = new MusicLevel[LEVEL_COUNT];
        public DDSImage cover;

        public Music(string rootDir, int index, string packName)
        {
            id = index;
            pack = packName;

            var doc = new XmlDocument();
            doc.Load(Path.Combine(rootDir, "Music.xml"));
            var root = doc.DocumentElement;
            name = root.SelectSingleNode("name/str").InnerText;
            musicId = Convert.ToInt32(root.SelectSingleNode("name/id").InnerText);
            version = root.SelectSingleNode("releaseTagName/str").InnerText;
            author = root.SelectSingleNode("artistName/str").InnerText;
            genre = root.SelectSingleNode("genreNames/list/StringID/str").InnerText;
            WEType = root.SelectSingleNode("worldsEndTagName/str")?.InnerText ?? WENull;
            cover = new DDSImage(Path.Combine(rootDir, root.SelectSingleNode("jaketFile/path").InnerText));

            var levelRoot = root.SelectSingleNode("fumens");
            foreach (XmlNode node in levelRoot.ChildNodes)
            {
                if (node.SelectSingleNode("enable")?.InnerText != "true") continue;
                var level = new MusicLevel(this, node);
                levels[level.index] = level;
            }
        }
    }

    public class MusicLevel : ISortableInList
    {
        public static string[] IndexNames = new string[] { "BASIC", "ADVANCED", "EXPERT", "MASTER", "UNTIMA", "WORLD'S END" };

        public Music music;
        public int index, level, levelSub;

        public int RatingInner { get => level * 100 + levelSub; }
        public string RankDisplay
        {
            get
            {
                if (index == 5) return music.WEType;
                var res = level.ToString();
                if (level >= 7 && levelSub > 50) res += "+";
                return res;
            }
        }
        public float Rating { get => RatingInner * 0.01f; }

        public MusicLevel(Music root, XmlNode data)
        {
            music = root;
            index = Convert.ToInt32(data.SelectSingleNode("type/id").InnerText);
            level = Convert.ToInt32(data.SelectSingleNode("level").InnerText);
            levelSub = Convert.ToInt32(data.SelectSingleNode("levelDecimal").InnerText);
        }

        public string GetDisplayTag(string type)
        {
            if (Selected.player != null)
            {
                var p = GetProfile();
                if (p == null)
                    switch (type)
                    {
                        case MusicSortType.Result:
                        case MusicSortType.Rating:
                        case MusicSortType.PlayCount:
                            return "未游玩";
                    }
                switch (type)
                {
                    case MusicSortType.Result:
                        return p.ToString();
                    case MusicSortType.Rating:
                        return Math.Round(p.Rating, 3).ToString();
                    //return p.Rating.ToString("0.000");
                    case MusicSortType.PlayCount:
                        return $"{p.playCount}PC";
                }
            }

            switch (type)
            {
                default:
                    return music.id.PadID(4);
                case MusicSortType.Level:
                    if (index == 5) return music.WEType;
                    return Rating.ToString("0.0");
                case MusicSortType.Name:
                    return music.name;
                case MusicSortType.Author:
                    return music.author;
                case MusicSortType.Version:
                    return music.version;
            }
        }

        public string GetGeneralDisplay() => $"{music.name} ({IndexNames[index]} {RankDisplay})";

        public override string ToString()
        {
            var res = GetGeneralDisplay();
            var tag = GetDisplayTag(MusicLevelLoader.sort);
            if (tag != music.name) res = $"[{tag}] {res}";
            return res;
        }

        public PlayerLevelResult GetProfile()
        {
            if (Selected.player == null) return null;
            PlayerLevelResult res;
            Selected.player.levelProfiles.TryGetValue(Tuple.Create(music.id, index), out res);
            return res;
        }

        public string SortKey { get => GetDisplayTag(MusicLevelLoader.sort); }

        public dynamic SortKeyInner
        {
            get
            {
                if (Selected.player != null)
                {
                    var p = GetProfile();
                    if (p == null)
                        switch (MusicLevelLoader.sort)
                        {
                            case MusicSortType.Result:
                            case MusicSortType.Rating:
                            case MusicSortType.PlayCount:
                                return float.MaxValue;
                        }
                    switch (MusicLevelLoader.sort)
                    {
                        case MusicSortType.Result:
                            return -p.score;
                        case MusicSortType.Rating:
                            return -p.Rating;
                        case MusicSortType.PlayCount:
                            return -p.playCount;
                    }
                }

                switch (MusicLevelLoader.sort)
                {
                    default:
                        return music.id;
                    case MusicSortType.Level:
                        if (index == 5) return 9999;
                        return RatingInner;
                    case MusicSortType.Name:
                        return music.name;
                    case MusicSortType.Author:
                        return music.author;
                    case MusicSortType.Version:
                        return music.version;
                }
            }
        }
    }

    public static class MusicLevelLoader
    {
        public static Action Loaded;
        public static List<MusicLevel> levelAll, levelShow;
        public static Dictionary<string, List<MusicLevel>> levelByGenre, levelByMusic;
        public static Pack lastLoadedPack;
        public static string genre, sort, rank, search = "";
        public static int typeIndex;
        public static bool onlyShowPlayed;

        public static void Init()
        {
            UIEvents.PackChoiceChanged += Load;
        }

        static void Load(Pack pack)
        {
            if (pack == lastLoadedPack) return;
            lastLoadedPack = pack;
            levelByGenre = new Dictionary<string, List<MusicLevel>>();
            levelByMusic = new Dictionary<string, List<MusicLevel>>();
            if (pack == PackLoader.packAll)
            {
                levelAll = new List<MusicLevel>();
                foreach (var m in Storage.Music.Values)
                    foreach (var l in m.levels)
                        if (l != null)
                            levelAll.Add(l);
            }
            else levelAll = pack.levels;
            foreach (var l in levelAll)
            {
                var g = l.music.genre;
                if (l.index == 5) g = MusicLevel.IndexNames[5];
                if (!levelByGenre.ContainsKey(g)) levelByGenre[g] = new List<MusicLevel>();
                levelByGenre[g].Add(l);

                var mn = l.music.name;
                if (!levelByMusic.ContainsKey(mn)) levelByMusic[mn] = new List<MusicLevel>();
                levelByMusic[mn].Add(l);
            }
            foreach (var lst in levelByMusic.Values)
                lst.Sort((MusicLevel a, MusicLevel b) => a.index - b.index);

            Loaded.Invoke();
        }

        public static List<MusicLevel> GetDisplayList()
        {
            List<MusicLevel> levelShow;
            levelByGenre.TryGetValue(genre, out levelShow);
            if (levelShow == null) levelShow = levelAll;

            // filters
            if (typeIndex != -1) levelShow = levelShow.FindAll((MusicLevel l) => l.index == typeIndex);
            if (rank != Config.ALL) levelShow = levelShow.FindAll((MusicLevel l) => l.RankDisplay == rank);
            if (search != "") levelShow = levelShow.FindAll((MusicLevel l) => l.music.name.Contains(search) || l.music.author.Contains(search));
            if (onlyShowPlayed) levelShow = levelShow.FindAll((MusicLevel l) => l.GetProfile() != null);

            // sort
            levelShow.Sort(Helpers.CompareSortable);

            return levelShow.ToList();
        }
    }

    public static class MusicSortType
    {
        public const string ID = "ID";
        public const string Name = "歌曲名称";
        public const string Author = "作者名称";
        public const string Version = "加入版本";
        public const string Level = "歌曲定数";
        public const string Result = "取得成绩";
        public const string Rating = "取得Rating";
        public const string PlayCount = "游玩次数";

        public static string[] AllMethods = new string[] { ID, Name, Author, Version, Level };
        public static string[] AllMethodsPlayer = new string[] { ID, Name, Author, Version, Level, Result, Rating, PlayCount };
    }
}
