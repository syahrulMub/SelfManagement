# 🎯 Self Management Application

A comprehensive personal management web application built with ASP.NET Core MVC that helps you organize your tasks, track expenses and income, and manage your daily activities all in one place.

![.NET](https://img.shields.io/badge/.NET-7.0-512BD4?style=flat-square&logo=dotnet)
![Entity Framework](https://img.shields.io/badge/Entity_Framework-7.0-512BD4?style=flat-square)
![SQLite](https://img.shields.io/badge/SQLite-Database-003B57?style=flat-square&logo=sqlite)
![License](https://img.shields.io/badge/license-MIT-green?style=flat-square)

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Running the Application](#running-the-application)
- [Project Structure](#project-structure)
- [Database](#database)
- [Screenshots](#screenshots)
- [Contributing](#contributing)
- [License](#license)

## 🌟 Overview

Self Management is a feature-rich personal productivity application designed to help individuals manage various aspects of their daily life. The application provides an intuitive interface for tracking tasks, managing finances (expenses and income), and monitoring activities with visual analytics and reporting.

## ✨ Features

### 📝 Task Management
- **Task Work**: Create, update, and delete tasks with categories
- **Task Categories**: Organize tasks into custom categories
- **Critical Tasks**: Mark and track high-priority tasks
- **Task Status Tracking**: Monitor task completion and progress

### 💰 Financial Management
- **Expense Tracking**: Record and categorize expenses
- **Income Tracking**: Monitor income from various sources
- **Category Management**: Create custom categories for expenses
- **Source Management**: Manage income sources
- **Visual Analytics**: Interactive charts for expense analysis
  - Expense breakdown by category
  - Timeline-based expense reports
  - Filtering by Day, Week, Month, and Year
- **Data Migration**: Import/export financial data

### 📊 Activity Tracking
- **Activity Monitoring**: Track and manage daily activities
- **Activity Analytics**: Visual reports and rankings
- **Reminders**: Set up activity reminders

### 🔐 User Management
- **Authentication**: Secure user login and registration
- **Identity Management**: ASP.NET Core Identity integration
- **Role-Based Access**: User roles and permissions

### 📈 Reporting & Visualization
- **Interactive Charts**: Powered by Chart.js
- **Data Tables**: Feature-rich tables with DataTables.net
- **Date Filtering**: Flatpickr integration for date selection
- **Responsive Design**: Mobile-friendly interface

## 🛠️ Technology Stack

### Backend
- **Framework**: ASP.NET Core 7.0 MVC
- **Language**: C# with .NET 7.0
- **ORM**: Entity Framework Core 7.0.12
- **Database**: SQLite 7.0.12
- **Authentication**: ASP.NET Core Identity 7.0.12

### Frontend
- **UI Framework**: Bootstrap 5
- **JavaScript**: Vanilla JavaScript with jQuery
- **Charts**: Chart.js
- **Data Tables**: DataTables.net 1.13.8
- **Date Picker**: Flatpickr
- **Template Engine**: Razor Pages

### Key NuGet Packages
```xml
- Microsoft.EntityFrameworkCore (7.0.12)
- Microsoft.EntityFrameworkCore.Sqlite (7.0.12)
- Microsoft.EntityFrameworkCore.Design (7.0.12)
- Microsoft.EntityFrameworkCore.Tools (7.0.12)
- Microsoft.AspNetCore.Identity.EntityFrameworkCore (7.0.12)
- Microsoft.AspNetCore.Identity.UI (7.0.12)
- datatables.net-dt (1.13.8)
```

## 🚀 Getting Started

### Prerequisites

Before you begin, ensure you have the following installed:
- [.NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0) or later
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/) (for cloning the repository)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/SelfManagement.git
   cd SelfManagement
   ```

2. **Navigate to the project directory**
   ```bash
   cd MyPrivateManager
   ```

3. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

4. **Update the database**
   
   The application uses SQLite with automatic database initialization. The database will be created automatically on first run using the `schema.sql` file.

   If you need to manually apply migrations:
   ```bash
   dotnet ef database update
   ```

5. **Configure application settings** (Optional)
   
   Review and modify `appsettings.json` if needed:
   ```json
   {
     "ConnectionStrings": {
       "DatabaseContext": "Data Source = SelfManagement.db",
       "schema": "schema.sql"
     }
   }
   ```

### Running the Application

1. **Build the project**
   ```bash
   dotnet build
   ```

2. **Run the application**
   ```bash
   dotnet run
   ```

3. **Access the application**
   
   Open your browser and navigate to:
   - `https://localhost:5001` (HTTPS)
   - `http://localhost:5000` (HTTP)

4. **Initial Setup**
   
   On first run:
   - The database will be automatically created
   - Default roles will be initialized
   - Register a new user account to get started

## 📁 Project Structure

```
SelfManagement/
├── MyPrivateManager/
│   ├── Areas/                    # Identity areas
│   ├── Controllers/              # MVC Controllers
│   │   ├── ActivityController.cs
│   │   ├── CategoryController.cs
│   │   ├── ExpenseController.cs
│   │   ├── HomeController.cs
│   │   ├── IncomeController.cs
│   │   ├── SourceController.cs
│   │   ├── TaskCategoryController.cs
│   │   ├── TaskWorkController.cs
│   │   └── UserLoginController.cs
│   ├── Data/                     # Database context
│   ├── DatabaseServices/         # Service implementations
│   ├── IDatabaseServices/        # Service interfaces
│   ├── Models/                   # Entity models
│   │   ├── Activity.cs
│   │   ├── Category.cs
│   │   ├── Expense.cs
│   │   ├── Income.cs
│   │   ├── Schedule.cs
│   │   ├── Source.cs
│   │   ├── TaskCategory.cs
│   │   ├── TaskWork.cs
│   │   └── User.cs
│   ├── Views/                    # Razor views
│   │   ├── Activity/
│   │   ├── Expense/
│   │   ├── Home/
│   │   ├── Income/
│   │   ├── TaskWork/
│   │   └── Shared/
│   ├── wwwroot/                  # Static files (CSS, JS, images)
│   ├── Program.cs                # Application entry point
│   ├── appsettings.json          # Configuration
│   ├── schema.sql                # Database schema
│   └── SelfManagement.db         # SQLite database
└── SelfManagement.sln            # Solution file
```

## 🗄️ Database

The application uses **SQLite** as the database engine with **Entity Framework Core** as the ORM.

### Database Schema

The database includes the following main entities:

- **User**: User accounts and authentication
- **TaskWork**: Task items with status and priority
- **TaskCategory**: Task categorization
- **Expense**: Expense records
- **Category**: Expense categories
- **Income**: Income records
- **Source**: Income sources
- **Activity**: User activity tracking
- **Schedule**: Scheduling information

### Database Initialization

The database is automatically initialized on application startup using:
- Entity Framework migrations
- SQL schema file (`schema.sql`)
- Role creation and seeding

## 📸 Screenshots

> **Note**: Add screenshots of your application here to showcase its features and UI.

```
[Dashboard Screenshot]
[Task Management Screenshot]
[Expense Tracking Screenshot]
[Reports & Analytics Screenshot]
```

## 🤝 Contributing

Contributions are welcome! If you'd like to contribute to this project, please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Development Guidelines

- Follow C# coding conventions and best practices
- Write meaningful commit messages
- Add unit tests for new features
- Update documentation as needed
- Ensure all tests pass before submitting PR

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👨‍💻 Author

**Syahrul**

- GitHub: [@syahrulMub](https://github.com/syahrulMub)

## 🙏 Acknowledgments

- ASP.NET Core Team for the excellent framework
- Chart.js for beautiful charts
- DataTables for powerful table functionality
- Bootstrap for responsive UI components
- All contributors and users of this project

## 📞 Support

If you encounter any issues or have questions:

1. Check the [Issues](https://github.com/syahrulMub/SelfManagement/issues) page
2. Create a new issue if your problem isn't already listed
3. Provide detailed information about the issue

---

<div align="center">
  Made with ❤️ using ASP.NET Core
  
  ⭐ Star this repository if you find it helpful!
</div>
