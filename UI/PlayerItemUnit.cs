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
    public partial class PlayerItemUnit : UserControl
    {
        static float imageShrinkRatio = 0.5f;

        public BaseItem item;
        bool oldSelect = false;

        public PlayerItemUnit(BaseItem source)
        {
            InitializeComponent();
            item = source;
            radioChoose.Text = item.name;

            var img = item.image?.Image;
            if (img != null)
            {
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
    }
}
