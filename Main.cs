using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minimizador
{
    public partial class Main : Form
    {
        OpenFileDialog ofd = new OpenFileDialog();
        JFF jflapitem;
        string filefolder;

        public Main()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = ofd.ShowDialog();
            if(result == DialogResult.OK)
            {
                textBox1.Text = ofd.FileName;
                System.IO.FileInfo fInfo = new System.IO.FileInfo(ofd.FileName);
                filefolder = fInfo.DirectoryName;
                label1.Text = "Estado: Cargado Exitosamente";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
           
                jflapitem = new JFF(textBox1.Text);

                
                jflapitem.minimizarDFA("");
           
        }
    }
}
