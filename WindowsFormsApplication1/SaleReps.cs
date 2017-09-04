using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace WindowsFormsApplication1
{
    public partial class SaleReps : Form
    {
        public SaleReps()
        {
            InitializeComponent();
        }

        private string connectionString = "Data Source=vmwsql07.uad.ac.uk; Initial Catalog=sql1501846; User ID=sql1501846; Password=D&1jGr&4";
        SqlConnection conn;

        private SqlConnection SetUpConn() //set up connection
        {
            conn = new SqlConnection(connectionString);
            return conn;
        }

        private void DisplayDataGrid(SqlCommand command) //display retrievd data in a gridView
        {
            SqlDataAdapter dAdapt = new SqlDataAdapter();
            dAdapt.SelectCommand = command;
            DataTable dTable = new DataTable();
            dAdapt.Fill(dTable);
            BindingSource bSource = new BindingSource();
            bSource.DataSource = dTable;
            dataGridView1.DataSource = bSource;
            dAdapt.Update(dTable);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //open connection
            SqlConnection conn = new SqlConnection(connectionString);
            SetUpConn().Open();

            string date1 = dateTimePicker1.Value.ToString("yyyy-MM-dd"); //convert dates
            string date2 = dateTimePicker2.Value.ToString("yyyy-MM-dd");

            //retrieve data and display in gridView
            string strSQLCommand = "SELECT ID, fullName, ROUND(SUM(price),2) AS RevGenerated, COUNT(bookingID) AS NumberOfSales FROM Reps WHERE Created_On_Date BETWEEN @date1 and @date2 GROUP BY ID, fullName ORDER BY ROUND(SUM(price),2) DESC";
            SqlCommand commDB = new SqlCommand(strSQLCommand, conn);
            commDB.Parameters.Add(new SqlParameter("date1", date1));
            commDB.Parameters.Add(new SqlParameter("date2", date2));

            try
            {
                DisplayDataGrid(commDB);
                conn.Close();


            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            conn.Close();

        }
    }
}
