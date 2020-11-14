using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Windows.Forms;
namespace SharpExpert
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        public OleDbDataAdapter adapter = new OleDbDataAdapter();
        public DataSet dataSet = new DataSet();
        private void Form2_Load(object sender, EventArgs e)
        {
            string command = "SELECT * FROM Alternatives";
            MakeRequest(command);
            adapter.Fill(dataSet);
            adapter.Update(dataSet);
            int num = 12;
            foreach (ComboBox combo in tabControl1.TabPages[0].Controls.OfType<ComboBox>())
            {
                List<string> l = new List<string>();
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    if (row[num].ToString().Length > 1)
                        l.Add(row[num].ToString());
                    l.Add("Не важливо");
                }
                var newlist = new List<string>(l.Distinct());
                foreach (var a in newlist)
                {
                    combo.Items.Add(a);
                }
                num--;
            }
        }
        public void MakeRequest(string comm)
        {
            string conStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Database.mdb";
            OleDbConnection connection = new OleDbConnection(conStr);
            connection.Open();
            OleDbCommand command = new OleDbCommand(comm, connection);
            connection.Close();
            adapter.SelectCommand = command;
        }
        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataSet.Clear();
            string command = "SELECT * FROM Alternatives WHERE" + ConstructQuerry();
            //MessageBox.Show(command.Substring(0, command.Length - 4));
            MakeRequest(command.Substring(0, command.Length - 4));
            adapter.Fill(dataSet);
            adapter.Update(dataSet);
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                dataGridView1.DataSource = dataSet.Tables[0];
                dataGridView1.AutoResizeColumns();
            }
            else
            {
                MessageBox.Show("Для обраних вами параметрів неможливо підібрати місце для відпочинку. Змініть умови.");
            }
        }
        public string ConstructQuerry()
        {
            String str = string.Empty;
            if (textBox1.Text != "")
                str += " Бюджет<=" + textBox1.Text.ToString() + " AND";
            if (textBox2.Text != "")
                str += " Строк<=" + textBox2.Text.ToString() + " AND";
            if (comboBox1.SelectedItem != null && comboBox1.SelectedItem.ToString() != "Не важливо")
                str += " Тип_бази='" + comboBox1.SelectedItem.ToString() + "' AND";
            if (comboBox2.SelectedItem != null && comboBox2.SelectedItem.ToString() != "Не важливо")
                str += " Сезон='" + comboBox2.SelectedItem.ToString() + "' AND";
            if (comboBox3.SelectedItem != null && comboBox3.SelectedItem.ToString() != "Не важливо")
                str += " Мова='" + comboBox3.SelectedItem.ToString() + "' AND";
            if (comboBox4.SelectedItem != null && comboBox4.SelectedItem.ToString() != "Не важливо")
                str += " Страх_польоту='" + comboBox4.SelectedItem.ToString() + "' AND";
            if (comboBox5.SelectedItem != null && comboBox5.SelectedItem.ToString() != "Не важливо")
                str += " Місцевість='" + comboBox5.SelectedItem.ToString() + "' AND";
            if (comboBox6.SelectedItem != null && comboBox6.SelectedItem.ToString() != "Не важливо")
                str += " Клімат='" + comboBox6.SelectedItem.ToString() + "' AND";
            if (comboBox7.SelectedItem != null && comboBox7.SelectedItem.ToString() != "Не важливо")
                str += " Екстрим='" + comboBox7.SelectedItem.ToString() + "' AND";
            if (comboBox8.SelectedItem != null && comboBox8.SelectedItem.ToString() != "Не важливо")
                str += " Додатково='" + comboBox8.SelectedItem.ToString() + "' AND";
            return str;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //Устанавливаем корректную версию браузера
                SetWebBrowserCompatiblityLevel();
                //Отображение нужного города на карте
                webBrowser1.Navigate("https://www.google.com.ua/maps/place/" + dataGridView1.CurrentRow.Cells[2].Value.ToString());
                tabControl1.SelectedTab = tabControl1.TabPages[1];
            }
            catch
            {
                MessageBox.Show("Перевірте, що таблиця результатів не порожня і що в ній обрано один з варіантів!");
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            dataSet.Clear();
            string command = "SELECT * FROM Alternatives";
            MakeRequest(command);
            adapter.Fill(dataSet);
            adapter.Update(dataSet);
            dataGridView1.DataSource = dataSet.Tables[0];
            dataGridView1.AutoResizeColumns();
        }
        //Функции SetWebBrowserCompatiblityLevel, WriteCompatiblityLevel и GetBrowserVersion отвечают за корректную работу браузера в соответствии
        //с установленной на конкретной машине версией IE, так как по умолчанию используется IE7, который воспринимается Google как устаревший
        private static void SetWebBrowserCompatiblityLevel()
        {
            string appName = Path.GetFileNameWithoutExtension(Application.ExecutablePath);
            int lvl = 1000 * GetBrowserVersion();
            bool fixVShost = File.Exists(Path.ChangeExtension(Application.ExecutablePath, ".vshost.exe"));
            WriteCompatiblityLevel("HKEY_LOCAL_MACHINE", appName + ".exe", lvl);
            if (fixVShost) WriteCompatiblityLevel("HKEY_LOCAL_MACHINE", appName + ".vshost.exe", lvl);
            WriteCompatiblityLevel("HKEY_CURRENT_USER", appName + ".exe", lvl);
            if (fixVShost) WriteCompatiblityLevel("HKEY_CURRENT_USER", appName + ".vshost.exe", lvl);
        }
        private static void WriteCompatiblityLevel(string root, string appName, int lvl)
        {
            try
            {
                Microsoft.Win32.Registry.SetValue(root + @"\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", appName, lvl);
            }
            catch (Exception) { }
        }
        public static int GetBrowserVersion()
        {
            string strKeyPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Internet Explorer";
            string[] ls = new string[] { "svcVersion", "svcUpdateVersion", "Version", "W2kVersion" };
            int maxVer = 0;
            for (int i = 0; i < ls.Length; ++i)
            {
                object objVal = Microsoft.Win32.Registry.GetValue(strKeyPath, ls[i], "0");
                string strVal = Convert.ToString(objVal);
                if (strVal != null)
                {
                    int iPos = strVal.IndexOf('.');
                    if (iPos > 0)
                        strVal = strVal.Substring(0, iPos);

                    int res = 0;
                    if (int.TryParse(strVal, out res))
                        maxVer = Math.Max(maxVer, res);
                }
            }
            return maxVer;
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
