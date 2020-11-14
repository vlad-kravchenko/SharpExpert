using System;
using System.Windows.Forms;
namespace SharpExpert
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
           
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.Show();
            this.Hide();
        }
        private void button2_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}