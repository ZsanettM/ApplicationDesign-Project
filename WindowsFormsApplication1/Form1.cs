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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        public bool approved = false;
        public int user=0;

        //set up connection
        SqlConnection conn;
        private string connectionString = "Data Source=vmwsql07.uad.ac.uk; Initial Catalog=sql1501846; User ID=sql1501846; Password=D&1jGr&4";

        private void gridDisplay(SqlCommand commDB) //Display selected data in gridDataView
        {
            SqlDataAdapter dAdapt = new SqlDataAdapter();
            dAdapt.SelectCommand = commDB;
            DataTable dTable = new DataTable();
            dAdapt.Fill(dTable);
            BindingSource bSource = new BindingSource();
            bSource.DataSource = dTable;
            results.DataSource = bSource;
            dAdapt.Update(dTable);
            conn.Close();
        }

        private void button1_Click(object sender, EventArgs e)  //Check connection
        {

            try
            {
                conn.Open();
                textBox1.Text = "Success";
                conn.Close();
            }
            catch (Exception error)
            {
                textBox1.Text = "Cannot connect to server";
                errorProvider1.SetError(textBox1,error.ToString());
                
            }
        }

        private void button2_Click(object sender, EventArgs e) //Enable customer options or open staff login window
        {
            if (listBox1.SelectedIndex == 0) {
                label2.Text = "Customer";
                label6.BackColor = System.Drawing.Color.Lime;
                label7.BackColor = System.Drawing.Color.Transparent;
                label8.BackColor = System.Drawing.Color.Transparent;
                CustomerOptions.Enabled = true; SalesRep.Enabled = false; Manager.Enabled = false;
            }
            else if (listBox1.SelectedIndex == 1) {

                this.Hide();
                login RepLog = new login();
                RepLog.getUser = "Rep";
                RepLog.ShowDialog();
                
                this.Close();

            }
            else if (listBox1.SelectedIndex == 2) {
                this.Hide();
                login RepLog = new login();
                RepLog.getUser = "Manager";
                RepLog.ShowDialog();
                
                this.Close();
            }
            
        
        }

        private void Form1_Load(object sender, EventArgs e) //If staff login id available, enable appropriate SalesRep or Manager options
        {
            conn = new SqlConnection(connectionString);

            if (approved == true)
            {
                if (user != 0 && user != 1)
                {
                    
                    label2.Text = "Rep";
                    label7.BackColor=System.Drawing.Color.Lime;
                    label6.BackColor = System.Drawing.Color.Transparent;
                    label8.BackColor = System.Drawing.Color.Transparent;

                    CustomerOptions.Enabled = false; SalesRep.Enabled = true; Manager.Enabled = false;
                }
                else if (user == 1)
                {
                    
                    label8.BackColor = System.Drawing.Color.Lime;
                    label6.BackColor = System.Drawing.Color.Transparent;
                    label7.BackColor = System.Drawing.Color.Transparent;
                    label2.Text = "Manager";
                    CustomerOptions.Enabled = false; SalesRep.Enabled = false; Manager.Enabled = true;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e) //Retrieve journeys data

        {
            results.Visible = true;

            conn = new SqlConnection(connectionString); conn.Open();
            string loc = comboBox1.Text;
            string type = comboBox2.Text;
            string selectStatement;
            if (loc != "" && type != "") //Depending on the selected searching conditions, declare appropriate select statement
            {
                selectStatement = "select * from dbo.Assess_Journeys where arrivalLoc=@arrivalLoc and type=@vehicle";
            }
            else if (loc != "")
            {
                selectStatement = "select * from dbo.Assess_Journeys where arrivalLoc=@arrivalLoc";
            }
            else if (type != "")
            {
                selectStatement = "select * from dbo.Assess_Journeys where type=@vehicle";
            }
            else
            {
                selectStatement = "select * from dbo.Assess_Journeys";
            }
            SqlCommand commDB = new SqlCommand(selectStatement, conn);
            commDB.Parameters.Add(new SqlParameter("arrivalLoc", loc));
            commDB.Parameters.Add(new SqlParameter("vehicle", type));
            commDB.ExecuteNonQuery();
            
            try
            {
                gridDisplay(commDB); //display
                conn.Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            comboBox1.Text = ""; comboBox2.Text = "";
        }

        private void SalesRep_SelectedIndexChanged(object sender, EventArgs e) //When a salesRep chooses a new option, open corresponding window
        {
            if (SalesRep.SelectedIndex == 0) {
                
                create CreateCustomer = new create(); 
                CreateCustomer.getUser = "SalesRep";
                CreateCustomer.ShowDialog();
                
            }
            else if (SalesRep.SelectedIndex == 2)
            {

                NewBooking CreateBooking = new NewBooking();
                CreateBooking.getUser = "SalesRep";
                CreateBooking.user = user;
                CreateBooking.ShowDialog();

            }
            else if (SalesRep.SelectedIndex == 1)
            {

                CheckBookingsStaff checkBooking = new CheckBookingsStaff();
                checkBooking.getUser = "SalesRep";
                checkBooking.ShowDialog();

            }
        }

        private void Manager_SelectedIndexChanged(object sender, EventArgs e) //When the Manager chooses a new option, open corresponding window
        {
            if (Manager.SelectedIndex == 0)
            {
                
                create CreateCustomer = new create();
                CreateCustomer.getUser = "Manager";
                CreateCustomer.ShowDialog();

            }
            else if (Manager.SelectedIndex == 1)
            {

                CheckBookingsStaff checkBooking = new CheckBookingsStaff();
                checkBooking.getUser = "Manager";
                checkBooking.ShowDialog();

            }
            else if (Manager.SelectedIndex == 2)
            {

                NewBooking CreateBooking = new NewBooking();
                CreateBooking.getUser = "Manager";
                CreateBooking.user = 1;
                CreateBooking.ShowDialog();

            }
            else if (Manager.SelectedIndex == 3)
            {

                SaleReps sRepsDetails = new SaleReps();
                sRepsDetails.ShowDialog();

            }
            else if (Manager.SelectedIndex == 4)
            {

                Upgrade upgrade = new Upgrade();
                upgrade.ShowDialog();

            }
        }

        private void button4_Click(object sender, EventArgs e) //Retrieve hotel details
        {
            results.Visible = true;

            
            conn = new SqlConnection(connectionString); conn.Open();
            string loc = comboBox3.Text;
            string selectStatement;

            if (loc != "") //Depending on the selected searching conditions, declare appropriate select statement
            {
                selectStatement = "select * from dbo.Assess_Hotel where city=@arrivalLoc";
            }
            else
            {
                selectStatement = "select * from dbo.Assess_Hotel";
            }
            SqlCommand commDB = new SqlCommand(selectStatement, conn);
            commDB.Parameters.Add(new SqlParameter("arrivalLoc", loc));
            commDB.ExecuteNonQuery();
            
            try
            {
                gridDisplay(commDB); //display
                conn.Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            comboBox3.Text = "";
        }

        private void CustomerOptions_SelectedIndexChanged(object sender, EventArgs e) 
        {
            checkBookings check = new checkBookings(); //Open Customer bookings&extras window
            check.ShowDialog();
        }

        public object Lime { get; set; }
    }


    }

