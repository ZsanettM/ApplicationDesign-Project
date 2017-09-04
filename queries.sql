
Create table Assess_Hotel (ID INT NOT NULL IDENTITY(1,1) PRIMARY KEY, name varchar (30) not null, stars smallint not null,
city varchar(30) not null, country varchar (30) not null, price float not null );

Create table Assess_Customers (ID INT NOT NULL IDENTITY(1,1) PRIMARY KEY, name varchar(35) not null, postcode varchar (10) not null, 
VIP varchar(4) not null, password varchar(20) not null);

Create table Assess_Bookings (
bookingID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
bookingType varchar(10) not null,
vehicleNumberTo int FOREIGN KEY REFERENCES Assess_Journeys (Id) not null,
seatClassTo varchar(10) not null,
vehicleNumberFrom int FOREIGN KEY REFERENCES Assess_Journeys (Id) default null,
seatClassFrom varchar(10) default null,
repID int FOREIGN KEY REFERENCES Assess_SaleReps (ID) not null,
customerID int FOREIGN KEY REFERENCES Assess_Customers (ID) not null,
hotelID  int FOREIGN KEY REFERENCES Assess_Hotel (ID) default null,
hotelNights int default null,
extras bit not null,
price float not null);

Create table Assess_Extras (
bookingID int FOREIGN KEY REFERENCES Assess_Booking (bookingID) not null,
vehicleNumber int FOREIGN KEY REFERENCES Assess_Journeys (Id) not null,
vehicleType varchar(10) not null,
WiFi bit default 0,
Meal bit default 0,
PreselectSeat bit default 0,
SeatNumber int default 0,
unique (SeatNumber, vehicleNumber),
unique (vehicleNumber, bookingID)
);


CREATE VIEW RepDetails
AS SELECT Assess_SaleReps.ID, Assess_SaleReps.fullName AS Name,ROUND(SUM(Assess_Booking.price),2 )AS RevenueGenerated, COUNT(Assess_Booking.bookingID) AS NumberOfSales 
FROM dbo.Assess_SaleReps INNER JOIN
dbo.Assess_Booking ON dbo.Assess_SaleReps.ID = dbo.Assess_Booking.repID
WHERE Assess_Booking.Created_On_Date BETWEEN @date1 AND @date2)
GROUP BY Assess_SaleReps.ID,Assess_SaleReps.fullName

CREATE VIEW BookingDetails
AS SELECT Assess_Booking.bookingID,
Assess_Booking.vehicleNumberTo AS VehicleID,
Assess_Journeys.arrivalLoc 
Assess_Booking.vehicleNumberFrom AS VehicleID_Back,
Assess_Hotel.name AS Hotel_Name,
Assess_Booking.price As Total_Cost,
Assess_Booking.extras
FROM Assess_Booking, Assess_Hotel, Assess_Journeys WHERE Assess_Hotel.ID = Assess_Booking.hotelID

/* Create View CustomersBookings AS SELECT Assess_Booking.bookingID, Assess_Booking.bookingType, Assess_Booking.vehicleNumberTo, Assess_Booking.seatClassTo,
Assess_Booking.vehicleNumberFrom, Assess_Booking.seatClassFrom ,Assess_Booking.Created_On_Date, Assess_Hotel.name AS Hotel_name, Assess_Booking.hotelNights,Assess_SaleReps.fullName AS Sales_Rep, Assess_Booking.extras,
Assess_Booking.price
FROM Assess_Booking, Assess_Hotel, Assess_SaleReps WHERE 
repID=Assess_SaleReps.ID AND hotelID = Assess_Hotel.ID Or Assess_Booking.hotelID is null Or Assess_Booking.vehicleNumberFrom is null */

Create View Cust AS SELECT Assess_Booking.bookingID, Assess_Booking.bookingType, Assess_Booking.vehicleNumberTo, Assess_Booking.seatClassTo,
CASE WHEN EXISTS (
    SELECT *
    FROM Assess_Extras
    WHERE Assess_Extras.bookingID = Assess_Booking.bookingID AND Assess_Booking.vehicleNumberTo = Assess_Extras.vehicleNumber
)
THEN CAST(1 AS BIT)
ELSE CAST(0 AS BIT) END As extrasTo,

Assess_Booking.vehicleNumberFrom, Assess_Booking.seatClassFrom ,
CASE WHEN EXISTS (
    SELECT *
    FROM Assess_Extras
    WHERE Assess_Extras.bookingID = Assess_Booking.bookingID AND Assess_Booking.vehicleNumberFrom = Assess_Extras.vehicleNumber
)
THEN CAST(1 AS BIT)
ELSE CAST(0 AS BIT) END As extrasFrom,
Assess_Booking.Created_On_Date, Assess_Booking.hotelID, Assess_Booking.hotelNights,Assess_SaleReps.fullName AS Sales_Rep,
Assess_Booking.price, Assess_Booking.customerID
FROM Assess_Booking, Assess_Hotel, Assess_SaleReps WHERE 
repID=Assess_SaleReps.ID Or Assess_Booking.vehicleNumberFrom is null