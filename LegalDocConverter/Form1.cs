using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LegalDocConverter
{
    public partial class Form1 : Form
    {
        convertSystem converter;
        public Form1()
        {
            InitializeComponent();
            converter = new convertSystem();
        }

        private void inputBox_TextChanged(object sender, EventArgs e)
        {
            string a = inputBox.Text;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //try
            //{
                converter.convert(inputBox.Text, outputBox.Text, templateBox.Text);
                MessageBox.Show("Document converted!");
            //}
            /*catch
            {
                MessageBox.Show("Error, incorrect path, try again.");
            }*/


        }
    }
}
