# LEI-SEM5-PI-2025-26-3DJ-01
After cloning, run `git submodule update --init --recursive` to include all submodules:
1. [Three.js](https://github.com/mrdoob/three.js)

### 1. Description of the Project

> The system is designed to enhance sustainable and efficient port logistics, focusing on vessel management, cargo handling, and resource allocation. It aims to improve coordination among port authorities, shipping agents, and logistics operators, ensuring timely vessel turnaround, optimal resource usage, and compliance with safety and environmental standards. The system incorporates intelligent tools for planning, scheduling, and monitoring port operations, including dock assignment, task sequencing, and resource management. It also provides 3D visualization features for users to interact with and track vessel positions and cargo locations.

> The prototype will be a modular, web-based solution that integrates core functionalities for port management, supporting both traditional operations and advanced features like scheduling algorithms. It will ensure GDPR compliance, disaster recovery, and business continuity for seamless, real-time decision-making and coordination. The system will demonstrate its feasibility through a simplified but realistic context, highlighting the benefits of advanced logistics tools and interactive visualization in improving port operations.

### 2. Planning and Technical Documentation

> [Planning and Technical Documentation](docs/readme.md)

### 3. How to Build
For a pre-built version of the software, refer to the [Releases](https://github.com/Departamento-de-Engenharia-Informatica/LEI-SEM5-PI-2025-26-3DJ-01/releases) page, available for Windows and Linux systems.

#### Building the Backend API
You will need the following dependencies:
- .NET==9.0

Navigate to the `Backend/Api` directory and use `dotnet build`. 
After building, the compiled output should be at `Backend/Api/bin/Debug/net9.0/Api.exe` (or `Api.elf`). 

#### Building the Frontend
You will need the following dependencies:
- Node.js (>=20.19.0 or >=22.12.0)

Navigate to the `FrontEnd` directory and run:
```
npm install
npm run build
```
The compiled output will be in the `FrontEnd/dist` directory.

#### Building the OEM Backend
You will need the following dependencies:
- Node.js (>=20.19.0 or >=22.12.0)
- TypeScript

Navigate to the `OEMBackend` directory and run:
```
npm install
npm run build
```
The compiled output will be in the `OEMBackend/dist` directory.

#### Building the Prolog Scheduling Server
You will need the following dependencies:
- SWI-Prolog (>=8.0.0)

No compilation is needed for the Prolog server. The server is ready to run directly from the `SchedulingNPlanning` directory.

### 4. How to Execute Tests
For the complete testing instructions read [this](docs/global_docs/testing-guide.md).

### 5. How to Run
To run the complete system in development mode, you will need:
- .NET==9.0
- Node.js (>=20.19.0 or >=22.12.0)
- SWI-Prolog (>=8.0.0)

#### Using the convenience scripts
Navigate to the `script` directory and execute `app-run.cmd` (Windows) to start all services.

#### Running components individually

**Backend API (Development)**
Navigate to the `Backend/Api` directory and use:
```
dotnet run
```
Alternatively from the `Backend` directory:
```
dotnet run --project Api
```
The API will be available at `http://localhost:5195` with Swagger UI at `http://localhost:5195/swagger/index.html?url=/openapi/v1.json`.

**Frontend (Development)**
Navigate to the `FrontEnd` directory and run:
```
npm install
npm run dev
```
For local testing mode:
```
npm run local
```
The frontend will be available at `http://localhost:5173` (or as indicated in the console).

**OEM Backend (Development)**
Navigate to the `OEMBackend` directory and run:
```
npm install
npm run dev
```
For local testing mode:
```
npm run local
```
The OEM backend will generate API documentation and start the development server with hot reload.

**Prolog Scheduling Server (Development)**
Navigate to the `SchedulingNPlanning` directory and run:
```
swipl main.pl
```
For testing mode:
```
swipl main.pl test
```
The server will start on port 4000 by default.
Testing mode uses the regular swipl user interaction console, regular mode is not interactible

For a comprehensive list of Backend API endpoints and their uses, refer to the [Developer Manual](docs/sprint_1/user_manual.md)/[Developer Manual (curl)](docs/sprint_1/user_manual_with_curl.md).

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

### 8. 3rd party assets used
Every asset that is not in the list bellow is of our own

- [Skiing penguins skybox pack (opengameart.org)](https://opengameart.org/content/skiingpenguins-skybox-pack)
- [Boat V2 3D Model](https://free3d.com/3d-model/boat-v2--225787.html)
- [Buoy 3D model by TepidGames](https://sketchfab.com/3d-models/free-low-poly-buoy-8e7797a922304f7aab23ffe534a47839)
- [Lighthouse 3D Model by apocalypse_67](https://free3d.com/3d-model/lighthouse-44581.html)
- [Threex.daynight](https://github.com/jeromeetienne/threex.daynight)
- [Wooden texture background by dotstudio](https://www.freepik.com/free-vector/wooden-texture-background_851099.htm#fromView=keyword&page=1&position=3&uuid=c4d347d5-55a6-479e-8835-00f7babebaa7&query=Wooden+road+texture)
- [Flying seagull](https://sketchfab.com/3d-models/flying-seagull-07dde3ea7d9048588d3c4edfd37ac20d)