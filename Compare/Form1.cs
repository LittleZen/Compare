using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework;
using Compare.src;

namespace Compare
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        private string path1 = default(string);
        private string path2 = default(string);


        public static bool IsDir(string path)
        {
            return Directory.Exists(path) ? true : false;
        }
        private static void GetResult(string path1, string path2)
        {
            string[] HasPath1 = Hash.GetFileHash(path1);
            string[] HasPath2 = Hash.GetFileHash(path2);

            string[] filePaths = Directory.GetFiles(path2);

            var ListOfDifference = Hash.GetDifference(HasPath1, HasPath2);
            foreach (int position in ListOfDifference)
            {
                Console.WriteLine(filePaths[position]); // Used for debug purpose
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            pbLoading.Visible = true;
            try
            {
                if (path1.Trim() == null || path2.Trim() == null)
                {
                    MessageBox.Show("ERROR");
                }
                await Task.Run(() => { GetResult(path1, path2); });
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Not valid folder selected\n{ex}");
            }
            finally
            {
                pbLoading.Visible = false;
                lbldnd1.Visible = true;
                lbldnd2.Visible = true;
            }
        }

        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] FileDrop = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            
            if (IsDir(FileDrop[0]))
            {
                path1 = FileDrop[0];
                lbldnd1.Visible = false;
            }
            else
            {
                MessageBox.Show("Seems you passed a file, would you like take the entire folder?"); // would you like take the main path from the file passed ? 
            }
        }

        private void textBox1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void textBox2_DragDrop(object sender, DragEventArgs e)
        {
            string[] FileDrop = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if(IsDir(FileDrop[0]))
            {
                path2 = FileDrop[0];
                lbldnd2.Visible = false;
            }
            else
            {
                MessageBox.Show("Seems you passed a file, would you like take the entire folder?");
            }
        }

        private void textBox2_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }
    }
}
