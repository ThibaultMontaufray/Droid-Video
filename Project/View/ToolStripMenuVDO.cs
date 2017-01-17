// log code 42 00
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using Tools4Libraries;
using Assistant;

namespace Droid_video
{
    public class ToolStripMenuVDO : RibbonTab
    {
        #region Attribute
        public event EventHandlerAction ActionAppened;

        private Panel _currentTabPage;
        private GUI _gui;
        private Interface_vdo _intVdo;
        private RibbonPanel _panelMain;
        private RibbonButton _rb_open_video;
        
        private RibbonPanel _panelScreen;
        private RibbonButton _rb_full_screeen;
        private RibbonButton _rb_16_9;
        private RibbonButton _rb_15;

        private RibbonPanel _panelNavigation;
        private RibbonButton _rb_play_pause;
        private RibbonButton _rb_stop;
        private RibbonButton _rb_speed_forward;
        private RibbonButton _rb_speed_back;

        private RibbonPanel _panelSubtile;
        private RibbonButton _rb_browseSubtitle;
        private RibbonTextBox _txt_automaticDownload;
        private RibbonLabel _label_AutomaticDownload;
        private RibbonButton _rb_subtitleList;
        #endregion

        #region Properties
        public GUI Gui
        {
            get { return _gui; }
            set { _gui = value; }
        }
        public Panel CurrentTabPage
        {
            get { return _currentTabPage; }
            set { _currentTabPage = value; }
        }
        #endregion

        #region Constructor
        public ToolStripMenuVDO(Interface_vdo interface_video)
        {
            try
            {
                _intVdo = interface_video;
                _gui = new GUI();
                buildButton();
                buildPanel();
                this.Text = "Video";
            }
            catch (Exception exp4200)
            {
                Log.write("[ CRT : 4200 ] Cannot open video menu.\n" + exp4200.Message);
                this.Dispose();
            }
        }
        #endregion

        #region Methods public
        public void OnAction(EventArgs e)
        {
            if (ActionAppened != null) ActionAppened(this, e);
        }
        #endregion

        #region Methods private
        private void buildButton()
        {
            _rb_open_video = new RibbonButton("Open");
            _rb_open_video.Image = Tools4Libraries.Resources.ResourceIconSet32Default.folder;
            _rb_open_video.SmallImage = Tools4Libraries.Resources.ResourceIconSet32Default.folder;
            _rb_open_video.Click += new EventHandler(rb_open_video_Click);

            _rb_full_screeen = new RibbonButton("Full");
            _rb_full_screeen.Image = Tools4Libraries.Resources.ResourceIconSet32Default.resize_picture;
            _rb_full_screeen.SmallImage = Tools4Libraries.Resources.ResourceIconSet32Default.resize_picture;
            _rb_full_screeen.Click += new EventHandler(rb_full_screeen_Click);

            _rb_16_9 = new RibbonButton("Horizontal");
            _rb_16_9.Image = Tools4Libraries.Resources.ResourceIconSet32Default.size_horizontal;
            _rb_16_9.SmallImage = Tools4Libraries.Resources.ResourceIconSet32Default.size_horizontal;
            _rb_16_9.Click += new EventHandler(rb_16_9_Click);

            _rb_15 = new RibbonButton("Vertical");
            _rb_15.Image = Tools4Libraries.Resources.ResourceIconSet32Default.size_vertical;
            _rb_15.SmallImage = Tools4Libraries.Resources.ResourceIconSet32Default.size_vertical;
            _rb_15.Click += new EventHandler(rb_15_Click);

            _rb_play_pause = new RibbonButton("Play");
            _rb_play_pause.Image = Tools4Libraries.Resources.ResourceIconSet32Default.control_play;
            _rb_play_pause.SmallImage = Tools4Libraries.Resources.ResourceIconSet32Default.control_play;
            _rb_play_pause.Click += new EventHandler(rb_play_pause_Click);

            _rb_stop = new RibbonButton("Stop");
            _rb_stop.Image = Tools4Libraries.Resources.ResourceIconSet32Default.control_stop;
            _rb_stop.SmallImage = Tools4Libraries.Resources.ResourceIconSet16Default.control_stop;
            _rb_stop.Click += new EventHandler(rb_stop_Click);

            _rb_speed_forward = new RibbonButton("Forward");
            _rb_speed_forward.Image = Tools4Libraries.Resources.ResourceIconSet32Default.control_fastforward;
            _rb_speed_forward.SmallImage = Tools4Libraries.Resources.ResourceIconSet16Default.control_fastforward;
            _rb_speed_forward.Click += new EventHandler(rb_speed_forward_Click);

            _rb_speed_back = new RibbonButton("Rewind");
            _rb_speed_back.Image = Tools4Libraries.Resources.ResourceIconSet32Default.control_rewind;
            _rb_speed_back.SmallImage = Tools4Libraries.Resources.ResourceIconSet16Default.control_rewind;
            _rb_speed_back.Click += new EventHandler(rb_speed_back_Click);

            _rb_browseSubtitle = new RibbonButton("Browse");
            _rb_browseSubtitle.Image = Tools4Libraries.Resources.ResourceIconSet32Default.folder_find;
            _rb_browseSubtitle.SmallImage = Tools4Libraries.Resources.ResourceIconSet16Default.folder_find;
            _rb_browseSubtitle.Click += _rb_browseSubtitle_Click;

            _label_AutomaticDownload = new RibbonLabel();
            _label_AutomaticDownload.Text = "Select your language : ";

            _txt_automaticDownload = new RibbonTextBox();
            _txt_automaticDownload.Image = Tools4Libraries.Resources.ResourceIconSet16Default.find;
            _txt_automaticDownload.TextBoxText = "French";

            _rb_subtitleList = new RibbonButton("Subtitle");
            _rb_subtitleList.Style = RibbonButtonStyle.DropDownListItem;
            _rb_subtitleList.MaxSizeMode = RibbonElementSizeMode.Medium;
            _rb_subtitleList.Image = Tools4Libraries.Resources.ResourceIconSet16Default.text_document;
        }
        private void buildPanel()
        {
            _panelMain = new RibbonPanel("Video");
            _panelMain.Items.Add(_rb_open_video);
            this.Panels.Add(_panelMain);

            _panelScreen = new RibbonPanel("Screen");
            _panelScreen.Items.Add(_rb_15);
            _panelScreen.Items.Add(_rb_16_9);
            _panelScreen.Items.Add(_rb_full_screeen);
            this.Panels.Add(_panelScreen); 

            _panelNavigation = new RibbonPanel("Navigation");
            _panelNavigation.Items.Add(_rb_speed_back);
            _panelNavigation.Items.Add(_rb_stop);
            _panelNavigation.Items.Add(_rb_play_pause);
            _panelNavigation.Items.Add(_rb_speed_forward);
            this.Panels.Add(_panelNavigation);

            _panelSubtile = new RibbonPanel("Subtitles");
            _panelSubtile.Items.Add(_rb_browseSubtitle);
            _panelSubtile.Items.Add(_label_AutomaticDownload);
            _panelSubtile.Items.Add(_txt_automaticDownload);
            _panelSubtile.Items.Add(_rb_subtitleList);
            this.Panels.Add(_panelSubtile);
        }
        #endregion

        #region Event
        void rb_open_video_Click(object sender, EventArgs e)
        {
            ToolBarEventArgs action = new ToolBarEventArgs("openVideo");
            OnAction(action);
        }
        void rb_show_library_Click(object sender, EventArgs e)
        {
            ToolBarEventArgs action = new ToolBarEventArgs("showLibrary");
            OnAction(action);
        }
        void rb_show_explorer_Click(object sender, EventArgs e)
        {
            ToolBarEventArgs action = new ToolBarEventArgs("showExplorer");
            OnAction(action);
        }
        void rb_speed_back_Click(object sender, EventArgs e)
        {
            ToolBarEventArgs action = new ToolBarEventArgs("speedBack");
            OnAction(action);
        }
        void rb_speed_forward_Click(object sender, EventArgs e)
        {
            ToolBarEventArgs action = new ToolBarEventArgs("speedForward");
            OnAction(action);
        }
        void rb_stop_Click(object sender, EventArgs e)
        {
            ToolBarEventArgs action = new ToolBarEventArgs("stop");
            OnAction(action);
        }
        void rb_play_pause_Click(object sender, EventArgs e)
        {
            ToolBarEventArgs action = new ToolBarEventArgs("playPause");
            OnAction(action);
        }
        void rb_15_Click(object sender, EventArgs e)
        {
            ToolBarEventArgs action = new ToolBarEventArgs("screen15");
            OnAction(action);
        }
        void rb_16_9_Click(object sender, EventArgs e)
        {
            ToolBarEventArgs action = new ToolBarEventArgs("screen169");
            OnAction(action);
        }
        void rb_full_screeen_Click(object sender, EventArgs e)
        {
            ToolBarEventArgs action = new ToolBarEventArgs("screenFull");
            OnAction(action);
        }
        private void _rb_browseSubtitle_Click(object sender, EventArgs e)
        {
            ToolBarEventArgs action = new ToolBarEventArgs("browseSubtitle");
            OnAction(action);
        }
        #endregion
    }
}
