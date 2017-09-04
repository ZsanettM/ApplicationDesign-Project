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
    public partial class login : Form
    {

        private string connectionString = "Data Source=vmwsql07.uad.ac.uk; Initial Catalog=sql1501846; User ID=sql1501846; Password=D&1jGr&4";
        private string selectStatement;
        private SqlCommand commDB;
        SqlConnection conn;

        private SqlConnection SetUpConn() //connection
        {
            conn = new SqlConnection(connectionString);
            return conn;
        }

        public login()
        {
            InitializeComponent();
        }
        
        public string getUser;

        private void login_Load(object sender, EventArgs e)
        {
            //if the main form (Form1) sent "Manager" as user, make the login id 1 by default
            if (getUser == "Manager") { label1.Text = "Manager"; textBox1.Text = "1";  textBox1.Enabled=false;}

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int id = 0;
            try
            {
                id = Int32.Parse(textBox1.Text); //try coverting id string into int
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            string password = textBox2.Text;

            //create connection
            SetUpConn().Open();

            //retrieve information
            selectStatement = "SELECT fullName FROM Assess_SaleReps WHERE ID=@id AND password=@pwd";
            commDB = new SqlCommand(selectStatement, conn);
            commDB.Parameters.Add(new SqlParameter("ID", id));
            commDB.Parameters.Add(new SqlParameter("pwd", password));

            if (Convert.ToString(commDB.ExecuteScalar()) == "") //unsuccessful login attempt if there is no match
            {
               
                MessageBox.Show("Incorrect login data");
                this.Hide();
                Form1 mainForm = new Form1(); //create new Form1
                mainForm.approved = false; //staff options will not be enabled
                mainForm.user = id; //id=0
                mainForm.ShowDialog(); //open new Form1
                
                this.Close();
            }
            else
            {
                this.Hide();
                Form1 mainForm = new Form1(); //create new Form1
                mainForm.approved=true; //enable appropriate staff options
                mainForm.user = id; 
                mainForm.ShowDialog(); //open new Form1
                
                this.Close(); //close this window
            }

        }


    }
}
