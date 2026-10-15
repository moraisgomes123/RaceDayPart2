# RaceDay Part 2 – Event Management REST API

## 1. Project Overview

RaceDay is an event management system designed for running, walking and cycling events in South Africa. Part 2 develops the backend API that allows users to register, log in, manage events, enter events and access event results.

The system is developed using ASP.NET Core Web API, C#, Entity Framework Core and Microsoft SQL Server. It follows a RESTful API design and uses JSON to exchange data between clients and the server.

The API uses authentication and role-based authorisation to help protect user information and restrict access to management operations.

## 2. Project Objectives

The main objectives of RaceDay Part 2 are to:

- Implement REST API endpoints based on the Part 1 design.
- Register users and authenticate them using email and password.
- Generate and validate JSON Web Tokens (JWT).
- Allow users to manage their own profiles.
- Allow organisers to manage events, routes and categories.
- Allow participants to enrol in events and view their enrolments.
- Allow organisers to record and manage event results.
- Store and retrieve data using Microsoft SQL Server and Entity Framework Core.
- Test API functionality and validation using automated tests.
- Use GitHub Actions to build the solution and run tests automatically.

## 3. User Roles

The system supports two main user roles.

### 3.1 Organiser

An Organiser is responsible for managing events and their related information.

An organiser can:

- Log in securely.
- Create, view, update and delete events.
- Manage event routes.
- Create and manage event categories.
- View participant enrolments for their events.
- Record event results.
- Update and delete results.
- View information related to events they manage.

### 3.2 Participant

A Participant is a user who takes part in running, walking or cycling events.

A participant can:

- Register an account.
- Log in securely.
- View available events.
- View event details and categories.
- Enrol in an event.
- View their own enrolments.
- Cancel an enrolment, subject to the API's rules.
- View their own results.
- View and update their own profile.

The API must enforce these permissions on the server. Hiding a feature in a user interface is not sufficient protection.

## 4. Technologies Used

| Technology | Purpose |
|---|---|
| C# | Main programming language |
| ASP.NET Core Web API | Builds the REST API |
| .NET 10 | Target framework |
| Entity Framework Core | Database access and object-relational mapping |
| Microsoft SQL Server | Relational database |
| JWT Bearer Authentication | Token-based authentication |
| Swagger / OpenAPI | API documentation and manual testing |
| xUnit | Automated testing framework |
| Git and GitHub | Source control and collaboration |
| GitHub Actions | Continuous integration and automated testing |
| Visual Studio | Development environment |

## 5. Project Structure

The project is organised into separate folders to make the code easier to maintain and understand.

```text
RaceDay/
│
├── RaceDay.Api/
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── UsersController.cs
│   │   ├── EventsController.cs
│   │   ├── RoutesController.cs
│   │   ├── CategoriesController.cs
│   │   ├── EnrolmentsController.cs
│   │   └── ResultsController.cs
│   │
│   ├── Data/
│   │   └── RaceDayContext.cs
│   │
│   ├── DTOs/
│   │   ├── RegisterDto.cs
│   │   ├── LoginDto.cs
│   │   ├── UpdateUserDto.cs
│   │   ├── EventDto.cs
│   │   ├── RouteDto.cs
│   │   ├── CategoryDto.cs
│   │   ├── EnrolmentDto.cs
│   │   └── ResultDto.cs
│   │
│   ├── Models/
│   │   ├── User.cs
│   │   ├── Event.cs
│   │   ├── Route.cs
│   │   ├── Category.cs
│   │   ├── Enrolment.cs
│   │   └── Result.cs
│   │
│   ├── Services/
│   │   └── TokenService.cs
│   │
│   ├── Properties/
│   │   └── launchSettings.json
│   │
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── Program.cs
│   └── RaceDay.Api.csproj
│
├── RaceDay.Tests/
│   ├── AuthTests.cs
│   ├── EventsTests.cs
│   ├── EnrolmentsTests.cs
│   ├── ResultsTests.cs
│   └── RaceDay.Tests.csproj
│
├── .github/
│   └── workflows/
│       └── part2.yml
│
├── README.md
└── RaceDay.Api.slnx
```

**Folder descriptions:**

- **Controllers:** Receive HTTP requests, apply application rules and return HTTP responses.
- **Data:** Contains the Entity Framework Core database context and database configuration.
- **DTOs:** Define the data accepted from and returned to API clients.
- **Models:** Represent the main database entities.
- **Services:** Contains reusable services, including JWT token generation.
- **RaceDay.Tests:** Contains automated tests.
- **.github/workflows:** Contains the GitHub Actions continuous integration workflow.

## 6. Database Design

RaceDay uses Microsoft SQL Server to store its information. Entity Framework Core provides access to the database through the `RaceDayContext` class.

The database contains six main entities.

| Table | Description |
|---|---|
| Users | Stores organiser and participant account information |
| Events | Stores event names, types, dates, times, locations and organiser references |
| Routes | Stores route information, including distance, start point and finish point |
| Categories | Stores the categories available for each event and their entry fees |
| Enrolments | Records participants entering events and selecting categories |
| Results | Stores finishing positions, finishing times and result statuses |

### 6.1 Database Relationships

The database relationships are designed as follows:

1. One organiser can manage many events.
2. Each event has one route.
3. One event can have many categories.
4. One participant can have many enrolments.
5. One event can have many enrolments.
6. One category can have many enrolments.
7. One enrolment can have zero or one result.

Primary keys uniquely identify records. Foreign keys connect related records and help maintain referential integrity.

Additional database constraints, such as `NOT NULL`, `UNIQUE`, `DEFAULT` and `CHECK`, should be configured according to the approved Part 1 database design.

**Important:** The EF Core model configuration must match the actual SQL Server schema from Part 1. The API should be tested against that schema before submission.

## 7. API Endpoints

The API is organised into functional areas. The following table lists the planned and implemented endpoint routes; each endpoint should be verified in Swagger before it is described as fully complete.

### 7.1 Authentication

| Method | Endpoint | Purpose |
|---|---|---|
| POST | `/api/auth/register` | Register a participant account |
| POST | `/api/auth/login` | Authenticate a user and return a JWT token |

Registration and login have been tested successfully in the development environment.

### 7.2 User Profile

| Method | Endpoint | Purpose |
|---|---|---|
| GET | `/api/users/me` | Retrieve the authenticated user's profile |
| PUT | `/api/users/me` | Update the authenticated user's profile |

These endpoints use the identity provided by the authenticated JWT rather than trusting a user ID supplied by the client.

### 7.3 Events

| Method | Endpoint | Purpose |
|---|---|---|
| GET | `/api/events` | Retrieve available events |
| GET | `/api/events/{eventId}` | Retrieve a specific event |
| POST | `/api/events` | Create an event |
| PUT | `/api/events/{eventId}` | Update an event |
| DELETE | `/api/events/{eventId}` | Delete an event |

Organiser permissions should be applied to event-management operations.

### 7.4 Routes

Routes are managed through the event-specific endpoints.

| Method | Endpoint | Purpose |
|---|---|---|
| GET | `/api/events/{eventId}/routes` | Retrieve the route for an event |
| POST | `/api/events/{eventId}/routes` | Create a route |
| PUT | `/api/events/{eventId}/routes/{routeId}` | Update a route |
| DELETE | `/api/events/{eventId}/routes/{routeId}` | Delete a route |

These routes describe the intended route-management interface. Verify that the controller implements the same paths and methods.

### 7.5 Categories

| Method | Endpoint | Purpose |
|---|---|---|
| GET | `/api/categories` | Retrieve categories |
| GET | `/api/events/{eventId}/categories` | Retrieve categories for an event |
| POST | `/api/events/{eventId}/categories` | Create a category for an event |
| PUT | `/api/categories/{categoryId}` | Update a category |
| DELETE | `/api/categories/{categoryId}` | Delete a category |

### 7.6 Enrolments

| Method | Endpoint | Purpose |
|---|---|---|
| POST | `/api/events/{eventId}/enrolments` | Enrol in an event |
| GET | `/api/enrolments/me` | Retrieve the authenticated participant's enrolments |
| GET | `/api/events/{eventId}/enrolments` | Retrieve enrolments for an event |
| DELETE | `/api/enrolments/{enrolmentId}` | Cancel or remove an enrolment |

Participants should only access their own enrolments. Organisers should only manage enrolments for events they are authorised to manage.

### 7.7 Results

| Method | Endpoint | Purpose |
|---|---|---|
| POST | `/api/events/{eventId}/results` | Record an event result |
| GET | `/api/results/me` | Retrieve the authenticated participant's results |
| GET | `/api/events/{eventId}/results` | Retrieve results for an event |
| PUT | `/api/results/{resultId}` | Update a result |
| DELETE | `/api/results/{resultId}` | Delete a result |

Results must be associated with valid enrolments, and access must follow the application's authorisation rules.

## 8. Authentication and Authorisation

RaceDay uses JSON Web Tokens (JWT) to authenticate users.

### 8.1 Registration

A participant submits registration information, including a first name, last name, email address and password. The API validates the information and creates the account if the request is valid.

Passwords should be stored as secure hashes rather than plain text.

### 8.2 Login

A user submits their email address and password. If the credentials are correct, the API generates a signed JWT containing identity and role claims.

The token can then be used to authenticate requests to protected endpoints.

### 8.3 Using the JWT

Protected requests must include an HTTP header in this format:

```http
Authorization: Bearer YOUR_JWT_TOKEN
```

Replace `YOUR_JWT_TOKEN` with the token returned by the login endpoint.

### 8.4 Authorisation

The API should verify that:

- The JWT signature is valid.
- The token has not expired.
- The issuer and audience are valid.
- The user has the required role.
- The user is permitted to access the requested resource.

A valid login does not automatically give a participant permission to perform organiser operations.

## 9. Running the Project Locally

### 9.1 Prerequisites

Install the following software:

- Visual Studio with the ASP.NET and web development workload.
- The .NET 10 SDK.
- Microsoft SQL Server or SQL Server LocalDB.
- SQL Server Management Studio (SSMS), if using SSMS to manage the database.
- Git, if cloning the repository.

### 9.2 Clone the Repository

Replace the placeholder below with the actual GitHub repository URL.

```bash
git clone YOUR_GITHUB_REPOSITORY_URL
```

Open the solution file:

```text
RaceDay.Api.slnx
```

### 9.3 Configure the Database Connection

Open the API project's configuration file and set the connection string to match your SQL Server instance.

Example for LocalDB:

```json
{
  "ConnectionStrings": {
    "RaceDayConnection": "Server=(localdb)\\MSSQLLocalDB;Database=RaceDay;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

If your SQL Server uses a different instance name or authentication method, update the connection string accordingly.

Ensure that the `RaceDay` database and its six tables exist before testing database-dependent endpoints.

### 9.4 Configure JWT Settings

The API requires JWT configuration for token creation and validation.

Configure the following settings using .NET User Secrets or environment variables:

- `Jwt:Key`
- `Jwt:Issuer`
- `Jwt:Audience`

Use a sufficiently strong secret key and ensure that the issuer and audience match between token generation and token validation.

**Security:** Never commit a production JWT signing key, database password or other credentials to GitHub. Keep real secrets out of source control.

### 9.5 Restore and Build

Open a terminal in the directory containing `RaceDay.Api.slnx` and run:

```bash
dotnet restore RaceDay.Api.slnx
dotnet build RaceDay.Api.slnx --configuration Release
```

Resolve any compilation errors before proceeding.

### 9.6 Run the API

Run the API project from Visual Studio or use:

```bash
dotnet run --project RaceDay.Api/RaceDay.Api.csproj
```

The actual project path may differ depending on the repository structure.

When the application starts, use the local HTTPS address shown in the terminal.

### 9.7 Open Swagger

Open the Swagger page using the HTTPS address printed by the application, followed by `/swagger`.

For example:

```text
https://localhost:7271/swagger
```

The port may be different on another computer.

Swagger can be used to inspect the API documentation, submit JSON requests and review the HTTP responses.

## 10. Testing the API

### 10.1 Manual Testing

Use Swagger to test the endpoints.

Recommended test sequence:

1. Register a participant using `POST /api/auth/register`.
2. Log in using `POST /api/auth/login`.
3. Copy the JWT token from the login response.
4. Authorise Swagger with the token if the Authorize feature is configured.
5. Test `GET /api/users/me`.
6. Test event listing and event details.
7. Test organiser-only operations using an authorised organiser account.
8. Test participant enrolments.
9. Test result retrieval and management.
10. Check validation failures and unauthorised requests.

### 10.2 HTTP Status Codes

The API should return suitable HTTP status codes.

| Status code | Meaning |
|---|---|
| 200 OK | The request completed successfully |
| 201 Created | A resource was created successfully |
| 204 No Content | The request succeeded without a response body |
| 400 Bad Request | The submitted information is invalid |
| 401 Unauthorized | Authentication is missing or invalid |
| 403 Forbidden | The user does not have permission |
| 404 Not Found | The requested resource does not exist |
| 409 Conflict | The request conflicts with existing data |
| 500 Internal Server Error | An unexpected server-side error occurred |

The exact response should be verified for each endpoint.


<img width="863" height="455" alt="Screenshot 2026-10-14 152218" src="https://github.com/user-attachments/assets/fcd296c6-dbc4-4c07-bbde-dd0acc8cbe36" />

<img width="941" height="535" alt="Screenshot 2026-10-14 152012" src="https://github.com/user-attachments/assets/bb3d20be-4b82-41de-90f0-41b516d1030f" />

<img width="934" height="496" alt="Screenshot 2026-10-14 152028" src="https://github.com/user-attachments/assets/01a19247-5408-4c08-bd5a-d61b8b67a872" />

<img width="928" height="444" alt="Screenshot 2026-10-14 152048" src="https://github.com/user-attachments/assets/ad49bbfd-0a01-4776-a880-685cfcd18d1b" />

<img width="561" height="272" alt="Screenshot 2026-10-14 152101" src="https://github.com/user-attachments/assets/3ecbe72f-439a-4f49-8bc8-c9c1f2d7cc76" />

<img width="931" height="484" alt="Screenshot 2026-10-14 152112" src="https://github.com/user-attachments/assets/ce3e09a7-5409-4cce-b0f1-456c08a23301" />



## 11. Automated Tests

RaceDay includes a separate xUnit test project named `RaceDay.Tests`.

The current test files are:

| Test file | Purpose |
|---|---|
| `AuthTests.cs` | Tests registration and login DTO validation |
| `EventsTests.cs` | Tests event DTO validation |
| `EnrolmentsTests.cs` | Tests enrolment DTO validation |
| `ResultsTests.cs` | Tests result DTO validation, including a valid enrolment ID |

The current validation tests check whether valid data passes validation and, where included, whether invalid data is rejected.

### Run the Tests

From the solution directory, run:

```bash
dotnet test RaceDay.Api.slnx --configuration Release
```

The build and tests must complete successfully before the results are considered passing.

**Testing limitation:** DTO validation tests do not, by themselves, verify database persistence, JWT authentication, endpoint permissions or complete API workflows. These behaviours should be covered by additional integration tests.

## 12. Continuous Integration with GitHub Actions

The workflow file is located at:

```text
.github/workflows/part2.yml
```

The workflow is configured to run when changes are pushed to the configured branches or when a pull request targets them. It can also be started manually through GitHub Actions.

The workflow performs these steps:

1. Checks out the repository.
2. Installs the .NET 10 SDK.
3. Restores NuGet dependencies.
4. Cleans and builds the solution.
5. Runs the automated test suite.

### Checking the Workflow

1. Push your changes to GitHub.
2. Open your repository.
3. Select the **Actions** tab.
4. Open the latest RaceDay Part 2 workflow run.
5. Review the build and test steps.
6. Confirm that the workflow finishes successfully.

A green check indicates that the configured workflow steps succeeded for that commit. It does not guarantee that every possible API scenario has been tested.

### CI Screenshot

<img width="706" height="283" alt="Screenshot 2026-10-14 213927" src="https://github.com/user-attachments/assets/57fa2a77-42fb-4b17-a7fe-4b1d55ee9089" />

<img width="943" height="485" alt="Screenshot 2026-10-14 214026" src="https://github.com/user-attachments/assets/88121df1-2ce6-42a8-b14d-b0808f7595c5" />

<img width="930" height="414" alt="Screenshot 2026-10-14 214003" src="https://github.com/user-attachments/assets/5262b06b-068a-4444-a897-ab1a52f874f7" />

## 13. Error Handling and Validation

The API uses DTOs and validation attributes to check incoming data.

Examples of validation include:

- Required names and email addresses.
- Valid email formats.
- Positive identifiers.
- Valid event and category information.
- Appropriate result data.

The API should reject invalid requests and return clear, appropriate HTTP responses.

Database constraints should also be maintained to prevent invalid relationships and duplicate data. Unexpected exceptions should be handled safely without exposing passwords, signing keys, connection strings or other sensitive information.

## 14. Security Considerations

The following security practices apply to RaceDay:

- Store passwords as secure hashes.
- Use JWT Bearer authentication for protected endpoints.
- Validate token issuer, audience, signature and expiry.
- Enforce organiser and participant permissions on the server.
- Prevent participants from accessing other participants' private records.
- Validate incoming request data.
- Use HTTPS during normal development and deployment.
- Keep database credentials and JWT secrets out of Git.
- Avoid returning password hashes or sensitive authentication information in API responses.

## 15. Known Limitations and Future Improvements

Possible future improvements include:

- Expand integration testing for all API endpoints.
- Add tests for organiser and participant permissions.
- Improve consistent error responses across all controllers.
- Add pagination and filtering for large event lists.
- Improve logging and monitoring.
- Add stronger validation for duplicate enrolments and event capacity.
- Verify that database transactions maintain data consistency.
- Add deployment configuration and production secret management.
- Expand automated tests for database operations and authentication.

These improvements should only be marked as completed after they have been implemented and tested.

## 16. GitHub Repository

https://github.com/moraisgomes123/RaceDayPart2

The repository should contain the API source code, test project, workflow configuration and documentation required for the submission.

## 17. Video Demonstration

https://www.youtube.com/watch?v=vX5R31mrAus

The video should demonstrate the actual implementation and explain:

1. The project structure.
2. The database models and relationships.
3. Registration and login.
4. JWT authentication and protected endpoints.
5. Event, category, enrolment and result endpoints.
6. Swagger testing and example responses.
7. The automated test suite.
8. The GitHub Actions workflow and its result.

Follow the video requirements provided by the lecturer, including any rules about voiceover and video visibility.


## 18. Conclusion

RaceDay Part 2 develops the backend API for the RaceDay Event Management System. It uses ASP.NET Core Web API, C#, Entity Framework Core and SQL Server to manage users, events, routes, categories, enrolments and results.

JWT authentication helps protect the API, while role-based authorisation controls access to management functions. Swagger supports manual API testing, xUnit supports automated validation tests, and GitHub Actions automates the build and test process.

The project should be considered ready for submission only after the database schema, required endpoints, authorisation rules, automated tests, CI workflow and documentation have been verified against the assessment requirements.
