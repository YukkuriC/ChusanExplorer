using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace ChusanExplorer
{
    public class Pack
    {
        public DirectoryInfo root, dirChara, dirCharaImage, dirMusic;
        public string name;

        public List<Character> characters;
        public List<MusicLevel> levels;

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
                }
            }

            // TODO maps, songs
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
                if (!sub.Name.StartsWith("chara")) continue;
                try
                {
                    int index = Convert.ToInt32(sub.Name.Substring(5));
                    var chara = new Character(dirChara.FullName, index, name);
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
                if (!sub.Name.StartsWith("music")) continue;
                try
                {
                    int index = Convert.ToInt32(sub.Name.Substring(5));
                    var music = new Music(sub.FullName, index, name);
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
            // TODO maps, songs, nameplates, etc.

            Loaded.Invoke();
        }
    }
}
