
# User Management API

## Description

This is a RESTful API for managing users, built with .NET 6. It provides endpoints for registering users, logging in, retrieving user details, updating user information, and deleting users. The API ensures secure password handling and user authentication using JWT tokens.

---

## Features

- **User Registration:** Allows new users to register.
- **User Login:** Authenticates users and provides a JWT token.
- **Retrieve User Details:** Fetches details of a specific user (authentication required).
- **Update User Information:** Updates user details with validation.
- **Delete User:** Deletes a user after confirmation.

---

## Prerequisites

1. [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
2. [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
3. [Postman](https://www.postman.com/downloads/) (for API testing)
4. A database connection string configured in `appsettings.json`.

---

## Setup Instructions

### 1. Clone the Repository
```bash
git clone <repository_url>
cd <repository_folder>
```

### 2. Configure the Database
- Open `appsettings.json` and update the `ConnectionStrings` section:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=<your_server>;Database=<your_database>;User Id=<your_user>;Password=<your_password>;"
}
```

### 3. Run Database Migrations
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. Configure JWT Settings
- In `appsettings.json`, configure the `Jwt` section:
```json
"Jwt": {
  "Key": "<your_secret_key>",
  "Issuer": "https://yourdomain.com",
  "Audience": "https://yourdomain.com",
  "Subject": "UserManagementAPI"
}
```

### 5. Build and Run the Application
```bash
dotnet build
dotnet run
```

- The API will run on `https://localhost:<port>` (check the terminal for the port number).

---

## API Endpoints

| Method | Endpoint                                    | Description                      | Auth Required |
|--------|---------------------------------------------|----------------------------------|---------------|
| POST   | /api/Users/Register                         | Register a new user              | No            |
| POST   | /api/Users/Login                            | Authenticate user and get token  | No            |
| GET    | /api/Users/GetUserById/{id}                 | Get user details by ID           | Yes           |
| PUT    | /api/Users/UpdateUser/{id}                  | Update user details              | No            |
| DELETE | /api/Users/DeleteUser/{id}?isConfirm=true   | Delete a user by ID              | No            |

---

## Postman Collection

The Postman collection for testing the API is included in the project. 

### Import Instructions:
1. Open Postman.
2. Go to **File** > **Import**.
3. Select the provided `UserManagementAPI.postman_collection.json` file.
4. Update the base URL and port in Postman to match your local setup.
5. Run the collection to test the endpoints.

---

## Testing Instructions

- Use **Postman** to test all endpoints.
- Ensure that the JWT token is included in the `Authorization` header for protected endpoints. Use the format:
  ```
  Bearer <your_jwt_token>
  ```

---

## Project Structure

```
UserManagementAPI/
│
├── Controllers/           # API Controllers
├── Models/                # Data Models and DTOs
├── Repositories/          # Data Access Layer
├── Migrations/            # EF Core Migrations
├── appsettings.json       # Configuration File
└── Program.cs             # Application Entry Point
```

---

## Unit Tests

Unit tests for critical functionalities are located in the `UserManagement.Tests` project. Run the tests using:
```bash
dotnet test
```

---

## Notes

- Always use a strong and unique JWT secret key.
- Use HTTPS in production to secure API communications.
- Properly configure CORS settings for your application if deploying for external use. 

---

## Contact

For any issues or feature requests, please contact Rushikesh Katkar at katkarrushikesh2002@gmail.com.
