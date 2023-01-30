using Leaf.xNet;
using Newtonsoft.Json.Linq;
using Octopus_FB.Properties;
using PInvoke;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Octopus_FB
{
    public partial class fControlPhone : Form
    {
        private static fControlPhone _Instance;
        public static Object thislock = new Object();
        [DllImport("user32")] private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32")] private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32")] private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int Width, int Height, bool Repaint);
        private const int GWL_STYLE = -16;
        private const int WS_VISIBLE = 0x10000000;
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        public static bool isOpen = false;
        public fControlPhone()
        {
            _Instance = this;
            InitializeComponent();
        }
        private void fControlPhone_Load(object sender, EventArgs e)
        {
            foreach (var process in Process.GetProcessesByName("scrcpy"))
            {
                process.Kill();
            }
            isOpen = true;
        }
        public static int widthScreen = Screen.PrimaryScreen.Bounds.Width;
        public static void AddPhonetoPanel(string deviceID, int size)
        {
            ClearPanel(deviceID,false);

            int width = widthScreen / size - 20;
            ScrcpyPhone(deviceID, width);
            IntPtr hwnd = IntPtr.Zero;
            Size sizeScry = new Size(0,0);
            int countError = 0;
            lock (thislock)
            {
                while (hwnd == IntPtr.Zero && sizeScry.Width < 1 && sizeScry.Height< 1)
                {
                    hwnd = FindWindow(null, "Octopus-" + deviceID);
                    Thread.Sleep(500);
                    sizeScry = GetControlSize(hwnd);
                    countError++;
                    if(countError > 15)
                    {
                        countError = 0;
                        ScrcpyPhone(deviceID, width);
                    }
                }
            }
            _Instance.PanelPhone.Invoke((Action)(() =>
            {
                int HControl = 27;
                int Hlabel = 25;
                int HButton = 25;
                Panel pMain = new Panel();
                pMain.Name = "phoneMain_" + deviceID;
                pMain.Size = new Size(sizeScry.Width, sizeScry.Height + HControl + Hlabel + HButton);
                Label lbTitle = new Label();
                lbTitle.Name = "lb_" + deviceID;
                lbTitle.Size = new Size(sizeScry.Width, Hlabel);
                lbTitle.Location = new Point(0, 0);
                lbTitle.AutoSize = false;
                lbTitle.BorderStyle = BorderStyle.None;
                lbTitle.TextAlign = ContentAlignment.MiddleCenter;
                lbTitle.BackColor = Color.FromArgb(150, 214, 255);
                lbTitle.Text = new DBOctopus().GetDeviceNameDisplay(deviceID);

                Panel pDisplay = new Panel();
                pDisplay.Size = new Size(sizeScry.Width, sizeScry.Height);
                pDisplay.Location = new Point(0, Hlabel);
                pDisplay.BackColor = Color.Aquamarine;

                Panel pControl = new Panel();
                pControl.Size = new Size(sizeScry.Width, HControl);
                pControl.Location = new Point(0, sizeScry.Height + Hlabel);
                pControl.BackColor = Color.FromArgb(150, 214, 255);

             

                int widthIcon = sizeScry.Width / 4;
                PictureBox pictureIconRecent = new PictureBox
                {
                    Image = Resources.icon_recent,
                    Location = new Point(widthIcon - 30, 0),
                    Size = new Size(25, 25),
                    Name = deviceID,
                    Cursor = Cursors.Hand
                };
                pictureIconRecent.Click += _Instance.Recent;
                pControl.Controls.Add(pictureIconRecent);
                _Instance.toolTip1.SetToolTip(pictureIconRecent, "Recent");
                PictureBox pictureIconHome = new PictureBox
                {
                    Image = Resources.icon_home,
                    Location = new Point(widthIcon + widthIcon - 30, 0),
                    Size = new Size(25, 25),
                    Name = deviceID,
                    Cursor = Cursors.Hand
                };
                pictureIconHome.Click += _Instance.Home;
                pControl.Controls.Add(pictureIconHome);
                _Instance.toolTip1.SetToolTip(pictureIconHome, "Home");
                PictureBox pictureIconBack = new PictureBox
                {
                    Image = Resources.icon_back,
                    Location = new Point(widthIcon + widthIcon + widthIcon - 30, 0),
                    Size = new Size(25, 25),
                    Name = deviceID,
                    Cursor = Cursors.Hand
                };
                pictureIconBack.Click += _Instance.Back;
                pControl.Controls.Add(pictureIconBack);
                _Instance.toolTip1.SetToolTip(pictureIconBack, "Back");
                PictureBox pictureIconPower = new PictureBox
                {
                    Image = Resources.icon_power,
                    Location = new Point(widthIcon + widthIcon + widthIcon + widthIcon - 30, 0),
                    Size = new Size(25, 25),
                    Name = deviceID,
                    Cursor = Cursors.Hand
                };
                pictureIconPower.Click += _Instance.Power;
                pControl.Controls.Add(pictureIconPower);
                _Instance.toolTip1.SetToolTip(pictureIconPower, "Power");


                Panel pButton = new Panel();
                pButton.Size = new Size(sizeScry.Width, HButton);
                pButton.Location = new Point(0, sizeScry.Height + Hlabel + HButton);
                pButton.BackColor = Color.AliceBlue;
                int widthXProxy = sizeScry.Width / 3;
                TextBox lbProxy = new TextBox();
                lbProxy.Location = new Point(0, 2);
                lbProxy.Size = new Size(sizeScry.Width - widthXProxy, HButton + 10);
                lbProxy.Text = new DBOctopus().GetXPROXYDevice(deviceID);
                lbProxy.Name = "txt_" + deviceID;
                pButton.Controls.Add(lbProxy);

                Button btnProxy = new Button();
                btnProxy.Name = "btn_" + deviceID;
                btnProxy.Location = new Point(sizeScry.Width - widthXProxy, 0);
                btnProxy.Size = new Size(widthXProxy, HButton);
                btnProxy.Text = "Change IP";
               
                btnProxy.Click += _Instance.ChangeIP;

                pButton.Controls.Add(btnProxy);
                


                pMain.Controls.Add(lbTitle);
                pMain.Controls.Add(pDisplay);
                pMain.Controls.Add(pControl);
                pMain.Controls.Add(pButton);


                _Instance.PanelPhone.Controls.Add(pMain);
                SetParent(hwnd, pDisplay.Handle);
            }));
            SetWindowLong(hwnd, GWL_STYLE, WS_VISIBLE);
            MoveWindow(hwnd, 0, 0, 300, 550, true);
        }
        
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        public static Size GetControlSize(IntPtr hWnd)
        {
            RECT pRect;
            Size cSize = new Size();
            GetWindowRect(hWnd, out pRect);
            cSize.Width = pRect.right - pRect.left;
            cSize.Height = pRect.bottom - pRect.top;
            return cSize;
        }
        public void ChangeIP(object sender, EventArgs e)
        {
            string deviceID = "";
            try
            {
                deviceID = (sender as Button).Name.Replace("btn_", "");
                if (deviceID.Length < 1)
                    return;
                new Task(() =>
                {
                    SetTitle(deviceID, "Running");
                    string url = new DBOctopus().GetXPROXYDevice(deviceID);
                    if (url.Length < 1)
                    {
                        MessageBox.Show(new DBOctopus().GetDeviceNameDisplay(deviceID) + " Không get được url - Url rỗng");
                        return;
                    }
                    SetTextProxy(deviceID, url);

                    //request ở đây
                    HttpRequest http = new HttpRequest();
                    var result = JObject.Parse(http.Get(url).ToString());
                    //msg":"command_sent
                    if (result["msg"].ToString().Contains("command_sent"))
                    {
                        SetTitle(deviceID, "Success");
                    }
                    else
                    {
                        Extension.NotifyMsg(result.ToString());
                        SetTitle(deviceID, "Fail");

                    }
                }).Start();
                
            }
            catch (Exception ex)
            {
                SetTitle(deviceID, "Change IP");

                Extension.NotifyMsg(ex.Message);
            }
        }
        public void SetTitle(string deviceID, string proxy)
        {
            foreach (Control item in _Instance.PanelPhone.Controls)
            {
                if (item.Name == "phoneMain_" + deviceID)
                {
                    foreach (Control item2 in item.Controls)
                    {
                        if (item2.Name == "lb_" + deviceID)
                        {
                            item2.Text = new DBOctopus().GetDeviceNameDisplay(deviceID) + " - " + proxy;
                        }
                    }
                }
            }
        }
        public void SetTextProxy(string deviceID, string proxy)
        {
            foreach(Control item in _Instance.PanelPhone.Controls)
            {
                if (item.Name == "phoneMain_" + deviceID)
                {
                    foreach(Control item2 in item.Controls)
                    {
                        if (item2.Name == "txt_" + deviceID)
                        {
                            item2.Text = proxy;
                        }
                    }
                }
            }
        }
        private void Back(object sender, EventArgs e)
        {
            try
            {
                string name = (sender as PictureBox).Name;
                Extension.RunCMD_Result(" -s " + name + " shell input keyevent 4");
            }
            catch (Exception ex)
            {
                Extension.NotifyMsg(ex.Message);
            }
        }
        private void Power(object sender, EventArgs e)
        {
            try
            {
                string name = (sender as PictureBox).Name;
                Extension.RunCMD_Result(" -s " + name + " shell input keyevent 26");
            }
            catch (Exception ex)
            {
                Extension.NotifyMsg(ex.Message);
            }
        }
        private void Home(object sender, EventArgs e)
        {
            try
            {
                string name = (sender as PictureBox).Name;
                Extension.RunCMD_Result(" -s " + name + " shell input keyevent 3");
            }
            catch (Exception ex)
            {
                Extension.NotifyMsg(ex.Message);
            }
        }
        private void Recent(object sender, EventArgs e)
        {
            try
            {
                string name = (sender as PictureBox).Name;
                Extension.RunCMD_Result(" -s " + name + " shell input keyevent 187");
            }
            catch (Exception ex)
            {
                Extension.NotifyMsg(ex.Message);
            }
        }
        public static void ClearAllPanel()
        {
            //for(int i = 0;i < _Instance.PanelPhone.Controls.Count;i++)
            //{
            //    _Instance.PanelPhone.Invoke((Action)(() =>
            //    {
            //        _Instance.PanelPhone.Controls.Remove(_Instance.PanelPhone.Controls[i]);
            //    }));
            //}
            _Instance.PanelPhone.Invoke((Action)(() =>
                {
                    _Instance.PanelPhone.Controls.Clear();
                }));

            foreach (var process in Process.GetProcessesByName("scrcpy"))
            {
                try
                {
                    process.Kill();
                }
                catch { }
            }
            listScrcpy.Clear();
        }
        public static void ClearPanel(string deviceID, bool isDelete = false)
        {
            foreach (Control item in _Instance.PanelPhone.Controls)
            {
                if (item.Name == "phoneMain_" + deviceID)
                {
                    _Instance.PanelPhone.Invoke((Action)(() =>
                    {
                        _Instance.PanelPhone.Controls.Remove(item);
                    }));
                }
            }
            if(isDelete)
            {
                var tempProcess = new Process();
                if (listScrcpy.TryGetValue(deviceID, out tempProcess))
                {
                    try
                    {
                        tempProcess.Kill();

                    }
                    catch { }
                }
            }
        }
        public static Dictionary<string, Process> listScrcpy = new Dictionary<string, Process>();
        public static void ScrcpyPhone(string deviceID,int width)
        {
            var tempProcess = new Process();
            if (listScrcpy.TryGetValue(deviceID, out tempProcess))
            {
                try
                {
                    tempProcess.Kill();

                }
                catch { }
            }
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = "scrcpy" + "\\scrcpy.exe",
                Arguments = string.Concat(new string[]
                {
                                    " -s ",
                                   deviceID,
                                    " --window-width " + width,
                                    " --rotation 0",
                                    " --window-borderless",
                                    " -b2M -m800 --max-fps 60",
                                    " --shortcut-mod=lctrl",
                                    " --power-off-on-close",
                                    " --turn-screen-off",
                                    " --stay-awake",
                                    " --window-title Octopus-" + deviceID,
                }),
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
            };
            Process temp;
            if (listScrcpy.TryGetValue(deviceID, out temp))
                listScrcpy[deviceID] = process;
            else
                listScrcpy.Add(deviceID, process);
            process.Start();
           
        }
        private void fControlPhone_FormClosed(object sender, FormClosedEventArgs e)
        {
            isOpen = false;
        }

        
    }
}
