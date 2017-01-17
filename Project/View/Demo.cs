using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Droid_video
{
    public partial class Demo : Form
    {
        #region Attribute
        private Interface_vdo _intVdo;
        #endregion

        #region Properties
        #endregion

        #region Constructor
        public Demo()
        {
            InitializeComponent();
            Init();
        }
        #endregion

        #region Methods public
        #endregion

        #region Methods private
        private void Init()
        {
            _intVdo = new Interface_vdo();
            _intVdo.Tsm.ActionAppened += Tsm_ActionAppened;

            _intVdo.Sheet.Dock = DockStyle.Fill;
            this.Controls.Add(_intVdo.Sheet);

            InitRibbon();
        }
        private void InitRibbon()
        {
            Ribbon rb = new Ribbon();
            rb.Tabs.Add(_intVdo.Tsm);
            rb.Height = 150;
            rb.ThemeColor = RibbonTheme.Black;
            rb.OrbDropDown.Width = 150;
            rb.OrbStyle = RibbonOrbStyle.Office_2013;
            rb.OrbText = "File";
            rb.QuickAccessToolbar.MenuButtonVisible = false;
            rb.QuickAccessToolbar.Visible = false;
            rb.QuickAccessToolbar.MinSizeMode = RibbonElementSizeMode.Compact;

            //rb.QuickAccessToolbar.Visible = false;

            RibbonButton b_open = new RibbonButton("Open");
            b_open.SmallImage = Tools4Libraries.Resources.ResourceIconSet32Default.open_folder;
            b_open.Click += B_open_Click;

            RibbonButton b_exit = new RibbonButton("Exit");
            b_exit.SmallImage = Tools4Libraries.Resources.ResourceIconSet32Default.door_out;
            b_exit.Click += B_exit_Click;

            rb.OrbDropDown.MenuItems.Add(b_open);
            rb.OrbDropDown.MenuItems.Add(b_exit);

            this.Controls.Add(rb);
        }

        #endregion

        #region Event
        private void Tsm_ActionAppened(object sender, EventArgs e)
        {
            _intVdo.GlobalAction(sender, e);
        }
        private void B_exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void B_open_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
