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
    public partial class Upgrade : Form
    {
        public Upgrade()
        {
            InitializeComponent();
        }

        public string connectionString = "Data Source=vmwsql07.uad.ac.uk; Initial Catalog=sql1501846; User ID=sql1501846; Password=D&1jGr&4";

        private SqlCommand commDB;
        private SqlConnection conn;

        public SqlConnection SetUpConn() //set up connection
        {
            conn = new SqlConnection(connectionString);
            conn.Open();
            return conn;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int id = 0; //Customer id
            try
            {
                id = Int32.Parse(textBox1.Text); //try to convert sting input into integer
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            //declare update statement
           string updateStatement = "UPDATE Assess_Customers SET VIP = 'Yes' WHERE ID = @id";
            commDB = new SqlCommand(updateStatement, SetUpConn());
            commDB.Parameters.Add(new SqlParameter("id", id));
            
            try {
                
                int result = commDB.ExecuteNonQuery();

                if (result == 0) //If there is no customer with that ID in the database, let the manager know
                {
                    errorProvider1.SetError(textBox1,"Invalid Customer ID");
                }
                else
                {
                    MessageBox.Show("Customer upgraded to VIP"); //confirm update
                    errorProvider1.Clear();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            conn.Close();
        }


    }
}
