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
    public partial class customers : Form
    {
        public customers()
        {
            InitializeComponent();
        }

        private string connectionString = "Data Source=vmwsql07.uad.ac.uk; Initial Catalog=sql1501846; User ID=sql1501846; Password=D&1jGr&4";
        private string selectStatement;
        private SqlCommand commDB;

        private SqlConnection SetUpConn() //set up connection
        {
            SqlConnection conn;
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

        private void customers_Load(object sender, EventArgs e)
        {
            SetUpConn().Open();

            selectStatement = "SELECT name,ID,postcode, VIP FROM Assess_Customers";

            try {
                commDB = new SqlCommand(selectStatement, SetUpConn());
                //display data in grid
                DisplayDataGrid(commDB);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

        }
    }
}
