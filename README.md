# PasteTrue

PasteTrue is a secure text sharing service built with ASP.NET Core, featuring JWT authentication and password-protected pastes. It allows users to create, manage, and share text snippets with optional password protection.

## Features

- 🔐 JWT Authentication
- 👤 User Registration and Login
- 📝 Create and manage text pastes
- 🔒 Optional password protection for pastes
- 🔍 Retrieve pastes by ID
- 🗑️ Delete own pastes
- 💼 User-specific paste management

## Technologies

- ASP.NET Core
- Entity Framework Core
- Microsoft Identity
- JWT Authentication
- SQLite (can be configured for other databases)

## API Endpoints

### Authentication

#### Register
```http
POST /api/Auth/register
Content-Type: application/json

{
    "userName": "string",
    "email": "string",
    "password": "string"
}
```

#### Login
```http
POST /api/Auth/login
Content-Type: application/json

{
    "userName": "string",
    "password": "string"
}
```

### Paste Management

#### Create Paste
```http
POST /api/Paste/create
Content-Type: application/json

{
    "title": "string",
    "content": "string",
    "password": "string" (optional)
}
```

#### Get Paste
```http
POST /api/Paste/getPaste/{id}?password=optional_password
```

#### Get User's Pastes
```http
GET /api/Paste/getMyPastes
Authorization: Bearer {token}
```

#### Delete Paste
```http
DELETE /api/Paste/{id}
Authorization: Bearer {token}
```

## Setup

1. Clone the repository:
```bash
git clone https://github.com/yourusername/PasteTrue.git
```

2. Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your_Connection_String_Here"
  }
}
```

4. Run the application:
```bash
dotnet run
```

## Authentication Flow

1. Users register with username, email, and password
2. Upon successful registration, users are assigned the "User" role
3. Login returns a JWT token used for authenticated requests
4. Protected endpoints require a valid JWT token in the Authorization header

## Paste Protection

- Pastes can be created with or without password protection
- Password-protected pastes require the correct password for access
- Authenticated users can manage their own pastes
- Anonymous users can create and access pastes (with password if required)

## Security Features

- Password hashing for user accounts
- JWT token-based authentication
- Optional password protection for individual pastes
- Role-based authorization
- Input validation and model state checking

## License

This project is licensed under the MIT License - see the LICENSE file for details.
