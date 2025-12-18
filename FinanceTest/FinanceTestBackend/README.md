# FinanceTest API

This is a simple API for managing financial transactions. It is built with .NET and uses a PostgreSQL database.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://www.docker.com/products/docker-desktop)

### Running the application with Docker

1.  **Clone the repository**

    ```bash
    git clone <repository-url>
    cd FinanceTest
    ```

2.  **Run with Docker Compose**

    To start the application and the PostgreSQL database, run the following command in the root directory of the project:

    ```bash
    docker-compose up --build
    ```

    This command will build the Docker image for the API and start the containers for the API and the database.

3.  **Accessing the application**

    Once the containers are running, you can access the API at `http://localhost:8080`.

4. **API Documentation**

   You can access the API documentation at [http://localhost:8080/scalar/v1](http://localhost:8080/scalar/v1).

5. **Accessing the database**

   The PostgreSQL database is exposed on port `22653`. You can connect to it using your favorite database client with the following credentials:

    *   **Host**: `localhost`
    *   **Port**: `22653`
    *   **Database**: `finance_test`
    *   **Username**: `postgres`
    *   **Password**: `123456`

Make sure you have a PostgreSQL instance running. Create a database named `finance_test` and update the connection string in `src/FinanceTest.Api/appsettings.json` if necessary.

2. **Run the database migrations**

    Navigate to the `src/FinanceTest.Api` directory and run the following command to apply the database migrations:

    ```bash
    dotnet ef database update
    ```

3.  **Run the application**

    In the same directory, run the following command to start the application:

    ```bash
    dotnet run
    ```

    The API will be available at `http://localhost:5289` or `https://localhost:7091`.

- **API Documentation (Local):** `http://localhost:5289/scalar/v1`

## Libraries Used

This project utilizes several open-source libraries to enhance its functionality. Below is a list of the key libraries and their roles:

- **ASP.NET Core:** A cross-platform, high-performance framework for building modern, cloud-based, internet-connected applications.
- **Entity Framework Core:** A modern object-database mapper for .NET. It supports LINQ queries, change tracking, updates, and schema migrations.
- **PostgreSQL:** A powerful, open-source object-relational database system.
- **Serilog:** A diagnostic logging library for .NET applications that is easy to set up, has a clean API, and runs on all recent .NET platforms.
- **FluentValidation:** A popular .NET library for building strongly-typed validation rules.
- **MediatR:** A simple, unambitious mediator implementation in .NET. It helps to decouple the in-process sending of messages from handling messages.
- **Scalar:** A powerful and customizable API documentation tool that provides a rich user interface for exploring and interacting with APIs.
- **Docker:** A set of platform-as-a-service products that use OS-level virtualization to deliver software in packages called containers.
- **Docker Compose:** A tool for defining and running multi-container Docker applications.

### Testing Libraries

- **xUnit:** A free, open-source, community-focused unit testing tool for the .NET Framework.
- **FluentAssertions:** A set of .NET extension methods that allow you to more naturally specify the expected outcome of a TDD or BDD-style test.
- **Moq:** The most popular and friendly mocking framework for .NET.
- **Coverlet:** A cross-platform code coverage framework for .NET.