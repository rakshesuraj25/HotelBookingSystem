-- MySQL dump 10.13  Distrib 8.4.11, for Win64 (x86_64)
--
-- Host: localhost    Database: hotel_booking_db
-- ------------------------------------------------------
-- Server version	8.4.11

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Current Database: `hotel_booking_db`
--

CREATE DATABASE /*!32312 IF NOT EXISTS*/ `hotel_booking_db` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;

USE `hotel_booking_db`;

--
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `applicationusers`
--

DROP TABLE IF EXISTS `applicationusers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `applicationusers` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `FullName` varchar(100) NOT NULL,
  `Email` varchar(150) NOT NULL,
  `PasswordHash` varchar(255) NOT NULL,
  `PhoneNumber` varchar(20) DEFAULT NULL,
  `Role` varchar(20) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `IsActive` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_ApplicationUsers_Email` (`Email`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `bookings`
--

DROP TABLE IF EXISTS `bookings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bookings` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `BookingReference` varchar(30) NOT NULL,
  `UserId` int NOT NULL,
  `RoomId` int NOT NULL,
  `CheckInDate` datetime(6) NOT NULL,
  `CheckOutDate` datetime(6) NOT NULL,
  `NumberOfGuests` int NOT NULL,
  `NumberOfNights` int NOT NULL,
  `PricePerNight` decimal(10,2) NOT NULL,
  `TotalAmount` decimal(10,2) NOT NULL,
  `BookingStatus` varchar(30) NOT NULL,
  `SpecialRequests` varchar(500) DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Bookings_BookingReference` (`BookingReference`),
  KEY `IX_Bookings_BookingStatus` (`BookingStatus`),
  KEY `IX_Bookings_RoomId_CheckInDate_CheckOutDate` (`RoomId`,`CheckInDate`,`CheckOutDate`),
  KEY `IX_Bookings_UserId` (`UserId`),
  CONSTRAINT `FK_Bookings_ApplicationUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `applicationusers` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Bookings_Rooms_RoomId` FOREIGN KEY (`RoomId`) REFERENCES `rooms` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `rooms`
--

DROP TABLE IF EXISTS `rooms`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `rooms` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `RoomNumber` varchar(20) NOT NULL,
  `Name` varchar(100) NOT NULL,
  `Description` varchar(1000) DEFAULT NULL,
  `RoomType` varchar(30) NOT NULL,
  `PricePerNight` decimal(10,2) NOT NULL,
  `Capacity` int NOT NULL,
  `BedType` varchar(50) NOT NULL,
  `ImageUrl` varchar(500) DEFAULT NULL,
  `Amenities` varchar(500) DEFAULT NULL,
  `RoomStatus` varchar(30) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Rooms_RoomNumber` (`RoomNumber`),
  KEY `IX_Rooms_RoomType_RoomStatus` (`RoomType`,`RoomStatus`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-11-25 17:28:02
-- MySQL dump 10.13  Distrib 8.4.11, for Win64 (x86_64)
--
-- Host: localhost    Database: hotel_booking_db
-- ------------------------------------------------------
-- Server version	8.4.11

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Dumping data for table `rooms`
--

LOCK TABLES `rooms` WRITE;
/*!40000 ALTER TABLE `rooms` DISABLE KEYS */;
INSERT INTO `rooms` VALUES (1,'101','Cozy Courtyard Standard','Quiet ground-floor room overlooking the landscaped courtyard with modern essentials for solo travelers or couples.','Standard',2000.00,2,'1 Queen Bed','/images/rooms/room-101.jpg','Free Wi-Fi, Air Conditioning, Smart TV, Work Desk, Tea & Coffee Maker','Available','2026-11-25 08:58:18.226057','2026-11-25 10:01:16.507213'),(2,'102','City View Standard Twin','Bright standard room featuring twin beds, blackout curtains, and ergonomic workspace for business or leisure stays.','Standard',2100.00,2,'2 Twin Beds','/images/rooms/room-102.jpg','Free Wi-Fi, Air Conditioning, Smart TV, Work Desk, Hair Dryer, In-Room Safe','Available','2026-11-25 08:58:18.226057','2026-11-25 10:01:16.507242'),(3,'103','Classic Comfort Standard','Warmly appointed room with plush bedding, rainfall shower, and high-speed wireless connectivity.','Standard',2200.00,2,'1 Queen Bed','/images/rooms/room-103.jpg','Free Wi-Fi, Air Conditioning, Smart TV, Rainfall Shower, Mini Fridge','Available','2026-11-25 08:58:18.226057','2026-11-25 10:01:16.507242'),(4,'201','Garden Balcony Deluxe','Spacious deluxe room with a private balcony overlooking the gardens, lounge seating, and upgraded bath amenities.','Deluxe',3500.00,3,'1 King Bed + Sofa','/images/rooms/room-201.jpg','Free Wi-Fi, Private Balcony, Air Conditioning, 50-inch Smart TV, Mini Bar, Espresso Machine','Available','2026-11-25 08:58:18.226057','2026-11-25 10:01:16.507243'),(5,'202','Skyline Corner Deluxe','Corner deluxe room bathed in natural light through floor-to-ceiling windows with panoramic city views.','Deluxe',3700.00,2,'1 King Bed','/images/rooms/room-202.jpg','Free Wi-Fi, Panoramic Windows, Air Conditioning, Smart TV, Bathrobes & Slippers, Espresso Machine','Available','2026-11-25 08:58:18.226057','2026-11-25 10:58:11.148403'),(6,'203','Poolside Terrace Deluxe','Stylish deluxe accommodation with direct terrace access near the outdoor pool and dedicated relaxation area.','Deluxe',3600.00,3,'1 King Bed + Daybed','/images/rooms/room-203.jpg','Free Wi-Fi, Poolside Terrace, Air Conditioning, Smart TV, Mini Bar, Rain Shower','Available','2026-11-25 08:58:18.226057','2026-11-25 10:58:11.148433'),(7,'301','Executive Junior Suite','Open-plan suite combining a king bedroom with a separate lounge and dining nook for extended stays.','Suite',5500.00,3,'1 King Bed + Lounge Sofa','/images/rooms/room-301.jpg','Free Wi-Fi, Separate Lounge, Soaking Tub, Smart TV, Nespresso Bar, Complimentary Breakfast','Available','2026-11-25 08:58:18.226057','2026-11-25 10:58:11.148434'),(8,'302','Ambassador Corner Suite','Refined suite featuring a private living room, marble bathroom with freestanding tub, and executive desk.','Suite',6000.00,4,'1 King Bed + Sofa Bed','/images/rooms/room-302.jpg','Free Wi-Fi, Living Room, Freestanding Tub, 55-inch Smart TV, Mini Bar, Lounge Access','Available','2026-11-25 08:58:18.226057','2026-11-25 10:58:11.148434'),(9,'303','Presidential Penthouse Suite','Top-floor luxury suite with private dining area, wrap-around terrace, and panoramic skyline views.','Suite',6800.00,4,'1 King Bed + 1 Queen Sofa Bed','/images/rooms/room-303.jpg','Free Wi-Fi, Private Terrace, Dining Area, Jacuzzi Tub, Concierge Service, Complimentary Breakfast','Available','2026-11-25 08:58:18.226057','2026-11-25 10:58:11.148434'),(10,'401','Sunshine Family Studio','Generously sized family room with two queen beds, kid-friendly layout, and plenty of luggage storage.','Family',4500.00,4,'2 Queen Beds','/images/rooms/room-401.jpg','Free Wi-Fi, Air Conditioning, Smart TV, Mini Fridge, Microwave, Extra Storage','Available','2026-11-25 08:58:18.226057','2026-11-25 10:01:16.507245'),(11,'402','Grand Two-Bedroom Family','Interconnected two-bedroom family accommodation designed for larger families seeking privacy and comfort.','Family',5200.00,6,'1 King Bed + 2 Twin Beds + Sofa Bed','/images/rooms/room-402.jpg','Free Wi-Fi, Two Bathrooms, Kitchenette, Smart TVs, Air Conditioning, Complimentary Crib on Request','Available','2026-11-25 08:58:18.226057','2026-11-25 10:58:11.148435'),(12,'403','Garden Wing Family Loft','Split-level family loft with a quiet sleeping alcove, dining table, and easy access to garden play areas.','Family',4800.00,5,'1 Queen Bed + 2 Twin Beds + Daybed','/images/rooms/family-room.jpg','Free Wi-Fi, Loft Layout, Dining Table, Smart TV, Mini Fridge, Air Conditioning','Available','2026-11-25 08:58:18.226057','2026-11-25 09:46:27.989314');
/*!40000 ALTER TABLE `rooms` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-11-25 17:28:13
