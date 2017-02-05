// LOG 01 - 7
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Vlc.DotNet.Forms;
using Vlc.DotNet.Core;
using Tools4Libraries.Slider;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Tools4Libraries;
using System.Configuration;
using OSDBnet;

namespace Droid_video
{
    public delegate void VideoPlayerEventHandler();
    public class VideoPlayer : UserControl
    {
        #region Attributes
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private System.ComponentModel.IContainer components = null;
        private System.ComponentModel.ComponentResourceManager resources;

        private Vlc.DotNet.Forms.VlcControl _vlcControl;
        private System.Windows.Forms.Button _myBtnPlayPause;
        private System.Windows.Forms.Button myBtnStop;
        private System.Windows.Forms.Label myLblMediaRest;
        private System.Windows.Forms.Label myLblVlcPosition;
        private Tools4Libraries.Slider.SliderTrackBar _trackBar;
        private Tools4Libraries.Slider.SliderTrackBar _trackBarSound;
        private Panel _panelControl;
        private SaveFileDialog saveFileDialog1;
        private bool _openned;
        private Button buttonUp30;
        private Button buttonMinus10;
        private bool _mouseDown;
        private Button _buttonMute;
        private Panel _panelQuickControls;
        private TransparentPanel _panelMouseControl;
        private Timer _hidePanel;
        private Timer _showPanel;
        private Timer _detectMouseMovement;
        private bool _hidding;
        private bool _showing;
        private Point _mousePosition;
        private DateTime _lastTimeMouseMove;
        private Panel _panelSound;
        private bool _fullScreen;
        private Interface_vdo _intVdo;
        private const string _vlcCommandQuiet = "--quiet";
        private const string _vlcCommandTransform = "--video-filter=transform";
        private const string _vlcCommandSubtitle = "--sub-file=";
        private string _vlcCommandTransformOrientation;

        public event VideoPlayerEventHandler FullScreenRequested;
        public event VideoPlayerEventHandler FullScreenExit;
        private DateTime _lastFrameMove;
        #endregion

        #region Properties
        public bool FullScreen
        {
            get { return _fullScreen; }
            set { _fullScreen = value; }
        }
        public bool IsPlaying
        {
            get
            {
                return _vlcControl.IsPlaying;
            }
        }
        public SliderTrackBar TrackBarSound
        {
            get { return _trackBarSound; }
        }
        #endregion

        #region Constructor
        public VideoPlayer(Interface_vdo intvdo)
        {
            _fullScreen = false;
            _intVdo = intvdo;
            InitializeComponent();
            Init();
        }
        #endregion

        #region Methods public
        public void OpenFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Video Files (.avi .mkv .mp4 .mov)|*.avi;*.mkv;*.mp4;*.mov|All Files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Open(ofd.FileName);
            }
        }
        public void OpenFile(string path)
        {
            Open(path);
        }
        public void KeyEvent(char? key)
        {
            switch (key)
            {
                case null:
                    if (_intVdo.CurrentVideo != null && _fullScreen == false) FullScreenRequested();
                    break;
                //case '0':
                //    Pause();
                //    break;
                case (char)Keys.Escape:
                    if (_intVdo.CurrentVideo != null && _fullScreen == true) FullScreenExit();
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
            if (_intVdo.CurrentVideo != null && !string.IsNullOrEmpty(_intVdo.CurrentVideo.Path))
            {
                if (!_openned)
                {
                    _trackBar.Value = 0;
                    _vlcControl.Play(new Uri(_intVdo.CurrentVideo.Path));
                    _openned = true;
                }
                else
                {
                    Pause();
                }
                _myBtnPlayPause.BackgroundImage = Tools4Libraries.Resources.ResourceIconSet32Default.control_pause;
                _trackBar.Enabled = true;
            }
        }
        public void Stop()
        {
            _vlcControl.Stop();
            _myBtnPlayPause.BackgroundImage = Tools4Libraries.Resources.ResourceIconSet32Default.control_play;
            _openned = false;
            _trackBar.Value = 0;
            _vlcControl.Time = 0;
            _intVdo.CurrentVideo = null;
            _intVdo.Tsm.UpdateVideoDetails();
        }
        public void Pause()
        {
            _vlcControl.Pause();
            _myBtnPlayPause.BackgroundImage = Tools4Libraries.Resources.ResourceIconSet32Default.control_play;
        }
        public void HideQuickControl()
        {
            _hidePanel.Start();
        }
        public void ShowQuickControl()
        {
            _showPanel.Start();
        }
        public void LoadPosition()
        {
            try
            {
                long time = _intVdo.GetMovieAdvancement(_intVdo.CurrentVideo.Path);
                if (time < _vlcControl.Length && time > 200)
                {
                    Pause();
                    _vlcControl.SuspendLayout();
                    _vlcControl.Time = time;
                    _vlcControl.ResumeLayout();
                    Pause();
                }
            }
            catch (Exception exp)
            {
                Log.Write("[ ERR : 0123 ] Cannot load the position of the movie \n\n" + exp.Message);
            }
            _myBtnPlayPause.BackgroundImage = _vlcControl.IsPlaying ? Tools4Libraries.Resources.ResourceIconSet32Default.control_pause : Tools4Libraries.Resources.ResourceIconSet32Default.control_play;
        }
        public void Rotation(int rotation)
        {
            string rotationCommand = string.Empty;
            switch (rotation)
            {
                case 0:
                    _vlcCommandTransformOrientation = string.Empty;
                    break;
                case 90:
                    _vlcCommandTransformOrientation = "--transform-type=90";
                    break;
                case 180:
                    _vlcCommandTransformOrientation = "--transform-type=180";
                    break;
                case 270:
                    _vlcCommandTransformOrientation = "--transform-type=270";
                    break;
                default:
                    _vlcCommandTransformOrientation = string.Empty;
                    break;
            }
            Pause();
            ((System.ComponentModel.ISupportInitialize)(this._vlcControl)).BeginInit();
            InitVlcControl();
            ((System.ComponentModel.ISupportInitialize)(this._vlcControl)).EndInit();

            _vlcControl.Play(new Uri(_intVdo.CurrentVideo.Path));
            _vlcControl.Time = _intVdo.CurrentVideo.Time;
            Pause();
        }
        #endregion

        #region Methods protected
        protected override void Dispose(bool disposing)
        {
            _intVdo.Close();

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Methods private
        private void Open(string path)
        {
            _intVdo.CurrentVideo = new Video();
            _intVdo.CurrentVideo.Path = path;
            _intVdo.CurrentVideo.SubtitleResearchCompleted += CurrentVideo_SubtitleResearchCompleted;
            _intVdo.CurrentVideo.SubtitleChanged += CurrentVideo_SubtitleChanged;

            _intVdo.LoadDirectoryFiles();
            _intVdo.CurrentDirectory = Path.GetDirectoryName(path);

            _openned = false;
            _trackBar.Enabled = true;

            Play();
        }
        private void Init()
        {
            _openned = false;
            _lastFrameMove = DateTime.MinValue;
            _hidding = false;
            _showing = false;

            InitPanelMouseControl();
            InitVlcControl();
            InitTrackBar();
            InitTrackBarSound();
            InitTimers();
            InitEvent();
        }
        private void InitVlcControl()
        {
            try
            {
                DisposeVlcControler();
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
                this._vlcControl.Text = "vlcRincewindControl";
                this._vlcControl.VlcLibDirectory = null;
                GetVlcLibraries();
                this._vlcControl.VlcLibDirectoryNeeded += new System.EventHandler<Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs>(this.OnVlcControlNeedLibDirectory);
                this._vlcControl.LengthChanged += new System.EventHandler<Vlc.DotNet.Core.VlcMediaPlayerLengthChangedEventArgs>(this.OnVlcMediaLengthChanged);
                this._vlcControl.PositionChanged += new System.EventHandler<Vlc.DotNet.Core.VlcMediaPlayerPositionChangedEventArgs>(this.OnVlcPositionChanged);
                this._vlcControl.Stopped += _vlcControl_Stopped;
                this._vlcControl.ImeMode = System.Windows.Forms.ImeMode.NoControl;
                this._vlcControl.Location = new System.Drawing.Point(0, 0);
                this._vlcControl.TabIndex = 99;
                this._vlcControl.Size = new System.Drawing.Size(0, 0);
                this._vlcControl.Rate = 0;
                this._vlcControl.TabStop = false;
                if (!string.IsNullOrEmpty(_vlcCommandTransformOrientation) && _intVdo.CurrentVideo != null && _intVdo.CurrentVideo.CurrentSubtitlePath != null) this._vlcControl.VlcMediaplayerOptions = new[] { _vlcCommandQuiet, _vlcCommandTransform, _vlcCommandTransformOrientation, _vlcCommandSubtitle + _intVdo.CurrentVideo.Subtitle.SubtitleFileName };
                else if (!string.IsNullOrEmpty(_vlcCommandTransformOrientation)) this._vlcControl.VlcMediaplayerOptions = new[] { _vlcCommandQuiet, _vlcCommandTransform, _vlcCommandTransformOrientation };
                else if (_intVdo.CurrentVideo != null && _intVdo.CurrentVideo.CurrentSubtitlePath != null) this._vlcControl.VlcMediaplayerOptions = new[] { _vlcCommandQuiet, _vlcCommandSubtitle + _intVdo.CurrentVideo.CurrentSubtitlePath};
                else this._vlcControl.VlcMediaplayerOptions = new[] { _vlcCommandQuiet};
                ((System.ComponentModel.ISupportInitialize)(this._vlcControl)).EndInit();
                this.Controls.Add(_vlcControl);
            }
            catch (Exception exp)
            {
                Log.Write("[ ERR : 0100 ] Cannot create vlc controler. \n" + exp.Message);
            }
        }
        private void InitTrackBar()
        {
            try
            {
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
            }
            catch (Exception exp)
            {
                Log.Write("[ ERR : 0101 ] Cannot create track bar. \n" + exp.Message);
            }
        }
        private void InitTrackBarSound()
        {
            try
            {
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

                try
                {
                    ConfigurationManager.RefreshSection("appSettings");
                    _trackBarSound.Value = Properties.Settings.Default.volume;
                }
                catch (Exception exp)
                {
                    _trackBarSound.Value = 50;
                    Log.Write("[ WRN : 0103 ] Cannot load volume default. \n" + exp.Message);

                }
            }
            catch (Exception exp)
            {
                Log.Write("[ ERR : 0102 ] Cannot create sound track bar. \n" + exp.Message);
            }
        }
        private void InitTimers()
        {
            try
            {
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
            }
            catch (Exception exp)
            {
                Log.Write("[ ERR : 0104 ] Cannot create timers. \n" + exp.Message);
            }
        }
        private void InitEvent()
        {
            try
            {
                this.MouseMove += MouseMoveEvent;
                this._panelControl.MouseMove += MouseMoveEvent;
                this._panelSound.MouseMove += MouseMoveEvent;
            }
            catch (Exception exp)
            {
                Log.Write("[ ERR : 0105 ] Cannot create events. \n" + exp.Message);
            }
        }
        private void InitPanelMouseControl()
        {
            _panelMouseControl = new TransparentPanel();
            _panelMouseControl.Top = 0;
            _panelMouseControl.Left = 0;
            _panelMouseControl.Width = this.Width;
            _panelMouseControl.Height = this.Height;
            _panelMouseControl.MouseClick += _panelMouseControl_MouseClick;
            _panelMouseControl.DoubleClick += _panelMouseControl_DoubleClick;
            _panelMouseControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Controls.Add(_panelMouseControl);
        }
        private void InitializeComponent()
        {
            resources = new System.ComponentModel.ComponentResourceManager(typeof(VideoPlayer));
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this._panelControl = new System.Windows.Forms.Panel();
            this._panelQuickControls = new System.Windows.Forms.Panel();
            this._panelSound = new System.Windows.Forms.Panel();
            this._myBtnPlayPause = new System.Windows.Forms.Button();
            this._buttonMute = new System.Windows.Forms.Button();
            this.buttonUp30 = new System.Windows.Forms.Button();
            this.buttonMinus10 = new System.Windows.Forms.Button();
            this.myBtnStop = new System.Windows.Forms.Button();
            this.myLblVlcPosition = new System.Windows.Forms.Label();
            this.myLblMediaRest = new System.Windows.Forms.Label();
            this._panelControl.SuspendLayout();
            this._panelQuickControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // _panelControl
            // 
            this._panelControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._panelControl.BackColor = System.Drawing.Color.Transparent;
            this._panelControl.Controls.Add(this._panelQuickControls);
            this._panelControl.Controls.Add(this.myLblVlcPosition);
            this._panelControl.Controls.Add(this.myLblMediaRest);
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
            this._panelQuickControls.Controls.Add(this._myBtnPlayPause);
            this._panelQuickControls.Controls.Add(this._buttonMute);
            this._panelQuickControls.Controls.Add(this.buttonUp30);
            this._panelQuickControls.Controls.Add(this.buttonMinus10);
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
            // myBtnPlayPause
            // 
            this._myBtnPlayPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._myBtnPlayPause.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("myBtnPlayPause.BackgroundImage")));
            this._myBtnPlayPause.FlatAppearance.BorderSize = 0;
            this._myBtnPlayPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._myBtnPlayPause.FlatAppearance.BorderColor = Color.Black;
            this._myBtnPlayPause.FlatAppearance.MouseDownBackColor = Color.Transparent;
            this._myBtnPlayPause.FlatAppearance.MouseOverBackColor = Color.Transparent;
            this._myBtnPlayPause.Location = new System.Drawing.Point(163, 4);
            this._myBtnPlayPause.Name = "myBtnPlayPause";
            this._myBtnPlayPause.Size = new System.Drawing.Size(32, 32);
            this._myBtnPlayPause.TabIndex = 1;
            this._myBtnPlayPause.UseVisualStyleBackColor = true;
            this._myBtnPlayPause.Click += new System.EventHandler(this.OnButtonPlayPauseClicked);
            this._myBtnPlayPause.KeyDown += _myBtnPlayPause_KeyDown;
            // 
            // buttonMute
            // 
            this._buttonMute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._buttonMute.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonMute.BackgroundImage")));
            this._buttonMute.FlatAppearance.BorderSize = 0;
            this._buttonMute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._buttonMute.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 255, 255, 255);
            this._buttonMute.FlatAppearance.MouseOverBackColor = Color.FromArgb(20, 255, 255, 255);
            this._buttonMute.Location = new System.Drawing.Point(338, 12);
            this._buttonMute.Name = "buttonMute";
            this._buttonMute.Size = new System.Drawing.Size(16, 16);
            this._buttonMute.TabIndex = 9;
            this._buttonMute.UseVisualStyleBackColor = true;
            this._buttonMute.Click += new System.EventHandler(this.buttonMute_Click);
            // 
            // buttonUp30
            // 
            this.buttonUp30.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonUp30.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonUp30.BackgroundImage")));
            this.buttonUp30.FlatAppearance.BorderSize = 0;
            this.buttonUp30.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonUp30.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 255, 255, 255);
            this.buttonUp30.FlatAppearance.MouseOverBackColor = Color.FromArgb(20, 255, 255, 255);
            this.buttonUp30.Location = new System.Drawing.Point(239, 3);
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
            this.buttonMinus10.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 255, 255, 255);
            this.buttonMinus10.FlatAppearance.MouseOverBackColor = Color.FromArgb(20, 255, 255, 255);
            this.buttonMinus10.Location = new System.Drawing.Point(125, 4);
            this.buttonMinus10.Name = "buttonMinus10";
            this.buttonMinus10.Size = new System.Drawing.Size(32, 32);
            this.buttonMinus10.TabIndex = 7;
            this.buttonMinus10.UseVisualStyleBackColor = true;
            this.buttonMinus10.Click += new System.EventHandler(this.buttonMinus10_Click);
            // 
            // myBtnStop
            // 
            this.myBtnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.myBtnStop.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("myBtnStop.BackgroundImage")));
            this.myBtnStop.FlatAppearance.BorderSize = 0;
            this.myBtnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.myBtnStop.FlatAppearance.MouseDownBackColor = Color.Transparent;
            this.myBtnStop.FlatAppearance.MouseOverBackColor = Color.Transparent;
            this.myBtnStop.Location = new System.Drawing.Point(201, 4);
            this.myBtnStop.Name = "myBtnStop";
            this.myBtnStop.Size = new System.Drawing.Size(32, 32);
            this.myBtnStop.TabIndex = 2;
            this.myBtnStop.UseVisualStyleBackColor = true;
            this.myBtnStop.Click += new System.EventHandler(this.OnButtonStopClicked);
            // 
            // myLblVlcPosition
            // 
            this.myLblVlcPosition.AutoSize = true;
            this.myLblVlcPosition.Location = new System.Drawing.Point(3, 17);
            this.myLblVlcPosition.Name = "myLblVlcPosition";
            this.myLblVlcPosition.Size = new System.Drawing.Size(49, 13);
            this.myLblVlcPosition.TabIndex = 4;
            this.myLblVlcPosition.Text = "00:00:00";
            // 
            // myLblMediaRest
            // 
            this.myLblMediaRest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.myLblMediaRest.AutoSize = true;
            this.myLblMediaRest.Location = new System.Drawing.Point(937, 17);
            this.myLblMediaRest.Name = "myLblMediaRest";
            this.myLblMediaRest.Size = new System.Drawing.Size(49, 13);
            this.myLblMediaRest.TabIndex = 3;
            this.myLblMediaRest.Text = "00:00:00";
            // 
            // VideoPlayer
            // 
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this._panelControl);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Name = "VideoPlayer";
            this.Size = new System.Drawing.Size(989, 517);
            this._panelControl.ResumeLayout(false);
            this._panelControl.PerformLayout();
            this._panelQuickControls.ResumeLayout(false);
            this.ResumeLayout(false);
            this.MouseWheel += VideoPlayer_MouseWheel;
        }
        private void DisposeVlcControler()
        {
            if (this._vlcControl != null)
            {
                try
                {
                    this._vlcControl.Dispose();
                    this.Controls.Remove(_vlcControl);
                    this._vlcControl = null;
                }
                catch (Exception exp)
                {
                    Tools4Libraries.Log.Write("[WRN: 0007] Exception in vlc controller disposing. \n" + exp.Message);
                }
            }
        }
        
        private void VolumeScroll(int delta)
        {
            delta = _trackBarSound.Value + (delta / 120);
            if (delta > _trackBarSound.Maximum) _trackBarSound.Value = _trackBarSound.Maximum;
            else if (delta < _trackBarSound.Minimum) _trackBarSound.Value = _trackBarSound.Minimum;
            else _trackBarSound.Value = delta;
        }
        private void GetVlcLibraries()
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            if (currentDirectory == null)
                return;

            if (AssemblyName.GetAssemblyName(currentAssembly.Location).ProcessorArchitecture == ProcessorArchitecture.X86)
                _vlcControl.VlcLibDirectory = new DirectoryInfo(Path.Combine(currentDirectory, @"Resources\x86\"));
            else
                _vlcControl.VlcLibDirectory = new DirectoryInfo(Path.Combine(currentDirectory, @"Resources\x64\"));

            if (!_vlcControl.VlcLibDirectory.Exists)
            {
                var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
                folderBrowserDialog.Description = "Select Vlc libraries folder.";
                folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
                folderBrowserDialog.ShowNewFolderButton = true;
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    _vlcControl.VlcLibDirectory = new DirectoryInfo(folderBrowserDialog.SelectedPath);
                }
            }
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
        }
        private void OnVlcMediaLengthChanged(object sender, Vlc.DotNet.Core.VlcMediaPlayerLengthChangedEventArgs e)
        {
            myLblMediaRest.InvokeIfRequired(l => l.Text = new DateTime(new TimeSpan((long)e.NewLength).Ticks).ToString("T"));
            _trackBar.InvokeIfRequired(l => l.Maximum = ((int)(e.NewLength / 10000)));
            _intVdo.CurrentVideo.Length = (long)e.NewLength;
        }
        private void OnVlcPositionChanged(object sender, Vlc.DotNet.Core.VlcMediaPlayerPositionChangedEventArgs e)
        {
            if (_vlcControl.GetCurrentMedia() != null)
            { 
                var time = _vlcControl.GetCurrentMedia().Duration.Ticks * e.NewPosition;
                myLblVlcPosition.InvokeIfRequired(l => l.Text = new DateTime((long)time).ToString("T"));
                if (!_mouseDown) _trackBar.InvokeIfRequired(l => l.Value = (int)_vlcControl.Time);
                _intVdo.CurrentVideo.Time = _vlcControl.Time;

                myLblMediaRest.InvokeIfRequired(l => l.Text = new DateTime(new TimeSpan(_intVdo.CurrentVideo.Length - (long)time).Ticks).ToString("T"));
                _vlcControl.Audio.Volume = _trackBarSound.Value;
            }
        }
        private void _trackBar_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                _lastFrameMove = DateTime.Now;
                _mouseDown = true;
                _vlcControl.Time = _trackBar.Value;
            }
            catch (Exception exp)
            {
                Tools4Libraries.Log.Write("[INF: 0001] Cannot move the movie position. \n" + exp.Message);
            }
        }
        private void _trackBar_MouseUp(object sender, MouseEventArgs e)
        {
            _lastFrameMove = DateTime.Now;
            _mouseDown = false;
            _vlcControl.Time = _trackBar.Value;
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
        }
        private void buttonMinus10_Click(object sender, EventArgs e)
        {
            _vlcControl.Time = _vlcControl.Time - 10000;
        }
        private void buttonUp30_Click(object sender, EventArgs e)
        {
            _vlcControl.Time = _vlcControl.Time + 30000;
        }
        private void buttonMute_Click(object sender, EventArgs e)
        {
            _vlcControl.Audio.ToggleMute();
            if (_vlcControl.Audio.IsMute) this._buttonMute.BackgroundImage = Tools4Libraries.Resources.ResourceIconSet16Default.sound_mute;
            else this._buttonMute.BackgroundImage = Tools4Libraries.Resources.ResourceIconSet16Default.sound_none;
        }
        private void _panelControl_Resize(object sender, EventArgs e)
        {
            _panelQuickControls.Left = (_panelControl.Width / 2) - (_panelQuickControls.Width / 2);
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
                _panelControl.Top -= 1;
            }
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
                _panelControl.Top += 1;
            }
        }
        private void _trackBarSound_ValueChanged(object sender, SliderTrackBarValueChangedEventArgs e)
        {
            _vlcControl.Audio.Volume = _trackBarSound.Value;
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
        private void VideoPlayer_MouseWheel(object sender, MouseEventArgs e)
        {
            VolumeScroll(e.Delta);
        }
        private void _vlcControl_Stopped(object sender, VlcMediaPlayerStoppedEventArgs e)
        {
            _intVdo.SaveUserParams();
            myLblMediaRest.Text = new TimeSpan(0).ToString("T");
            myLblVlcPosition.Text = new TimeSpan(0).ToString("T");
        }
        private void _myBtnPlayPause_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData.ToString() == Keys.Enter.ToString() + ", " + Keys.Alt.ToString())
            {
                KeyEvent(null);
            }
            else if (e.KeyCode != Keys.Space)
            {
                KeyEvent(Convert.ToChar(e.KeyValue));
            }
        }
        private void CurrentVideo_SubtitleResearchCompleted()
        {
            _intVdo.Tsm.UpdateVideoDetails();
        }
        private void _panelMouseControl_DoubleClick(object sender, EventArgs e)
        {
            if (_intVdo.CurrentVideo != null && _fullScreen == false)
            {
                Pause();
                FullScreenRequested();
            }
        }
        private void _panelMouseControl_MouseClick(object sender, MouseEventArgs e)
        {
            Pause();
        }
        private void CurrentVideo_SubtitleChanged()
        {
            ((System.ComponentModel.ISupportInitialize)(this._vlcControl)).BeginInit();
            InitVlcControl();
            _vlcControl.Play(new Uri(_intVdo.CurrentVideo.Path));
            _vlcControl.Time = _intVdo.CurrentVideo.Time;
            ((System.ComponentModel.ISupportInitialize)(this._vlcControl)).EndInit();
        }
        #endregion
    }
}
