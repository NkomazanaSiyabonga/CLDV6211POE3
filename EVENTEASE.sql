
-- DATABASE CREATION SCRIPT
USE master;
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'EVENTEASE')
    DROP DATABASE EVENTEASE;
CREATE DATABASE EVENTEASE;
USE EVENTEASE;

-- Create Venue table
CREATE TABLE Venue (
    VenueId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Location NVARCHAR(200) NOT NULL,
    Capacity INT NOT NULL,
    ImageUrl NVARCHAR(255),
    CONSTRAINT CHK_CapacityPositive CHECK (Capacity > 0)
);
ALTER TABLE Venue ADD ImageFileName NVARCHAR(255);

-- Create EventType table
CREATE TABLE EventType (
    EventTypeID INT IDENTITY (1,1) PRIMARY KEY,
    Name NVARCHAR (100) NOT NULL
);

-- Create Event table with renamed "EventName" column
CREATE TABLE Event (
    EventId INT PRIMARY KEY IDENTITY(1,1),
    EventName NVARCHAR(100) NOT NULL,
    EventDate DATETIME NOT NULL,
    Description NVARCHAR(MAX),
    VenueId INT NOT NULL,
    EventTypeID INT NULL,
    CONSTRAINT FK_Event_Venue FOREIGN KEY (VenueId) REFERENCES Venue(VenueId),
    FOREIGN KEY (EventTypeID) REFERENCES EventType(EventTypeID)
);

-- Create Booking table
CREATE TABLE Booking (
    BookingId INT PRIMARY KEY IDENTITY(1,1),
    VenueId INT NOT NULL,
    EventId INT NOT NULL,
    BookingDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Booking_Venue FOREIGN KEY (VenueId) REFERENCES Venue(VenueId),
    CONSTRAINT FK_Booking_Event FOREIGN KEY (EventId) REFERENCES Event(EventId),
    CONSTRAINT UQ_VenueEvent UNIQUE (VenueId, EventId)
);

-- Create indexes
CREATE INDEX IX_Booking_VenueDate ON Booking(VenueId, BookingDate);
CREATE INDEX IX_Event_Dates ON Event(EventDate);

-- Insert EventTypes
INSERT INTO EventType (Name)
VALUES 
('Motoring Race'),
('Football Match'),
('Product Launch');

-- Insert sample venues
INSERT INTO Venue (Name, Location, Capacity, ImageUrl)
VALUES
('Kyalami Race Track', 'Midrand, South Africa', 500, 'https://www.google.com/url?sa=i&url=https%3A%2F%2Fwww.planetf1.com%2Fnews%2Fafrican-grand-prix-f1-update-kyalami&psig=AOvVaw3l_XyNCTlmPeDtj4RK3U8m&ust=1747246071847000&source=images&cd=vfe&opi=89978449&ved=0CBQQjRxqFwoTCNC36f-EoY0DFQAAAAAdAAAAABAE'),
('Santiago Bernabeu', 'Madrid, Spain', 70000, 'https://www.spain.info/export/sites/segtur/.content/imagenes/cabeceras-grandes/madrid/santiago-bernabeu_c-david-benito-shutterstock-s2468268069.jpg_1014274486.jpg'),
('Apple Headquarters', 'California, USA', 500, 'https://www.google.com/url?sa=i&url=https%3A%2F%2Fwww.fosterandpartners.com%2Fprojects%2Fapple-park%2F&psig=AOvVaw0BSbobNGq0q99aDoj_jUM5&ust=1747245840530000&source=images&cd=vfe&opi=89978449&ved=0CBQQjRxqFwoTCJDF_I-EoY0DFQAAAAAdAAAAABAE');

-- Insert sample events
INSERT INTO Event (EventName, EventDate, Description, VenueId, EventTypeID)
VALUES
('Automotive Event', '2026-10-21 09:00:00', 'Motoring Festival', 1,1),
('FIFA Football Match', '2026-12-12 17:00:00', 'FIFA World Cup Final Match', 2,2),
('Apple device and software Launch', '2026-03-05 10:00:00', 'New product launch', 3,3);

-- Insert sample bookings
INSERT INTO Booking (VenueId, EventId, BookingDate)
VALUES 
(1, 1, '2023-01-15 10:00:00'),
(2, 2, '2023-02-20 11:30:00'),
(3, 3, '2023-03-10 09:15:00');

-- View data
SELECT * FROM Venue;
SELECT * FROM Event;
SELECT * FROM Booking;
SELECT * FROM EventType;


-- (Commented out for testing; remove if you want to drop after verification)
 DROP TABLE Booking;
 DROP TABLE Event;
 DROP TABLE Venue;
