using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BaseLibrary;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Text = Crypting.Encrypt(textBox2.Text);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            textBox4.Text = Crypting.Encrypt(textBox3.Text);
        }

    }
}
