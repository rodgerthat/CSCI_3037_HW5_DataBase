using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace hw5DataBase
{
    public partial class Form1 : Form
    {

        public DataTable dataTable;

        public Form1()
        {
            InitializeComponent();
        }

        // Load the DataBase
        private void button1_Click(object sender, EventArgs e)
        {

            ImportCSV();
            DataTableToDataBase(this.dataTable);
            LoadDataGridViewWithDataTable(this.dataTable);

            label1.Text = "DataBase has been Loaded!";
        }

        // Clear the DataBase
        private void button2_Click(object sender, EventArgs e)
        {

            label1.Text = "DataBase has been Cleared!";
        }

        // Create the DataBase
        private void button3_Click(object sender, EventArgs e)
        {
            CreateDataBase();

        }

        // Add Lines to DB
        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void CreateDataBase()
        {

            String str = "CREATE DATABASE studentGrade ON PRIMARY " +
                "(NAME = studentGrade, " +
                "FILENAME = 'studentGrade.mdf', " +
                "SIZE = 2MB, MAXSIZE = 10MB, FILEGROWTH = 10%) " +
                "LOG ON (NAME = studentGrade_Log, " +
                "FILENAME = 'studentGrade_Log.ldf', " +
                "SIZE = 1MB, " +
                "MAXSIZE = 5MB, " +
                "FILEGROWTH = 10%)";

            SqlConnection myConn = new SqlConnection("Server=localhost;Integrated security=SSPI;database=master");

            SqlCommand myCommand = new SqlCommand(str, myConn);

            try
            {
                myConn.Open();
                myCommand.ExecuteNonQuery();
                label1.Text = "DataBase is Created Successfully";
            }
            catch (System.Exception ex)
            {
                label1.Text = ex.ToString();
            }
            finally
            {
                if (myConn.State == ConnectionState.Open)
                {
                    myConn.Close();
                }
            }
        }

        protected void ImportCSV()
        {
            // the csv file we're using
            string csvFile = @"./csvDBData.txt";

            // Create a DataTable.  
            // Student Name,Class,Earned Hours,GPA,Course,CourseNumber,FinalGrade,TotalCSCICreditHrs,CSCI GPA
            this.dataTable = new DataTable();
            this.dataTable.Columns.AddRange(new DataColumn[9] {
                new DataColumn("Student Name", typeof(string)),
                new DataColumn("Class", typeof(string)),
                new DataColumn("Earned Hours", typeof(int)),
                new DataColumn("GPA",typeof(double)),
                new DataColumn("Course",typeof(string)),
                new DataColumn("CourseNumber",typeof(int)),
                new DataColumn("FinalGrade",typeof(string)),
                new DataColumn("TotalCSCICreditHrs",typeof(int)),
                new DataColumn("CSCI GPA",typeof(double)),
            });

            // Read the contents of CSV file.  
            string csvData = File.ReadAllText(csvFile);

            // Execute a loop over the rows.  
            foreach (string row in csvData.Split('\n'))
            {
                if (!string.IsNullOrEmpty(row))
                {
                    this.dataTable.Rows.Add();
                    int i = 0;

                    // Execute a loop over the columns.  
                    foreach (string cell in row.Split(','))
                    {
                        this.dataTable.Rows[this.dataTable.Rows.Count - 1][i] = cell;
                        i++;
                    }
                }
            }

        }

        void LoadDataGridViewWithDataTable(DataTable dataTable)
        {

            // TODO : left off here, populating dataGridView with datatable
            dataGridView1.DataSource = 
        }

        // take note of SqlBulkCopyOptions.KeepIdentity , you may or may not want to use this for your situation.  
        private void DataTableToDataBase(DataTable dataTable)
        {
            string connectionString = GetConnectionString();
            // string consString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {

                using (var bulkCopy = new SqlBulkCopy(sqlCon.ConnectionString, SqlBulkCopyOptions.KeepIdentity))
                {

                    // my DataTable column names match my SQL Column names, 
                    // so I simply made this loop. However if your column names don't match, 
                    // just pass in which datatable name matches the SQL column name in Column Mappings
                    foreach (DataColumn col in dataTable.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                    }

                    bulkCopy.BulkCopyTimeout = 600;
                    bulkCopy.DestinationTableName = "studentGrade";
                    bulkCopy.WriteToServer(dataTable);

                }
            }

            // To avoid storing the connection string in your code, 
            // you can retrieve it from a configuration file. 
            string GetConnectionString()
            {
                return "Data Source=(local); " +
                       " Integrated Security=true;" +
                       "Initial Catalog=AdventureWorks;";
            }

            void DataBaseToDataTable()
            {

            }

            void populateDataGridView()
            {

            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

    }

}
