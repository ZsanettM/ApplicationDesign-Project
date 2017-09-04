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
using System.Security.Cryptography;
using System.Text;

namespace WindowsFormsApplication1
{
    public partial class create : Form
    {
        public create()
        {
            InitializeComponent();
            
        }
        public string Vip = "No"; //default value, this is used when a SalesRep creates a new customer
        public string getUser; //Manager or SalesRep
        public int user = 0;

        

        private void create_Load(object sender, EventArgs e)
        {
            if (getUser == "Manager" || user==1 )
            {
                createManager createM = new createManager();
                this.Hide();
                createM.ShowDialog();
                this.Close();
            }
        }

        public void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "") {
            SqlConnection conn;
            
            string connectionString = "Data Source=vmwsql07.uad.ac.uk; Initial Catalog=sql1501846; User ID=sql1501846; Password=D&1jGr&4";
            conn = new SqlConnection(connectionString); conn.Open();
            
            //retrieve and store infromation entered into the form
            string name  = textBox1.Text;
            string postcode = textBox2.Text;
            string password = textBox3.Text;
            
            


            try
            {
                //Add new customer to the Assess_Customers table
                string selectStatement = "INSERT INTO dbo.Assess_Customers (name, postcode, VIP, password) VALUES (@name, @post,@Vip,@pwd)";
                SqlCommand commDB = new SqlCommand(selectStatement, conn);
                commDB.Parameters.Add(new SqlParameter("name", name));
                commDB.Parameters.Add(new SqlParameter("post", postcode));
                commDB.Parameters.Add(new SqlParameter("VIP", Vip));
                commDB.Parameters.Add(new SqlParameter("pwd", password));
                commDB.ExecuteNonQuery();

                MessageBox.Show("Customer created"); //Confirm
                textBox1.Text = ""; textBox2.Text = ""; textBox3.Text = "";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            
        }
            else { MessageBox.Show("Invalid values"); }
        }
        
        

    }
}