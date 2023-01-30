using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus_FB
{
	public class DBOctopus
    {
		public static string DataSource = "Data Source=DATA\\DBOctober.db";
		private SQLiteCommand SQLitecommand;
		private SQLiteDataAdapter sqliteDataAdapter;
		private DataSet dataSet = new DataSet();
		private DataTable dataTable = new DataTable();

		public DataTable ExecuteQuery(string CommandText)
		{
			using (SQLiteConnection sqliteConnection = new SQLiteConnection(DataSource))
			{
				try
				{
					sqliteConnection.Open();
					SQLitecommand = sqliteConnection.CreateCommand();
					this.sqliteDataAdapter = new SQLiteDataAdapter(CommandText, sqliteConnection);
					this.dataSet.Reset();
					this.sqliteDataAdapter.Fill(this.dataSet);
					if (this.dataSet.Tables.Count > 0)
					{
						this.dataTable = this.dataSet.Tables[0];
					}
					sqliteConnection.Close();
					return this.dataTable;
				}
				catch (Exception ee)
				{
					Console.WriteLine(ee.Message);
				}
			}
			return null;
		}
		public string GetDeviceNameDisplay(string deviceID)
        {
			string result = "";
			DataTable dataTable = ExecuteQuery("SELECT * FROM Device");
			foreach(DataRow Row in dataTable.Rows)
            {
                if (Row["DEVICEID"].ToString().Contains(deviceID))
                {
					return Row["NAMEDISPLAY"].ToString();
				}
            }
			return deviceID;
        }
		public void DeleteGroupWhenDelete(string groupbox)
		{
			string result = "";
			DataTable dataTable = ExecuteQuery("SELECT * FROM Device");
			foreach (DataRow Row in dataTable.Rows)
			{
				if (Row["GROUPPHONE"].ToString().Contains(groupbox))
				{
					string group = Row["GROUPPHONE"].ToString();
					string newGROUP = "";
					if (group.StartsWith(groupbox))
					{
						if (group.Contains(groupbox + "|"))
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
					string deviceID = Row["DEVICEID"].ToString();
					new DBOctopus().ExecuteQuery("UPDATE Device SET GROUPPHONE='" + newGROUP + "' WHERE DEVICEID='" + deviceID + "'");

				}
			}
			return;
		}
		public string GetXPROXYDevice(string deviceID)
		{
			string result = "";
			DataTable dataTable = ExecuteQuery("SELECT * FROM Device");
			foreach (DataRow Row in dataTable.Rows)
			{
				if (Row["DEVICEID"].ToString().Contains(deviceID))
				{
					return Row["XPROXY"].ToString();
				}
			}
			return "NO XPROXY";
		}
		public string GetGroupNameDevice(string deviceID)
		{
			string result = "";
			DataTable dataTable = ExecuteQuery("SELECT * FROM Device");
			foreach (DataRow Row in dataTable.Rows)
			{
				if (Row["DEVICEID"].ToString().Contains(deviceID))
				{
					return Row["GROUPPHONE"].ToString();
				}
			}
			return "No Group";
		}

		public JObject GetDeviceInfo(string deviceID)
		{
			JObject json = new JObject();
			json["DEVICEID"] = deviceID;
			DataTable dataTable = ExecuteQuery("SELECT * FROM Device WHERE DEVICEID='" + deviceID + "'");
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				json["NAMEDISPLAY"] = dataTable.Rows[0]["NAMEDISPLAY"].ToString();
				json["GROUPPHONE"] = dataTable.Rows[0]["GROUPPHONE"].ToString();
				json["XPROXY"] = dataTable.Rows[0]["XPROXY"].ToString();
			}
			else
            {
				InsertDevice(new JObject
				{
					["DEVICEID"] = deviceID,
					["NAMEDISPLAY"] = deviceID,
					["GROUPPHONE"] = "No Group",
					["XPROXY"] = "No Xproxy"
				});
				json["NAMEDISPLAY"] = deviceID;
				json["GROUPPHONE"] = "No Group";
				json["XPROXY"] = "No Xproxy";
			}
			return json;
		}
		public void InsertDevice(JObject JSON)
		{
			string DEVICEID = JSON["DEVICEID"].ToString();
			string GROUP = JSON["GROUPPHONE"].ToString();
			string XPRORY = JSON["XPROXY"].ToString();
			string NAMEDISPLAY = JSON["NAMEDISPLAY"].ToString();
			this.ExecuteQuery(string.Format("Insert Into Device(DEVICEID,NAMEDISPLAY,GROUPPHONE,XPROXY) values ('{0}','{1}','{2}','{3}')", new object[]
					{
					 DEVICEID,
					 NAMEDISPLAY,
					 GROUP,
					 XPRORY
					}));
		}
		public void UpdateDevice(JObject JSON)
		{
			string DEVICEID = JSON["DEVICEID"].ToString();
			string GROUP = JSON["GROUPPHONE"].ToString();
			string XPRORY = JSON["XPROXY"].ToString();
			string NAMEDISPLAY = JSON["NAMEDISPLAY"].ToString();
			this.ExecuteQuery(string.Format("UPDATE Device SET NAMEDISPLAY='{1}', GROUPPHONE='{2}',XPROXY='{3}' WHERE DEVICEID='{0}'", new object[]
					{
						DEVICEID,
						NAMEDISPLAY,
						GROUP,
						XPRORY
					}));
		}
		public int GetSizeGroupBox(string namegroup)
        {
			int size = 0;
			DataTable dataTable = ExecuteQuery("SELECT SIZE FROM Groupdevice WHERE NAMEGROUP='" + namegroup + "'");
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				string valueSize = dataTable.Rows[0]["SIZE"].ToString();
				if (valueSize.Length > 0)
                {
					size = Convert.ToInt32(valueSize);
                }
			}
			else
			{
				size = 4;
			}
			return size;
        }
		


	}
}
