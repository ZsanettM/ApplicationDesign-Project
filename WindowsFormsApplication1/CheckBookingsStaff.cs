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
    public partial class CheckBookingsStaff : Form
    {
        public CheckBookingsStaff()
        {
            InitializeComponent();
        }

        private string connectionString = "Data Source=vmwsql07.uad.ac.uk; Initial Catalog=sql1501846; User ID=sql1501846; Password=D&1jGr&4";
        private string selectStatement;
        private SqlCommand commDB;

        private SqlConnection SetUpConn() //connection
        {
            SqlConnection conn;
            conn = new SqlConnection(connectionString);
            return conn;
        }

        private void DisplayDataGrid(SqlCommand command) //Display selected data in gridDataView
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

        public string getUser;

        private void button1_Click(object sender, EventArgs e)
        {
            SetUpConn().Open();
            int id = 0;

            if (listBox1.SelectedIndex == 0) //diplay bookings data where customerID=id
            {
                if (textBox1.Text != "")
                {
                    try
                    {
                        id = Int32.Parse(textBox1.Text);
                    }
                    catch  { MessageBox.Show("Input is not a valid number"); }

                    selectStatement = "SELECT * FROM dbo.BookingDetails WHERE customerID=@ID";
                    commDB = new SqlCommand(selectStatement, SetUpConn());
                    commDB.Parameters.Add(new SqlParameter("ID", id));
                    try
                    {
                        //display data in grid
                        DisplayDataGrid(commDB);
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                }
                else { MessageBox.Show("No input provided"); }
            }
            else if (listBox1.SelectedIndex == 1) //diplay bookings data where bookingID=id
            {

                if (textBox1.Text != "")
                {
                    try
                    {
                        id = Int32.Parse(textBox1.Text);
                    }
                    catch (Exception ex) { MessageBox.Show("Input is not a valid number"); }

                    selectStatement = "SELECT * FROM dbo.BookingDetails WHERE bookingID=@ID";
                    commDB = new SqlCommand(selectStatement, SetUpConn());
                    commDB.Parameters.Add(new SqlParameter("ID", id));
                    try
                    {
                        //display data in grid
                        DisplayDataGrid(commDB);
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                }
                else { MessageBox.Show("No input provided"); }
            }
            else if (listBox1.SelectedIndex == 2) //diplay bookings that were made in a given time period
            {

                string date1 = dateTimePicker1.Value.ToString("yyyy-MM-dd");
                string date2 = dateTimePicker2.Value.ToString("yyyy-MM-dd");

                selectStatement = "SELECT * FROM dbo.BookingDetails WHERE Created_On_Date BETWEEN @date1 and @date2";
                commDB = new SqlCommand(selectStatement, SetUpConn());
                commDB.Parameters.Add(new SqlParameter("date1", date1));
                commDB.Parameters.Add(new SqlParameter("date2", date2));
                try
                {
                    //display data in grid
                    DisplayDataGrid(commDB);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
            else if (listBox1.SelectedIndex == 3) //display all bookings
            {
               
                //display data in grid
                selectStatement = "SELECT * FROM dbo.BookingDetails";
                commDB = new SqlCommand(selectStatement, SetUpConn());
                commDB.Parameters.Add(new SqlParameter("ID", id));
                try
                {
                    //display data in grid
                    DisplayDataGrid(commDB);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) //change labelText and make the appropriate form elements visible or invisible
        {
            if (listBox1.SelectedIndex == 0)
            {
                label2.Visible = false;
                label3.Visible = false;
                dateTimePicker1.Visible = false;
                dateTimePicker2.Visible = false;
                label1.Visible = true;
                textBox1.Visible = true;
                label1.Text = "Customer ID: ";
                button1.Enabled = true;
                
            }
            else if (listBox1.SelectedIndex == 1)
            {
                label2.Visible = false;
                label3.Visible = false;
                dateTimePicker1.Visible = false;
                dateTimePicker2.Visible = false;
                label1.Visible = true;
                textBox1.Visible = true;
                label1.Text = "Booking ID: ";
                button1.Enabled = true;
                
            }
            else if (listBox1.SelectedIndex == 2)
            {
                label2.Visible = true;
                label3.Visible = true;
                dateTimePicker1.Visible = true;
                dateTimePicker2.Visible = true;
                label1.Visible = false;
                textBox1.Visible = false;
                button1.Enabled = true;
               
            }
            else if (listBox1.SelectedIndex == 3)
            {
                label2.Visible = false;
                label3.Visible = false;
                dateTimePicker1.Visible = false;
                dateTimePicker2.Visible = false;
                label1.Visible = false;
                textBox1.Visible = false;
                button1.Enabled = true;
                
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e) //when a rep clicks on certain cells, display more information associated with its value
        {
            richTextBox1.Text = "";

            SqlConnection conn;

            string connectionString = "Data Source=vmwsql07.uad.ac.uk; Initial Catalog=sql1501846; User ID=sql1501846; Password=D&1jGr&4";
            conn = new SqlConnection(connectionString);
            conn.Open();

            int retrieve;
            int bookingID;
            bool converted = false;

            if (e.ColumnIndex == 2 || e.ColumnIndex == 5) //if the cell is in the vehicleNumberTo or vehicleNumberFrom column, retrieve journey information associated with the vehicleID
            {
                try
                {
                    retrieve = (int)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                }
                catch { retrieve = 0; }

                string selectStatement = "SELECT departureLoc, arrivalLoc, departureDate,departureTime, operator, type FROM Assess_Journeys WHERE Id=@number";
                SqlCommand commDB = new SqlCommand(selectStatement, conn);
                commDB.Parameters.Add(new SqlParameter("number", retrieve));

                using (SqlDataReader sqlreader = commDB.ExecuteReader())
                {
                    if (sqlreader.Read()) //Display retrieved information in a richTextBox (at the top-right corner of the form)
                    {
                        string departure = sqlreader.GetString(sqlreader.GetOrdinal("departureLoc"));
                        string arrival = sqlreader.GetString(sqlreader.GetOrdinal("arrivalLoc"));
                        string date = sqlreader.GetDateTime(sqlreader.GetOrdinal("departureDate")).ToString("yyyy-MM-dd");
                        string time = sqlreader.GetSqlValue(sqlreader.GetOrdinal("departureTime")).ToString();
                        string op = sqlreader.GetString(sqlreader.GetOrdinal("operator"));
                        string type = sqlreader.GetString(sqlreader.GetOrdinal("type")).ToUpper();


                        richTextBox1.Text = " Departures from: " + departure + " - Arrives to: " + arrival + "\n Departures at: " + date + " " + time + "\n Operator: " + op + "\n " + type;
                    }
                }
            }
            else if (e.ColumnIndex == 9) //if the cell is in the HotelID column, retrieve hotel information associated with the ID
            {

                try
                {
                    retrieve = (int)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                }
                catch { retrieve = 0; }

                string selectStatement = "SELECT name,stars, city, country, price FROM Assess_Hotel WHERE ID=@number";
                SqlCommand commDB = new SqlCommand(selectStatement, conn);
                commDB.Parameters.Add(new SqlParameter("number", retrieve));

                using (SqlDataReader sqlreader = commDB.ExecuteReader())
                {
                    if (sqlreader.Read()) //Display retrieved information in a richTextBox (at the top-right corner of the form)
                    {
                        string name = sqlreader.GetString(sqlreader.GetOrdinal("name"));
                        string stars = sqlreader.GetSqlValue(sqlreader.GetOrdinal("stars")).ToString();
                        string city = sqlreader.GetString(sqlreader.GetOrdinal("city"));
                        string country = sqlreader.GetString(sqlreader.GetOrdinal("country"));
                        string price = sqlreader.GetSqlDouble(sqlreader.GetOrdinal("price")).ToString();


                        richTextBox1.Text = " Name: " + name + "\n City: " + city + ", Country: " + country + "\n Stars: " + stars + "\n Price: £" + price;
                    }
                }
            }

            else if (e.ColumnIndex == 4) //if the cell is in the extrasTo column, retrieve addedd extras information associated with the BookingID and vehicleNumber
            {
                try
                {
                    converted = (bool)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                }
                catch { }

                if (converted == false) { richTextBox1.Text = "No extras added"; }
                else
                {
                    bookingID = (int)dataGridView1.Rows[e.RowIndex].Cells[0].Value;
                    retrieve = (int)dataGridView1.Rows[e.RowIndex].Cells[2].Value;
                    string selectStatement = "SELECT vehicleType, WiFi, Meal, PreselectSeat, SeatNumber FROM Assess_Extras WHERE bookingID=@id and vehicleNumber=@number";
                    SqlCommand commDB = new SqlCommand(selectStatement, conn);
                    commDB.Parameters.Add(new SqlParameter("id", bookingID));
                    commDB.Parameters.Add(new SqlParameter("number", retrieve));

                    using (SqlDataReader sqlreader = commDB.ExecuteReader())
                    {
                        if (sqlreader.Read()) //Display retrieved information in a richTextBox (at the top-right corner of the form)
                        {
                            string type = sqlreader.GetString(sqlreader.GetOrdinal("vehicleType"));
                            string wifi = sqlreader.GetBoolean(sqlreader.GetOrdinal("WiFi")).ToString();
                            string meal = sqlreader.GetBoolean(sqlreader.GetOrdinal("Meal")).ToString();
                            string pre = sqlreader.GetBoolean(sqlreader.GetOrdinal("PreselectSeat")).ToString();
                            string seat = sqlreader.GetInt32(sqlreader.GetOrdinal("SeatNumber")).ToString();
                            if (seat == "0") { seat = " - "; }

                            richTextBox1.Text = type.ToUpper() + "\n WiFi included: " + wifi + "\n Meal included: " + meal + "\n Seat selected: " + pre + "\n Seat number: " + seat;
                        }
                    }
                }
            }
            else if (e.ColumnIndex == 7) //if the cell is in the extrasFrom column, retrieve addedd extras information associated with the BookingID and vehicleNumber
            {
                try
                {
                    converted = (bool)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                }
                catch { }

                if (converted == false) { richTextBox1.Text = "No extras added"; }
                else
                {
                    bookingID = (int)dataGridView1.Rows[e.RowIndex].Cells[0].Value;
                    retrieve = (int)dataGridView1.Rows[e.RowIndex].Cells[5].Value;
                    string selectStatement = "SELECT vehicleType, WiFi, Meal, PreselectSeat, SeatNumber FROM Assess_Extras WHERE bookingID=@id and vehicleNumber=@number";
                    SqlCommand commDB = new SqlCommand(selectStatement, conn);
                    commDB.Parameters.Add(new SqlParameter("id", bookingID));
                    commDB.Parameters.Add(new SqlParameter("number", retrieve));

                    using (SqlDataReader sqlreader = commDB.ExecuteReader())
                    {
                        if (sqlreader.Read()) //Display retrieved information in a richTextBox (at the top-right corner of the form)
                        {
                            string type = sqlreader.GetString(sqlreader.GetOrdinal("vehicleType"));
                            string wifi = sqlreader.GetBoolean(sqlreader.GetOrdinal("WiFi")).ToString();
                            string meal = sqlreader.GetBoolean(sqlreader.GetOrdinal("Meal")).ToString();
                            string pre = sqlreader.GetBoolean(sqlreader.GetOrdinal("PreselectSeat")).ToString();
                            string seat = sqlreader.GetInt32(sqlreader.GetOrdinal("SeatNumber")).ToString();
                            if (seat == "0") { seat = " - "; }

                            richTextBox1.Text = type.ToUpper() + "\n WiFi included: " + wifi + "\n Meal included: " + meal + "\n Seat selected: " + pre + "\n Seat number: " + seat;
                        }
                    }
                }
            }

            else if (e.ColumnIndex == 13) //Customer IDs
            {
                retrieve = (int)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

                string selectStatement = "SELECT name, postcode, VIP FROM Assess_Customers WHERE ID=@id";
                SqlCommand commDB = new SqlCommand(selectStatement, conn);
                commDB.Parameters.Add(new SqlParameter("id", retrieve));

                using (SqlDataReader sqlreader = commDB.ExecuteReader())
                {
                    if (sqlreader.Read()) //Display retrieved information in a richTextBox (at the top-right corner of the form)
                    {
                        string name = sqlreader.GetString(sqlreader.GetOrdinal("name"));
                        string post = sqlreader.GetString(sqlreader.GetOrdinal("postcode"));
                        string vip = sqlreader.GetString(sqlreader.GetOrdinal("VIP"));

                        richTextBox1.Text = " Name: " + name + "\n Postcode: " + post + "\n VIP: " + vip;
                    }
                }
            }
        }
    }
}
