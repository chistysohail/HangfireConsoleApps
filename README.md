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

sql scripts used in testing:
/*
CREATE TABLE Jobs (
    JobId INT IDENTITY(1,1) PRIMARY KEY,
    Status NVARCHAR(50),
    ProcessedBy NVARCHAR(100),
    CreatedAt DATETIME DEFAULT GETDATE()
);


INSERT INTO Jobs (Status, ProcessedBy)
VALUES 
    ('Pending', NULL),
    ('Pending', NULL),
    ('Pending', NULL),
    ('Pending', NULL),
    ('Pending', NULL);
	*/

	select * from Jobs
	--delete from Jobs
	--drop Table Jobs

	/*

DECLARE @sql NVARCHAR(MAX) = N'';
-- Generate DROP TABLE statements for all tables in the HangFire schema
SELECT @sql += N'DROP TABLE HangFire.' + QUOTENAME(name) + ';'
FROM sys.tables
WHERE schema_id = SCHEMA_ID('HangFire');

-- Execute the generated SQL statements
EXEC sp_executesql @sql;

*/

if cmd "docker compose up --build" not working try :

# Build the Docker images

docker build -t hangfire-server -f ./HangfireServerApp/Dockerfile .

docker build -t consoleapp1 -f ./HangfireConsoleApp1/Dockerfile .

docker build -t consoleapp2 -f ./HangfireConsoleApp2/Dockerfile .

docker build -t consoleapp3 -f ./HangfireConsoleApp3/Dockerfile .

# Create a custom network

docker network create hangfire-net

# Run the Docker containers

docker run -d --name hangfire-server --network hangfire-net -e ASPNETCORE_ENVIRONMENT=Production -p 5000:80 hangfire-server

docker run -d --name consoleapp1 --network hangfire-net -e ASPNETCORE_ENVIRONMENT=Production --link hangfire-server consoleapp1

docker run -d --name consoleapp2 --network hangfire-net -e ASPNETCORE_ENVIRONMENT=Production --link hangfire-server consoleapp2

docker run -d --name consoleapp3 --network hangfire-net -e ASPNETCORE_ENVIRONMENT=Production --link hangfire-server consoleapp3

# Stop and remove the containers

docker stop hangfire-server consoleapp1 consoleapp2 consoleapp3

docker rm hangfire-server consoleapp1 consoleapp2 consoleapp3


