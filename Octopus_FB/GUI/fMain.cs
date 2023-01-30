using Leaf.xNet;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using Octopus_FB.GUI;
using SharpAdbClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Octopus_FB
{
    public partial class fMain : Form
    {
        private static fMain _Instance;
        public static Object thislock = new Object();
        public fMain()
        {
            _Instance = this;
            InitializeComponent();
        }
        AdbServer server = new AdbServer();
        private void fMain_Load(object sender, EventArgs e)
        {
            LoadDanhMuc();
            try
            {
                var result = server.StartServer(Application.StartupPath + "\\adb.exe", restartServerIfNewer: false);
                Task.Run(() =>
                {
                    var monitor = new DeviceMonitor(new AdbSocket(new IPEndPoint(IPAddress.Loopback, AdbClient.AdbServerPort)));
                    monitor.DeviceConnected += this.OnDeviceConnected;
                    monitor.DeviceDisconnected += this.OnDeviceDisconnect;
                    monitor.Start();
                });
            }
            catch(Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
           
        }
        void OnDeviceDisconnect(object sender, DeviceDataEventArgs e)
        {
            string deviceName = e.Device.ToString();
            foreach(var item in dtgv.Rows.Cast<DataGridViewRow>().Where(p => p.Cells[0].Value.ToString().Contains(deviceName)).ToList())
            {
                dtgv.Invoke(new Action(delegate ()
                {
                    dtgv.Rows.Remove(item);
                }));
            }
            ReloadSTTCount();
            if (fControlPhone.isOpen)
            {
                lock (thislock)
                {
                    fControlPhone.ClearPanel(deviceName,true);
                }
            }
        }
        void OnDeviceConnected(object sender, DeviceDataEventArgs e)
        {
            string groupbox = "";
            this.Invoke(new Action(delegate ()
            {
                groupbox = comboBox1.Text;
            }));
            
            string deviceName = e.Device.ToString();
            var deviceinfo = new DBOctopus().GetDeviceInfo(deviceName);
            string displayName = deviceinfo["NAMEDISPLAY"].ToString();
            string group = deviceinfo["GROUPPHONE"].ToString();
            string xproxy = deviceinfo["XPROXY"].ToString();
            if (groupbox != "All" && !group.Contains("No Group") && !group.Contains(groupbox))
                return;
            List<string> listGroup = group.Split('|').Where(p => !p.Contains("No Group")).ToList();
            if (listGroup.Count < 1 && !listGroup.Contains(groupbox) && groupbox != "All")
            {
                return;
            }
            if (dtgv.Rows.Cast<DataGridViewRow>().Where(p => p.Cells[0].Value.ToString().Contains(deviceName)).ToList().Count < 1)
            {
                dtgv.Invoke(new Action(delegate ()
                {
                    this.dtgv.Rows.Add(deviceName,"1", false, displayName, group,xproxy);
                }));
            }
            if (fControlPhone.isOpen)
            {
                int size = Convert.ToInt32(numericUpDown1.Value);

                lock (thislock)
                {
                    fControlPhone.AddPhonetoPanel(deviceName, size);
                }
            }
            ReloadSTTCount();
        }
        public void ReloadSTTCount()
        {
            int countstt = 1;
            dtgv.Invoke(new Action(delegate ()
            {
                foreach(var line in dtgv.Rows.Cast<DataGridViewRow>())
                {
                    line.Cells[1].Value = countstt;
                    countstt++;
                }
                this.txtDevice.Text = "Device Count : " + (countstt - 1).ToString();
            }));
        }
        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            if(!fControlPhone.isOpen)
            {
                new fControlPhone().Show();
            }
        }
        private void chọnTôĐenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var line in dtgv.Rows.Cast<DataGridViewRow>())
            {
                line.Cells[2].Value = false;
            }
            foreach (var line in dtgv.Rows.Cast<DataGridViewRow>().
                Where(p => p.Selected))
            {
                line.Cells[2].Value = true;
            }
            dtgv.Refresh();
        }
        private void chọnTấtCảToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dtgv.Rows.Count; i++)
            {
                this.dtgv.Rows[i].Cells[2].Value = true;
            }
        }
        private void bỏChọnTấtCảToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dtgv.Rows.Count; i++)
            {
                this.dtgv.Rows[i].Cells[2].Value = false;
            }
        }
      

        private void bắtĐầuToolStripMenuItem_Click(object sender, EventArgs e)
        { 

        }
        public static void UpdateGirdView(string deviceID, string text = "")
        {
            try
            {
                foreach (var line in _Instance.dtgv.Rows.Cast<DataGridViewRow>().
                Where(p => p.Cells[0].Value.ToString().Contains(deviceID)))
                {

                    line.Cells[4].Value = text;
                }
            }
            catch { }
        }
        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var line in dtgv.Rows.Cast<DataGridViewRow>().
               Where(p => Convert.ToBoolean(p.Cells[2].Value.ToString())).ToList())
            {
                string deviceID = line.Cells[0].Value.ToString();
                var deviceinfo = new DBOctopus().GetDeviceInfo(deviceID);
                string displayName = deviceinfo["NAMEDISPLAY"].ToString();
                string group = deviceinfo["GROUPPHONE"].ToString();
                string xproxy = deviceinfo["XPROXY"].ToString();

                 string input = Interaction.InputBox(
                  "Nhập tên hiện thị : ",
                  "DEVICE ID : " + deviceID,
                  displayName.Length > 0 ? displayName : "Octopus-" + deviceID,
                  500,
                  500);
                if (input.Length < 1)
                    return;
                new DBOctopus().UpdateDevice(new JObject
                {
                    ["DEVICEID"] = deviceID,
                    ["NAMEDISPLAY"] = input,
                    ["GROUPPHONE"] = group,
                    ["XPROXY"] = xproxy
                });

                line.Cells[3].Value = input;
            }
            LoadDevice();
            MessageBox.Show("Rename thành công");
        }

        private void bunifuButton6_Click(object sender, EventArgs e)
        {
            string input = Interaction.InputBox(
                  "Tên danh mục : ",
                  "Input Box", "",
                  500,
                  500);
            if (input.Length < 1)
                return;
            new DBOctopus().ExecuteQuery(string.Format("Insert Into Groupdevice(NAMEGROUP,SIZE) values ('{0}','{1}')", new object[]
                    {
                     input,
                     "3"
                    }));
            LoadDanhMuc();
            Extension.NotifyMsg("Success");
        }
        private void LoadDanhMuc()
        {
            comboBox1.Items.Clear();
            comboBox1.ResetText();
            comboBox1.Items.Add("All");
            string size = "";
            var ListDanhMuc = new DBOctopus().ExecuteQuery("SELECT * FROM Groupdevice");
            for (int i = 0; i < ListDanhMuc.Rows.Count; i++)
            {
                DataRow row = ListDanhMuc.Rows[i];
                comboBox1.Items.Add(row["NAMEGROUP"].ToString());
            }
            comboBox1.SelectedIndex = comboBox1.FindStringExact("All");
          
        }
        private void bunifuButton7_Click(object sender, EventArgs e)
        {
            string input = comboBox1.Text;
            if (input.Length < 1 || input == "All")
                return;
            if(Extension.NotifyMsg("Bạn có chắc muốn xóa " + input + " ??", 1) == DialogResult.Yes)
            {
                new DBOctopus().ExecuteQuery("DELETE FROM Groupdevice WHERE NAMEGROUP='" + input + "'");
                new DBOctopus().DeleteGroupWhenDelete(input);
                LoadDanhMuc();
               
                Extension.NotifyMsg("Xóa thành công " + input);
            }

           
        }
        private void bunifuButton2_Click(object sender, EventArgs e)
        {
            string groupbox = comboBox1.Text;
            int size = Convert.ToInt32(numericUpDown1.Value);
            if (groupbox == "All")
                return;
            
            new DBOctopus().ExecuteQuery("UPDATE Groupdevice SET SIZE='" + size + "' WHERE NAMEGROUP='" + groupbox + "'");
            Extension.NotifyMsg("Success");

        }
        private void addToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var listDevice = dtgv.Rows.Cast<DataGridViewRow>().Where(p => Convert.ToBoolean(p.Cells[2].Value)).Select(p => p.Cells[0].Value.ToString()).ToList();
            if (listDevice.Count > 0)
            {
                new fDanhmuc(listDevice).Show();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDevice();
        }

        private void loadDeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadDevice();
        }
        public static void LoadDevice()
        {
            try
            {
                if (fControlPhone.isOpen)
                {
                    fControlPhone.ClearAllPanel();
                }
                _Instance.dtgv.Rows.Clear();
                string groupbox = "";
                _Instance.Invoke(new Action(delegate ()
                {
                    groupbox = _Instance.comboBox1.Text;
                }));
                _Instance.numericUpDown1.Value = new DBOctopus().GetSizeGroupBox(groupbox);
                int size = Convert.ToInt32(_Instance.numericUpDown1.Value);
                foreach (var deviceData in new AdbClient().GetDevices())
                {
                    string deviceName = deviceData.Serial.ToString();
                    var deviceinfo = new DBOctopus().GetDeviceInfo(deviceName);
                    string displayName = deviceinfo["NAMEDISPLAY"].ToString();
                    string group = deviceinfo["GROUPPHONE"].ToString();
                    if (groupbox != "All" && !group.Contains("No Group") && !group.Contains(groupbox))
                        continue;
                    List<string> listGroup = group.Split('|').Where(p => !p.Contains("No Group")).ToList();
                    if (listGroup.Count < 1 && !listGroup.Contains(groupbox) && groupbox != "All")
                    {
                        continue;
                    }
                    string xproxy = deviceinfo["XPROXY"].ToString();
                    if (_Instance.dtgv.Rows.Cast<DataGridViewRow>().Where(p => p.Cells[0].Value.ToString().Contains(deviceName)).ToList().Count < 1)
                    {
                        _Instance.dtgv.Invoke(new Action(delegate ()
                        {
                            _Instance.dtgv.Rows.Add(deviceName, "1", false, displayName, group, xproxy);
                        }));
                    }
                    //if (fControlPhone.isOpen)
                    //{
                    //    fControlPhone.AddPhonetoPanel(deviceName,size);
                    //}

                }
                _Instance.ReloadSTTCount();
            }
            catch(Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
            
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string groupbox = comboBox1.Text;
            if (groupbox == "All")
                return;
            var listDevice = dtgv.Rows.Cast<DataGridViewRow>().Where(p => Convert.ToBoolean(p.Cells[2].Value)).ToList();
            foreach(var line in listDevice)
            {
                string group = line.Cells[4].Value.ToString();
                string newGROUP = "";
                if (group.StartsWith(groupbox))
                {
                    if(group.Contains(groupbox + "|"))
                    {
                        newGROUP = group.Replace(groupbox + "|", "");
                    }
                    else
                        newGROUP = group.Replace(groupbox, "");

                }
                else
                {
                    newGROUP = group.Replace("|" + groupbox, "");
                }
                if (newGROUP.Length < 1)
                    newGROUP = "No Group";
                new DBOctopus().ExecuteQuery("UPDATE Device SET GROUPPHONE='" + newGROUP + "' WHERE DEVICEID='" + line.Cells[0].Value.ToString() + "'");
            }
            LoadDevice();
        }

        private void bunifuButton3_Click_1(object sender, EventArgs e)
        {
            if (!fControlPhone.isOpen)
            {
                new fControlPhone().Show();
            }
            int size = Convert.ToInt32(numericUpDown1.Value);
            new Task(() =>
            {
                fControlPhone.ClearAllPanel();
                foreach (var deviceData in dtgv.Rows.Cast<DataGridViewRow>().Select(p => p.Cells[0].Value.ToString()).ToList())
                {
                    fControlPhone.AddPhonetoPanel(deviceData, size);
                }
            }).Start();
            
        }

        private void shơToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int size = Convert.ToInt32(numericUpDown1.Value);
            new Task(() =>
            {
                var listDevice = dtgv.Rows.Cast<DataGridViewRow>().Where(p => Convert.ToBoolean(p.Cells[2].Value)).ToList();

                foreach (var line in listDevice)
                {
                    if (!fControlPhone.isOpen)
                    {
                        new fControlPhone().Show();
                    }
                    fControlPhone.AddPhonetoPanel(line.Cells[0].Value.ToString(), size);
                }

            }).Start();
         
        }

        private void removeShowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Task(() =>
            {
                var listDevice = dtgv.Rows.Cast<DataGridViewRow>().Where(p => Convert.ToBoolean(p.Cells[2].Value)).ToList();
                foreach (var line in listDevice)
                {
                    if (fControlPhone.isOpen)
                    {
                        fControlPhone.ClearPanel(line.Cells[0].Value.ToString(), true);
                    }
                }
            }).Start();
        }

        private void dtgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void editProxyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var line in dtgv.Rows.Cast<DataGridViewRow>().
               Where(p => Convert.ToBoolean(p.Cells[2].Value.ToString())).ToList())
            {
                string deviceID = line.Cells[0].Value.ToString();
                var deviceinfo = new DBOctopus().GetDeviceInfo(deviceID);
                string displayName = deviceinfo["NAMEDISPLAY"].ToString();
                string group = deviceinfo["GROUPPHONE"].ToString();
                string xproxy = deviceinfo["XPROXY"].ToString();

                string input = Interaction.InputBox(
                 "Nhập URL Proxy : ",
                 "Proxy: " + deviceID,
                  xproxy,
                 500,
                 500);
                if (input.Length < 1)
                    return;
                new DBOctopus().UpdateDevice(new JObject
                {
                    ["DEVICEID"] = deviceID,
                    ["NAMEDISPLAY"] = displayName,
                    ["GROUPPHONE"] = group,
                    ["XPROXY"] = input
                });

                line.Cells[5].Value = input;
            }
            LoadDevice();
            MessageBox.Show("Edit thành công");
        }

        private void removeAllViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fControlPhone.ClearAllPanel();
        }
    }
}
