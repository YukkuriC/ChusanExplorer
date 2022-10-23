using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChusanExplorer
{
    public partial class Main : Form
    {
        Random rand;
        bool isLoading;

        public Main()
        {
            isLoading = true;
            rand = new Random();
            InitializeComponent();
            InitLoaders();
            BindEvents();
            InitComponentDataStatic();
            InitComponentData();
            Config.Init();
            PackLoader.LoadPacks();
            onFirstLoad();
            isLoading = false;
        }

        #region loading flow
        void InitComponentDataStatic()
        {
        }

        void InitComponentData()
        {
            if (Selected.player != null)
            {
                chooseMusicSort.DataSource = MusicSortType.AllMethodsPlayer;
                chooseCharaSort.DataSource = CharaSortType.AllMethodsPlayer;
            }
            else
            {
                chooseMusicSort.DataSource = MusicSortType.AllMethods;
                chooseCharaSort.DataSource = CharaSortType.AllMethods;
            }
        }

        void InitLoaders()
        {
            CharacterListLoader.Init();
            MusicLevelLoader.Init();
            DBLoader.Init();
        }

        public void BindEvents()
        {
            PackLoader.Loaded += () =>
            {
                choosePack.Items.Clear();
                choosePack.Items.AddRange(PackLoader.packs.ToArray());
                choosePack.SelectedIndex = 0;
                choosePack.Enabled = choosePack.Items.Count > 0;
            };
            DBLoader.Loaded += () =>
            {
                choosePlayer.Items.Clear();
                choosePlayer.Items.AddRange(DBLoader.users.ToArray());
                choosePlayer.SelectedIndex = 0;
                choosePlayer.Enabled = choosePlayer.Items.Count > 0;
            };

            #region flushers
            UIEvents.PlayerCharaProfileChanged += () =>
            {
                flusherCharaProfile.Enabled = true;
                if (Selected.chara != null && Selected.chara.GetProfile() == null && Selected.chara.id == Selected.player?.chara) // 玩家选定角色被撅了
                {
                    PlayerCharaProfileLoader.SetPlayerChoice(Storage.Characters.Values.First((Character c) => c.GetProfile() != null));
                    refreshResultPageImages();
                }
            };
            UIEvents.CharaListApply += () => flusherCharaList.Enabled = true;
            UIEvents.MusicListApply += () => flusherMusicList.Enabled = true;
            UIEvents.RefreshResultPage += () => flusherResultPage.Enabled = true;
            #endregion

            #region tab chara
            CharacterListLoader.Loaded += () =>
            {
                chooseCharaType.Items.Clear();
                chooseCharaType.Items.Add(Config.ALL);
                var types = CharacterListLoader.charaByType.Keys.ToList();
                types.Sort();
                chooseCharaType.Items.AddRange(types.ToArray());
                chooseCharaType.SelectedIndex = 0;
                UIEvents.CharaListApply.Invoke();
            };
            #endregion

            #region tab music
            MusicLevelLoader.Loaded += () =>
            {
                chooseMusicGenre.Items.Clear();
                chooseMusicGenre.Items.Add(Config.ALL);
                var types = MusicLevelLoader.levelByGenre.Keys.ToArray();
                chooseMusicGenre.Items.AddRange(types);

                HashSet<int> allTypes = new HashSet<int>();
                HashSet<string> allRanks = new HashSet<string>();
                foreach (var l in MusicLevelLoader.levelAll)
                {
                    allTypes.Add(l.index);
                    allRanks.Add(l.RankDisplay);
                }
                chooseLevelType.Items.Clear();
                chooseLevelType.Items.Add(Config.ALL);
                for (int i = 0; i < 6; i++)
                    if (allTypes.Contains(i))
                        chooseLevelType.Items.Add(MusicLevel.IndexNames[i]);
                chooseLevelRank.Items.Clear();
                chooseLevelRank.Items.Add(Config.ALL);
                for (int i = 1; i <= 15; i++)
                {
                    var s = i.ToString();
                    if (allRanks.Contains(s)) chooseLevelRank.Items.Add(s);
                    s += "+";
                    if (allRanks.Contains(s)) chooseLevelRank.Items.Add(s);
                }

                chooseMusicGenre.SelectedIndex = 0;
                chooseLevelType.SelectedIndex = 0;
                chooseLevelRank.SelectedIndex = 0;
                UIEvents.MusicListApply.Invoke();
            };
            #endregion
        }

        void onFirstLoad()
        {
            if (Selected.player != null)
            {
                Storage.Characters.TryGetValue(Selected.player.chara, out Selected.chara);
            }
        }
        #endregion

        private void choosePlayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            var plr = (Player)choosePlayer.SelectedItem;
            Selected.player = plr;
            plr.InitAdvance();
            InitComponentData();
            UIEvents.PlayerCharaProfileChanged.Invoke();
            UIEvents.RefreshResultPage.Invoke();
        }

        private void choosePack_SelectedIndexChanged(object sender, EventArgs e)
        {
            PackLoader.packMap.TryGetValue(choosePack.Text, out Pack pack);
            UIEvents.PackChoiceChanged.Invoke(pack);
            UIEvents.RefreshResultPage.Invoke();
        }

        #region tab chara
        private void charaTypeChanged(object sender, EventArgs e)
        {
            CharacterListLoader.type = chooseCharaType.Text;
            UIEvents.CharaListApply.Invoke();
        }
        private void charaFilterTextChanged(object sender, EventArgs e)
        {
            CharacterListLoader.search = textCharaSearch.Text;
            UIEvents.CharaListApply.Invoke();
        }
        private void charaSortChanged(object sender, EventArgs e)
        {
            CharacterListLoader.sort = chooseCharaSort.Text;
            UIEvents.CharaListApply.Invoke();
        }

        void clearCharacterDisplay()
        {
            labelCharaInfoHeader.Text = "[114514] 田所 浩二";
            labelCharaInfoType.Text = "野獸";
            labelCharaInfoVersion.Text = "v1 91.98.10";
            labelCharaInfoImgCount.Text = "114514";
            showCharacterImage(null);
            charaAltImageContainer.Controls.Clear();
        }

        void showCharacterImage(CharaImageGroup grp)
        {
            if (grp == null)
            {
                imgCharaIcon.Image = null;
                imgCharaBig.Image = null;
                grpCharaImgDescrip.Text = "角色立绘: 寄";
                return;
            }
            imgCharaIcon.Image = grp.dds[2].Image;
            imgCharaBig.Image = grp.dds[0].Image;
            grpCharaImgDescrip.Text = $"角色立绘: {grp.name}";
        }

        private void chooseCharacter(object sender, EventArgs e)
        {
            var chara = (Character)listCharacters.SelectedItem;
            if (chara == Selected.chara) return;
            Selected.chara = chara;
            UIEvents.PlayerCharaProfileChanged.Invoke();
            clearCharacterDisplay();
            if (chara == null) return;
            labelCharaInfoHeader.Text = $"[{chara.id.PadID()}]";
            labelCharaInfoName.Text = chara.name;
            labelCharaInfoType.Text = chara.type;
            labelCharaInfoVersion.Text = $"{chara.version} ({chara.pack})";
            labelCharaInfoImgCount.Text = chara.images.Count.ToString();
            labelCharaInfoIllustrator.Text = chara.illustrator;
            showCharacterImage(chara.images[0]);

            // 立绘切换链接
            for (var i = 0; i < chara.images.Count; i++)
            {
                var img = chara.images[i];
                var link = new LinkLabel();
                link.Text = $"[{i}] {img.name}";
                link.Margin = new Padding(3);
                link.AutoSize = true;
                link.LinkClicked += (object _, LinkLabelLinkClickedEventArgs a) =>
                {
                    showCharacterImage(img);
                };
                charaAltImageContainer.Controls.Add(link);

                var activeProfile = chara.GetProfile();
                if (activeProfile != null && img.id == activeProfile.illustId)
                    showCharacterImage(img);
            }
        }

        private void rollCharacter(object sender, EventArgs e)
        {
            listCharacters.SelectedIndex = rand.Next(listCharacters.Items.Count);
        }

        private void labelCharaInfoType_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            chooseCharaType.SelectedItem = labelCharaInfoType.Text;
        }

        private void labelCharaInfoName_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            textCharaSearch.Text = labelCharaInfoName.Text;
        }

        private void btnChooseChara_Click(object sender, EventArgs e)
        {
            PlayerCharaProfileLoader.SetPlayerChoice();
            refreshResultPageImages();
        }

        private void buttonEditChara_Click(object sender, EventArgs e)
        {
            new EditCharaForm(Selected.chara, Selected.chara.GetProfile()).ShowDialog();
        }
        private void checkCharaOwned_CheckedChanged(object sender, EventArgs e)
        {
            CharacterListLoader.onlyShowOwned = checkCharaOwned.Checked;
            UIEvents.CharaListApply.Invoke();
        }

        private void flushCharaList(object sender, EventArgs e)
        {
            if (isLoading) return;
            var oldChara = Selected.chara;
            listCharacters.DataSource = CharacterListLoader.GetDisplayList();
            if (oldChara != null) listCharacters.SelectedItem = oldChara;
            btnRollChara.Enabled = listCharacters.Enabled = listCharacters.Items.Count > 0;
            flusherCharaList.Enabled = false;
        }

        private void flushCharaProfile(object sender, EventArgs e)
        {
            if (isLoading) return;
            var activeProfile = Selected.chara.GetProfile();
            labelPlayerCharaInfo.Text = activeProfile?.Description ?? "当前用户未拥有该角色";
            btnChooseChara.Enabled = DBLoader.Active && activeProfile != null &&
                Selected.player.chara != Selected.chara?.id;
            flusherCharaProfile.Enabled = false;
        }

        #endregion

        #region tab music
        private void chooseMusicGenre_SelectedIndexChanged(object sender, EventArgs e)
        {
            MusicLevelLoader.genre = chooseMusicGenre.Text;
            UIEvents.MusicListApply.Invoke();
        }

        private void chooseMusicSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            MusicLevelLoader.sort = chooseMusicSort.Text;
            UIEvents.MusicListApply.Invoke();
        }

        private void chooseLevelType_SelectedIndexChanged(object sender, EventArgs e)
        {
            MusicLevelLoader.typeIndex = chooseLevelType.SelectedIndex - 1;// -1 for ALL
            UIEvents.MusicListApply.Invoke();
        }

        private void chooseLevelRank_SelectedIndexChanged(object sender, EventArgs e)
        {
            MusicLevelLoader.rank = chooseLevelRank.Text;
            UIEvents.MusicListApply.Invoke();
        }

        private void textMusicSearch_TextChanged(object sender, EventArgs e)
        {
            MusicLevelLoader.search = textMusicSearch.Text;
            UIEvents.MusicListApply.Invoke();
        }

        private void flushMusicList(object sender, EventArgs e)
        {
            if (isLoading) return;
            var oldlevel = Selected.level;
            listLevels.DataSource = MusicLevelLoader.GetDisplayList();
            if (oldlevel != null) listLevels.SelectedItem = oldlevel;
            btnRollLevel.Enabled = listLevels.Enabled = listLevels.Items.Count > 0;
            flusherMusicList.Enabled = false;
        }

        static Brush[] rainbowColors = new Brush[] {
            Brushes.Red,
            Brushes.Orange,
            Brushes.Yellow,
            Brushes.Green,
            Brushes.Blue,
            Brushes.Cyan,
            Brushes.Violet,
        };
        private void listLevels_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            Brush myBrush = Brushes.Black;
            var item = (MusicLevel)listLevels.Items[e.Index];
            switch (item.index)
            {
                case 0:
                    myBrush = Brushes.DarkGreen;
                    break;
                case 1:
                    myBrush = Brushes.DarkOrange;
                    break;
                case 2:
                    myBrush = Brushes.DarkRed;
                    break;
                case 3:
                    myBrush = Brushes.Purple;
                    break;
                case 5:
                    myBrush = rainbowColors[(e.Index) % 7];
                    break;
            }
            //e.DrawFocusRectangle();
            if (item.index >= 4)
            {
                var rectOrig = e.Bounds;
                e.Graphics.DrawString(item.ToString(), e.Font, Brushes.Gray, new Rectangle(rectOrig.Location + new Size(-1, 0), rectOrig.Size), StringFormat.GenericDefault);
                e.Graphics.DrawString(item.ToString(), e.Font, Brushes.Gray, new Rectangle(rectOrig.Location + new Size(0, -1), rectOrig.Size), StringFormat.GenericDefault);
            }
            e.Graphics.DrawString(item.ToString(), e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);
        }

        private void rollLevel(object sender, EventArgs e)
        {
            listLevels.SelectedIndex = rand.Next(listLevels.Items.Count);
        }

        private void checkLevelPlayed_CheckedChanged(object sender, EventArgs e)
        {
            MusicLevelLoader.onlyShowPlayed = checkLevelPlayed.Checked;
            UIEvents.MusicListApply.Invoke();
        }

        private void listLevels_SelectedIndexChanged(object sender, EventArgs e)
        {
            var level = (MusicLevel)listLevels.SelectedItem;
            if (level == Selected.level) return;
            Selected.level = (MusicLevel)listLevels.SelectedItem;
            clearLevelDisplay();
            if (level == null) return;
            imgMusicCover.Image = level.music.cover.Image;
            labelLevelInfoName.Text = level.music.name;
            labelLevelInfoAuthor.Text = level.music.author;
            labelLevelInfoGenre.Text = level.music.genre;
            labelLevelInfoVersion.Text = $"{level.music.version} ({level.music.pack})";
            labelLevelInfoRank.Text = $"{MusicLevel.IndexNames[level.index]} {level.RankDisplay}";
            if (level.index != 5) labelLevelInfoRank.Text += $" ({level.Rating})";
            var profile = level.GetProfile();
            if (profile != null)
            {
                var txt = profile.ToString();
                if (level.index != 5) txt = $"[{profile.Rating}]{txt}";
                labelLevelInfoResult.Text = txt;
            }

            // level link
            var levelPool = MusicLevelLoader.levelByMusic[level.music.name];
            foreach (var l in levelPool)
            {
                var txt = $"{MusicLevel.IndexNames[l.index]} {l.RankDisplay}";
                if (l == level)
                {
                    var lvlCurr = new Label();
                    lvlCurr.Text = txt;
                    levelSwitchContainer.Controls.Add(lvlCurr);
                }
                else
                {
                    var lvlSwitch = new LinkLabel();
                    lvlSwitch.Text = txt;
                    lvlSwitch.LinkClicked += (object ss, LinkLabelLinkClickedEventArgs ee) =>
                    {
                        listLevels.SelectedItem = l;
                    };
                    levelSwitchContainer.Controls.Add(lvlSwitch);
                }
            }
        }

        void clearLevelDisplay()
        {
            labelLevelInfoName.Text = "三哼经";
            labelLevelInfoAuthor.Text = "田所 浩二";
            labelLevelInfoGenre.Text = "野獸";
            labelLevelInfoVersion.Text = "v1 91.98.10";
            labelLevelInfoRank.Text = "MASTER 19 (8.10)";
            labelLevelInfoResult.Text = "未游玩";
            levelSwitchContainer.Controls.Clear();
            imgMusicCover.Image = null;
        }

        private void fillLevelSearchText(object sender, LinkLabelLinkClickedEventArgs e)
        {
            textMusicSearch.Text = (sender as LinkLabel).Text;
        }

        private void labelLevelInfoGenre_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            chooseMusicGenre.SelectedItem = labelLevelInfoGenre.Text;
        }
        #endregion

        #region tab result
        void refreshResultPageImages()
        {
            imgResultPlayerIcon.Image = null;
            if (Selected.player == null) return;
            if (Storage.Characters.ContainsKey(Selected.player.chara))
                imgResultPlayerIcon.Image = Storage.Characters[Selected.player.chara].images[0].dds[2].Image;
        }

        private void flushResultPage(object sender, EventArgs e)
        {
            refreshResultPageImages();
            #region clear page
            labelRatingSummary.Text = "寄";
            #endregion
            if (Selected.player == null) return;
            PlayerRatingCalculator.Calc();
            labelRatingSummary.Text = PlayerRatingCalculator.GetRatingSummary();

            flusherResultPage.Enabled = false;
        }
        #endregion
    }
}
