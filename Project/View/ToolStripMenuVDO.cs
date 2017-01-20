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
        
        private RibbonPanel _panelSubtile;
        private RibbonButton _rb_browseSubtitle;
        private RibbonTextBox _txt_automaticDownload;
        private RibbonLabel _label_AutomaticDownload;
        private RibbonButton _rb_disableSubtitle;
        private RibbonButton _rb_subtitleList;

        private RibbonPanel _panelInfo;
        private RibbonLabel _lblInfo1; // depend of what data available we have
        private RibbonLabel _lblInfo2;
        private RibbonLabel _lblInfo3;
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
        public void UpdateVideoDetails()
        {
            RibbonButton rbSubLang;
            List<string> data = new List<string>();
            
            if (_intVdo.CurrentVideo != null)
            {
                _rb_subtitleList.DropDownItems.Clear();
                if (_intVdo.CurrentVideo.DownloadableSubtilteLanguages != null)
                { 
                    foreach (string subtitleLanguage in _intVdo.CurrentVideo.DownloadableSubtilteLanguages)
                    {
                        rbSubLang = new RibbonButton(subtitleLanguage);
                        rbSubLang.Name = subtitleLanguage;
                        rbSubLang.Click += RbSubLang_Click;
                        _rb_subtitleList.DropDownItems.Add(rbSubLang);
                    }
                }

                if (!string.IsNullOrEmpty(_intVdo.CurrentVideo.NameClean)) data.Add(string.Format("Video : {0}", _intVdo.CurrentVideo.NameClean));
                if (_intVdo.CurrentVideo.Date != null) data.Add(string.Format("Release : {0}", _intVdo.CurrentVideo.Date));
                if (!string.IsNullOrEmpty(_intVdo.CurrentVideo.Language)) data.Add(string.Format("Language : {0}", _intVdo.CurrentVideo.Language));
                if (!string.IsNullOrEmpty(_intVdo.CurrentVideo.SubtitleLanguage)) data.Add(string.Format("Subtitle : {0}", _intVdo.CurrentVideo.SubtitleLanguage));
                if (!string.IsNullOrEmpty(_intVdo.CurrentVideo.Season)) data.Add(string.Format("Season {0} - Episod {1}", _intVdo.CurrentVideo.Season, _intVdo.CurrentVideo.Episod));
                if (!string.IsNullOrEmpty(_intVdo.CurrentVideo.Format)) data.Add(string.Format("Format : {0}", _intVdo.CurrentVideo.Format));
                if (!string.IsNullOrEmpty(_intVdo.CurrentVideo.Source)) data.Add(string.Format("Source : {0}", _intVdo.CurrentVideo.Source));

                if (data.Count > 2)
                {
                    _lblInfo1.Text = data[0];
                    _lblInfo2.Text = data[1];
                    _lblInfo3.Text = data[2];
                }
                else if (data.Count == 2)
                {
                    _lblInfo1.Text = data[0];
                    _lblInfo2.Text = data[1];
                    _lblInfo3.Text = string.Empty;
                }
                else if (data.Count == 1)
                {
                    _lblInfo1.Text = data[0];
                    _lblInfo2.Text = string.Format("Path : ", _intVdo.CurrentVideo.Path);
                    _lblInfo3.Text = string.Empty;
                }
                else
                {
                    _lblInfo1.Text = string.Format("Path : ", _intVdo.CurrentVideo.Path);
                    _lblInfo2.Text = string.Empty;
                    _lblInfo3.Text = string.Empty;
                }
            }
            else
            {
                _lblInfo1.Text = "Video : ";
                _lblInfo2.Text = "Release : ";
                _lblInfo3.Text = "Language : ";
            }
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
            
            _rb_browseSubtitle = new RibbonButton("Select subtitle");
            _rb_browseSubtitle.Image = Tools4Libraries.Resources.ResourceIconSet32Default.folder_find;
            _rb_browseSubtitle.SmallImage = Tools4Libraries.Resources.ResourceIconSet16Default.folder_find;
            _rb_browseSubtitle.Click += _rb_browseSubtitle_Click;

            _rb_disableSubtitle = new RibbonButton("Disable subtitle");
            _rb_disableSubtitle.Image = Tools4Libraries.Resources.ResourceIconSet32Default.tab_delete;
            _rb_disableSubtitle.SmallImage = Tools4Libraries.Resources.ResourceIconSet16Default.tab_delete;
            _rb_disableSubtitle.Click += _rb_disableSubtitle_Click;

            _rb_subtitleList = new RibbonButton("Download subtitle");
            _rb_subtitleList.Style = RibbonButtonStyle.DropDown;
            _rb_subtitleList.Image = Tools4Libraries.Resources.ResourceIconSet32Default.page_world;
            _rb_subtitleList.SmallImage = Tools4Libraries.Resources.ResourceIconSet16Default.page_world;
            _rb_subtitleList.Click += RbSubLang_Click;

            BuildPanelDetail();
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
            
            _panelSubtile = new RibbonPanel("Subtitles");
            _panelSubtile.Items.Add(_rb_browseSubtitle);
            _panelSubtile.Items.Add(_rb_subtitleList);
            _panelSubtile.Items.Add(_rb_disableSubtitle);
            this.Panels.Add(_panelSubtile);

            _panelInfo = new RibbonPanel("Details");
            _panelInfo.Items.Add(_lblInfo1);
            _panelInfo.Items.Add(_lblInfo2);
            _panelInfo.Items.Add(_lblInfo3);
            this.Panels.Add(_panelInfo);
        }
        private void BuildPanelDetail()
        {
            _lblInfo1 = new RibbonLabel();
            _lblInfo1.LabelWidth = 200;

            _lblInfo2 = new RibbonLabel();
            _lblInfo2.LabelWidth = 200;

            _lblInfo3 = new RibbonLabel();
            _lblInfo3.LabelWidth = 200;

            UpdateVideoDetails();
        }
        #endregion

        #region Event
        void rb_open_video_Click(object sender, EventArgs e)
        {
            ToolBarEventArgs action = new ToolBarEventArgs("openVideo");
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
        private void RbSubLang_Click(object sender, EventArgs e)
        {
            _intVdo.CurrentVideo.SubtitleRequested = ((RibbonButton)sender).Name;
            ToolBarEventArgs action = new ToolBarEventArgs("subtitleDownloadRequest");
            OnAction(action);
        }

        private void _rb_disableSubtitle_Click(object sender, EventArgs e)
        {
            ToolBarEventArgs action = new ToolBarEventArgs("disableSubtitle");
            OnAction(action);
        }
        #endregion
    }
}
