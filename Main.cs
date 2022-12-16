using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ChusanExplorer
{
    public partial class Main : Form
    {
        Random rand;
        bool isLoading;
        public static Main instance;

        public Main()
        {
            instance = this;
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
            chooseItemType.DataSource = ItemDescriptor.choices;
            chooseItemSort.DataSource = ItemSortTypes.AllMethods;
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
            ImageLRU.Init();
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

            InitEventsChara();
            InitEventsMusic();
            InitEventsResults();
            InitEventsItem();
        }

        void onFirstLoad()
        {
            if (Selected.player != null)
            {
                Storage.Characters.TryGetValue(Selected.player.chara, out Selected.chara);
                var lastPlayed = Selected.player.r30[0];
                Storage.Music.TryGetValue(lastPlayed.musicId, out Music lastMusic);
                if (lastMusic != null) Selected.level = lastMusic.levels[lastPlayed.levelId];
            }
        }
        #endregion

        #region frame
        private void choosePlayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            var plr = (Player)choosePlayer.SelectedItem;
            Selected.player = plr;
            plr.InitAdvance();
            InitComponentData();
            currentItemProfile = plr.itemProfile.Clone();
            UIEvents.PlayerCharaProfileChanged.Invoke();
            UIEvents.RefreshResultPage.Invoke();
            UIEvents.RefreshPlayerItems.Invoke();
        }

        private void choosePack_SelectedIndexChanged(object sender, EventArgs e)
        {
            PackLoader.packMap.TryGetValue(choosePack.Text, out Selected.pack);
            UIEvents.PackChoiceChanged.Invoke(Selected.pack);
            UIEvents.RefreshResultPage.Invoke();
        }

        private void tabControlMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            ImageLRU.TryRelease();
        }

        private void clickSaveImage(object sender, EventArgs e)
        {
            var imgBox = contextMenuSaveImg.SourceControl as PictureBox;
            if (imgBox == null) return;
            saveImgDialog.FileName = $"{imgBox.Name}";
            var saveRes = saveImgDialog.ShowDialog();
            if (saveRes == DialogResult.OK)
            {
                var img = imgBox.Image as Bitmap;
                img.Save(saveImgDialog.FileName);
            }
        }
        #endregion

        #region interfaces
        public string ItemSortType => chooseItemSort.Text;
        public int CurrentTab => tabControlMain.SelectedIndex;
        #endregion

        #region tab chara
        void InitEventsChara()
        {
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
        }

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
                var link = new LinkLabel
                {
                    Text = $"[{i}] {img.name}",
                    Margin = new Padding(3),
                    AutoSize = true,
                    LinkBehavior = LinkBehavior.HoverUnderline,
                };
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
        void InitEventsMusic()
        {
            UIEvents.MusicListApply += () => flusherMusicList.Enabled = true;
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
        }

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
            if (item.index <= 3) myBrush = new SolidBrush(Config.levelColors[item.index]);
            else if (item.index == 5) myBrush = rainbowColors[(e.Index) % 7];
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
                    var lvlSwitch = new LinkLabel
                    {
                        Text = txt,
                        LinkBehavior = LinkBehavior.HoverUnderline,
                    };
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
        void InitEventsResults()
        {
            UIEvents.RefreshResultPage += () => flusherResultPage.Enabled = true;
        }

        void refreshResultPageImages()
        {
            imgResultPlayerIcon.Image = null;
            if (Selected.player == null) return;
            if (Storage.Characters.ContainsKey(Selected.player.chara))
                imgResultPlayerIcon.Image = Storage.Characters[Selected.player.chara].images[0].dds[2].Image;
        }

        static readonly int[] recommandTableIndexes = new int[] { 0, 14, 29, 39 };
        static readonly float[] recommandTableRatingDeltas = new float[] { 0, 0.6f, 1f, 1.5f, 2f, 2.15f };

        void switchToLevel(MusicLevel target)
        {
            tabControlMain.SelectedTab = tabLevel;
            listLevels.SelectedItem = target;
        }

        LinkLabelLinkClickedEventHandler genRecommendCellCallback(float baseRating, float targetRating)
        {
            return (object txt, LinkLabelLinkClickedEventArgs args) =>
            {
                for (var delta = 0.1; delta <= 0.3; delta++)
                {
                    var filters = from lvl in MusicLevelLoader.levelAll
                                  where Math.Abs(lvl.Rating - targetRating) < delta
                                  select lvl;
                    if (checkRecommandLvlNew.Checked) filters = filters.Where((lvl) => lvl.GetProfile() == null);
                    if (checkRecommandLvlPlayed.Checked) filters = filters.Where((lvl) => lvl.GetProfile() != null);
                    if (checkRecommandHighScore.Checked) filters = filters.Where((lvl) => (lvl.GetProfile()?.Rating ?? 0) < baseRating);
                    var pool = filters.ToList();
                    if (pool.Count > 0)
                    {
                        switchToLevel(pool[rand.Next(pool.Count)]);
                        return;
                    }
                }
                MessageBox.Show("寄");
            };
        }

        LinkLabel genResultLink(string prefix, PlayerLevelResult data)
        {
            var level = data.GetLevel();
            var txt = new LinkLabel
            {
                Text = $"{prefix}[{data.Rating.ToString("0.00")}|{data}]{level.GetGeneralDisplay()}",
                AutoSize = true,
                Margin = new Padding(0),
                LinkBehavior = LinkBehavior.HoverUnderline,
            };
            if (level.index <= 3) txt.LinkColor = Config.levelColors[level.index];
            txt.LinkClicked += (o, e) => switchToLevel(level);
            return txt;
        }

        private void flushResultPage(object sender, EventArgs e)
        {
            if (isLoading) return;
            tabResult.Hide();

            refreshResultPageImages();
            #region clear page
            labelRatingSummary.Text = "寄";
            for (int c = 1; c < tableRecommand.ColumnCount; c++)
            {
                for (int r = 1; r < tableRecommand.RowCount; r++)
                {
                    var cell = tableRecommand.GetControlFromPosition(c, r);
                    cell?.Dispose();
                }
            }
            lstB30.Controls.Clear();
            lstR10.Controls.Clear();
            #endregion
            if (Selected.player == null) return;
            PlayerRatingCalculator.Calc();
            labelRatingSummary.Text = PlayerRatingCalculator.GetRatingSummary();

            // table
            for (int r = 1; r < 5; r++)
            {
                var idx = recommandTableIndexes[r - 1];
                var ratingBase = idx < PlayerRatingCalculator.b40.Count ?
                    PlayerRatingCalculator.b40[idx].Rating :
                    0;
                for (int c = 1; c < 8; c++)
                {
                    Label cellToAdd;
                    if (c == 1) cellToAdd = new Label { Text = ratingBase.ToString(), };
                    else
                    {
                        var ratingDelta = ratingBase - recommandTableRatingDeltas[c - 2];
                        if (ratingDelta >= 1 && ratingDelta <= 15.4)
                        {
                            cellToAdd = new LinkLabel
                            {
                                Text = Math.Round(ratingDelta, 1).ToString(),
                                LinkBehavior = LinkBehavior.HoverUnderline,
                            };
                            (cellToAdd as LinkLabel).LinkClicked += genRecommendCellCallback(ratingBase, ratingDelta);
                        }
                        else cellToAdd = new Label { Text = "-", };
                    }
                    cellToAdd.Dock = DockStyle.Fill;
                    cellToAdd.TextAlign = ContentAlignment.MiddleCenter;
                    tableRecommand.Controls.Add(cellToAdd);
                    tableRecommand.SetCellPosition(cellToAdd, new TableLayoutPanelCellPosition(c, r));
                }
            }

            // level lists
            for (var i = 0; i < PlayerRatingCalculator.b40.Count; i++)
            {
                lstB30.Controls.Add(genResultLink(
                    $"{i + 1:00}.",
                    PlayerRatingCalculator.b40[i]
                ));
                if (i == 29) lstB30.Controls.Add(new Label
                {
                    Text = "------------------------------",
                    Margin = new Padding(0),
                    AutoSize = true,
                });
            }
            for (var i = 0; i < PlayerRatingCalculator.r10.Count; i++)
            {
                var res = PlayerRatingCalculator.r10[i];
                lstR10.Controls.Add(genResultLink("", res));
            }

            if (tabControlMain.SelectedTab == tabResult) tabResult.Show();
            flusherResultPage.Enabled = false;
        }

        private void checkRecommandLvlFilters_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked)
            {
                if (sender == checkRecommandLvlNew) checkRecommandLvlPlayed.Checked = false;
                else checkRecommandLvlNew.Checked = false;
            }
        }
        #endregion

        #region tab items
        bool playerItemSetDirty, playerItemPoolDirty;
        PlayerItemProfile currentItemProfile;
        void InitEventsItem()
        {
            UIEvents.PackChoiceChanged += (Pack pack) =>
            {
                updatePlayerItemSource();
                UIEvents.RefreshPlayerItemPool.Invoke();
            };
            UIEvents.RefreshPlayerItemSet += () =>
            {
                playerItemSetDirty = true;
                flusherPlayerItems.Enabled = true;
            };
            UIEvents.RefreshPlayerItemPool += () =>
            {
                playerItemPoolDirty = true;
                flusherPlayerItems.Enabled = true;
            };
            UIEvents.RefreshPlayerItems += () =>
            {
                playerItemSetDirty = true;
                playerItemPoolDirty = true;
                flusherPlayerItems.Enabled = true;
            };
            UIEvents.PlayerItemSelect += (BaseItem item) =>
            {
                currentItemProfile.fields[((ItemDescriptor)chooseItemType.SelectedItem).nameField] = item.id;
                foreach (var i in poolPlayerItems.Controls)
                {
                    var choice = i as PlayerItemUnit;
                    choice.SetChecked(choice.item == item);
                }
                flusherPlayerItems.Enabled = true;
            };
        }
        void updatePlayerItemSource()
        {
            var oldDescrip = (ItemDescriptor)chooseItemType.SelectedItem;
            chooseItemType.DataSource = ItemDescriptor.choices.Where(d => d.GetItems(Selected.pack).Count() > 0).ToList();
            if (oldDescrip != null) chooseItemType.SelectedItem = oldDescrip;
        }
        void updatePlayerCurrentItems()
        {
            var plr = Selected.player;
            imgNamePlate.Image = imgMapIcon.Image = imgSystemVoice.Image = null;
            if (plr != null)
            {
                var p = plr.itemProfile;
                BaseItem
                    namePlate = Storage.NamePlate.TryGet(p.NamePlate),
                    sysVoice = Storage.SystemVoice.TryGet(p.SystemVoice),
                    mapIcon = Storage.MapIcon.TryGet(p.MapIcon),
                    trophy = Storage.Trophy.TryGet(plr.itemProfile.Trophy);
                imgNamePlate.Image = namePlate?.image.Image;
                imgSystemVoice.Image = sysVoice?.image.Image;
                imgMapIcon.Image = mapIcon?.image.Image;
                labelTrophy.Text = trophy?.name ?? "寄";
                labelTrophy.BackColor = Config.rarityColors[trophy?.rarity ?? 0];
            }
            playerItemSetDirty = false;
        }
        void updatePlayerItemChoices()
        {
            poolPlayerItems.Hide();
            poolPlayerItems.Controls.Clear();
            var descrip = (ItemDescriptor)chooseItemType.SelectedItem;
            if (descrip == null || Selected.player == null) return;
            var data = descrip.GetItems(Selected.pack);
            if (!checkItemShowNotOwned.Checked)
            {
                var owned = descrip.getterPlayerSet();
                data = data.Where(i => owned.ContainsKey(i.id));
            }
            data = data.Where(i => i.name.Contains(textItemSearch.Text));
            data = data.OrderBy(i => i.SortKeyInner);
            data = data.Take(1000);
            foreach (var item in data)
            {
                var choice = new PlayerItemUnit(item);
                poolPlayerItems.Controls.Add(choice);
            }

            poolPlayerItems.Show();
            playerItemPoolDirty = false;
        }
        void updatePlayerItemMinorUI()
        {
            var descrip = (ItemDescriptor)chooseItemType.SelectedItem;
            btnSaveCurrentItem.Enabled = descrip != null && currentItemProfile.fields[descrip.nameField] != Selected.player.itemProfile.fields[descrip.nameField];
            btnResetItemProfile.Enabled = btnSaveAllItems.Enabled = currentItemProfile != Selected.player.itemProfile;
            UIEvents.PlayerItemSelect.Invoke(descrip.getterStorage().TryGet(currentItemProfile.fields[descrip.nameField]));
        }
        private void flushPlayerItems(object sender, EventArgs e)
        {
            if (isLoading) return;
            if (playerItemSetDirty) updatePlayerCurrentItems();
            if (playerItemPoolDirty) updatePlayerItemChoices();
            updatePlayerItemMinorUI();
            flusherPlayerItems.Enabled = false;
            ImageLRU.TryRelease();
        }

        public bool itemPoolShowImages => checkItemShowImages.Checked;
        private void checkItemShowImages_CheckedChanged(object sender, EventArgs e)
        {
            poolPlayerItems.Hide();
            foreach (var i in poolPlayerItems.Controls)
            {
                (i as PlayerItemUnit).Init();
            }
            poolPlayerItems.Show();
        }

        private void btnSaveCurrentItem_Click(object sender, EventArgs e)
        {
            var descrip = (ItemDescriptor)chooseItemType.SelectedItem;
            var oldVal = Selected.player.itemProfile.fields[descrip.nameField];
            var newVal = currentItemProfile.fields[descrip.nameField];
            if (oldVal != newVal)
            {
                var sql = $"update chusan_user_data set {descrip.nameSQL}={newVal} where id={Selected.player.id}";
                DBLoader.Write(sql);
                Selected.player.itemProfile.fields[descrip.nameField] = currentItemProfile.fields[descrip.nameField];
            }
            UIEvents.RefreshPlayerItemSet.Invoke();
        }

        private void btnSaveAllItems_Click(object sender, EventArgs e)
        {
            var pool = new List<string>();
            foreach (var descrip in ItemDescriptor.choices)
            {
                var oldVal = Selected.player.itemProfile.fields[descrip.nameField];
                var newVal = currentItemProfile.fields[descrip.nameField];
                if (oldVal != newVal) pool.Add($"{descrip.nameSQL}={newVal}");
            }
            if (pool.Count > 0)
            {
                var sql = $"update chusan_user_data set {string.Join(",", pool)} where id={Selected.player.id}";
                DBLoader.Write(sql);
                Selected.player.itemProfile = currentItemProfile.Clone();
            }
            UIEvents.RefreshPlayerItemSet.Invoke();
        }

        private void btnResetItemProfile_Click(object sender, EventArgs e)
        {
            currentItemProfile = Selected.player.itemProfile.Clone();
            flusherPlayerItems.Enabled = true;
        }

        private void playerItem_MouseHover(object sender, EventArgs e)
        {
            var p = Selected.player?.itemProfile;
            if (p == null) return;
            string tip = null;
            if (sender == imgNamePlate) tip = Storage.NamePlate.TryGet(p.NamePlate)?.name;
            else if (sender == imgSystemVoice) tip = Storage.SystemVoice.TryGet(p.SystemVoice)?.name;
            else if (sender == imgMapIcon) tip = Storage.MapIcon.TryGet(p.MapIcon)?.name;
            else if (sender == labelTrophy) tip = Storage.Trophy.TryGet(p.Trophy)?.descrip;
            if (tip != null && tip != Config.NO_DESCRIP)
                toolTipGeneral.Show(tip, sender as Control);
        }

        private void playerItemFilterChanged(object sender, EventArgs e)
        {
            UIEvents.RefreshPlayerItemPool.Invoke();
        }
        #endregion
    }
}
