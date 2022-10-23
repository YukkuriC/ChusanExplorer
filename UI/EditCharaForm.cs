using System;
using System.Windows.Forms;

namespace ChusanExplorer
{
    public partial class EditCharaForm : Form
    {
        Character chara;
        PlayerCharaProfile profileInput, profileEdit;

        public EditCharaForm(Character character, PlayerCharaProfile profile)
        {
            InitializeComponent();
            chara = character;
            profileInput = profile;
            profileEdit = profile?.Clone();
            flushDisplay();
        }

        void flushDisplay()
        {
            updateCharaImages(0);
            var canSwitchIllust = chara.images.Count > 1;
            if (illustSwitcher.Enabled = canSwitchIllust)
            {
                illustSwitcher.Maximum = chara.images.Count - 1;
                var illustToSet = 0;
                for (var i = 0; i < chara.images.Count; i++)
                {
                    if (chara.images[i].id == profileEdit?.illustId)
                    {
                        illustToSet = i;
                        break;
                    }
                }
                illustSwitcher.Value = illustToSet;
            }
            var hasProfile = profileEdit != null;
            btnAddChara.Enabled = !hasProfile;
            btnRemoveChara.Enabled = hasProfile;
            panelEditProfile.Enabled = hasProfile;
            if (hasProfile)
            {
                trackMaxLevel.Value = profileEdit.maxLevel;
                trackLevel.Value = profileEdit.level;
                trackExp.Value = profileEdit.exp;
                inputPlayCount.Value = profileEdit.playCount;
                updateText();
            }
        }

        void updateCharaImages(int i)
        {
            var imgGrp = chara.images[i];
            imgCharaIcon.Image = imgGrp.dds[2].Image;
        }

        void updateText(int level = 114514)
        {
            grpExp.Text = $"当前等级经验: {trackExp.Value}/{profileEdit.LevelExpReq}";
            if (level >= 1)
                grpLevel.Text = $"角色等级: {trackLevel.Value}";
            if (level >= 2)
                grpMaxLevel.Text = $"最大等级: {trackMaxLevel.Value}";
        }

        void updateCanSave()
        {
            btnSave.Enabled = profileEdit != profileInput;
        }

        private void illustSwitcher_ValueChanged(object sender, EventArgs e)
        {
            if (profileEdit != null) profileEdit.illustId = chara.images[illustSwitcher.Value].id;
            updateCharaImages(illustSwitcher.Value);
            updateCanSave();
        }

        private void trackMaxLevel_ValueChanged(object sender, EventArgs e)
        {
            trackMaxLevel.Value = (int)Math.Round(trackMaxLevel.Value * 0.2f) * 5;
            profileEdit.maxLevel = trackLevel.Maximum = trackMaxLevel.Value;
            updateText(2);
            updateCanSave();
        }

        private void trackLevel_ValueChanged(object sender, EventArgs e)
        {
            profileEdit.level = trackLevel.Value;
            var canLvlUp = trackLevel.Value < trackMaxLevel.Value;
            trackExp.Enabled = canLvlUp;
            if (trackLevel.Value < trackMaxLevel.Value) trackExp.Maximum = profileEdit.LevelExpReq - 1;
            else trackExp.Maximum = 0;
            updateText(1);
            updateCanSave();
        }

        private void trackExp_ValueChanged(object sender, EventArgs e)
        {
            profileEdit.exp = trackExp.Value;
            updateText(0);
            updateCanSave();
        }

        private void btnRemoveChara_Click(object sender, EventArgs e)
        {
            profileEdit = null;
            flushDisplay();
            updateCanSave();
        }

        private void btnAddChara_Click(object sender, EventArgs e)
        {
            profileEdit = PlayerCharaProfile.CreateDefault(chara.images[illustSwitcher.Value].id);
            flushDisplay();
            updateCanSave();
        }

        private void inputPlayCount_ValueChanged(object sender, EventArgs e)
        {
            profileEdit.playCount = (int)inputPlayCount.Value;
            updateCanSave();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            PlayerCharaProfileLoader.UpdateCharaProfile(profileEdit);
            UIEvents.PlayerCharaProfileChanged.Invoke();
            if (CharacterListLoader.onlyShowOwned && (profileEdit == null) != (profileInput == null))
                UIEvents.CharaListApply.Invoke();
            Close();
        }
    }
}
