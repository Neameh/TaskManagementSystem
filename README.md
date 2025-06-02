# 📝 Task Management System

## 📌 Project Description

The Task Management System is a web-based application built with ASP.NET Core MVC. It allows users to register, log in, and manage their personal tasks. Each user can create, update, delete, search, and mark tasks as complete or incomplete. The interface is clean and responsive, designed with Bootstrap. Authentication is handled using ASP.NET Core Identity.

---

## 🧰 Technologies Used

- **.NET Core Version:** .NET 9.0 (ASP.NET Core MVC)
- **Database:** SQL Server
- **ORM:** Entity Framework Core
- **Authentication:** ASP.NET Core Identity
- **UI:** Razor Views + Bootstrap
- **IDE:** Visual Studio 2022

---

## 🛠️ How to Run the Project Locally

### 1. Prerequisites

Make sure you have the following installed:

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) with ASP.NET and web development workload
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- (Optional) [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)

---

### 2. Clone the Repository

```bash
git clone https://github.com/your-username/TaskManagementSystem.git
cd TaskManagementSystem
```
### 3. Configure the Database Connection

Open the `appsettings.json` file in the root of the project and update the `DefaultConnection` string with your SQL Server settings.

**Windows Authentication example:**

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=TaskManagementDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

**SQL Authentication example:**
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=TaskManagementDb;User Id=your_username;Password=your_password;MultipleActiveResultSets=true"
```
Make sure your SQL Server is installed, running, and the connection string matches your setup.

### 4. Apply Migrations and Create the Database

```powershell
Update-Database
```
This will create the TaskManagementDb and apply all migrations.

### 5. Run the Application
In Visual Studio:

Press F5 or click the green Run button to start the app,
The default page should open in your browser (e.g., https://localhost:5001).

