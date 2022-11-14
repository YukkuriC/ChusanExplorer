using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace ChusanExplorer
{
    public partial class Pack
    {
        public DirectoryInfo root, dirChara, dirCharaImage, dirMusic;
        public DirectoryInfo dirNamePlate, dirSystemVoice, dirTrophy, dirMapIcon;
        public string name;

        public List<Character> characters;
        public List<MusicLevel> levels;
        public List<BaseItem> namePlates, trophies, mapIcons, systemVoices;

        public bool hasContent
        {
            get => characters.Count > 0;
        }

        public Pack()
        {
            name = Config.ALL;
        }

        public Pack(DirectoryInfo folder)
        {
            root = folder;
            name = folder.Name;

            foreach (var sub in folder.GetDirectories())
            {
                switch (sub.Name)
                {
                    case "ddsImage":
                        dirCharaImage = sub;
                        break;
                    case "chara":
                        dirChara = sub;
                        break;
                    case "music":
                        dirMusic = sub;
                        break;
                    case "namePlate":
                        dirNamePlate = sub;
                        break;
                    case "systemVoice":
                        dirSystemVoice = sub;
                        break;
                    case "trophy":
                        dirTrophy = sub;
                        break;
                    case "mapIcon":
                        dirMapIcon = sub;
                        break;
                }
            }
        }
        public void LoadCharaImages()
        {
            if (dirCharaImage == null) return;
            foreach (var sub in dirCharaImage.GetDirectories())
            {
                if (!sub.Name.StartsWith("ddsImage")) continue;
                try
                {
                    int index = Convert.ToInt32(sub.Name.Substring(8));
                    var imgGrp = new CharaImageGroup(dirCharaImage.FullName, index);
                    Storage.DDSChara.Push(imgGrp);
                }
                catch (Exception e)
                {
                    e.ShowError($"加载角色图像{sub.Name}出错 ({name})");
                }
            }
        }
        public void LoadCharacters()
        {
            characters = new List<Character>();
            if (dirChara == null) return;
            foreach (var sub in dirChara.GetDirectories())
            {
                try
                {
                    var chara = new Character(sub, name);
                    characters.Add(chara);
                    Storage.Characters.Push(chara);
                }
                catch (Exception e)
                {
                    e.ShowError($"加载角色{sub.Name}出错 ({name})");
                }
            }
        }
        public void LoadMusic()
        {
            levels = new List<MusicLevel>();
            if (dirMusic == null) return;
            foreach (var sub in dirMusic.GetDirectories())
            {
                try
                {
                    var music = new Music(sub, name);
                    Storage.Music.Push(music);
                    foreach (var level in music.levels)
                    {
                        if (level == null) continue;
                        levels.Add(level);
                    }
                }
                catch (Exception e)
                {
                    e.ShowError($"加载乐曲{sub.Name}出错 ({name})");
                }
            }
        }
        void loadItemInFolder(DirectoryInfo dir, out List<BaseItem> res, ref IDStorage<BaseItem> storage)
        {
            res = new List<BaseItem>();
            if (dir == null) return;
            foreach (var sub in dir.GetDirectories())
            {
                try
                {
                    var item = new BaseItem(sub);
                    res.Add(item);
                    storage.Push(item);
                }
                catch (Exception e)
                {
                    e.ShowError($"加载道具{sub.Name}出错 ({name})");
                }
            }

            return;
        }

        public override string ToString() => name;
    }
    public static class PackLoader
    {
        public static Action Loaded;

        public static List<Pack> packs;
        public static Dictionary<string, Pack> packMap;
        public static Pack packAll = new Pack();

        static void loadPack(string path)
        {
            var info = new DirectoryInfo(path);
            if (packMap.ContainsKey(info.Name))
            {
                MessageBox.Show($"重复包名: ${info.Name}");
                return;
            }
            var pack = new Pack(info);
            //if (pack.hasContent) linkPack(pack);
            linkPack(pack);
        }

        static void linkPack(Pack pack)
        {
            packs.Add(pack);
            packMap.Add(pack.name, pack);
        }

        public static void LoadPacks()
        {
            packs = new List<Pack>();
            packMap = new Dictionary<string, Pack>();
            linkPack(packAll);
            if (Directory.Exists(Config.DirA000)) loadPack(Config.DirA000);
            if (Directory.Exists(Config.DirOptions))
            {
                foreach (var sub in Directory.GetDirectories(Config.DirOptions))
                    loadPack(sub);
            }
            foreach (var p in packs) p.LoadCharaImages();
            foreach (var p in packs) p.LoadCharacters();
            foreach (var p in packs) p.LoadMusic();
            foreach (var p in packs) p.LoadItems();
            // TODO maps, etc.

            Loaded.Invoke();
        }
    }
}
