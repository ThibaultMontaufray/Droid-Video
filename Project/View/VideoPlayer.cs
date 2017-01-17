using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Vlc.DotNet.Forms;
using Tools4Libraries.Slider;
using System.Text.RegularExpressions;

namespace Droid_video
{
    public delegate void VideoPlayerEventHandler();
    public class VideoPlayer : UserControl
    {
        #region Attributes
        private System.ComponentModel.IContainer components = null;

        private Vlc.DotNet.Forms.VlcControl _vlcControl;
        private System.Windows.Forms.Button myBtnPlayPause;
        private System.Windows.Forms.Button myBtnStop;
        private System.Windows.Forms.Label myLblMediaLength;
        private System.Windows.Forms.Label myLblVlcPosition;
        private System.Windows.Forms.Label label2;
        private Tools4Libraries.Slider.SliderTrackBar _trackBar;
        private Tools4Libraries.Slider.SliderTrackBar _trackBarSound;
        private Panel _panelControl;
        private string _videoPath;
        private SaveFileDialog saveFileDialog1;
        private bool _openned;
        private Button buttonUp30;
        private Button buttonMinus10;
        private bool _mouseDown;
        private Button buttonMute;
        private Panel _panelQuickControls;
        private Timer _hidePanel;
        private Timer _showPanel;
        private Timer _detectMouseMovement;
        private bool _hidding;
        private bool _showing;
        private bool _quickAccessHidden;
        private Point _mousePosition;
        private DateTime _lastTimeMouseMove;
        private Panel _panelSound;
        private bool _fullScreen;

        private WebBrowser _textBoxSubtitle;
        private Subtitle _sutbtitles;
        private UserControl _subtitlesUserControl;
        private int _subtitlesRows;
        private int _minSubTop = 105;
        private string _subText;

        public event VideoPlayerEventHandler FullScreenRequested;
        public event VideoPlayerEventHandler FullScreenExit;
        public VideoPlayerEventHandler HideSubtitlePanel;
        public VideoPlayerEventHandler DisplaySubtitlePanel;
        private DateTime _lastFrameMove;
        #endregion

        #region Properties
        public bool FullScreen
        {
            get { return _fullScreen; }
            set { _fullScreen = value; }
        }
        public Subtitle Subtitles
        {
            get { return _sutbtitles; }
            set { _sutbtitles = value; }
        }
        #endregion

        #region Constructor
        public VideoPlayer()
        {
            InitializeComponent();
            Init();
        }
        #endregion

        #region Methods public
        public void OpenFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _videoPath = ofd.FileName;
                _openned = false;
                _trackBar.Enabled = true;
                Play();
            }
        }
        public void KeyEvent(char key)
        {
            switch (key)
            {
                case '0':
                    Pause();
                    break;
                case (char)Keys.Escape:
                    if (FullScreenExit != null) FullScreenExit();
                    break;
                case (char)Keys.Space:
                    Pause();
                    break;
                //case (char)Keys.Enter:
                //    if (_quickAccessHidden) ShowQuickControl();
                //    else HideQuickControl();
                //    break;
            }
        }
        public void Play()
        {
            if (!string.IsNullOrEmpty(_videoPath))
            {
                if (!_openned)
                {
                    _trackBar.Value = 0;
                    _vlcControl.Play(new Uri(_videoPath));
                    _openned = true;
                }
                else
                {
                    Pause();
                }
                myBtnPlayPause.BackgroundImage = Tools4Libraries.Resources.ResourceIconSet32Default.control_pause;
                _trackBar.Enabled = true;
            }
        }
        public void Stop()
        {
            _vlcControl.Stop();
            myBtnPlayPause.BackgroundImage = Tools4Libraries.Resources.ResourceIconSet32Default.control_play;
            _openned = false;
            _trackBar.Value = 0;
            _vlcControl.Time = 0;
        }
        public void Pause()
        {
            _vlcControl.Pause();
            myBtnPlayPause.BackgroundImage = Tools4Libraries.Resources.ResourceIconSet32Default.control_play;
        }
        public void HideQuickControl()
        {
            _hidePanel.Start();
        }
        public void ShowQuickControl()
        {
            _showPanel.Start();
        }
        #endregion

        #region Methods protected
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Methods private
        private void Init()
        {
            _openned = false;
            _lastFrameMove = DateTime.MinValue;
            _hidding = false;
            _showing = false;
            _quickAccessHidden = false;
            _subtitlesUserControl.Visible = false;

            HideSubtitlePanel = new VideoPlayerEventHandler(HideSubtitle);
            DisplaySubtitlePanel = new VideoPlayerEventHandler(ShowSubtitle);

            #region VLCControl
            this._vlcControl = new VlcControl();
            ((System.ComponentModel.ISupportInitialize)(this._vlcControl)).BeginInit();
            this._vlcControl.Dock = DockStyle.Fill;
            this._vlcControl.BackColor = System.Drawing.Color.Black;
            this._vlcControl.BackgroundImage = new Bitmap(Droid_video.Properties.Resources.video_bg);
            this._vlcControl.BackgroundImageLayout = ImageLayout.Center;
            this._vlcControl.Location = new System.Drawing.Point(12, 12);
            this._vlcControl.Name = "myVlcControl";
            this._vlcControl.Size = new System.Drawing.Size(564, 338);
            this._vlcControl.TabIndex = 0;
            this._vlcControl.Text = "vlcRincewindControl1";
            this._vlcControl.VlcLibDirectory = null;
            this._vlcControl.VlcLibDirectoryNeeded += new System.EventHandler<Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs>(this.OnVlcControlNeedLibDirectory);
            this._vlcControl.LengthChanged += new System.EventHandler<Vlc.DotNet.Core.VlcMediaPlayerLengthChangedEventArgs>(this.OnVlcMediaLengthChanged);
            this._vlcControl.PositionChanged += new System.EventHandler<Vlc.DotNet.Core.VlcMediaPlayerPositionChangedEventArgs>(this.OnVlcPositionChanged);
            ((System.ComponentModel.ISupportInitialize)(this._vlcControl)).EndInit();
            this.Controls.Add(_vlcControl);
            #endregion

            #region TrackBar
            _trackBar = new SliderTrackBar();
            _trackBar.BackColor = System.Drawing.Color.Transparent;
            _trackBar.EmptyTrackColor = System.Drawing.Color.Black;
            _trackBar.Dock = DockStyle.Top;
            _trackBar.Height = 20;
            _trackBar.ScaleType = SliderTrackBar.SliderTrackBarScaleType.None;
            _trackBar.SliderButtonSize = new Size(14, 7);
            _trackBar.ShowSlider = SliderTrackBar.SliderTrackBarShowSlider.OnHover;
            _trackBar.TrackLowerColor = Color.FromName("SaddleBrown");
            _trackBar.TrackUpperColor = Color.FromName("Orange");
            _trackBar.MouseUp += _trackBar_MouseUp;
            _trackBar.MouseDown += _trackBar_MouseDown;
            _trackBar.MouseMove += _trackBar_MouseMove;
            _trackBar.UseSeeking = false;
            _trackBar.Enabled = false;
            _panelControl.Controls.Add(_trackBar);
            #endregion

            #region TrackBarSound
            _trackBarSound = new SliderTrackBar();
            _trackBarSound.BackColor = System.Drawing.Color.Transparent;
            _trackBarSound.EmptyTrackColor = System.Drawing.Color.Black;
            _trackBarSound.Height = 20;
            _trackBarSound.ScaleType = SliderTrackBar.SliderTrackBarScaleType.ScaleFields;
            _trackBarSound.ScaleFieldColor = Color.Gray;
            _trackBarSound.SliderButtonSize = new Size(14, 7);
            _trackBarSound.ShowSlider = SliderTrackBar.SliderTrackBarShowSlider.Always;
            _trackBarSound.TrackLowerColor = Color.FromName("SaddleBrown");
            _trackBarSound.TrackUpperColor = Color.FromName("Orange");
            _trackBarSound.Dock = DockStyle.Fill;
            _trackBarSound.Maximum = 100;
            _trackBarSound.UseSeeking = false;
            _trackBarSound.ValueChanged += _trackBarSound_ValueChanged;
            _panelSound.Controls.Add(_trackBarSound);
            #endregion

            #region Timers
            _showPanel = new Timer();
            _showPanel.Interval = 1;
            _showPanel.Tick += _showPanel_Tick;

            _hidePanel = new Timer();
            _hidePanel.Interval = 1;
            _hidePanel.Tick += _hidePanel_Tick;

            _detectMouseMovement = new Timer();
            _detectMouseMovement.Interval = 3000;
            _detectMouseMovement.Tick += _detectMouseMovement_Tick;
            _detectMouseMovement.Start();
            #endregion

            #region Event
            this.MouseMove += MouseMoveEvent;
            this._panelControl.MouseMove += MouseMoveEvent;
            this._panelSound.MouseMove += MouseMoveEvent;

            this._vlcControl.KeyPress += KeyPressEvent;
            this._panelControl.KeyPress += KeyPressEvent;
            this._panelSound.KeyPress += KeyPressEvent;

            this.Resize += VideoPlayer_Resize;
            #endregion
        }
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VideoPlayer));
            this.myBtnPlayPause = new System.Windows.Forms.Button();
            this.myBtnStop = new System.Windows.Forms.Button();
            this.myLblMediaLength = new System.Windows.Forms.Label();
            this.myLblVlcPosition = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._panelControl = new System.Windows.Forms.Panel();
            this._panelQuickControls = new System.Windows.Forms.Panel();
            this._panelSound = new System.Windows.Forms.Panel();
            this.buttonMute = new System.Windows.Forms.Button();
            this.buttonUp30 = new System.Windows.Forms.Button();
            this.buttonMinus10 = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this._textBoxSubtitle = new System.Windows.Forms.WebBrowser();
            this._subtitlesUserControl = new System.Windows.Forms.UserControl();
            this._panelControl.SuspendLayout();
            this._panelQuickControls.SuspendLayout();
            this._subtitlesUserControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // myBtnPlayPause
            // 
            this.myBtnPlayPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.myBtnPlayPause.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("myBtnPlayPause.BackgroundImage")));
            this.myBtnPlayPause.FlatAppearance.BorderSize = 0;
            this.myBtnPlayPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.myBtnPlayPause.Location = new System.Drawing.Point(29, 4);
            this.myBtnPlayPause.Name = "myBtnPlayPause";
            this.myBtnPlayPause.Size = new System.Drawing.Size(32, 32);
            this.myBtnPlayPause.TabIndex = 1;
            this.myBtnPlayPause.UseVisualStyleBackColor = true;
            this.myBtnPlayPause.Click += new System.EventHandler(this.OnButtonPlayPauseClicked);
            // 
            // myBtnStop
            // 
            this.myBtnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.myBtnStop.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("myBtnStop.BackgroundImage")));
            this.myBtnStop.FlatAppearance.BorderSize = 0;
            this.myBtnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.myBtnStop.Location = new System.Drawing.Point(67, 4);
            this.myBtnStop.Name = "myBtnStop";
            this.myBtnStop.Size = new System.Drawing.Size(32, 32);
            this.myBtnStop.TabIndex = 2;
            this.myBtnStop.UseVisualStyleBackColor = true;
            this.myBtnStop.Click += new System.EventHandler(this.OnButtonStopClicked);
            // 
            // myLblMediaLength
            // 
            this.myLblMediaLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.myLblMediaLength.AutoSize = true;
            this.myLblMediaLength.Location = new System.Drawing.Point(230, 14);
            this.myLblMediaLength.Name = "myLblMediaLength";
            this.myLblMediaLength.Size = new System.Drawing.Size(49, 13);
            this.myLblMediaLength.TabIndex = 3;
            this.myLblMediaLength.Text = "00:00:00";
            // 
            // myLblVlcPosition
            // 
            this.myLblVlcPosition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.myLblVlcPosition.AutoSize = true;
            this.myLblVlcPosition.Location = new System.Drawing.Point(157, 14);
            this.myLblVlcPosition.Name = "myLblVlcPosition";
            this.myLblVlcPosition.Size = new System.Drawing.Size(49, 13);
            this.myLblVlcPosition.TabIndex = 4;
            this.myLblVlcPosition.Text = "00:00:00";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(212, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(12, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "/";
            // 
            // _panelControl
            // 
            this._panelControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this._panelControl.Controls.Add(this._panelQuickControls);
            this._panelControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._panelControl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this._panelControl.Location = new System.Drawing.Point(0, 462);
            this._panelControl.Name = "_panelControl";
            this._panelControl.Size = new System.Drawing.Size(989, 55);
            this._panelControl.TabIndex = 13;
            this._panelControl.Resize += new System.EventHandler(this._panelControl_Resize);
            // 
            // _panelQuickControls
            // 
            this._panelQuickControls.Controls.Add(this._panelSound);
            this._panelQuickControls.Controls.Add(this.myBtnPlayPause);
            this._panelQuickControls.Controls.Add(this.buttonMute);
            this._panelQuickControls.Controls.Add(this.myLblMediaLength);
            this._panelQuickControls.Controls.Add(this.buttonUp30);
            this._panelQuickControls.Controls.Add(this.label2);
            this._panelQuickControls.Controls.Add(this.buttonMinus10);
            this._panelQuickControls.Controls.Add(this.myLblVlcPosition);
            this._panelQuickControls.Controls.Add(this.myBtnStop);
            this._panelQuickControls.Location = new System.Drawing.Point(307, 13);
            this._panelQuickControls.Name = "_panelQuickControls";
            this._panelQuickControls.Size = new System.Drawing.Size(460, 39);
            this._panelQuickControls.TabIndex = 10;
            // 
            // _panelSound
            // 
            this._panelSound.BackColor = System.Drawing.Color.Transparent;
            this._panelSound.Dock = System.Windows.Forms.DockStyle.Right;
            this._panelSound.Location = new System.Drawing.Point(363, 0);
            this._panelSound.Name = "_panelSound";
            this._panelSound.Size = new System.Drawing.Size(97, 39);
            this._panelSound.TabIndex = 10;
            // 
            // buttonMute
            // 
            this.buttonMute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonMute.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonMute.BackgroundImage")));
            this.buttonMute.FlatAppearance.BorderSize = 0;
            this.buttonMute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonMute.Location = new System.Drawing.Point(338, 12);
            this.buttonMute.Name = "buttonMute";
            this.buttonMute.Size = new System.Drawing.Size(16, 16);
            this.buttonMute.TabIndex = 9;
            this.buttonMute.UseVisualStyleBackColor = true;
            this.buttonMute.Click += new System.EventHandler(this.buttonMute_Click);
            // 
            // buttonUp30
            // 
            this.buttonUp30.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonUp30.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonUp30.BackgroundImage")));
            this.buttonUp30.FlatAppearance.BorderSize = 0;
            this.buttonUp30.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonUp30.Location = new System.Drawing.Point(285, 1);
            this.buttonUp30.Name = "buttonUp30";
            this.buttonUp30.Size = new System.Drawing.Size(32, 32);
            this.buttonUp30.TabIndex = 8;
            this.buttonUp30.UseVisualStyleBackColor = true;
            this.buttonUp30.Click += new System.EventHandler(this.buttonUp30_Click);
            // 
            // buttonMinus10
            // 
            this.buttonMinus10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonMinus10.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonMinus10.BackgroundImage")));
            this.buttonMinus10.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonMinus10.FlatAppearance.BorderSize = 0;
            this.buttonMinus10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonMinus10.Location = new System.Drawing.Point(112, 2);
            this.buttonMinus10.Name = "buttonMinus10";
            this.buttonMinus10.Size = new System.Drawing.Size(32, 32);
            this.buttonMinus10.TabIndex = 7;
            this.buttonMinus10.UseVisualStyleBackColor = true;
            this.buttonMinus10.Click += new System.EventHandler(this.buttonMinus10_Click);
            // 
            // _textBoxSubtitle
            // 
            this._textBoxSubtitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this._textBoxSubtitle.Location = new System.Drawing.Point(0, 0);
            this._textBoxSubtitle.Name = "_textBoxSubtitle";
            this._textBoxSubtitle.ScrollBarsEnabled = false;
            this._textBoxSubtitle.Size = new System.Drawing.Size(612, 72);
            this._textBoxSubtitle.TabIndex = 0;
            // 
            // _subtitlesUserControl
            // 
            this._subtitlesUserControl.BackColor = System.Drawing.Color.Transparent;
            this._subtitlesUserControl.Controls.Add(this._textBoxSubtitle);
            this._subtitlesUserControl.Location = new System.Drawing.Point(194, 384);
            this._subtitlesUserControl.Name = "_subtitlesUserControl";
            this._subtitlesUserControl.Size = new System.Drawing.Size(612, 72);
            this._subtitlesUserControl.TabIndex = 0;
            // 
            // VideoPlayer
            // 
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this._subtitlesUserControl);
            this.Controls.Add(this._panelControl);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Name = "VideoPlayer";
            this.Size = new System.Drawing.Size(989, 517);
            this.SizeChanged += new System.EventHandler(this.VideoPlayer_SizeChanged);
            this._panelControl.ResumeLayout(false);
            this._panelQuickControls.ResumeLayout(false);
            this._panelQuickControls.PerformLayout();
            this._subtitlesUserControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        private void SetSubtitle(TimeSpan now)
        {
            string text;
            try
            {
                if (_sutbtitles != null)
                { 
                    text = _sutbtitles.GetText(now);
                    _subText = text;
                    _subtitlesRows = Regex.Split(text, "</br>").Length - 1;
                    _subtitlesUserControl.Invoke(DisplaySubtitlePanel);
                }
                else
                {
                    _subtitlesUserControl.Invoke(HideSubtitlePanel);
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
        }
        private void ShowSubtitle()
        {
            //int top = _panelControl.Top - _subtitlesUserControl.Height - 5;
            //if (top > this.Height - _subtitlesUserControl.Height - _minSubTop) top = this.Height - _subtitlesUserControl.Height - _minSubTop;

            _subtitlesUserControl.Height = _subtitlesRows * 24;
            _subtitlesUserControl.Top = this.Height - _minSubTop;
            _textBoxSubtitle.DocumentText = _subText;
            _subtitlesUserControl.Visible = true;
        }
        private void HideSubtitle()
        {
            _subtitlesUserControl.Visible = false;
        }
        #endregion

        #region Event
        private void OnVlcControlNeedLibDirectory(object sender, VlcLibDirectoryNeededEventArgs e)
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            if (currentDirectory == null)
                return;
            if (AssemblyName.GetAssemblyName(currentAssembly.Location).ProcessorArchitecture == ProcessorArchitecture.X86)
                e.VlcLibDirectory = new DirectoryInfo(Path.Combine(currentDirectory, @"Resources\x86\"));
            else
                e.VlcLibDirectory = new DirectoryInfo(Path.Combine(currentDirectory, @"Resources\x64\"));

            if (!e.VlcLibDirectory.Exists)
            {
                var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
                folderBrowserDialog.Description = "Select Vlc libraries folder.";
                folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
                folderBrowserDialog.ShowNewFolderButton = true;
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    e.VlcLibDirectory = new DirectoryInfo(folderBrowserDialog.SelectedPath);
                }
            }
        }
        private void OnButtonPlayPauseClicked(object sender, EventArgs e)
        {
            if (_vlcControl.IsPlaying)
            {
                Pause();
            }
            else
            {
                Play();
            }
        }
        private void OnButtonStopClicked(object sender, EventArgs e)
        {
            Stop();
            myBtnPlayPause.Focus();
        }
        private void OnVlcMediaLengthChanged(object sender, Vlc.DotNet.Core.VlcMediaPlayerLengthChangedEventArgs e)
        {
            myLblMediaLength.InvokeIfRequired(l => l.Text = new DateTime(new TimeSpan((long)e.NewLength).Ticks).ToString("T"));
            _trackBar.InvokeIfRequired(l => l.Maximum = ((int)(e.NewLength / 10000)));
        }
        private void OnVlcPositionChanged(object sender, Vlc.DotNet.Core.VlcMediaPlayerPositionChangedEventArgs e)
        {
            var position = _vlcControl.GetCurrentMedia().Duration.Ticks * e.NewPosition;
            myLblVlcPosition.InvokeIfRequired(l => l.Text = new DateTime((long)position).ToString("T"));
            if (!_mouseDown) _trackBar.InvokeIfRequired(l => l.Value = (int)_vlcControl.Time);
            _trackBarSound.Value = _vlcControl.Audio.Volume;
            SetSubtitle(new TimeSpan((long)position));
        }
        private void _trackBar_MouseDown(object sender, MouseEventArgs e)
        {
            _lastFrameMove = DateTime.Now;
            _mouseDown = true;
            _vlcControl.Time = _trackBar.Value;
            myBtnPlayPause.Focus();
        }
        private void _trackBar_MouseUp(object sender, MouseEventArgs e)
        {
            _lastFrameMove = DateTime.Now;
            _mouseDown = false;
            _vlcControl.Time = _trackBar.Value;
            myBtnPlayPause.Focus();
        }
        private void _trackBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseDown && _lastFrameMove < DateTime.Now - new TimeSpan(10000))
            {
                _trackBar.ToolTipActive = true;
                _trackBar.ToolTipText = new DateTime((long)_trackBar.Value * 10000).ToString("T");
                _vlcControl.Time = _trackBar.Value;
            }
            else
            {
                _trackBar.ToolTipActive = false;
            }
            myBtnPlayPause.Focus();
        }
        private void buttonMinus10_Click(object sender, EventArgs e)
        {
            _vlcControl.Time = _vlcControl.Time - 10000;
            myBtnPlayPause.Focus();
        }
        private void buttonUp30_Click(object sender, EventArgs e)
        {
            _vlcControl.Time = _vlcControl.Time + 30000;
            myBtnPlayPause.Focus();
        }
        private void buttonMute_Click(object sender, EventArgs e)
        {
            _vlcControl.Audio.ToggleMute();
            if (_vlcControl.Audio.IsMute) this.buttonMute.BackgroundImage = Tools4Libraries.Resources.ResourceIconSet16Default.sound_mute;
            else this.buttonMute.BackgroundImage = Tools4Libraries.Resources.ResourceIconSet16Default.sound_none;
            myBtnPlayPause.Focus();
        }
        private void _panelControl_Resize(object sender, EventArgs e)
        {
            _panelQuickControls.Left = (_panelControl.Width / 2) - (_panelQuickControls.Width / 2);
            myBtnPlayPause.Focus();
        }
        private void _showPanel_Tick(object sender, EventArgs e)
        {
            if (_panelControl.Height >= 55)
            {
                _showPanel.Stop();
                _showing = false;
            }
            else if (!_hidding)
            {
                _showing = true;
                _panelControl.Height += 1;
            }
            _quickAccessHidden = false;
        }
        private void _hidePanel_Tick(object sender, EventArgs e)
        {
            if (_panelControl.Height <= 0)
            {
                _hidePanel.Stop();
                _hidding = false;
            }
            else if (!_showing)
            {
                _hidding = true;
                _panelControl.Height -= 1;
            }
            _quickAccessHidden = true;
        }
        private void _trackBarSound_ValueChanged(object sender, SliderTrackBarValueChangedEventArgs e)
        {
            _vlcControl.Audio.Volume = _trackBarSound.Value;
        }
        private void KeyPressEvent(object sender, KeyPressEventArgs e)
        {
            KeyEvent(e.KeyChar);
        }
        private void MouseMoveEvent(object sender, MouseEventArgs e)
        {
            if (_mousePosition == null)
            {
                _lastTimeMouseMove = DateTime.Now;
                _mousePosition = new Point(e.X, e.Y);
                ShowQuickControl();
            }
            else
            {
                if (e.X != _mousePosition.X || e.Y != _mousePosition.Y)
                {
                    _lastTimeMouseMove = DateTime.Now;
                    _mousePosition.X = e.X;
                    _mousePosition.Y = e.Y;
                }
            }
        }
        private void _detectMouseMovement_Tick(object sender, EventArgs e)
        {
            if (_fullScreen && (Cursor.Position.X <= this.Top ||
                Cursor.Position.Y <= this.Left ||
                Cursor.Position.X >= this.Left + this.Width - 10 ||
                Cursor.Position.Y >= this.Top + this.Height - 10))
            {
                HideQuickControl();
            }
            else
            {
                ShowQuickControl();
            }
        }
        private void VideoPlayer_SizeChanged(object sender, EventArgs e)
        {
            _subtitlesUserControl.Left = (_vlcControl.Width / 2) - (_subtitlesUserControl.Width / 2);
            _subtitlesUserControl.Top = _panelControl.Top - _subtitlesUserControl.Height - 5;
        }

        private void VideoPlayer_Resize(object sender, EventArgs e)
        {
            //int top = _panelControl.Top - _subtitlesUserControl.Height - 5;
            //if (top > this.Height - _subtitlesUserControl.Height - _minSubTop) top = this.Height - _subtitlesUserControl.Height - _minSubTop;

            _subtitlesUserControl.Left = (_vlcControl.Width / 2) - (_subtitlesUserControl.Width / 2);
            _subtitlesUserControl.Top = this.Height - _minSubTop;
        }
        #endregion
    }
}
