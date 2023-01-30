using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Octopus_FB.GUI
{
    public partial class fDanhmuc : Form
    {
        private static List<string> listDevice = new List<string>();
        public fDanhmuc(List<string> _listDevice)
        {
            listDevice = _listDevice;
            InitializeComponent();
        }
        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            string danhmuc = comboBox2.Text;
            if (danhmuc.Length < 1)
            {
                MessageBox.Show("chưa chọn danh mục");
                return;
            }
            DBOctopus dBOctopus = new DBOctopus();
            foreach (var item in listDevice)
            {
                string group = dBOctopus.GetGroupNameDevice(item);
                if(group != "No Group")
                {
                    group = group + "|" + danhmuc;
                }
                else
                {
                    group = danhmuc;
                }
                dBOctopus.ExecuteQuery("UPDATE Device SET GROUPPHONE='" + group + "' WHERE DEVICEID='" + item+ "'");
            }
            fMain.LoadDevice();
            this.Close();
        }
        private void LoadDanhMuc()
        {
            comboBox2.Items.Clear();
            comboBox2.ResetText();

            var ListDanhMuc = new DBOctopus().ExecuteQuery("SELECT * FROM Groupdevice");
            for (int i = 0; i < ListDanhMuc.Rows.Count; i++)
            {
                DataRow row = ListDanhMuc.Rows[i];
                comboBox2.Items.Add(row["NAMEGROUP"].ToString());
            }
            comboBox2.SelectedIndex = comboBox2.FindStringExact("All");

        }
        private void fDanhmuc_Load(object sender, EventArgs e)
        {
            LoadDanhMuc();
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
                     ""
                    }));
            LoadDanhMuc();
            Extension.NotifyMsg("Success");
        }

        private void bunifuButton7_Click(object sender, EventArgs e)
        {
            string input = comboBox2.Text;
            if (input.Length < 1 || input == "All")
                return;
            new DBOctopus().ExecuteQuery("DELETE FROM Groupdevice WHERE NAMEGROUP='" + input + "'");
            LoadDanhMuc();
            Extension.NotifyMsg("Xóa thành công " + input);
        }
    }
}
