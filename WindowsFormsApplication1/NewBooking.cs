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
    public partial class NewBooking : Form
    {
        public NewBooking()
        {
            InitializeComponent();
        }

        public string getUser;
        public int user = 0;

        SqlConnection conn;

        private string connectionString = "Data Source=vmwsql07.uad.ac.uk; Initial Catalog=sql1501846; User ID=sql1501846; Password=D&1jGr&4";

        private void UpdateSeats(string type, string classTo, string classFrom, int vehicleNumber, int vehicleNumber2, SqlConnection conn) //update available numbers of seats in Assess_Journeys table after a booking is made
        {
            if (type == "Return" || type == "Full")
            {
                if (classTo == "First")
                {
                    string updateStatement = "UPDATE dbo.Assess_Journeys SET seatsFirstC=seatsFirstC-1 WHERE Id=@number";
                    SqlCommand commDBupdate = new SqlCommand(updateStatement, conn);
                    commDBupdate.Parameters.Add(new SqlParameter("number", vehicleNumber));
                    commDBupdate.ExecuteNonQuery();
                }
                else
                {
                    string updateStatement = "UPDATE dbo.Assess_Journeys SET seatsStandC=seatsStandC-1 WHERE Id=@number";
                    SqlCommand commDBupdate = new SqlCommand(updateStatement, conn);
                    commDBupdate.Parameters.Add(new SqlParameter("number", vehicleNumber));
                    commDBupdate.ExecuteNonQuery();
                }
                if (classFrom == "First")
                {
                    string updateStatement1 = "UPDATE dbo.Assess_Journeys SET seatsFirstC=seatsFirstC-1 WHERE Id=@number2";
                    SqlCommand commDBupdate1 = new SqlCommand(updateStatement1, conn);
                    commDBupdate1.Parameters.Add(new SqlParameter("number2", vehicleNumber2));
                    commDBupdate1.ExecuteNonQuery();
                }

                else
                {
                    string updateStatement1 = "UPDATE dbo.Assess_Journeys SET seatsStandC=seatsStandC-1 WHERE Id=@number2";
                    SqlCommand commDBupdate1 = new SqlCommand(updateStatement1, conn);
                    commDBupdate1.Parameters.Add(new SqlParameter("number2", vehicleNumber2));
                    commDBupdate1.ExecuteNonQuery();
                }
            }
            else
            {
                if (classTo == "First")
                {
                    string updateStatement = "UPDATE dbo.Assess_Journeys SET seatsFirstC=seatsFirstC-1 WHERE Id=@number";
                    SqlCommand commDBupdate = new SqlCommand(updateStatement, conn);
                    commDBupdate.Parameters.Add(new SqlParameter("number", vehicleNumber));
                    commDBupdate.ExecuteNonQuery();
                }
                else
                {
                    string updateStatement = "UPDATE dbo.Assess_Journeys SET seatsStandC=seatsStandC-1 WHERE Id=@number";
                    SqlCommand commDBupdate = new SqlCommand(updateStatement, conn);
                    commDBupdate.Parameters.Add(new SqlParameter("number", vehicleNumber));
                    commDBupdate.ExecuteNonQuery();
                }
            }
        }

        private void NewBooking_Load(object sender, EventArgs e)
        {
            textBox2.Text = user.ToString(); //automaticaly add salesRep ID (the staff login form sends the data)
        }

        private void button1_Click(object sender, EventArgs e) //retrieve and store the information entered by the user
        {
            int vehicleNumber2=0;
            int hotelID=0;
            int nights=1;
            int Custid =Int32.Parse(textBox1.Text);
            
            int salesId = Int32.Parse(textBox2.Text);
            int vehicleNumber = Int32.Parse(textBox3.Text);
            string bookingType = comboBox1.Text;
            

            string classTo;
            if      (radioButton1.Checked == true) { classTo="First";} 
            else                                   { classTo="Stand";}
            string classFrom;
            if      (radioButton2.Checked == true) { classFrom = "First"; }
            else                                   { classFrom = "Stand"; }

            double price=0.0;
            string vip;

            if (textBox5.Visible == true) { vehicleNumber2 = Int32.Parse(textBox5.Text); }
            if (textBox4.Visible == true) { hotelID = Int32.Parse(textBox4.Text); }
            if ((textBox6.Visible == true) && (textBox6.Text != "" && Int32.Parse(textBox6.Text)>0))
            { nights = Int32.Parse(textBox6.Text); }
           
                
            conn = new SqlConnection(connectionString); conn.Open();

            //get price for customer
                string selectStatement = "Select VIP from dbo.Assess_Customers where ID=@id";
                SqlCommand commDB1 = new SqlCommand(selectStatement, conn);
                commDB1.Parameters.Add(new SqlParameter("id", Custid));
                vip = commDB1.ExecuteScalar().ToString();

                string selectStatement2 = "Select price from dbo.Assess_Journeys where Id=@id";  //To Dest. price
                SqlCommand commDB2 = new SqlCommand(selectStatement2, conn);
                commDB2.Parameters.Add(new SqlParameter("id", vehicleNumber));
                    if (classTo == "Stand")
                    {
                        price += Convert.ToDouble(commDB2.ExecuteScalar());
                    }
                    else if (vip=="Yes")
                    {
                        price += (Convert.ToDouble(commDB2.ExecuteScalar())) * 1.10; //VIP customers get 5% off from First class tickets
                    }
                    else
                    {
                        price += (Convert.ToDouble(commDB2.ExecuteScalar())) * 1.15;
                    }

                if (vehicleNumber2 != 0) //return vehicle information entered
                {
                    string selectStatement3 = "Select price from dbo.Assess_Journeys where Id=@id"; //get price
                    SqlCommand commDB3 = new SqlCommand(selectStatement3, conn);
                    commDB3.Parameters.Add(new SqlParameter("id", vehicleNumber2));
                    if (classFrom == "Stand")
                    {
                        price += Convert.ToDouble(commDB3.ExecuteScalar());
                    }
                    else if (vip == "Yes")
                    {
                        price += (Convert.ToDouble(commDB3.ExecuteScalar())) * 1.10; //VIP customers get 5% off from First class tickets
                    }
                    else
                    {
                        price += (Convert.ToDouble(commDB3.ExecuteScalar())) * 1.15;
                    }
                }

                if (hotelID != 0) //hotel information entered
                {
                    string selectStatement3 = "Select price from dbo.Assess_Hotel where ID=@id"; //Hotel price
                    SqlCommand commDB3 = new SqlCommand(selectStatement3, conn);
                    commDB3.Parameters.Add(new SqlParameter("id", hotelID));
                    price += Convert.ToDouble(commDB3.ExecuteScalar()) * nights;
                }

                if (vip == "Yes") { price = price * 0.95; } // VIP customers also get 5% off the end price
                price = Math.Round(price, 2);

            //insert data into bookings table
                try
                {
                    if (hotelID != 0 && vehicleNumber2 != 0)
                    {
                        string insertStatement =
                        "INSERT INTO dbo.Assess_Booking (Bookingtype, vehicleNumberTo, vehicleNumberFrom, repID, customerId, hotelID, hotelNights, price, seatClassTo, seatClassFrom) VALUES (@type, @number, @number2, @repID, @customerId, @hotelID, @nights, @price, @classTo, @classFrom)";
                        SqlCommand commDB = new SqlCommand(insertStatement, conn);
                        commDB.Parameters.Add(new SqlParameter("type", bookingType));
                        commDB.Parameters.Add(new SqlParameter("number", vehicleNumber));
                        commDB.Parameters.Add(new SqlParameter("number2", vehicleNumber2));
                        commDB.Parameters.Add(new SqlParameter("repID", salesId));
                        commDB.Parameters.Add(new SqlParameter("customerId", Custid));
                        commDB.Parameters.Add(new SqlParameter("hotelID", hotelID));
                        commDB.Parameters.Add(new SqlParameter("nights", nights));
                        commDB.Parameters.Add(new SqlParameter("price", price));
                        commDB.Parameters.Add(new SqlParameter("classTo", classTo));
                        commDB.Parameters.Add(new SqlParameter("classFrom", classFrom));
                        commDB.ExecuteNonQuery();

                        UpdateSeats(bookingType, classTo, classFrom, vehicleNumber, vehicleNumber2, conn);
                       
                    }
                    else if (vehicleNumber2 != 0)
                    {
                        string insertStatement =
                        "INSERT INTO dbo.Assess_Booking (Bookingtype, vehicleNumberTo, vehicleNumberFrom, repID, customerId, price, seatClassTo, seatClassFrom) VALUES (@type, @number, @number2, @repID, @customerId, @price, @classTo, @classFrom)";
                        SqlCommand commDB = new SqlCommand(insertStatement, conn);
                        commDB.Parameters.Add(new SqlParameter("type", bookingType));
                        commDB.Parameters.Add(new SqlParameter("number", vehicleNumber));
                        commDB.Parameters.Add(new SqlParameter("number2", vehicleNumber2));
                        commDB.Parameters.Add(new SqlParameter("repID", salesId));
                        commDB.Parameters.Add(new SqlParameter("customerId", Custid));
                        commDB.Parameters.Add(new SqlParameter("price", price));
                        commDB.Parameters.Add(new SqlParameter("classTo", classTo));
                        commDB.Parameters.Add(new SqlParameter("classFrom", classFrom));
                        commDB.ExecuteNonQuery();

                        UpdateSeats(bookingType, classTo, classFrom, vehicleNumber, vehicleNumber2, conn);
                       
                    }
                    else
                    {
                        string insertStatement =
                        "INSERT INTO dbo.Assess_Booking (Bookingtype, vehicleNumberTo, repID, customerId, price, seatClassTo) VALUES (@type, @number,  @repID, @customerId, @price, @classTo)";
                        SqlCommand commDB = new SqlCommand(insertStatement, conn);
                        commDB.Parameters.Add(new SqlParameter("type", bookingType));
                        commDB.Parameters.Add(new SqlParameter("number", vehicleNumber));
                        commDB.Parameters.Add(new SqlParameter("repID", salesId));
                        commDB.Parameters.Add(new SqlParameter("customerId", Custid));
                        commDB.Parameters.Add(new SqlParameter("price", price));
                        commDB.Parameters.Add(new SqlParameter("classTo", classTo));
                        commDB.ExecuteNonQuery();

                        UpdateSeats(bookingType, classTo, classFrom, vehicleNumber, vehicleNumber2, conn);
                       
                    }

                    MessageBox.Show("Journey booked.");
                    
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }

                label7.Text = "Price: £"+price.ToString();
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 1) // if return booking was selected
            {
                label8.Visible = true; textBox5.Visible = true; groupBox2.Visible = true;
               
            }
            else if (comboBox1.SelectedIndex == 0) // if full booking was selected
            {
                label8.Visible = true; textBox5.Visible = true; groupBox2.Visible = true;
                label3.Visible = true; textBox4.Visible = true;
                label6.Visible = true; textBox6.Visible = true;
            }
        }

        private void button2_Click(object sender, EventArgs e) //create new customer
        {
            create CreateCustomer = new create();
            CreateCustomer.ShowDialog();
            CreateCustomer.user = user;
        }

        private void button3_Click(object sender, EventArgs e) //see all customers
        {
            customers details = new customers();
            details.Show();
        }

        



    }
}
