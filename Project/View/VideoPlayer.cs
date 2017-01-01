using System;
using System.Drawing;
using System.Windows.Forms;
using QuartzTypeLib;
using Tools4Libraries;
using System.Runtime.InteropServices;
using System.Text;

namespace Droid_video
{

    public delegate void VideoPlayerEventHandler();
    public class VideoPlayer : UserControl
    {
        #region Attribute
        [DllImport("winmm.dll")]
        static extern Int32 mciSendString(String command, StringBuilder buffer, Int32 bufferSize, IntPtr hwndCallback);

        public event VideoPlayerEventHandler FullScreenRequested;
        public event VideoPlayerEventHandler FullScreenExit;

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Timer timer1;
        private System.ComponentModel.IContainer components;

        private const int WM_APP = 0x8000;
        private const int WM_GRAPHNOTIFY = WM_APP + 1;
        private const int EC_COMPLETE = 0x01;
        private const int WS_CHILD = 0x40000000;
        private const int WS_CLIPCHILDREN = 0x2000000;

        private FilgraphManager _objFilterGraph = null;
        private IBasicAudio _objBasicAudio = null;
        private IVideoWindow _objVideoWindow = null;
        private IMediaEvent _objMediaEvent = null;
        private IMediaEventEx _objMediaEventEx = null;
        private IMediaPosition _objMediaPosition = null;
        private IMediaControl _objMediaControl = null;
        private bool _isRunning = false;
        private bool _autoChange = false;
        private bool _fullscreen = false;
        private ScreenFormat _screenFormat = ScreenFormat.CLASSIC;
        
        private Panel panelSettings;
        private TrackBar trackBarDuration;
        private Panel panelUserCommand;
        private Button buttonPlayPause;
        private Button buttonStop;
        private Button buttonRewind;
        private Button buttonForward;
        private TrackBar trackBarVolume;
        private Button buttonMute;
        private Button buttonEject;
        private Button buttonFullScreen;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem screenFormatToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem169;
        private ToolStripMenuItem toolStripMenuItem34;
        private ToolStripMenuItem automatiqueToolStripMenuItem;
        private ToolStripMenuItem fullScreenToolStripMenuItem;
        private ToolStripMenuItem subtitleToolStripMenuItem;
        private ToolStripMenuItem englishToolStripMenuItem;
        private ToolStripMenuItem frenchToolStripMenuItem;
        private Panel panelVideo;
        private Label labelText;

        private bool _keyAltPress = false;
        #endregion

        #region Properties
        public ScreenFormat ScreenFrt
        {
            get { return _screenFormat; }
            set { _screenFormat = value; }
        }
        #endregion

        #region Enum
        public enum MediaStatus { None, Stopped, Paused, Running };
        public enum ScreenFormat { CLASSIC, QUARTERS, FULL };
        #endregion

        #region Constructor
        public VideoPlayer()
        {
            InitializeComponent();

            UpdateStatus();
            UpdateToolBar();

            this.Resize += VideoPlayer_Resize;
        }
        #endregion

        #region Designer
        /// <summary>
        /// Die verwendeten Ressourcen bereinigen.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            CleanUp();

            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VideoPlayer));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panelSettings = new System.Windows.Forms.Panel();
            this.panelUserCommand = new System.Windows.Forms.Panel();
            this.buttonFullScreen = new System.Windows.Forms.Button();
            this.buttonEject = new System.Windows.Forms.Button();
            this.buttonMute = new System.Windows.Forms.Button();
            this.trackBarVolume = new System.Windows.Forms.TrackBar();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonRewind = new System.Windows.Forms.Button();
            this.buttonForward = new System.Windows.Forms.Button();
            this.buttonPlayPause = new System.Windows.Forms.Button();
            this.labelText = new System.Windows.Forms.Label();
            this.trackBarDuration = new System.Windows.Forms.TrackBar();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.screenFormatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem169 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem34 = new System.Windows.Forms.ToolStripMenuItem();
            this.automatiqueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fullScreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.subtitleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.englishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.frenchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelVideo = new System.Windows.Forms.Panel();
            this.panelSettings.SuspendLayout();
            this.panelUserCommand.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarDuration)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "pause");
            this.imageList1.Images.SetKeyName(1, "run");
            this.imageList1.Images.SetKeyName(2, "fullscreen");
            this.imageList1.Images.SetKeyName(3, "partscreen");
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panelSettings
            // 
            this.panelSettings.BackColor = System.Drawing.Color.Black;
            this.panelSettings.Controls.Add(this.panelUserCommand);
            this.panelSettings.Controls.Add(this.trackBarDuration);
            this.panelSettings.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelSettings.Location = new System.Drawing.Point(0, 154);
            this.panelSettings.MaximumSize = new System.Drawing.Size(0, 67);
            this.panelSettings.MinimumSize = new System.Drawing.Size(0, 67);
            this.panelSettings.Name = "panelSettings";
            this.panelSettings.Size = new System.Drawing.Size(794, 67);
            this.panelSettings.TabIndex = 4;
            this.panelSettings.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ComponentDoubleClick);
            // 
            // panelUserCommand
            // 
            this.panelUserCommand.Controls.Add(this.buttonFullScreen);
            this.panelUserCommand.Controls.Add(this.buttonEject);
            this.panelUserCommand.Controls.Add(this.buttonMute);
            this.panelUserCommand.Controls.Add(this.trackBarVolume);
            this.panelUserCommand.Controls.Add(this.buttonStop);
            this.panelUserCommand.Controls.Add(this.buttonRewind);
            this.panelUserCommand.Controls.Add(this.buttonForward);
            this.panelUserCommand.Controls.Add(this.buttonPlayPause);
            this.panelUserCommand.Controls.Add(this.labelText);
            this.panelUserCommand.Location = new System.Drawing.Point(132, 28);
            this.panelUserCommand.Name = "panelUserCommand";
            this.panelUserCommand.Size = new System.Drawing.Size(494, 36);
            this.panelUserCommand.TabIndex = 0;
            // 
            // buttonFullScreen
            // 
            this.buttonFullScreen.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonFullScreen.BackgroundImage")));
            this.buttonFullScreen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonFullScreen.FlatAppearance.BorderSize = 0;
            this.buttonFullScreen.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.buttonFullScreen.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonFullScreen.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonFullScreen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFullScreen.Location = new System.Drawing.Point(449, 6);
            this.buttonFullScreen.Name = "buttonFullScreen";
            this.buttonFullScreen.Size = new System.Drawing.Size(24, 24);
            this.buttonFullScreen.TabIndex = 7;
            this.buttonFullScreen.UseVisualStyleBackColor = true;
            this.buttonFullScreen.Click += new System.EventHandler(this.buttonFullScreen_Click);
            this.buttonFullScreen.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.component_KeyPress);
            // 
            // buttonEject
            // 
            this.buttonEject.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonEject.BackgroundImage")));
            this.buttonEject.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonEject.FlatAppearance.BorderSize = 0;
            this.buttonEject.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.buttonEject.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonEject.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonEject.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonEject.Location = new System.Drawing.Point(165, 11);
            this.buttonEject.Name = "buttonEject";
            this.buttonEject.Size = new System.Drawing.Size(16, 16);
            this.buttonEject.TabIndex = 6;
            this.buttonEject.UseVisualStyleBackColor = true;
            this.buttonEject.Click += new System.EventHandler(this.buttonEject_Click);
            this.buttonEject.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.component_KeyPress);
            // 
            // buttonMute
            // 
            this.buttonMute.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonMute.BackgroundImage")));
            this.buttonMute.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonMute.FlatAppearance.BorderSize = 0;
            this.buttonMute.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.buttonMute.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonMute.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonMute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonMute.Location = new System.Drawing.Point(315, 11);
            this.buttonMute.Name = "buttonMute";
            this.buttonMute.Size = new System.Drawing.Size(16, 16);
            this.buttonMute.TabIndex = 5;
            this.buttonMute.UseVisualStyleBackColor = true;
            this.buttonMute.Click += new System.EventHandler(this.buttonMute_Click);
            this.buttonMute.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.component_KeyPress);
            // 
            // trackBarVolume
            // 
            this.trackBarVolume.AutoSize = false;
            this.trackBarVolume.Location = new System.Drawing.Point(337, 10);
            this.trackBarVolume.Name = "trackBarVolume";
            this.trackBarVolume.Size = new System.Drawing.Size(106, 19);
            this.trackBarVolume.TabIndex = 1;
            this.trackBarVolume.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarVolume.ValueChanged += new System.EventHandler(this.trackBarVolume_ValueChanged);
            this.trackBarVolume.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.component_KeyPress);
            // 
            // buttonStop
            // 
            this.buttonStop.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonStop.BackgroundImage")));
            this.buttonStop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonStop.FlatAppearance.BorderSize = 0;
            this.buttonStop.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.buttonStop.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonStop.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonStop.Location = new System.Drawing.Point(187, 7);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(24, 24);
            this.buttonStop.TabIndex = 4;
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            this.buttonStop.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.component_KeyPress);
            // 
            // buttonRewind
            // 
            this.buttonRewind.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonRewind.BackgroundImage")));
            this.buttonRewind.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonRewind.FlatAppearance.BorderSize = 0;
            this.buttonRewind.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.buttonRewind.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonRewind.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonRewind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRewind.Location = new System.Drawing.Point(217, 7);
            this.buttonRewind.Name = "buttonRewind";
            this.buttonRewind.Size = new System.Drawing.Size(24, 24);
            this.buttonRewind.TabIndex = 3;
            this.buttonRewind.UseVisualStyleBackColor = true;
            this.buttonRewind.Click += new System.EventHandler(this.buttonRewind_Click);
            this.buttonRewind.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.component_KeyPress);
            // 
            // buttonForward
            // 
            this.buttonForward.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonForward.BackgroundImage")));
            this.buttonForward.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonForward.FlatAppearance.BorderSize = 0;
            this.buttonForward.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.buttonForward.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonForward.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonForward.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonForward.Location = new System.Drawing.Point(285, 7);
            this.buttonForward.Name = "buttonForward";
            this.buttonForward.Size = new System.Drawing.Size(24, 24);
            this.buttonForward.TabIndex = 2;
            this.buttonForward.UseVisualStyleBackColor = true;
            this.buttonForward.Click += new System.EventHandler(this.buttonForward_Click);
            this.buttonForward.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.component_KeyPress);
            // 
            // buttonPlayPause
            // 
            this.buttonPlayPause.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonPlayPause.BackgroundImage")));
            this.buttonPlayPause.FlatAppearance.BorderSize = 0;
            this.buttonPlayPause.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonPlayPause.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonPlayPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPlayPause.Location = new System.Drawing.Point(247, 3);
            this.buttonPlayPause.Name = "buttonPlayPause";
            this.buttonPlayPause.Size = new System.Drawing.Size(32, 32);
            this.buttonPlayPause.TabIndex = 1;
            this.buttonPlayPause.UseVisualStyleBackColor = true;
            this.buttonPlayPause.Click += new System.EventHandler(this.buttonPlayPause_Click);
            this.buttonPlayPause.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.component_KeyPress);
            // 
            // labelText
            // 
            this.labelText.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelText.ForeColor = System.Drawing.Color.Silver;
            this.labelText.Location = new System.Drawing.Point(11, 7);
            this.labelText.Name = "labelText";
            this.labelText.Size = new System.Drawing.Size(170, 22);
            this.labelText.TabIndex = 1;
            this.labelText.Text = "00:00:00  -  00:00:00";
            this.labelText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelText.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ComponentDoubleClick);
            // 
            // trackBarDuration
            // 
            this.trackBarDuration.AutoSize = false;
            this.trackBarDuration.Cursor = System.Windows.Forms.Cursors.Default;
            this.trackBarDuration.Dock = System.Windows.Forms.DockStyle.Top;
            this.trackBarDuration.Location = new System.Drawing.Point(0, 0);
            this.trackBarDuration.Margin = new System.Windows.Forms.Padding(0);
            this.trackBarDuration.Maximum = 0;
            this.trackBarDuration.Name = "trackBarDuration";
            this.trackBarDuration.Size = new System.Drawing.Size(794, 25);
            this.trackBarDuration.TabIndex = 0;
            this.trackBarDuration.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarDuration.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.component_KeyPress);
            this.trackBarDuration.MouseDown += new System.Windows.Forms.MouseEventHandler(this.trackBarDuration_MouseDown);
            this.trackBarDuration.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBarDuration_MouseUp);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.screenFormatToolStripMenuItem,
            this.subtitleToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(149, 48);
            // 
            // screenFormatToolStripMenuItem
            // 
            this.screenFormatToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem169,
            this.toolStripMenuItem34,
            this.automatiqueToolStripMenuItem,
            this.fullScreenToolStripMenuItem});
            this.screenFormatToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("screenFormatToolStripMenuItem.Image")));
            this.screenFormatToolStripMenuItem.Name = "screenFormatToolStripMenuItem";
            this.screenFormatToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.screenFormatToolStripMenuItem.Text = "Screen format";
            // 
            // toolStripMenuItem169
            // 
            this.toolStripMenuItem169.Name = "toolStripMenuItem169";
            this.toolStripMenuItem169.Size = new System.Drawing.Size(142, 22);
            this.toolStripMenuItem169.Text = "16 / 9";
            this.toolStripMenuItem169.Click += new System.EventHandler(this.toolStripMenuItem169_Click);
            // 
            // toolStripMenuItem34
            // 
            this.toolStripMenuItem34.Name = "toolStripMenuItem34";
            this.toolStripMenuItem34.Size = new System.Drawing.Size(142, 22);
            this.toolStripMenuItem34.Text = "3 / 4";
            this.toolStripMenuItem34.Click += new System.EventHandler(this.toolStripMenuItem34_Click);
            // 
            // automatiqueToolStripMenuItem
            // 
            this.automatiqueToolStripMenuItem.Name = "automatiqueToolStripMenuItem";
            this.automatiqueToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.automatiqueToolStripMenuItem.Text = "automatique";
            this.automatiqueToolStripMenuItem.Click += new System.EventHandler(this.automatiqueToolStripMenuItem_Click);
            // 
            // fullScreenToolStripMenuItem
            // 
            this.fullScreenToolStripMenuItem.Name = "fullScreenToolStripMenuItem";
            this.fullScreenToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.fullScreenToolStripMenuItem.Text = "full screen";
            this.fullScreenToolStripMenuItem.Click += new System.EventHandler(this.fullScreenToolStripMenuItem_Click);
            // 
            // subtitleToolStripMenuItem
            // 
            this.subtitleToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.englishToolStripMenuItem,
            this.frenchToolStripMenuItem});
            this.subtitleToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("subtitleToolStripMenuItem.Image")));
            this.subtitleToolStripMenuItem.Name = "subtitleToolStripMenuItem";
            this.subtitleToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.subtitleToolStripMenuItem.Text = "Subtitle";
            // 
            // englishToolStripMenuItem
            // 
            this.englishToolStripMenuItem.Enabled = false;
            this.englishToolStripMenuItem.Name = "englishToolStripMenuItem";
            this.englishToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.englishToolStripMenuItem.Text = "English";
            // 
            // frenchToolStripMenuItem
            // 
            this.frenchToolStripMenuItem.Enabled = false;
            this.frenchToolStripMenuItem.Name = "frenchToolStripMenuItem";
            this.frenchToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.frenchToolStripMenuItem.Text = "French";
            // 
            // panelVideo
            // 
            this.panelVideo.BackColor = System.Drawing.Color.Black;
            this.panelVideo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panelVideo.BackgroundImage")));
            this.panelVideo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.panelVideo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelVideo.Location = new System.Drawing.Point(0, 0);
            this.panelVideo.Name = "panelVideo";
            this.panelVideo.Size = new System.Drawing.Size(794, 154);
            this.panelVideo.TabIndex = 5;
            this.panelVideo.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ComponentDoubleClick);
            // 
            // VideoPlayer
            // 
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.panelVideo);
            this.Controls.Add(this.panelSettings);
            this.Name = "VideoPlayer";
            this.Size = new System.Drawing.Size(794, 221);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.panelSettings.ResumeLayout(false);
            this.panelUserCommand.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarDuration)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion
        #endregion

        #region Methods public
        public void KeyEvent(int keyCode)
        {
            try
            {
                switch (keyCode)
                {
                    case 27:
                        OnFullSreenExit();
                        break;
                    case 13:
                        OnFullScreenRequested();
                        break;
                    case 32:
                        PlayPause();
                        break;
                }
            }
            catch (Exception exp8402)
            {
                Log.write("[ WRN : 8402 ] Cannot translate the keycommand\n" + exp8402.Message);
            }
        }
        public void PlayPause()
        {
            if (_objMediaControl != null)
            {
                if (_isRunning)
                {
                    _objMediaControl.Pause();
                    buttonPlayPause.BackgroundImage = imageList1.Images[imageList1.Images.IndexOfKey("run")];
                }
                else
                {
                    _objMediaControl.Run();
                    buttonPlayPause.BackgroundImage = imageList1.Images[imageList1.Images.IndexOfKey("pause")];
                }

                _isRunning = !_isRunning;
            }
        }
        public void OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Media Files|*.mpg;*.avi;*.wma;*.mov;*.wav;*.mp2;*.mp3|All Files|*.*";

            if (DialogResult.OK == openFileDialog.ShowDialog())
            {
                PlayVideo(openFileDialog.FileName);
            }
        }
        public void FullScreen()
        {   
            if (_objVideoWindow != null) _objVideoWindow.FullScreenMode = EC_COMPLETE; 
        }
        public void RestaureSize()
        {
            if (_objVideoWindow != null) _objVideoWindow.FullScreenMode = WS_CLIPCHILDREN;
        }
        public void Stop()
        {
            if (_objMediaControl != null) _objMediaControl.Stop();
        }
        #endregion

        #region Methods private
        private void PlayVideo(string videoPath)
        {
            if (videoPath.ToLower().EndsWith("mkv"))
            {
                MessageBox.Show("I'm so sorry, I cannot read that excelent movie because of it's MKV format :(", "Too good for me", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            CleanUp();
            
            _objFilterGraph = new FilgraphManager();
            _objFilterGraph.RenderFile(videoPath);
            
            _objBasicAudio = _objFilterGraph as IBasicAudio;
            trackBarVolume.Value = _objBasicAudio.Volume;

            BuildRectangle();

            _objMediaEvent = _objFilterGraph as IMediaEvent;
            
            _objMediaEventEx = _objFilterGraph as IMediaEventEx;
            _objMediaEventEx.SetNotifyWindow((int)this.Handle, WM_GRAPHNOTIFY, 0);
            
            _objMediaPosition = _objFilterGraph as IMediaPosition;
            
            _objMediaControl = _objFilterGraph as IMediaControl;

            _objMediaControl.Run();
            _isRunning = true;

            UpdateStatus();
            UpdateToolBar();

            buttonPlayPause.BackgroundImage = imageList1.Images[imageList1.Images.IndexOfKey("pause")];
        }
        private void BuildRectangle()
        {
            try
            {
                _objVideoWindow = _objFilterGraph as IVideoWindow;
                _objVideoWindow.Owner = (int)panelVideo.Handle;
                _objVideoWindow.WindowStyle = WS_CHILD | WS_CLIPCHILDREN;
                _objVideoWindow.SetWindowPosition(panelVideo.ClientRectangle.Left,
                    panelVideo.ClientRectangle.Top,
                    panelVideo.ClientRectangle.Width,
                    panelVideo.ClientRectangle.Height);
                
                panelSettings_Resize(null, null);
            }
            catch (Exception)
            {
                _objVideoWindow = null;
            }
        }

        private void UpdateStatus()
        {
            string displayText = string.Empty;
            UpdateTrackBar();

            if (_objMediaPosition != null)
            {
                int s = (int)_objMediaPosition.CurrentPosition;
                int h = s / 3600;
                int m = (s - (h * 3600)) / 60;
                s = s - (h * 3600 + m * 60);

                displayText += String.Format("{0:D2}:{1:D2}:{2:D2}", h, m, s);

                s = (int)_objMediaPosition.Duration;
                h = s / 3600;
                m = (s - (h * 3600)) / 60;
                s = s - (h * 3600 + m * 60);

                displayText += String.Format(" - {0:D2}:{1:D2}:{2:D2}", h, m, s);
            }
            else
            {
                displayText += "00:00:00 - 00:00:00";
            }
            labelText.Text = displayText;
        }
        private void UpdateTrackBar()
        {
            try
            {
                if (_objMediaPosition != null)
                {
                    _autoChange = true;

                    if (trackBarDuration.Maximum != (int)_objMediaPosition.Duration) trackBarDuration.Maximum = (int)_objMediaPosition.Duration;
                    trackBarDuration.Value = (int)_objMediaPosition.CurrentPosition;
                    trackBarDuration.LargeChange = trackBarDuration.Maximum / 100;

                    if (trackBarDuration.Value == trackBarDuration.Maximum) _isRunning = false;
                }
            }
            catch (Exception exc)
            {
                Log.write("[ NFO 0001 : cannot display the trackbar value : " + exc.Message);
            }
            finally
            {
                _autoChange = false;
            }
        }
        private void UpdateToolBar()
        {
            //switch (m_CurrentStatus)
            //{
            //    case MediaStatus.None: toolBarButton1.Enabled = false;
            //        toolBarButton2.Enabled = false;
            //        toolBarButton3.Enabled = false;
            //        break;

            //    case MediaStatus.Paused: toolBarButton1.Enabled = true;
            //        toolBarButton2.Enabled = false;
            //        toolBarButton3.Enabled = true;
            //        break;

            //    case MediaStatus.Running: toolBarButton1.Enabled = false;
            //        toolBarButton2.Enabled = true;
            //        toolBarButton3.Enabled = true;
            //        break;

            //    case MediaStatus.Stopped: toolBarButton1.Enabled = true;
            //        toolBarButton2.Enabled = false;
            //        toolBarButton3.Enabled = false;
            //        break;
            //}
        }
        private void CleanUp()
        {
            if (_objMediaControl != null)
                _objMediaControl.Stop();

            if (_objMediaEventEx != null)
                _objMediaEventEx.SetNotifyWindow(0, 0, 0);

            if (_objVideoWindow != null)
            {
                _objVideoWindow.Visible = 0;
                _objVideoWindow.Owner = 0;
            }

            if (_objMediaControl != null) _objMediaControl = null;
            if (_objMediaPosition != null) _objMediaPosition = null;
            if (_objMediaEventEx != null) _objMediaEventEx = null;
            if (_objMediaEvent != null) _objMediaEvent = null;
            if (_objVideoWindow != null) _objVideoWindow = null;
            if (_objBasicAudio != null) _objBasicAudio = null;
            if (_objFilterGraph != null) _objFilterGraph = null;
        }
        private void ScreenChange()
        {
            if (_fullscreen)
            {
                buttonFullScreen.BackgroundImage = imageList1.Images[imageList1.Images.IndexOfKey("fullscreen")];
                _fullscreen = !_fullscreen; 
                OnFullSreenExit();
            }
            else
            {
                buttonFullScreen.BackgroundImage = imageList1.Images[imageList1.Images.IndexOfKey("partscreen")];
                _fullscreen = !_fullscreen;
                OnFullScreenRequested();
            }
            ResizeScreen();
        }
        private void ResizeScreen()
        {
            //switch (_screenFormat)
            //{
            //    //case ScreenFormat.CLASSIC:
            //    //    panelVideo.Dock = DockStyle.None;
            //    //    panelVideo.Height = (this.Width * 9) / 16;
            //    //    break;
            //    //case ScreenFormat.FULL:
            //    //    panelVideo.Dock = DockStyle.Fill;
            //    //    panelVideo.Height = (this.Width * 9) / 16;
            //    //    break;
            //    //case ScreenFormat.QUARTERS:
            //    //    panelVideo.Dock = DockStyle.None;
            //    //    panelVideo.Height = (this.Width * 3) / 4;
            //    //    break;
            //    //default:
            //    //    panelVideo.Dock = DockStyle.None;
            //    //    panelVideo.Height = (this.Width * 9) / 16;
            //    //    break;
            //}
            //panelVideo.Width = this.Width;
            //panelVideo.Left = 0;
            //panelVideo.Top = (this.Height / 2) - (panelVideo.Height / 2);
        }
        #endregion

        #region Methods protected
        protected void OnFullScreenRequested()
        {
            if (FullScreenRequested != null) FullScreenRequested();
        }
        protected void OnFullSreenExit()
        {
            if (FullScreenExit != null) FullScreenExit();
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_GRAPHNOTIFY)
            {
                int lEventCode;
                int lParam1, lParam2;

                while (true)
                {
                    try
                    {
                        _objMediaEventEx.GetEvent(out lEventCode,
                            out lParam1,
                            out lParam2,
                            0);

                        _objMediaEventEx.FreeEventParams(lEventCode, lParam1, lParam2);

                        if (lEventCode == EC_COMPLETE)
                        {
                            _objMediaControl.Stop();
                            _objMediaPosition.CurrentPosition = 0;
                            UpdateStatus();
                            UpdateToolBar();
                        }
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
            }

            base.WndProc(ref m);
        }
        #endregion

        #region Event
        private void component_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                contextMenuStrip1.PointToScreen(new Point(e.Y + this.Top, e.X + this.Left));
                contextMenuStrip1.Show();
            }
        }
        private void VideoPlayer_Resize(object sender, EventArgs e)
        {
            panelUserCommand.Left = this.Width / 2 - panelUserCommand.Width / 2;
            if (panelUserCommand.Left < 10) { panelUserCommand.Left = 10; }
            panelUserCommand.Top = 25;

            ResizeScreen();
        }
        private void panelSettings_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            KeyEvent(e.ToString().ToCharArray()[0]);
        }
        private void panelVideo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            KeyEvent(e.KeyCode.ToString().ToCharArray()[0]);
        }
        private void panelSettings_Resize(object sender, EventArgs e)
        {
            panelUserCommand.Left = this.Width / 2 - panelUserCommand.Width / 2;
            if (panelUserCommand.Left < 10) { panelUserCommand.Left = 10; }
            panelUserCommand.Top = 25;
        }
        private void Form1_SizeChanged(object sender, System.EventArgs e)
        {
            if (_objVideoWindow != null)
            {
                _objVideoWindow.SetWindowPosition(panelVideo.ClientRectangle.Left,
                    panelVideo.ClientRectangle.Top,
                    panelVideo.ClientRectangle.Width,
                    panelVideo.ClientRectangle.Height);
            }
        }
        private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
        {
            //switch (toolBar1.Buttons.IndexOf(e.Button))
            //{
            //    case 0: _objMediaControl.Run();
            //        m_CurrentStatus = MediaStatus.Running;
            //        break;

            //    case 1: _objMediaControl.Pause();
            //        m_CurrentStatus = MediaStatus.Paused;
            //        break;

            //    case 2: _objMediaControl.Stop();
            //        _objMediaPosition.CurrentPosition = 0;
            //        m_CurrentStatus = MediaStatus.Stopped;
            //        break;
            //}

            //UpdateStatusBar();
            //UpdateToolBar();
        }
        private void timer1_Tick(object sender, System.EventArgs e)
        {
            UpdateStatus();
        }
        private void trackBarDuration_ValueChanged(object sender, EventArgs e)
        {
            if (!_autoChange && _objMediaPosition != null) _objMediaPosition.CurrentPosition = trackBarDuration.Value;
        }
        private void trackBarDuration_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (_objMediaPosition != null)
            {
                _objMediaPosition.CurrentPosition = trackBarDuration.Value;
                _autoChange = false;
            }
        }
        private void trackBarDuration_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            _autoChange = true;
        }
        private void panelVideo_DoubleClick(object sender, EventArgs e)
        {
            if (_objVideoWindow != null)
            {
                if (_objVideoWindow.FullScreenMode == EC_COMPLETE) FullScreen();
                else RestaureSize();
            }
        }
        private void buttonPlayPause_Click(object sender, EventArgs e)
        {
            PlayPause();
        }
        private void buttonEject_Click(object sender, EventArgs e)
        {
            mciSendString("set CDAudio door open", null, 0, IntPtr.Zero);
        }
        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (_objMediaPosition != null)
            {
                trackBarDuration.Value = 0;
                _objMediaPosition.CurrentPosition = 0;
                buttonPlayPause.BackgroundImage = imageList1.Images[imageList1.Images.IndexOfKey("run")];
            }
        }
        private void buttonRewind_Click(object sender, EventArgs e)
        {
        }
        private void buttonForward_Click(object sender, EventArgs e)
        {

        }
        private void buttonMute_Click(object sender, EventArgs e)
        {

        }
        private void trackBarVolume_ValueChanged(object sender, EventArgs e)
        {
            if (_objBasicAudio != null) _objBasicAudio.Volume = trackBarVolume.Value;
        }
        private void component_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyEvent(e.KeyChar);
        }
        private void buttonFullScreen_Click(object sender, EventArgs e)
        {
            ScreenChange();
        }
        private void toolStripMenuItem169_Click(object sender, EventArgs e)
        {
            _screenFormat = ScreenFormat.CLASSIC;
            ResizeScreen();
        }
        private void toolStripMenuItem34_Click(object sender, EventArgs e)
        {
            _screenFormat = ScreenFormat.QUARTERS;
            ResizeScreen();
        }
        private void automatiqueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _screenFormat = ScreenFormat.FULL;
            ResizeScreen();
        }
        private void fullScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _screenFormat = ScreenFormat.FULL;
            ResizeScreen();
        }
        private void ComponentDoubleClick(object sender, MouseEventArgs e)
        {
            ScreenChange();
        }
        #endregion
    }
}
