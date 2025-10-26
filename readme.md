# LEI-SEM5-PI-2025-26-3DJ-01

### 1. Description of the Project

> The system is designed to enhance sustainable and efficient port logistics, focusing on vessel management, cargo handling, and resource allocation. It aims to improve coordination among port authorities, shipping agents, and logistics operators, ensuring timely vessel turnaround, optimal resource usage, and compliance with safety and environmental standards. The system incorporates intelligent tools for planning, scheduling, and monitoring port operations, including dock assignment, task sequencing, and resource management. It also provides 3D visualization features for users to interact with and track vessel positions and cargo locations.

> The prototype will be a modular, web-based solution that integrates core functionalities for port management, supporting both traditional operations and advanced features like scheduling algorithms. It will ensure GDPR compliance, disaster recovery, and business continuity for seamless, real-time decision-making and coordination. The system will demonstrate its feasibility through a simplified but realistic context, highlighting the benefits of advanced logistics tools and interactive visualization in improving port operations.

### 2. Planning and Technical Documentation

> [Planning and Technical Documentation](docs/readme.md)

### 3. How to Build
For a pre-built version of the software, refer to the [Releases](https://github.com/Departamento-de-Engenharia-Informatica/LEI-SEM5-PI-2025-26-3DJ-01/releases) page, available for Windows and Linux systems. 
You will need the following dependencies to build this project:
- .NET==9.0

#### Building with dotnet
Navigate to the `Backend/Api` directory and use `dotnet build`. 
After building, the compiled output should be at `Backend/Api/bin/Debug/net9.0/Api.exe` (or `Api.elf`). 

Unlike the development version, this production build has no UI. To check if it's working, curl (or visit through a web browser) this URL: `http://localhost:5000/Qualification` (host and port subject to change, please double check with the console logs).

### 4. How to Execute Tests
You will need the following dependencies to test this project:
- .NET==9.0

To test, there are two options:
#### Using the scripts
Navigate to the `scripts` directory and from there execute either `test.cmd` or `test.sh` depending on your system.

#### Testing with dotnet
Navigate to the `Backend/Tests` directory and use `dotnet test`

### 5. How to Run
You will need the following dependencies to run this project:
- .NET==9.0

To run in development mode, there are two options:
#### Using the scripts
Navigate to the `scripts` directory and from there execute either `run.cmd` or `run.sh` depending on your system.

#### Running with dotnet
Navigate to the `Backend/Api` directory and use `dotnet run`, alternatively use `dotnet run --project Api` from the `Backend` directory.

This version of the software allows you to visualise and interact with the API via a [Swagger UI](https://swagger.io/tools/swagger-ui/).
To access it, visit `http://localhost:5195/swagger/index.html?url=/openapi/v1.json` (host and port subject to change, please double check with the console logs) from any capable browser.

For a comprehensive list of endpoints and their uses, refer to the [Developer Manual](docs/user_manual.md)

### 6. How to read Logs
Logs of the application are made virtually everytime the API has to run any sort of code. 
They are achieved with [Serilog](https://github.com/serilog/serilog) and output to both the console and a file.
To edit the configuration, open `Backend/Api/appsettings.json` and modify it as you see fit.

All logs can be found on the `Backend/Api/Logs` folder, they are seperated by day, which means if you want to consult a certain day's logs you must check: `Backend/Api/Logs/audit-{year}{month}{day}.log`.

An example log message is: 
```
[2025-10-17 14:30:19 INF] ({ Name: "ExecutingEndpoint" }) Executing endpoint 'Api.Application.Controllers.QualificationController.Filter (Api)'
```
They all follow the same format, [date time LEVEL] (information) Message

Since the logs are private and essential for troubleshooting, things like which routes were accessed by who and even database querries are all being logged.
If that is not of need for you, for a cleaner logging experience modify the `LogLevel` in `appsettings.json` to `Error`.

### 7. How to Install/Deploy into Another Machine (or Virtual Machine)

> TODO

