# Aarohan Residency – Hotel Room Booking System

An ASP.NET Core MVC web application for managing hotel room discovery, date-range availability search, guest reservations, and administrative operations for **Aarohan Residency**.

---

## 1. Project Overview

- **Project Name:** Aarohan Residency – Hotel Room Booking System
- **Purpose & Problem Statement:** Provides a centralized online reservation platform where guests can browse rooms, check real-time date-range availability, view transparent Indian Rupee (INR) pricing, and manage their bookings, while hotel staff can manage room inventory, reservations, and guest accounts from a role-protected Admin dashboard.
- **Intended Users:**
  - **Guests:** Browse rooms, search availability by dates/guests/category, register and sign in, create bookings, view booking confirmations/history, and cancel active reservations.
  - **Administrators:** Monitor property KPIs, add/edit/delete rooms, update room availability statuses (`Available`, `Maintenance`, `Inactive`), filter and update booking statuses, and activate/deactivate guest accounts.

---

## 2. Technology Stack

Verified directly from `HotelBookingSystem.csproj`, `obj/project.assets.json`, `Program.cs`, and `wwwroot/lib/bootstrap/dist/css/bootstrap.min.css`:

- **Framework:** ASP.NET Core MVC
- **Runtime / Target Framework:** `.NET 10.0` (`net10.0`, built with .NET SDK `10.0.404`)
- **Language:** C# (default language version for `net10.0`; no `<LangVersion>` override in `.csproj`, `Nullable` and `ImplicitUsings` enabled)
- **ORM:** Entity Framework Core `10.0.15` (`Microsoft.EntityFrameworkCore` transitive, `Microsoft.EntityFrameworkCore.Design` `10.0.15`)
- **Database Provider:** Oracle official MySQL EF Core provider `MySql.EntityFrameworkCore` `10.0.12`
- **Database Engine:** MySQL 8.0+ with InnoDB (`mysql.exe` `8.4.11.0` observed on development machine)
- **Frontend:** Razor Views, Bootstrap `v5.3.3`, HTML5, CSS3, JavaScript (ES6)
- **Authentication & Password Hashing:** ASP.NET Core Cookie Authentication (`CookieAuthenticationDefaults.AuthenticationScheme`) and `Microsoft.AspNetCore.Identity.PasswordHasher<ApplicationUser>` (PBKDF2) wrapped by `PasswordHasherService`

---

## 3. Project Architecture

The solution follows ASP.NET Core MVC with a service layer (`Interfaces/` + `Services/`) and an isolated `Admin` Area:

```text
HotelBookingSystem/
├── Areas/
│   └── Admin/
│       ├── Controllers/     # Admin-only Dashboard, Rooms, Bookings, and Guests controllers
│       ├── ViewModels/      # Dashboard KPIs, Room form, Booking list/details, Guest views
│       └── Views/           # Admin Razor layout (_AdminLayout) and management pages
├── Controllers/             # Guest-facing Home, Rooms, Bookings, and Account controllers
├── Data/                    # ApplicationDbContext (EF Core) and DbInitializer (seed data)
├── Helpers/                 # CurrencyFormatter (centralized en-IN ₹ formatting)
├── Interfaces/              # Service contracts (IAuthService, IAdminService, IRoomService, IBookingService, IPasswordHasherService)
├── Migrations/              # EF Core InitialSchema migration and model snapshot
├── Models/                  # Database entities (ApplicationUser, Room, Booking, ErrorViewModel)
│   └── Enums/               # UserRole, RoomType, RoomStatus, BookingStatus
├── Properties/              # launchSettings.json (HTTP/HTTPS development profiles)
├── Services/                # Business logic implementations (AuthService, AdminService, RoomService, BookingService, PasswordHasherService)
├── ViewModels/              # Strongly typed guest ViewModels and RoomMappingExtensions
├── Views/                   # Guest Razor views (Home, Rooms, Bookings, Account, Shared)
├── wwwroot/                 # Static assets (site.css, site.js, Bootstrap 5.3.3, jQuery, hotel & room images)
├── Program.cs               # DI registrations, cookie auth config, startup seeding, routing
├── appsettings.json         # Base configuration with placeholder DefaultConnection
└── HotelBookingSystem.csproj
```

---

## 4. Guest Features

Verified from `HomeController`, `RoomsController`, `BookingsController`, `AccountController`, and `Views/`:

- **Homepage (`Home/Index`):** Hero showcase, quick availability search bar, featured rooms from `IRoomService.GetFeaturedRoomsAsync(4)`, essential hotel amenities, why-choose section, illustrative sample guest reviews, and reservation CTA.
- **Room Listing & Search (`Rooms/Index`):** Browse all `Available` rooms or filter by check-in date, check-out date, minimum guest capacity, and `RoomType` (`Standard`, `Deluxe`, `Suite`, `Family`). Preserves selected filter values and displays a clear empty-results state when no rooms match.
- **Room Details (`Rooms/Details`):** Displays large room photography, room number, category, description, capacity, bed type, parsed amenity badges, check-in/check-out policies, INR nightly rate, and a **Book Now** action preserving selected dates and guests.
- **Registration & Login (`Account/Register`, `Account/Login`, `Account/Logout`):** Guest registration with server-side validation, case-insensitive email uniqueness, automatic `Guest` role assignment, secure login with `IsActive` check, and `POST` logout with antiforgery protection.
- **Booking Creation (`Bookings/Create`):** Authenticated booking form with date selection, guest count bounded by room capacity, special requests, live client-side INR estimate, and authoritative server-side price recalculation.
- **Booking Confirmation (`Bookings/Confirmation`):** Displays generated `BookingReference` (`HBS-yyyyMMdd-XXXXXX`), status, stay dates, locked nightly rate, and total INR amount.
- **My Bookings & Details (`Bookings/MyBookings`, `Bookings/Details`):** Lists the signed-in guest's reservations newest-first and provides a detail view with ownership enforcement (`Forbid()` if accessing another user's booking).
- **Booking Cancellation (`Bookings/Cancel`):** Allows guests to cancel their own `Pending` or `Confirmed` bookings via `POST` with antiforgery validation.
- **Guest Profile (`Account/Profile`):** Displays the signed-in user's name, email, phone number, role, join date, and total reservation count without exposing `PasswordHash`.

---

## 5. Admin Features

Verified from `Areas/Admin/Controllers/` and `Areas/Admin/Views/` (all protected with `[Area("Admin")]` and `[Authorize(Roles = "Admin")]`):

- **Admin Dashboard (`Admin/Dashboard/Index`):** Displays 8 live database KPI cards (`Total Rooms`, `Available Rooms`, `Total Bookings`, `Pending Bookings`, `Confirmed Bookings`, `Cancelled Bookings`, `Completed Bookings`, `Registered Guests`) and a Recent Reservations table.
- **Room Management (`Admin/Rooms`):** List all rooms with photo thumbnails, add new rooms (`Create`), edit existing rooms (`Edit`), change room status inline (`Available`, `Maintenance`, `Inactive`), and delete rooms (`Delete`, blocked safely if the room has existing booking history).
- **Booking Management (`Admin/Bookings`):** List all bookings, search by `BookingReference`, filter by `BookingStatus`, view full guest/room/pricing details (`Details`), and update booking status (`Pending`, `Confirmed`, `Cancelled`, `Completed`).
- **Guest Management (`Admin/Guests`):** List registered guest accounts (`UserRole.Guest`), view guest details and booking history, and activate/deactivate guest accounts (`ToggleActive`). `PasswordHash` is never projected or displayed.

---

## 6. Database Design

Verified from `ApplicationDbContext.cs`, entity classes in `Models/`, and `Migrations/20261125082243_InitialSchema.cs`:

### Tables & Columns
- **`ApplicationUsers`:** `Id` (PK, `int`, auto-increment), `FullName` (`varchar(100)`), `Email` (`varchar(150)`), `PasswordHash` (`varchar(255)`), `PhoneNumber` (`varchar(20)`, nullable), `Role` (`varchar(20)`), `CreatedAt` (`datetime(6)`), `IsActive` (`tinyint(1)`).
- **`Rooms`:** `Id` (PK, `int`, auto-increment), `RoomNumber` (`varchar(20)`), `Name` (`varchar(100)`), `Description` (`varchar(1000)`, nullable), `RoomType` (`varchar(30)`), `PricePerNight` (`decimal(10,2)`), `Capacity` (`int`), `BedType` (`varchar(50)`), `ImageUrl` (`varchar(500)`, nullable), `Amenities` (`varchar(500)`, nullable), `RoomStatus` (`varchar(30)`), `CreatedAt` (`datetime(6)`), `UpdatedAt` (`datetime(6)`, nullable).
- **`Bookings`:** `Id` (PK, `int`, auto-increment), `BookingReference` (`varchar(30)`), `UserId` (FK `int`), `RoomId` (FK `int`), `CheckInDate` (`datetime(6)`), `CheckOutDate` (`datetime(6)`), `NumberOfGuests` (`int`), `NumberOfNights` (`int`), `PricePerNight` (`decimal(10,2)`), `TotalAmount` (`decimal(10,2)`), `BookingStatus` (`varchar(30)`), `SpecialRequests` (`varchar(500)`, nullable), `CreatedAt` (`datetime(6)`), `UpdatedAt` (`datetime(6)`, nullable).

### Relationships, Constraints & Indexes
- **One-to-Many Relationships:** `ApplicationUser` → `Bookings` (`FK_Bookings_ApplicationUsers_UserId`) and `Room` → `Bookings` (`FK_Bookings_Rooms_RoomId`).
- **Delete Behavior:** Both foreign keys use `DeleteBehavior.Restrict` (`ReferentialAction.Restrict`) to prevent accidental deletion of users or rooms with booking history.
- **Unique Constraints:** `IX_ApplicationUsers_Email`, `IX_Rooms_RoomNumber`, `IX_Bookings_BookingReference`.
- **Query & Composite Indexes:** `IX_Rooms_RoomType_RoomStatus`, `IX_Bookings_RoomId_CheckInDate_CheckOutDate`, `IX_Bookings_UserId`, `IX_Bookings_BookingStatus`.
- **Decimal Precision & Enums:** Monetary columns use `decimal(10,2)`; all enums (`UserRole`, `RoomType`, `RoomStatus`, `BookingStatus`) are stored as strings via `.HasConversion<string>()`.

---

## 7. Booking Business Rules

Verified from `Services/BookingService.cs` and `Services/RoomService.cs`:

- **Date Validation:** `CheckInDate.Date` cannot be in the past (`< DateTime.UtcNow.Date`), and `CheckOutDate.Date` must be strictly later than `CheckInDate.Date`.
- **Capacity & Room Status:** `NumberOfGuests` must be `>= 1` and `<= room.Capacity`. Only rooms with `RoomStatus.Available` can be booked (`Maintenance` and `Inactive` rooms are rejected).
- **Overlap Logic:** A room is unavailable if any booking for the same `RoomId` has status `Pending` or `Confirmed` and satisfies:
  ```text
  newCheckIn < existing.CheckOutDate AND newCheckOut > existing.CheckInDate
  ```
- **Status Behavior:** `Pending` and `Confirmed` bookings block overlapping dates; `Cancelled` and `Completed` bookings do not block availability. `UpdateBookingStatusAsync` prevents transitioning a `Cancelled` booking back to another status.
- **Server-Side Price Calculation:** `NumberOfNights = (checkOut - checkIn).Days`, `TotalAmount = room.PricePerNight * NumberOfNights`. Both `PricePerNight` and `TotalAmount` are stored as snapshots on the `Booking` record so future room rate changes do not alter historical bookings.
- **Transaction & Concurrency Protection:** `CreateBookingAsync` opens a database transaction (`BeginTransactionAsync()`) and acquires an InnoDB row-level exclusive lock on the target room row using:
  ```sql
  SELECT * FROM Rooms WHERE Id = {roomId} FOR UPDATE
  ```
  before checking overlapping bookings, rolling back explicitly on any validation failure or exception.

---

## 8. Installation Prerequisites

- **.NET 10 SDK** (`10.0.404` or compatible .NET 10 SDK)
- **MySQL Server 8.0+** (InnoDB engine enabled)
- **Entity Framework Core CLI (`dotnet-ef` `10.0.15`):**
  ```powershell
  dotnet tool install --global dotnet-ef
  ```
- **Git**

---

## 9. Local Setup Instructions

```powershell
# 1. Clone the repository
git clone https://github.com/rakshesuraj25/HotelBookingSystem.git
cd HotelBookingSystem

# 2. Restore NuGet packages and build
dotnet restore
dotnet build

# 3. Configure User Secrets (see Section 10)
```

### Database Initialization (Choose ONE Option Only)

- **Option A — EF Core Migrations (Recommended for clean setup):**
  Create an empty `hotel_booking_db` database in MySQL and apply the EF Core migration:
  ```sql
  CREATE DATABASE IF NOT EXISTS hotel_booking_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
  ```
  ```powershell
  dotnet ef database update
  ```

- **Option B — Import SQL Export:**
  Import `database/HotelBookingDB.sql` (a MySQL 8.4-compatible SQL script) into a fresh MySQL database:
  ```powershell
  mysql -u root -p hotel_booking_db < database/HotelBookingDB.sql
  ```
  > **Important:** Do **not** run `dotnet ef database update` after importing the SQL script as currently exported, because its `__efmigrationshistory` table has no migration records (or expects a clean state) and the tables already exist—running EF migrations afterward will fail.

### Run the Application

```powershell
dotnet run
```

*(Note: `DbInitializer.InitializeAsync()` runs on startup to seed the 12 rooms and optional configured admin account idempotently, without running automatic EF migrations.)*

---

## 10. Secure Configuration

`HotelBookingSystem.csproj` defines `UserSecretsId` (`hotel-booking-system-dev-secrets`). `appsettings.json` contains only a placeholder `DefaultConnection`. Configure real local credentials using **.NET User Secrets** (or environment variables):

```powershell
# MySQL connection string (database name: hotel_booking_db)
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=3306;Database=hotel_booking_db;User=root;Password=YOUR_MYSQL_PASSWORD;"

# Initial Admin account seeding (optional; skipped with a warning if omitted)
dotnet user-secrets set "SeedAdmin:Email" "admin@example.com"
dotnet user-secrets set "SeedAdmin:FullName" "Aarohan Residency Administrator"
dotnet user-secrets set "SeedAdmin:Password" "YOUR_STRONG_ADMIN_PASSWORD"
```

Never commit real passwords, connection strings, or `secrets.json` files to Git.

---

## 11. Test Accounts

- **Admin Account:** Created idempotently on startup by `DbInitializer` when `SeedAdmin:Email`, `SeedAdmin:FullName`, and `SeedAdmin:Password` are configured in User Secrets or environment variables. No default admin password is hardcoded in source code.
- **Guest Account:** Register a new guest account via `/Account/Register` (self-registration always assigns `UserRole.Guest`).

---

## 12. Design Decisions

- **ASP.NET Core MVC + Service Layer:** Keeps controllers thin and separates HTTP routing/ModelState handling from EF Core queries and business rules (`RoomService`, `BookingService`, `AuthService`, `AdminService`) without introducing redundant generic repository wrappers.
- **Entity Framework Core 10 + `MySql.EntityFrameworkCore` 10.0.12:** Provides native EF Core 10 compatibility and InnoDB support without mixing incompatible EF Core 9 providers.
- **Cookie Authentication & Role-Based Authorization:** Uses `HotelBooking.Auth` `HttpOnly` cookies with `SameSite=Lax`, 8-hour sliding expiration, `[Authorize]` for guest bookings, and `[Authorize(Roles = "Admin")]` for the `Admin` Area.
- **Password Hashing:** Uses ASP.NET Core Identity's PBKDF2 `PasswordHasher<ApplicationUser>` via `IPasswordHasherService`.
- **Responsive UI & Centralized INR Formatting:** Custom warm ivory/charcoal/bronze design tokens layered on Bootstrap `5.3.3`, with `CurrencyFormatter` (`en-IN` / `₹`) shared across guest and admin views.
- **Transaction + Row Locking:** `SELECT ... FOR UPDATE` on the `Rooms` row serializes concurrent reservation attempts for the same room inside a single MySQL transaction.

---

## 13. Completed Features

- Project baseline, User Secrets setup, and `.gitignore` hygiene
- `ApplicationUser`, `Room`, and `Booking` entities with enums, constraints, indexes, and `InitialSchema` migration
- Idempotent `DbInitializer` for 12 rooms (with INR pricing) and optional configured admin seeding
- `RoomService`, `BookingService` (with `SELECT ... FOR UPDATE`), `AuthService`, `AdminService`, and `PasswordHasherService`
- Guest-facing website: Homepage, Room Search & Listing, Room Details, Booking Create/Confirmation/Details/Cancellation, My Bookings, Register/Login/Logout/Profile
- Admin Area: Dashboard (8 KPI cards + recent reservations), Room CRUD & safe delete, Booking search/filter/status updates, and Guest list/details/activation toggling
- Centralized INR (`₹`) formatting and local hotel/room photography (with `/images/room-placeholder.svg` fallback)

---

## 14. Incomplete Features

- **Room 403 Dedicated Photograph:** Room `403` (*Garden Wing Family Loft*) currently uses `/images/rooms/family-room.jpg` (shared with `family-room.jpg` / Room `401` visually) because a matching loft photo showing 1 queen bed, 2 twin beds, and a separate daybed was not yet available.
- **SQL Server Migration & `.bak` Backup:** Not implemented (see Section 17).
- **Automated Test Suite:** No xUnit/NUnit integration test project is included in the repository.

---

## 15. Known Issues and Limitations

- **Database Requirement Deviation:** Built on MySQL rather than SQL Server (see Section 17).
- **Room 403 Photo:** Shares the family room photo until a dedicated loft photograph is added to `wwwroot/images/rooms/room-403.jpg` and mapped in `DbInitializer.cs`.
- **Single-Node Row Locking:** Under heavy contention on a single room, concurrent transactions wait on the InnoDB row lock and could hit `innodb_lock_wait_timeout` if held too long.
- **Startup Seeding Dependency:** `DbInitializer.InitializeAsync()` expects `hotel_booking_db` tables to exist (applied via `dotnet ef database update`); if started before migrating, it logs the error without crashing.

---

## 16. Future Improvements

- Add a dedicated photograph for Room `403` (`room-403.jpg`) and update `DbInitializer.cs`.
- Add an automated xUnit test project covering `BookingService` overlap/concurrency rules and controller authorization.
- Add email notifications for booking confirmations and cancellations.
- Add pagination to Admin booking and guest tables as data volume grows.

---

## 17. Database Technology – Assessment Requirement Deviation

> **IMPORTANT NOTICE**
>
> - **The assessment specifies SQL Server as mandatory.**
> - **This implementation currently uses MySQL** (`MySql.EntityFrameworkCore` `10.0.12` targeting `hotel_booking_db`).
> - **The application has not been migrated to SQL Server.**
> - **A SQL Server `.bak` backup is not available.**
> - **A MySQL SQL dump is not equivalent to a SQL Server `.bak` backup.**
> - **`HotelBookingDB.sql` contains MySQL 8.4-compatible SQL syntax and is not compatible with SQL Server without conversion.**
> - **This is a known deviation and may affect assessment evaluation.**
> - **The current project has not been verified as SQL Server compatible.**
>
> MySQL was used intentionally during development and this limitation is documented transparently rather than misrepresented.

---

## 18. Screenshots

Verified desktop screenshots of the running application are stored in `screenshots/` (**14 desktop screenshots** total, including **5 additional scroll-position captures**; **0 mobile screenshots**):

- **Guest Pages:**
  - `screenshots/01-home-desktop.png` — Homepage hero and availability search bar
  - `screenshots/01-home-featured-rooms-desktop.jpg` — Homepage scrolled to Featured Rooms & Suites
  - `screenshots/02-rooms-desktop.png` — Rooms & Suites search and filter view
  - `screenshots/02-rooms-standard-desktop.jpg` — Rooms listing (Standard rooms 101–103)
  - `screenshots/02-rooms-deluxe-desktop.jpg` — Rooms listing (Deluxe rooms 201–203)
  - `screenshots/02-rooms-suite-desktop.jpg` — Rooms listing (Suite rooms 301–303)
  - `screenshots/02-rooms-family-desktop.jpg` — Rooms listing (Family rooms 401–403)
  - `screenshots/04-login-desktop.png` — Guest Sign In page
  - `screenshots/08-my-bookings-desktop.png` — Guest My Bookings page
  - `screenshots/10-profile-desktop.png` — Guest Profile page
- **Admin Pages:**
  - `screenshots/11-admin-dashboard-desktop.png` — Admin Dashboard (8 KPI cards and recent reservations)
  - `screenshots/12-admin-rooms-desktop.png` — Admin Room Management
  - `screenshots/14-admin-bookings-desktop.png` — Admin Booking Management
  - `screenshots/16-admin-guests-desktop.png` — Admin Guest Management
