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
    public partial class checkBookings : Form
    {
        public checkBookings()
        {
            InitializeComponent();
        }

        public int id = 0;
        public string password;

        //connection elemenets
        SqlConnection conn;
        string connectionString = "Data Source=vmwsql07.uad.ac.uk; Initial Catalog=sql1501846; User ID=sql1501846; Password=D&1jGr&4";
        private string selectStatement;

        private void gridDisplay(SqlCommand commDB) //Display selected data in gridDataView
        {
            SqlDataAdapter dAdapt = new SqlDataAdapter();
            dAdapt.SelectCommand = commDB;
            DataTable dTable = new DataTable();
            dAdapt.Fill(dTable);
            BindingSource bSource = new BindingSource();
            bSource.DataSource = dTable;
            dataGridView1.DataSource = bSource;
            dAdapt.Update(dTable);
            conn.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            button1.BackColor = SystemColors.Control; //format form
            button1.Text = "Search";
            textBox2.Text = ""; textBox4.Text = "";
            groupBox2.Enabled = false;


            conn = new SqlConnection(connectionString); //set up connection
            conn.Open();

            if (textBox1.Text != "" && textBox3.Text != "") //only proceed with query if there was input entered
            {
                try
                {
                    id = Int32.Parse(textBox1.Text);    //try to convert entered id string
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                password = textBox3.Text;

                try
                {

                    selectStatement = "SELECT name FROM dbo.Assess_Customers WHERE ID=@ID and password=@pwd";
                    SqlCommand commDB = new SqlCommand(selectStatement, conn);
                    commDB.Parameters.Add(new SqlParameter("ID", id));
                    commDB.Parameters.Add(new SqlParameter("pwd", password));
                    if (Convert.ToString(commDB.ExecuteScalar()) == "") //let the user know if there is no match in the database for the entered login details
                    {
                        MessageBox.Show("Incorrect login data");
                        checkBookings newCheck = new checkBookings();
                        this.Hide();
                        newCheck.ShowDialog();
                        this.Close();
                    }
                    else //successful login
                    {
                        groupBox1.Enabled = true;
                        label10.Text = "Hi " + Convert.ToString(commDB.ExecuteScalar());

                        label2.Enabled = true; label3.Enabled = true; label8.Enabled = true; label10.Visible = true;
                        textBox2.Enabled = true; textBox4.Enabled = true; button4.Enabled = true;

                        conn.Close(); conn.Open();
                        string select = "select * from dbo.BookingDetails where customerID=@ID";
                        SqlCommand commDB1 = new SqlCommand(select, conn);
                        commDB1.Parameters.Add(new SqlParameter("ID", id));

                        try
                        {
                            gridDisplay(commDB1); //Display customer's booking  details
                            conn.Close();
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }

            }
            else { MessageBox.Show("Incorrect login data"); }

        } //Login process && retrieve booking data

        int vehicleNum = 0;
        int bookingID = 0;
        string type = "";
        string seatClass = "";

        private void button4_Click(object sender, EventArgs e) //BookingId - Vehicle number pair validation 
        {
            checkBox1.Checked = false; checkBox2.Checked = false; checkBox3.Checked = false;

            if (textBox2.Text != "" && textBox4.Text != "")
            {
                bookingID = Int32.Parse(textBox2.Text);
                vehicleNum = Int32.Parse(textBox4.Text);

                try
                {
                    
                    conn = new SqlConnection(connectionString);
                    conn.Open();

                    selectStatement = "SELECT * FROM dbo.Assess_Booking WHERE bookingID=@ID and vehicleNumberTo=@num and CustomerID=@user";
                    SqlCommand commDB = new SqlCommand(selectStatement, conn);
                    commDB.Parameters.Add(new SqlParameter("ID", bookingID));
                    commDB.Parameters.Add(new SqlParameter("num", vehicleNum));
                    commDB.Parameters.Add(new SqlParameter("user", id));

                    string selectStatement2 = "SELECT * FROM dbo.Assess_Booking WHERE bookingID=@ID and vehicleNumberFrom=@num and CustomerID=@user";
                    SqlCommand commDB2 = new SqlCommand(selectStatement2, conn);
                    commDB2.Parameters.Add(new SqlParameter("ID", bookingID));
                    commDB2.Parameters.Add(new SqlParameter("num", vehicleNum));
                    commDB2.Parameters.Add(new SqlParameter("user", id));

                    if (Convert.ToString(commDB.ExecuteScalar()) == "" && Convert.ToString(commDB2.ExecuteScalar()) == "") //check whether either the bookingId with the vehicleNumberTo or the bookingId with the vehicleNumberFrom has a match in the database
                    {
                        MessageBox.Show("Invalid input");
                        conn.Close(); conn.Open();
                    }
                    
                    else
                    {
                        if (Convert.ToString(commDB.ExecuteScalar()) != "") //retrieve seat class (1st class or standard)
                        {
                            
                            SqlDataReader dr = commDB.ExecuteReader();
                            while (dr.Read()) {
                                seatClass = dr["seatClassTo"].ToString();
                            }
                            conn.Close(); conn.Open();
                        }
                        else if (Convert.ToString(commDB2.ExecuteScalar()) != "")
                        {
                            
                            SqlDataReader dr = commDB2.ExecuteReader();
                            while (dr.Read()) {
                                seatClass = dr["seatClassFrom"].ToString();                        
                            }
                            conn.Close(); conn.Open();
                        }

                        string sStatement = "SELECT type FROM Assess_Journeys WHERE ID=@id";
                        SqlCommand commDBSelect = new SqlCommand(sStatement, conn);
                        commDBSelect.Parameters.Add(new SqlParameter("id", vehicleNum));

                        type = Convert.ToString(commDBSelect.ExecuteScalar());
                        if (type == "Flight") //Decide what kind of extras the customer can add (Flights - meals,preselectedSeats || Trains - wifi)
                        {
                                checkBox2.Enabled = true; checkBox3.Enabled = true; textBox5.Enabled = true;
                                label6.Enabled = true; label7.Enabled = true; label9.Visible = true; textBox5.Visible = true;
                                button2.Enabled = true;
                                checkBox1.Enabled = false; label5.Enabled = false;
                                groupBox2.Enabled = true;
                        }
                        else
                        {
                                checkBox2.Enabled = false; checkBox3.Enabled = false; textBox5.Enabled = false;
                                label6.Enabled = false; label7.Enabled = false; label9.Visible = false; textBox5.Visible = false;
                                checkBox1.Enabled = true; label5.Enabled = true; button2.Enabled = true;
                                groupBox2.Enabled = true;
                        }
                     }
                   }


                
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.BackColor = System.Drawing.Color.Lime;
            button1.Text = "Refresh";
            richTextBox1.Text = "";

            conn = new SqlConnection(connectionString);
            conn.Open();


            //save extras
            int wifi = 0; int meals = 0; int preselect = 0; int seatNum = 0;
            int minSeat = 1; int maxSeat = 212;

            //define selectable seat number range
            if (seatClass=="First"){
                minSeat = 1; maxSeat = 61; //first class seats
            }
            else{
                minSeat = 62; maxSeat = 212; //standard class seats
            }

            float addCost = 0; //store cost of added extras

            if (checkBox1.Checked == true) { wifi = 1; addCost = addCost + 2; }
            if (checkBox2.Checked == true) { meals = 1; addCost = addCost + 8; }
            if(checkBox3.Checked==true)
            {
                 
                if (textBox5.Text != "" && textBox5.Text !="0"){ //Check wether the entered seat number is valid
                    try
                    {
                        seatNum = Int32.Parse(textBox5.Text);
                        if (seatNum >= minSeat && seatNum <= maxSeat)
                        {
                            preselect = 1;
                            addCost = addCost + 4;
                        }
                        else
                        {
                            MessageBox.Show(" First class seat numbers: 1-61 \n Standard class seat numbers: 62-212"); 
                        }
                        
                    }
                    catch (Exception ex) {
                        MessageBox.Show(ex.Message);
                        preselect = 0;
                    }
                }
                else { MessageBox.Show("No seat number provided, seat reservation data will not be updated."); }
            }
            try
            {
                if (preselect == 0 && meals == 0 && wifi == 0) //if no valid extras were selected, do not add to Assess_Extras table
                {
                    MessageBox.Show("Nothing was selected, extras will not be added");
                }
                else
                {

                    string insertStatement = "INSERT INTO dbo.Assess_Extras VALUES (@ID, @num, @type, @wifi, @meals,@pre,@seat)";
                    SqlCommand commDB = new SqlCommand(insertStatement, conn);
                    commDB.Parameters.Add(new SqlParameter("ID", bookingID));
                    commDB.Parameters.Add(new SqlParameter("num", vehicleNum));
                    commDB.Parameters.Add(new SqlParameter("type", type));
                    commDB.Parameters.Add(new SqlParameter("wifi", wifi));
                    commDB.Parameters.Add(new SqlParameter("meals", meals));
                    commDB.Parameters.Add(new SqlParameter("pre", preselect));
                    commDB.Parameters.Add(new SqlParameter("seat", seatNum));
                    commDB.ExecuteNonQuery();

                    MessageBox.Show("Extras added");
                }
            }
            catch (SqlException ex) {
                if (ex.Number == 2627)  //if a record already exists in Assess_Extras table, only update it, keep already stored extras associated with the journey
                {
                    int meals2 = 0; int preselect2 = 0; int seatNum2 = 0; addCost = 0;
                                        

                    string selectStatement2 = "SELECT PreselectSeat, Meal, SeatNumber from dbo.Assess_Extras WHERE BookingID=@ID and vehicleNumber=@num";
                    SqlCommand commDB2 = new SqlCommand(selectStatement2, conn);
                    commDB2.Parameters.Add(new SqlParameter("ID", bookingID));
                    commDB2.Parameters.Add(new SqlParameter("num", vehicleNum));
                    SqlDataReader dr = commDB2.ExecuteReader();

                    //retrieve already stored extras info, so previously added extras would not be lost, and the customer will not get charged twice
                    while (dr.Read())
                    {
                        meals2 = Convert.ToInt32(dr["Meal"]);
                        preselect2 = Convert.ToInt32(dr["PreselectSeat"]);
                        seatNum2 = Convert.ToInt32(dr["SeatNumber"]);
                    }
                    
                    if (preselect2 == 1 && preselect == 0)
                    {
                        preselect = preselect2; seatNum = seatNum2;
                    }
                    else if (preselect2 == 0 && preselect == 1)
                    {
                        addCost = addCost + 4;
                    }
                    if (meals2 == 1 && meals == 0)
                    {
                        meals = meals2;
                    }
                    else if (meals2 == 0 && meals == 1)
                    {
                        addCost = addCost + 8;
                    }
                    conn.Close(); conn.Open();


                    string insertStatement = "UPDATE dbo.Assess_Extras SET WiFi=@wifi, Meal=@meals, PreselectSeat=@pre, SeatNumber=@seat where bookingID=@ID and vehicleNumber=@num";
                    SqlCommand commDB = new SqlCommand(insertStatement, conn);
                    commDB.Parameters.Add(new SqlParameter("ID", bookingID));
                    commDB.Parameters.Add(new SqlParameter("num", vehicleNum));
                    commDB.Parameters.Add(new SqlParameter("type", type));
                    commDB.Parameters.Add(new SqlParameter("wifi", wifi));
                    commDB.Parameters.Add(new SqlParameter("meals", meals));
                    commDB.Parameters.Add(new SqlParameter("pre", preselect));
                    commDB.Parameters.Add(new SqlParameter("seat", seatNum));

                    try
                    {
                        commDB.ExecuteNonQuery();
                        MessageBox.Show("Extras updated");
                    }
                    catch (Exception ex1) { MessageBox.Show(ex1.Message); }
                }
                else { MessageBox.Show(ex.Message); }
            }
            groupBox2.Enabled = false;
            //Add the cost of selected extras to the journey price
            string updateStatement = "UPDATE dbo.Assess_Booking SET price=price+@p where bookingID=@ID";
            SqlCommand commDB1 = new SqlCommand(updateStatement, conn);
            commDB1.Parameters.Add(new SqlParameter("p", addCost));
            commDB1.Parameters.Add(new SqlParameter("ID", bookingID));
            try
            {
                commDB1.ExecuteNonQuery();
                MessageBox.Show("Booking price updated +£"+ addCost.ToString());
            }
            catch (Exception ex1) { MessageBox.Show(ex1.Message); }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e) //When the user clicks on a grid cell display more information associated with its value
        {
            richTextBox1.Text = "";

            SqlConnection conn;

            string connectionString = "Data Source=vmwsql07.uad.ac.uk; Initial Catalog=sql1501846; User ID=sql1501846; Password=D&1jGr&4";
            conn = new SqlConnection(connectionString);
            conn.Open();

            int retrieve;
            int bookingID;
            bool converted=false;

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


                        richTextBox1.Text = " Departures from: " + departure + " - Arrives to: " +arrival+ "\n Departures at: "+date+" "+time +"\n Operator: "+op + "\n "+type;
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
        }

        private void checkBookings_Load(object sender, EventArgs e)
        {
            groupBox1.Enabled = false;

            toolTip1.SetToolTip(this.textBox5, "First class seats: 1-61 \n Standard seats: 62-212");
            toolTip1.SetToolTip(this.checkBox3, "First class seats: 1-61 \n Standard seats: 62-212");
        }






       
    }
}
