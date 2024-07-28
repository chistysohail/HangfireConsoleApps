# Hangfire Console Apps

This project demonstrates how to use Hangfire to manage background jobs in .NET console applications. The project includes multiple console applications that interact with a Hangfire server to process jobs and update their statuses in a SQL Server database.

## Project Structure

- `HangfireConsoleApp1`: Console application that enqueues jobs for processing.
- `HangfireConsoleApp2`: Another console application that enqueues jobs for processing.
- `HangfireConsoleApp3`: Another console application that enqueues jobs for processing.
- `HangfireServerApp`: The Hangfire server application that manages job processing.
- `JobProcessorLib`: A library containing the job processing logic.

## Prerequisites

- Docker
- Docker Compose

## Setup Instructions

1. **Clone the repository**:
   ```bash
   git clone https://github.com/chistysohail/HangfireConsoleApps.git
   cd hangfire-console-apps
   
2. Build and start the Docker containers:
 docker-compose up --build

This command will:

Build the Docker images for each application.
Start the SQL Server container.
Start the Hangfire server and console applications.

3. Stop the Docker containers:
   docker-compose down

Database Setup
Connection String
All projects connect to an MS SQL Server database running in a separate Docker container using the following connection string:
Server=host.docker.internal,1434;Database=HangfireApps;User Id=sa;Password=YourNewStrong(!)Password;MultipleActiveResultSets=true;TrustServerCertificate=True;Connection Timeout=30;

Run the following SQL script to create the database, tables, and insert initial data.

-- Create the database

CREATE DATABASE HangfireApps;

-- Use the database

USE HangfireApps;


-- Create the Jobs table

CREATE TABLE Jobs (
    JobId INT PRIMARY KEY IDENTITY(1,1),
    Status NVARCHAR(50) NOT NULL,
    ProcessedBy NVARCHAR(50) NULL,
    CreatedAt DATETIME NOT NULL
);


-- Insert initial data

INSERT INTO Jobs (Status, CreatedAt) VALUES ('Pending', GETDATE());
INSERT INTO Jobs (Status, CreatedAt) VALUES ('Pending', GETDATE());
INSERT INTO Jobs (Status, CreatedAt) VALUES ('Pending', GETDATE());
INSERT INTO Jobs (Status, CreatedAt) VALUES ('Pending', GETDATE());
INSERT INTO Jobs (Status, CreatedAt) VALUES ('Pending', GETDATE());

Job Processing Logic
The JobProcessorLib library contains the job processing logic. Here's a brief overview of how jobs are processed:

A job is enqueued using Hangfire's BackgroundJob.Enqueue method.
The job processor updates the job status to "Processing" and sets the ProcessedBy field to the worker name.
After processing, the job status is updated to "Completed".

License
This project is licensed under the MIT License. See the LICENSE file for details.
This `README.md` file now includes details about the MS SQL Server database connection, the connection string, and the SQL script to create the necessary database and tables. You can further customize it as needed.

![image](https://github.com/user-attachments/assets/cb39b9e3-c976-4b2b-976d-7af0aa200986)
![image](https://github.com/user-attachments/assets/f206f425-0abc-4d86-beae-446c116f46e0)
