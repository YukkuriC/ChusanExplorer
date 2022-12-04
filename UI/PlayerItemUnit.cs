using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChusanExplorer
{
    public partial class PlayerItemUnit : UserControl
    {
        static float imageShrinkRatio = 0.5f;

        public BaseItem item;
        bool oldSelect = false;

        public PlayerItemUnit(BaseItem source)
        {
            InitializeComponent();
            imgMain.ContextMenuStrip = Main.instance.contextMenuSaveImg;

            item = source;
            radioChoose.Text = item.name;
            if (item.rarity >= 0) radioChoose.BackColor = Config.rarityColors[item.rarity];

            Init();
        }
        public void Init()
        {
            if (Main.instance.itemPoolShowImages && item.image?.Image != null)
            {
                var img = item.image.Image;
                imgMain.Show();
                radioChoose.AutoSize = false;
                imgMain.Image = img;
                radioChoose.Width = imgMain.Width = (int)Math.Round(img.Width * imageShrinkRatio);
                imgMain.Height = (int)Math.Round(img.Height * imageShrinkRatio);
            }
            else
            {
                imgMain.Hide();
                imgMain.Height = 0;
                radioChoose.AutoSize = true;
                radioChoose.PerformLayout();
            }
            Width = radioChoose.Width;
            Height = radioChoose.Height + imgMain.Height;
        }

        public void SetChecked(bool flag)
        {
            if (flag != oldSelect) radioChoose.Checked = flag;
        }

        private void imgMain_Click(object sender, EventArgs e)
        {
            radioChoose.Select();
        }

        private void radioChoose_CheckedChanged(object sender, EventArgs e)
        {
            var newSelect = radioChoose.Checked;
            var shouldUpdate = !oldSelect && newSelect;
            oldSelect = newSelect;
            if (shouldUpdate) UIEvents.PlayerItemSelect.Invoke(item);
        }

        private void radioChoose_MouseHover(object sender, EventArgs e)
        {
            if (item.descrip != Config.NO_DESCRIP)
                Main.instance.toolTipGeneral.Show(item.descrip, sender as Control);
        }
    }
}
