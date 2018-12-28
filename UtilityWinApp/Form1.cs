using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace UtilityWinApp
{
    public partial class Form1 : Form
    {
        #region Variables & declarations

        TextBox[] txtBox;
        Label[] lbl;
        Button btnSavetoCsv;
        int n = 0;
        int space = 0;
        string assemblyName = ConfigurationManager.AppSettings["AssemblyName"];

        #endregion

        #region Form methods

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.ExportCSV.Enabled = false;
            FillCombobox();
            //dataGridView1_CellContentClick(dataGridView1, new DataGridViewCellEventArgs(0, 0));
        }

        #endregion

        #region Helper methods

        private Type[] GetTypes()
        {
            Assembly assembly = Assembly.Load(assemblyName);
            Type[] types = assembly.GetTypes();
            foreach (var item in types)
            {
                var props = item.GetProperties();
                string filename = item.Name + ".csv";
                string filepath = String.Format(@"{0}\" + filename, Application.StartupPath);
                if (!File.Exists(filepath))
                {
                    using (var writer = new StreamWriter(filepath))
                    {
                        writer.WriteLine(string.Join(", ", props.Select(p => p.Name)));
                    }
                }
            }
            return types;
        }

        protected void FillCombobox()
        {
            try
            {
                Type[] types = GetTypes();
                List<string> lstClassEntities = new List<string>();
                foreach (var item in types)
                {
                    lstClassEntities.Add(item.Name);
                }

                comboBox1.DataSource = lstClassEntities;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private object GetNewObject(Type t)
        {
            try
            {
                return t.GetConstructor(new Type[] { }).Invoke(new object[] { });
            }
            catch
            {
                return null;
            }
        }

        private DataTable BindDatasourceGrid(string csvName)
        {
            // string path = @"C:\Users\NandaKishore\source\repos\UntilityBuilder\CsvFolder\" + csvName + ".csv";

            string path = String.Format(@"{0}\" + csvName + ".csv", Application.StartupPath);

            //  get all lines of csv file
            string[] str = File.ReadAllLines(path);

            // create new datatable
            DataTable dt = new DataTable();

            // get the column header means first line
            string[] temp = str[0].Split(',');

            // creates columns of gridview as per the header name
            foreach (string t in temp)
            {
                dt.Columns.Add(t, typeof(string));
            }

            // now retrive the record from second line and add it to datatable
            for (int i = 1; i < str.Length; i++)
            {
                str[i] = str[i].TrimEnd(',');
                string[] t = str[i].Split(',');
                dt.Rows.Add(t);

            }

            return dt;
        }

        #endregion

        #region Event methods

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string cmbTxt = Convert.ToString(comboBox1.SelectedValue);
            //string path = @"C:\Users\NandaKishore\source\repos\UntilityBuilder\CsvFolder\" + cmbTxt + ".csv";
            string path = String.Format(@"{0}\" + cmbTxt + ".csv", Application.StartupPath);

            // clear the complete panel controls
            this.pnlDyanamicControls.Controls.Clear();

            // assign gridview datasource property by datatable
            dataGridView1.DataSource = BindDatasourceGrid(cmbTxt);
            // dataGridView1.Click += new System.EventHandler(this.dataGridView1_CellContentClick);            
            this.ExportCSV.Enabled = true;

            if (!string.IsNullOrEmpty(cmbTxt))
            {
                space = 50;
                Type[] types = GetTypes();
                foreach (var item in types)
                {
                    if (item.Name == cmbTxt)
                    {
                        var props = item.GetProperties();
                        n = props.Count();
                        txtBox = new TextBox[n];
                        lbl = new Label[n];
                        btnSavetoCsv = new Button();

                        int i = 0;
                        foreach (var prop in props)
                        {
                            txtBox[i] = new TextBox();
                            txtBox[i].Name = props[i].Name;

                            lbl[i] = new Label();
                            lbl[i].Name = props[i].Name;
                            lbl[i].Text = props[i].Name;

                            txtBox[i].Visible = true;
                            lbl[i].Visible = true;
                            txtBox[i].Location = new Point(100, 50 + space);
                            lbl[i].Location = new Point(10, 50 + space);
                            this.pnlDyanamicControls.Controls.Add(txtBox[i]);
                            this.pnlDyanamicControls.Controls.Add(lbl[i]);
                            space += 50;

                            i++;
                        }

                        btnSavetoCsv.Text = "Save to CSV";
                        btnSavetoCsv.Location = new Point(100, 50 + space);
                        btnSavetoCsv.Size = new System.Drawing.Size(100, 20);
                        btnSavetoCsv.Click += new System.EventHandler(this.btnSavetoCsv_Click);
                        this.pnlDyanamicControls.Controls.Add(btnSavetoCsv);
                    }
                }
            }

        }

        private void btnSavetoCsv_Click(object sender, EventArgs e)
        {
            string allTextBoxValues = string.Empty;

            foreach (Control c in this.pnlDyanamicControls.Controls)
            {
                if (c is TextBox)
                {
                    TextBox txt = (TextBox)c;
                    allTextBoxValues += txt.Text + ",";
                }
            }

            string cmbTxt = Convert.ToString(comboBox1.SelectedValue);
            //string path = @"C:\Users\NandaKishore\source\repos\UntilityBuilder\CsvFolder\" + cmbTxt + ".csv";

            string path = String.Format(@"{0}\" + cmbTxt + ".csv", Application.StartupPath);


            File.AppendAllText(path, allTextBoxValues + Environment.NewLine);

            #region data source load
            //  get all lines of csv file
            string[] str = File.ReadAllLines(path);

            // create new datatable
            DataTable dt = new DataTable();

            // get the column header means first line
            string[] temp = str[0].Split(',');

            // creates columns of gridview as per the header name
            foreach (string t in temp)
            {
                dt.Columns.Add(t, typeof(string));
            }

            // now retrive the record from second line and add it to datatable
            for (int i = 1; i < str.Length; i++)
            {
                str[i] = str[i].TrimEnd(',');
                string[] t = str[i].Split(',');
                dt.Rows.Add(t);
            }

            // assign gridview datasource property by datatable
            dataGridView1.DataSource = dt;
            dataGridView1.AutoResizeColumns();
            // bind the gridview
            // dataGridView1.DataBind();
            #endregion

            MessageBox.Show("File write completed", "CSV Data");
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dataGridView1.DataSource = dt;

            // clear the complete panel controls
            this.pnlDyanamicControls.Controls.Clear();

        }

        private void btnExportCSV_Click(object sender, EventArgs e)
        {
            string cmbTxt = Convert.ToString(comboBox1.SelectedValue);
            //string path = @"C:\Users\NandaKishore\source\repos\UntilityBuilder\CsvFolder\" + cmbTxt + ".csv";

            string path = String.Format(@"{0}\" + cmbTxt + ".csv", Application.StartupPath);

            //Build the CSV file data as a Comma separated string.
            string csv = string.Empty;

            //Add the Header row for CSV file.
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                csv += column.HeaderText + ',';
            }

            csv = csv.TrimEnd(',');

            //Add new line.
            csv += "\r\n";

            //Adding the Rows
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    //Add the Data rows.
                    csv += Convert.ToString(cell.Value).Replace(",", ";") + ',';
                }

                //Add new line.
                csv += "\r\n";
            }

            //Exporting to CSV.            
            File.WriteAllText(path, csv);

            MessageBox.Show("Exported successfully to " + cmbTxt + ".csv");
        }


        #endregion

        #region commented code

        //public void writeCSV(DataGridView gridIn, string outputFile)
        //{
        //    //test to see if the DataGridView has any rows
        //    if (gridIn.RowCount > 0)
        //    {
        //        string value = "";
        //        DataGridViewRow dr = new DataGridViewRow();
        //        StreamWriter swOut = new StreamWriter(outputFile);

        //        //write header rows to csv
        //        for (int i = 0; i <= gridIn.Columns.Count - 1; i++)
        //        {
        //            if (i > 0)
        //            {
        //                swOut.Write(",");
        //            }
        //            swOut.Write(gridIn.Columns[i].HeaderText);
        //        }

        //        swOut.WriteLine();

        //        //write DataGridView rows to csv
        //        for (int j = 0; j <= gridIn.Rows.Count - 1; j++)
        //        {
        //            if (j > 0)
        //            {
        //                swOut.WriteLine();
        //            }

        //            dr = gridIn.Rows[j];

        //            for (int i = 0; i <= gridIn.Columns.Count - 1; i++)
        //            {
        //                if (i > 0)
        //                {
        //                    swOut.Write(",");
        //                }

        //                value = dr.Cells[i].Value.ToString();
        //                //replace comma's with spaces
        //                value = value.Replace(',', ' ');
        //                //replace embedded newlines with spaces
        //                value = value.Replace(Environment.NewLine, " ");

        //                swOut.Write(value);
        //            }
        //        }
        //        swOut.Close();
        //    }
        //}       

        //public static void AppendAllText(string path, string contents, Encoding encoding)
        //{
        //    using (StreamWriter writer = new StreamWriter(path, true, encoding))
        //    {
        //        writer.Write(contents);
        //    }
        //}

        //---------------------------------------
        //for (int i = 0; i < n; i++)
        //{
        //    txtBox[i] = new TextBox();
        //    txtBox[i].Name = props[i].Name;

        //    lbl[i] = new Label();
        //    lbl[i].Name = props[i].Name;
        //    lbl[i].Text = props[i].Name;
        //}


        //for (int i = 0; i < n; i++)
        //{
        //    txtBox[i].Visible = true;
        //    lbl[i].Visible = true;
        //    txtBox[i].Location = new Point(40, 50 + space);
        //    lbl[i].Location = new Point(10, 50 + space);
        //    this.Controls.Add(txtBox[i]);
        //    this.Controls.Add(lbl[i]);
        //    space += 50;
        //}

        //------------------------------
        //Label lblCsvData = new Label();
        //lblCsvData.Text = allTextBoxValues;
        //lblCsvData.Location = new Point(120, 500);

        //foreach (Control childc in Controls)
        //{
        //    if (childc is TextBox && childc.Name.Contains("txt"))
        //        allTextBoxValues += ((TextBox)childc).Text + ",";
        //}

        #endregion

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
