# User Management System - ASP.NET Core MVC

## 🚀 Project Overview
This project is a **Login and User Management System** built using **ASP.NET Core MVC** with **Entity Framework (EF) Core** and **SQL Server**. The system includes **user registration, login, password hashing, JWT authentication**, and **role-based access control**.

---
## 🛠️ Setup and Installation

### 1️⃣ Adjust Connection String
Before running the project, **update your `appsettings.json` file** with your SQL Server credentials.

#### **File:** `appsettings.json`
- my connection string
```json
"ConnectionStrings": {
  "DBCON": "Server=DESKTOP-PAJ1QPJ\\SQLEXPRESS;Database=User_Management; Trusted_Connection=true;TrustServerCertificate=true"
}
```
- Replace `YOUR_SERVER_NAME` with your system’s SQL Server instance.
- If using **SQL Server Authentication**, replace `YOUR_USER_ID` and `YOUR_PASSWORD` accordingly.
- If using **Windows Authentication**, remove `User Id` and `Password`, and set `Trusted_Connection=True`.

---
### 2️⃣ Apply Database Migrations
We have already **added migrations**, so you just need to **update the database**:

Run the following command in the **Package Manager Console (PMC)**:
```powershell
Update-Database
```
This will **automatically generate** the required database structure with two tables:
- **Users** (Stores user details)
- **Status** (Stores status-related data)

---
## 🔑 Authentication & Security

### **User Registration & Login**
- Users can **register** with their details, which will be stored securely in the database.
- **Password Hashing** is implemented using `BCrypt.Net` for security.
- On **successful login**, a **JWT token** is generated and stored in **HTTP cookies** for authentication.

### **JWT Token Handling**
- The JWT token is generated upon successful login.
- It is stored in **secure, HTTP-only cookies** to prevent XSS attacks.
- The token includes claims such as `UserId`, `Username`, and `Email` for session management.

---
## 📌 Features Implemented in this Task
✔ **User Registration with Validation**
✔ **Login with Password Hashing**
✔ **JWT Authentication (Stored in Cookies)**
✔ **User List with DataTables Integration**
✔ **Status Management for Users**
✔ **Secure Database Connection Handling**

---
## ▶️ Running the Project
1️⃣ **Open the project** in **Visual Studio**
2️⃣ **Adjust Connection String** in `appsettings.json`
3️⃣ **Run the database migration command**:
   ```powershell
   Update-Database
   ```
4️⃣ **Run the project** (Press `F5` or use `dotnet run`)
5️⃣ **Access the application** in your browser at `http://localhost:xxxx/Auth/Login`

---
## 📞 Support
For any issues, feel free to reach out or check the **GitHub Issues** section.

Happy Coding! 🚀
