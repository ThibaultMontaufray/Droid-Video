// log code 42 00
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using Tools4Libraries;
using Assistant;
using System.Text.RegularExpressions;

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
        private RibbonButton _rb_continueVideo;
        
        private RibbonPanel _panelScreen;
        private RibbonButton _rb_full_screeen;
        private RibbonButton _rb_16_9;
        private RibbonButton _rb_15;
        
        private RibbonPanel _panelSubtile;
        private RibbonButton _rb_browseSubtitle;
        private RibbonButton _rb_disableSubtitle;
        private RibbonButton _rb_subtitleList;

        private RibbonPanel _panelInfo;
        private RibbonLabel _lblInfo1; // depend of what data available we have
        private RibbonLabel _lblInfo2;
        private RibbonLabel _lblInfo3;
        private RibbonLabel _lblSeries;
        private RibbonButton _rb_serieNext;
        private RibbonButton _rb_seriePreview;

        private RibbonPanel _panelAudio;
        private RibbonTextBox _adjustAudioTrack;
        private RibbonButton _adjustAudioValidate;
        private RibbonButton _adjustAudioReset;
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
                BuildPanelOpen();
                BuildPanelScreen();
                BuildPanelSubtitle();
                BuildPanelAudio();
                BuildPanelInfo();

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
                EnableMenu();
                _rb_continueVideo.Enabled = _intVdo.IsMovieProgressionAvailable();
                _rb_disableSubtitle.Enabled = _intVdo.CurrentVideo.Subtitle != null;

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

                if (!string.IsNullOrEmpty(_intVdo.CurrentVideo.NameClean)) data.Add(string.Format("{1} : {0}", _intVdo.CurrentVideo.NameClean, GetText.Text("Video")));
                if (_intVdo.CurrentVideo.Date != null) data.Add(string.Format("{1} : {0}", _intVdo.CurrentVideo.Date, GetText.Text("Release")));
                if (!string.IsNullOrEmpty(_intVdo.CurrentVideo.Language)) data.Add(string.Format("{1} : {0}", _intVdo.CurrentVideo.Language, GetText.Text("Language")));
                if (!string.IsNullOrEmpty(_intVdo.CurrentVideo.SubtitleLanguage)) data.Add(string.Format("{1} : {0}", _intVdo.CurrentVideo.SubtitleLanguage, GetText.Text("Subtitle")));
                if (!string.IsNullOrEmpty(_intVdo.CurrentVideo.Format)) data.Add(string.Format("{1} : {0}", _intVdo.CurrentVideo.Format, GetText.Text("Format")));
                if (!string.IsNullOrEmpty(_intVdo.CurrentVideo.Source)) data.Add(string.Format("{1} : {0}", _intVdo.CurrentVideo.Source, GetText.Text("Source")));

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
                    _lblInfo2.Text = string.Format("{0} : {1}", GetText.Text("Path"), _intVdo.CurrentVideo.Path);
                    _lblInfo3.Text = string.Empty;
                }
                else
                {
                    _lblInfo1.Text = string.Format("{0} : {1}", GetText.Text("Path"), _intVdo.CurrentVideo.Path);
                    _lblInfo2.Text = string.Empty;
                    _lblInfo3.Text = string.Empty;
                }

                if (!string.IsNullOrEmpty(_intVdo.CurrentVideo.Season))
                {
                    _lblSeries.Text = string.Format("{2} {0} - {3} {1}", _intVdo.CurrentVideo.Season, _intVdo.CurrentVideo.Episod, GetText.Text("Season"), GetText.Text("Episod"));
                    _lblSeries.Visible = true;
                    _rb_seriePreview.Visible = true;
                    _rb_serieNext.Visible = true;
                    _rb_seriePreview.Enabled = !string.IsNullOrEmpty(_intVdo.SeriePathPreview);
                    _rb_serieNext.Enabled = !string.IsNullOrEmpty(_intVdo.SeriePathNext);
                }

                _rb_subtitleList.Enabled = _intVdo.CurrentVideo.DownloadableSubtilteLanguages.Count != 0;
            }
            else
            {
                _lblInfo1.Text = GetText.Text("Video") + " : ";
                _lblInfo2.Text = GetText.Text("Release") + " : ";
                _lblInfo3.Text = GetText.Text("Language") + " : ";
                DisableMenu();
            }
        }
        private void DisableMenu()
        {
            _panelAudio.Enabled = false;
            _panelScreen.Enabled = false;
            _panelSubtile.Enabled = false;
            _rb_continueVideo.Enabled = false;
            _lblSeries.Visible = false;
            _rb_seriePreview.Visible = false;
            _rb_serieNext.Visible = false;
        }
        private void EnableMenu()
        {
            // TODO : finish the dev of audio
            _panelAudio.Enabled = false;

            _panelScreen.Enabled = true;
            _panelSubtile.Enabled = true;
        }
        #endregion

        #region Methods private
        private void BuildPanelScreen()
        {
            _rb_full_screeen = new RibbonButton("Full");
            _rb_full_screeen.Image = Tools4Libraries.Resources.ResourceIconSet32Default.resize_picture;
            _rb_full_screeen.SmallImage = Tools4Libraries.Resources.ResourceIconSet32Default.resize_picture;
            _rb_full_screeen.Click += new EventHandler(rb_full_screeen_Click);
            _rb_full_screeen.ToolTip = "Alt + Enter";

            _rb_16_9 = new RibbonButton("Horizontal");
            _rb_16_9.Image = Tools4Libraries.Resources.ResourceIconSet32Default.size_horizontal;
            _rb_16_9.SmallImage = Tools4Libraries.Resources.ResourceIconSet32Default.size_horizontal;
            _rb_16_9.Click += new EventHandler(rb_16_9_Click);

            _rb_15 = new RibbonButton("Vertical");
            _rb_15.Image = Tools4Libraries.Resources.ResourceIconSet32Default.size_vertical;
            _rb_15.SmallImage = Tools4Libraries.Resources.ResourceIconSet32Default.size_vertical;
            _rb_15.Click += new EventHandler(rb_15_Click);

            _panelScreen = new RibbonPanel("Screen");
            _panelScreen.Items.Add(_rb_15);
            _panelScreen.Items.Add(_rb_16_9);
            _panelScreen.Items.Add(_rb_full_screeen);
            this.Panels.Add(_panelScreen);

        }
        private void BuildPanelOpen()
        {
            _rb_open_video = new RibbonButton(GetText.Text("Open"));
            _rb_open_video.Image = Tools4Libraries.Resources.ResourceIconSet32Default.folder;
            _rb_open_video.SmallImage = Tools4Libraries.Resources.ResourceIconSet16Default.folder;
            _rb_open_video.Click += new EventHandler(rb_open_video_Click);

            _rb_continueVideo = new RibbonButton(GetText.Text("Continue"));
            _rb_continueVideo.Image = Tools4Libraries.Resources.ResourceIconSet32Default.edit_free;
            _rb_continueVideo.SmallImage = Tools4Libraries.Resources.ResourceIconSet16Default.edit_free;
            _rb_continueVideo.Click += _rb_continueVideo_Click;

            _panelMain = new RibbonPanel(GetText.Text("Video"));
            _panelMain.Items.Add(_rb_open_video);
            _panelMain.Items.Add(_rb_continueVideo);
            this.Panels.Add(_panelMain);

        }
        private void BuildPanelSubtitle()
        {
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

            _panelSubtile = new RibbonPanel("Subtitles");
            _panelSubtile.Items.Add(_rb_browseSubtitle);
            _panelSubtile.Items.Add(_rb_subtitleList);
            _panelSubtile.Items.Add(_rb_disableSubtitle);
            this.Panels.Add(_panelSubtile);

        }
        private void BuildPanelAudio()
        {
            _adjustAudioTrack = new RibbonTextBox();
            _adjustAudioTrack.Text = "Décaler (s)";
            _adjustAudioTrack.TextBoxText = "0";
            _adjustAudioTrack.TextBoxWidth = 30;
            _adjustAudioTrack.TextBoxKeyPress += _adjustAudioTrack_TextBoxKeyPress;

            _adjustAudioValidate = new RibbonButton("Appliquer");
            _adjustAudioValidate.Image = Tools4Libraries.Resources.ResourceIconSet32Default.accept;
            _adjustAudioValidate.SmallImage = Tools4Libraries.Resources.ResourceIconSet16Default.accept;
            _adjustAudioValidate.MaxSizeMode = RibbonElementSizeMode.Medium;
            _adjustAudioValidate.TextAlignment = RibbonItem.RibbonItemTextAlignment.Center;
            _adjustAudioValidate.Click += _adjustAudioValidate_Click;

            _adjustAudioReset = new RibbonButton("Réinitialiser");
            _adjustAudioReset.Image = Tools4Libraries.Resources.ResourceIconSet32Default.delete;
            _adjustAudioReset.SmallImage = Tools4Libraries.Resources.ResourceIconSet16Default.delete;
            _adjustAudioReset.MaxSizeMode = RibbonElementSizeMode.Medium;
            _adjustAudioReset.TextAlignment = RibbonItem.RibbonItemTextAlignment.Center;
            _adjustAudioReset.Click += _adjustAudioReset_Click;

            _panelAudio = new RibbonPanel("Audio");
            _panelAudio.Items.Add(_adjustAudioTrack);
            _panelAudio.Items.Add(_adjustAudioValidate);
            _panelAudio.Items.Add(_adjustAudioReset);
            this.Panels.Add(_panelAudio);
        }
        private void BuildPanelInfo()
        {
            _lblInfo1 = new RibbonLabel();
            _lblInfo1.LabelWidth = 200;

            _lblInfo2 = new RibbonLabel();
            _lblInfo2.LabelWidth = 200;

            _lblInfo3 = new RibbonLabel();
            _lblInfo3.LabelWidth = 200;

            _lblSeries = new RibbonLabel();

            _rb_serieNext = new RibbonButton(GetText.Text("Next_episod"));
            _rb_serieNext.SmallImage = Tools4Libraries.Resources.ResourceIconSet16Default.document_page_next;
            _rb_serieNext.MaxSizeMode = RibbonElementSizeMode.Medium;
            _rb_serieNext.Click += _rb_serieNext_Click;

            _rb_seriePreview = new RibbonButton(GetText.Text("Preview_episod"));
            _rb_seriePreview.SmallImage = Tools4Libraries.Resources.ResourceIconSet16Default.document_page_previous;
            _rb_seriePreview.MaxSizeMode = RibbonElementSizeMode.Medium;
            _rb_seriePreview.Click += _rb_seriePreview_Click;

            _panelInfo = new RibbonPanel(GetText.Text("Details"));
            _panelInfo.Items.Add(_lblSeries);
            _panelInfo.Items.Add(_rb_serieNext);
            _panelInfo.Items.Add(_rb_seriePreview);
            _panelInfo.Items.Add(_lblInfo1);
            _panelInfo.Items.Add(_lblInfo2);
            _panelInfo.Items.Add(_lblInfo3);
            this.Panels.Add(_panelInfo);

            UpdateVideoDetails();
        }
        #endregion

        #region Event
        private void rb_open_video_Click(object sender, EventArgs e)
        {
            ToolBarEventArgs action = new ToolBarEventArgs("openVideo");
            OnAction(action);
        }
        private void rb_15_Click(object sender, EventArgs e)
        {
            ToolBarEventArgs action = new ToolBarEventArgs("screen15");
            OnAction(action);
        }
        private void rb_16_9_Click(object sender, EventArgs e)
        {
            ToolBarEventArgs action = new ToolBarEventArgs("screen169");
            OnAction(action);
        }
        private void rb_full_screeen_Click(object sender, EventArgs e)
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
        private void _adjustAudioTrack_TextBoxKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != '-'))
            {
                e.Handled = true;
            }
            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as RibbonTextBox).TextBoxText.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
            if ((e.KeyChar == '-') && (sender as RibbonTextBox).TextBoxText.StartsWith("-"))
            {
                e.Handled = true;
            }
        }
        private void _adjustAudioReset_Click(object sender, EventArgs e)
        {
            _intVdo.CurrentVideo.AudioAdjustment = 0;
            ToolBarEventArgs action = new ToolBarEventArgs("moveAudio");
            OnAction(action);
        }
        private void _adjustAudioValidate_Click(object sender, EventArgs e)
        {
            double res = 0;
            if (double.TryParse(_adjustAudioTrack.TextBoxText, out res))
            {
                _intVdo.CurrentVideo.AudioAdjustment = res;
                ToolBarEventArgs action = new ToolBarEventArgs("moveAudio");
                OnAction(action);
            }
        }
        private void _rb_continueVideo_Click(object sender, EventArgs e)
        {
            ToolBarEventArgs action = new ToolBarEventArgs("relaunchVideo");
            OnAction(action);
        }
        private void _rb_seriePreview_Click(object sender, EventArgs e)
        {
            ToolBarEventArgs action = new ToolBarEventArgs("seriePreview");
            OnAction(action);
        }
        private void _rb_serieNext_Click(object sender, EventArgs e)
        {
            ToolBarEventArgs action = new ToolBarEventArgs("serieNext");
            OnAction(action);
        }
        #endregion
    }
}
