# Fleet Management System

Fleet Management System is a web application designed to streamline the management of a company's fleet, including drivers, vehicles, service history, fines, and more. It provides an easy-to-use interface for handling everyday fleet operations and helps maintain comprehensive records of vehicles and their activities.

## Table of Contents

- [Features](#features)
- [Technologies Used](#technologies-used)
- [Installation](#installation)
- [Database Setup](#database-setup)
- [Usage](#usage)
  - [Driver Management](#driver-management)
  - [Vehicle Management](#vehicle-management)
  - [Service History](#service-history)
  - [Insurance & Fines](#insurance--fines)
- [Contributing](#contributing)
- [License](#license)

## Features

- **Driver Management:** Add, update, delete, and assign drivers to vehicles.
- **Vehicle Management:** Add, update, and track vehicle details including MOT, tax, and assigned driver.
- **Service History:** Keep a detailed log of vehicle service history with type, date, cost, and status.
- **Insurance & Fines:** Manage vehicle insurance records and driver fines.
- **File Upload:** Upload and manage documents such as MOT and insurance files.
- **Search:** Search for drivers, vehicles, and service records using various filters.
- **Responsive Design:** Mobile-friendly interface for fleet management on the go.

## Technologies Used

- **ASP.NET Core MVC** - Backend framework
- **Entity Framework Core** - ORM for database interaction
- **SQLite** - Database used for local development
- **Bootstrap** - Front-end framework for responsive design
- **jQuery** - Simplifies DOM manipulation and AJAX calls

## Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/FleetManagementSystem.git
   cd FleetManagementSystem
   ```
2. Install the required .NET dependencies:
   ```bash
   dotnet restore
   ```

3. Build the project:
   ```bash
   dotnet build
   ```

4. Run the application:
   ```bash
   dotnet run
   ```

5. Open your browser and navigate to:
   ```
   https://localhost:5001
   ```
   or 
   ```
   http://localhost:5000
   ```

## Database Setup

By default, the project uses **SQLite** as the database for local development.

1. Apply the database migrations to set up the database schema:
   ```bash
   dotnet ef database update
   ```

   This will create the SQLite database and apply the necessary tables for the application.

2. (Optional) If you need to make changes to the database schema, add a new migration:
   ```bash
   dotnet ef migrations add MigrationName
   ```

3. (Optional) After creating a migration, apply the changes:
   ```bash
   dotnet ef database update
   ```

## Usage

### Driver Management

- Navigate to the **Driver** section via the navbar.
- Add a new driver using the "Add New Driver" button.
- Search for drivers by first name, last name, or license number.
- Edit or delete existing drivers from the driver list.

### Vehicle Management

- In the **Vehicle** section, you can:
  - Add new vehicles with fields like license plate, manufacturer, model, VIN, and more.
  - Upload MOT files (PDF) for the vehicle.
  - Assign a driver to the vehicle.
  - Search for vehicles by license plate, manufacturer, or model.

### Service History

- The **Service History** module allows you to keep track of all vehicle maintenance:
  - Log service date, type of service, cost, and reimbursement status.
  - View and manage service histories from the vehicle details page.
  - Search service histories by various criteria.

### Insurance & Fines

- Manage vehicle insurance and driver fines.
- Keep track of insurance expiry and fine payment statuses.
- Add documents related to insurance and fines (e.g., PDFs).

## Contributing

Contributions are welcome! Please follow the steps below to contribute:

1. Fork the repository.
2. Create a new branch (`git checkout -b feature/your-feature-name`).
3. Commit your changes (`git commit -m 'Add a new feature'`).
4. Push to the branch (`git push origin feature/your-feature-name`).
5. Open a Pull Request.

## License

This project is licensed under the MIT License.
