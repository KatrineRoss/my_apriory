using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Office.Interop.Excel;
using System.IO;
using System.Windows.Forms;

namespace MyApriory
{
    public partial class Form1 : Form
    {
        private Microsoft.Office.Interop.Excel.Application ExcelApp;
        private Microsoft.Office.Interop.Excel.Workbook ObjWorkBook;
        private Microsoft.Office.Interop.Excel.Worksheet ObjWorkSheet;

        public Form1()
        {
            InitializeComponent();

            openFileDialog1.Filter = "MS Exel files(*.xlsx)|*.xlsx|All files(*.*)|*.*";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            /*if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;*/

            // получаем выбранный файл
            string filename = "C:\\prjs\\sharp\\tranzactions.xlsx";//openFileDialog1.FileName;
            // читаем файл в строку
            string fileText = System.IO.File.ReadAllText(filename);

            ExcelApp = new Microsoft.Office.Interop.Excel.Application();
            ObjWorkBook = ExcelApp.Workbooks.Open(filename, 0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
            ObjWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)ObjWorkBook.Sheets[1];
            fileText = ObjWorkSheet.Cells[1, 2].Text.ToString();

            ExelParser exel = new ExelParser(ObjWorkSheet);
            Apriory a = new Apriory();

            exel.parse();

            a.processTransaction(0.5, 0.3, ExelParser.itemsArray, ExelParser.apriorySets);
            richTextBox1.Text = exel.getTransactionsForPrint();
           
            System.Windows.Forms.Application.DoEvents();

            ExcelApp.Quit();
        }

        private void Form1_FormClosing(Object sender, FormClosingEventArgs e)
        {
            ExcelApp.Quit();
        }
    }
}
