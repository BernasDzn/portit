# Developer Manual (with PowerShell curl examples)
This is a copy of the developer manual with one-line PowerShell `curl` examples (single-line, http://localhost:5000, port 5000, double quotes only) inserted after each request example.

## Staff

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /Staff

> Gets all staff members in the system.

> When we execute, we should see 3 staff that we bootstraped:
<br>**João Pedro**, **Carlos Santos** and **Maria Silva**

```
curl -Method GET -Uri "http://localhost:5000/Staff" -Headers @{accept="text/plain"; "Content-Type"="application/json"}
```

### <img src="svg/post.svg" height="20" style="position: relative; top: 4px;"> /Staff

> Staff called Rodrigo Pinto<br>
works from 8 to 17 on work days<br>
has "Yard Crane Operator" qualification

```json
{
  "mechanograficNumber": "TESTMEC01",
  "name": "Rodrigo Pinto",
  "email": "r.pinto@gmail.com",
  "phoneNumber": "912021021",
  "status": 0,
  "operationalWindow": {
    "shifts": [
		{
			"day": 1,
			"startTime": "08:00:00",
			"endTime": "17:00:00"
		},
		{
			"day": 2,
			"startTime": "08:00:00",
			"endTime": "17:00:00"
		},
		{
			"day": 3,
			"startTime": "08:00:00",
			"endTime": "17:00:00"
		},
		{
			"day": 4,
			"startTime": "08:00:00",
			"endTime": "17:00:00"
		},
		{
			"day": 5,
			"startTime": "08:00:00",
			"endTime": "17:00:00"
		}
	]
  },
  "qualificationsCodes": [
    "YACOP"
  ]
}
```

```
curl -Method POST -Uri "http://localhost:5000/Staff" -Headers @{accept="text/plain"; "Content-Type"="application/json"} -Body "{`"mechanograficNumber`": `"TESTMEC01`",`"name`": `"Rodrigo Pinto`",`"email`": `"r.pinto@gmail.com`",`"phoneNumber`": `"912021021`",`"status`": 0,`"operationalWindow`": {`"shifts`": [{`"day`":1,`"startTime`":`"08:00:00`",`"endTime`":`"17:00:00`"}]},`"qualificationsCodes`": [`"YACOP`"]}"
```

### <img src="svg/put.svg" height="20" style="position: relative; top: 4px;"> /Staff/{mecanographicNumber}

> Staff with mechanografic number TESTMEC01<br>
Full name is actually **Rodrigo Faria Pinto**<br>
Email had a typo, actual email is: **r.fpinto@gmail.com**
Changed work period to: **Mondays to Tuesdays, 8 to 18**<br>
Has "Yard Crane Operator" **and Truck Driver** qualification<br>

```json
{
  "mechanograficNumber": "TESTMEC01",
  "name": "Rodrigo Faria Pinto",
  "email": "r.fpinto@gmail.com",
  "phoneNumber": "912021021",
  "status": 0,
  "operationalWindow": {
    "shifts": [
		{
			"day": 1,
			"startTime": "08:00:00",
			"endTime": "18:00:00"
		},
		{
			"day": 2,
			"startTime": "08:00:00",
			"endTime": "18:00:00"
		},
		{
			"day": 3,
			"startTime": "08:00:00",
			"endTime": "18:00:00"
		},
		{
			"day": 4,
			"startTime": "08:00:00",
			"endTime": "18:00:00"
		}
	]
  },
  "qualificationsCodes": [
    "YACOP",
	"TRKDR"
  ]
}
```

```
curl -Method PUT -Uri "http://localhost:5000/Staff/TESTMEC01" -Headers @{accept="text/plain"; "Content-Type"="application/json"} -Body "{`"mechanograficNumber`": `"TESTMEC01`",`"name`": `"Rodrigo Faria Pinto`",`"email`": `"r.fpinto@gmail.com`",`"phoneNumber`": `"912021021`",`"status`": 0,`"operationalWindow`": {`"shifts`": [{`"day`":1,`"startTime`":`"08:00:00`",`"endTime`":`"18:00:00`"}]},`"qualificationsCodes`": [`"YACOP`",`"TRKDR`"]}"
```

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /Staff/filter

> Filters staffs by any field.

> Let's try filtering all Staffs with **"Faria"** in their names and the **Truck Driver** qualification.
<br>Since **Rodrigo Faria Pinto** is the only staff that meets there requirements, only he should be returned.

```
Name: Faria

QualificationCodes: TRKDR
```

```
curl -Method GET -Uri "http://localhost:5000/Staff/filter?Name=Faria&QualificationCodes=TRKDR" -Headers @{accept="text/plain"; "Content-Type"="application/json"}
```

### <img src="svg/delete.svg" height="20" style="position: relative; top: 4px;"> /Staff/{mecanographicNumber}

> Acts as a soft delete, in domain terms, it **deactivates** the staff member.
<br>The company now wants to deactivate the staff **Rodrigo Faria Pinto**:

```json
TESTMEC01
```

```
curl -Method DELETE -Uri "http://localhost:5000/Staff/TESTMEC01" -Headers @{accept="text/plain"; "Content-Type"="application/json"}
```

## Dock

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /Dock

> Gets all docks in the system.

> When we execute, we should see 3 docks that we bootstraped:
<br>**DCK001 (Dock A)**, **DCK002 (Dock B)** and **DCK003 (Dock C)**

```
curl -Method GET -Uri "http://localhost:5000/Dock" -Headers @{accept="text/plain"; "Content-Type"="application/json"}
```

### <img src="svg/post.svg" height="20" style="position: relative; top: 4px;"> /Dock

> Dock with code DCK004<br>
Name is Dock D,
has supported vessel type "Panamax"
and location "North Harbor". <br> 
The physical characteristics need to be bigger than the vessel type selected.

```json
{
  "code": "DCK004",
  "name": "Dock D",
  "location": "North Harbor",
  "physicalCharacteristics": {
    "length": 300,
    "depth": 30,
    "draft": 20
  },
  "supportedVesselTypes": [
    "Panamax"
  ]
}
```

```
curl -Method POST -Uri "http://localhost:5000/Dock" -Headers @{accept="text/plain"; "Content-Type"="application/json"} -Body "{`"code`": `"DCK004`",`"name`": `"Dock D`",`"location`": `"North Harbor`",`"physicalCharacteristics`": {`"length`":300,`"depth`":30,`"draft`":20},`"supportedVesselTypes`": [`"Panamax`"]}"
```

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /Dock/{code}

Should return the dock with code (eg. DCK004, created previously):
```json
{
  "code": "DCK004",
  "name": "Dock Test",
  "location": "East Harbor",
  "physicalCharacteristics": {
    "length": 500,
    "depth": 30,
    "draft": 20
  },
  "supportedVesselTypes": [
    {
      "name": "Post-Panamax",
      "description": "Larger than Panamax",
      "maxNumberOfRows": 30,
      "maxNumberOfBays": 15,
      "maxNumberOfTiers": 7,
      "capacity": 3150,
      "physicalCharacteristics": {
        "length": 400,
        "depth": 18,
        "draft": 14
      }
    },
    {
      "name": "Panamax",
      "description": "Max size for Panama Canal",
      "maxNumberOfRows": 20,
      "maxNumberOfBays": 10,
      "maxNumberOfTiers": 5,
      "capacity": 1000,
      "physicalCharacteristics": {
        "length": 300,
        "depth": 15,
        "draft": 12
      }
    }
  ]
}
```

```
curl -Method GET -Uri "http://localhost:5000/Dock/DCK004" -Headers @{accept="text/plain"; "Content-Type"="application/json"}
```

### <img src="svg/put.svg" height="20" style="position: relative; top: 4px;"> /Dock/{code}

> Dock with code DCK004 (Created previously)<br>
Name is actually **Dock Test**<br>
Location was wrong and the new one is: **East Harbor**
The physical characteristics are bigger than expected: **Length: 500, Depth: 50, Draft: 35**<br>
Has "Panamax" **and Post-Panamax** supported vessel types<br>

```json
{
  "code": "DCK004",
  "name": "Dock Test",
  "location": "East Harbor",
  "physicalCharacteristics": {
    "length": 500,
    "depth": 30,
    "draft": 20
  },
  "supportedVesselTypes": [
    "Panamax",
    "Post-Panamax"
  ]
}
```

```
curl -Method PUT -Uri "http://localhost:5000/Dock/DCK004" -Headers @{accept="text/plain"; "Content-Type"="application/json"} -Body "{`"code`":`"DCK004`",`"name`":`"Dock Test`",`"location`":`"East Harbor`",`"physicalCharacteristics`":{`"length`":500,`"depth`":30,`"draft`":20},`"supportedVesselTypes`":[`"Panamax`",`"Post-Panamax`"]}"
```

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /Dock/filter

> Filters docks by any field.

Query Parameters:
- name (string, optional): Filter by dock name.
- location (string, optional): Filter by dock location.
- type (string, optional): Filter by supported vessel type.
- pageNumber (int, optional): Page number for pagination (default is 1).
- pageSize (int, optional): Number of items per page (default is 10).

> Let's try filtering all docks with **"Dock"** in their names, **"North"** in their location and the **Panamax** vessel type.

Since **DCK001** is the only dock that meets the requirements, it is the only one that's returned.

```
curl -Method GET -Uri "http://localhost:5000/Dock/filter?name=Dock&location=North&type=Panamax&pageNumber=1&pageSize=10" -Headers @{accept="text/plain"; "Content-Type"="application/json"}
```

## Vessel

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /Vessel

> Should return a list of all vessels in the database.

```
curl -Method GET -Uri "http://localhost:5000/Vessel" -Headers @{accept="text/plain"; "Content-Type"="application/json"}
```

### <img src="svg/post.svg" height="20" style="position: relative; top: 4px;"> /Vessel
```json
{
  "name": "New Vessel",
  "imoNumber": "IMO 4569858",
  "type": "Panamax",
  "owner": "Global Shipping Co.",
  "length": 300,
  "depth": 15,
  "draft": 12
}
```

```
curl -Method POST -Uri "http://localhost:5000/Vessel" -Headers @{accept="text/plain"; "Content-Type"="application/json"} -Body "{`"name`":`"New Vessel`",`"imoNumber`":`"IMO 4569858`",`"type`":`"Panamax`",`"owner`":`"Global Shipping Co.`",`"length`":300,`"depth`":15,`"draft`":12}"
```

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /Vessel/{imo}

> Should return the vessel with IMO Number (eg. IMO 3815389):

```
curl -Method GET -Uri "http://localhost:5000/Vessel/IMO%203815389" -Headers @{accept="text/plain"; "Content-Type"="application/json"}
```

### <img src="svg/put.svg" height="20" style="position: relative; top: 4px;"> /Vessel/{imo}

```json
{
  "name": "Update Vessel Name",
  "imoNumber": "IMO 3815389",
  "type": "Post-Panamax",
  "owner": "Global Shipping Co.",
  "length": 400,
  "depth": 18,
  "draft": 14
}
```

```
curl -Method PUT -Uri "http://localhost:5000/Vessel/IMO%203815389" -Headers @{accept="text/plain"; "Content-Type"="application/json"} -Body "{`"name`":`"Update Vessel Name`",`"imoNumber`":`"IMO 3815389`",`"type`":`"Post-Panamax`",`"owner`":`"Global Shipping Co.`",`"length`":400,`"depth`":18,`"draft`":14}"
```

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /Vessel/filter

>Should return a list of vessels that match the filter criteria.\
>Query Parameters:
>- name (string, optional): Filter by vessel name.
>- type (string, optional): Filter by vessel type.
>- owner (string, optional): Filter by vessel owner.
>- pageNumber (int, optional): Page number for pagination (default is 1).
>- pageSize (int, optional): Number of items per page (default is 10).\
>When filtering, after running the previous requests, the "Vessel" name filter should be a good example, returning both the "New Vessel" and "Update Vessel Name" entries.

```
curl -Method GET -Uri "http://localhost:5000/Vessel/filter?name=Vessel&pageNumber=1&pageSize=10" -Headers @{accept="text/plain"; "Content-Type"="application/json"}
```

## Vessel Type

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /VesselType

> Gets all vessel types in the system.

```
curl -Method GET -Uri "http://localhost:5000/VesselType" -Headers @{accept="text/plain"; "Content-Type"="application/json"}
```

### <img src="svg/post.svg" height="20" style="position: relative; top: 4px;"> /VesselType

```json
{
  "name": "Large Vessel",
  "description": "A really large vessel type",
  "maxNumberOfRows": 20,
  "maxNumberOfBays": 15,
  "maxNumberOfTiers": 10,
  "physicalCharacteristics": {
    "length": 200,
    "depth": 30,
    "draft": 15
  }
}
```

```
curl -Method POST -Uri "http://localhost:5000/VesselType" -Headers @{accept="text/plain"; "Content-Type"="application/json"} -Body "{`"name`":`"Large Vessel`",`"description`":`"A really large vessel type`",`"maxNumberOfRows`":20,`"maxNumberOfBays`":15,`"maxNumberOfTiers`":10,`"physicalCharacteristics`":{`"length`":200,`"depth`":30,`"draft`":15}}"
```

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /VesselType/{name}

```
curl -Method GET -Uri "http://localhost:5000/VesselType/Panamax" -Headers @{accept="text/plain"; "Content-Type"="application/json"}
```

### <img src="svg/put.svg" height="20" style="position: relative; top: 4px;"> /VesselType/{name}

```
curl -Method PUT -Uri "http://localhost:5000/VesselType/Large%20Vessel" -Headers @{accept="text/plain"; "Content-Type"="application/json"} -Body "{`"name`":`"Large Vessel`",`"description`":`"Bigger than usual vessel type`",`"maxNumberOfRows`":50,`"maxNumberOfBays`":45,`"maxNumberOfTiers`":40,`"physicalCharacteristics`":{`"length`":800,`"depth`":150,`"draft`":75}}"
```

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /VesselType/filter

```
curl -Method GET -Uri "http://localhost:5000/VesselType/filter?name=Panamax&description=Panama%20Canal" -Headers @{accept="text/plain"; "Content-Type"="application/json"}
```

## Qualifications
### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /Qualification

```
curl -Method GET -Uri "http://localhost:5000/Qualification" -Headers @{accept="text/plain"; "Content-Type"="application/json"}
```

### <img src="svg/post.svg" height="20" style="position: relative; top: 4px;"> /Qualification

```
curl -Method POST -Uri "http://localhost:5000/Qualification" -Headers @{accept="text/plain"; "Content-Type"="application/json"} -Body "{`"idCode`":`"EXQL`",`"qualificationName`":`"example qualification`"}"
```

### <img src="svg/put.svg" height="20" style="position: relative; top: 4px;"> /Qualification/{id}

```
curl -Method PUT -Uri "http://localhost:5000/Qualification/EXQL" -Headers @{accept="text/plain"; "Content-Type"="application/json"} -Body "{`"idCode`":`"EXQL`",`"qualificationName`":`"new name`"}"
```

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /Qualification/{id}

```
curl -Method GET -Uri "http://localhost:5000/Qualification/STSOP" -Headers @{accept="text/plain"; "Content-Type"="application/json"}
```

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /Qualification/filter

```
curl -Method GET -Uri "http://localhost:5000/Qualification/filter?code=STSOP&name=Crane&pageNumber=1&pageSize=10" -Headers @{accept="text/plain"; "Content-Type"="application/json"}
```

## Physical Resources
### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /PhysicalResource

```
curl -Method GET -Uri "http://localhost:5000/PhysicalResource" -Headers @{accept="text/plain"; "Content-Type"="application/json"}
```

### <img src="svg/post.svg" height="20" style="position: relative; top: 4px;"> /PhysicalResource/AddSTSCrane

```
curl -Method POST -Uri "http://localhost:5000/PhysicalResource/AddSTSCrane" -Headers @{accept="text/plain"; "Content-Type"="application/json"} -Body "{`"containersPerHour`":10,`"liftingCapacity`":10,`"servingDockCode`":`"DCK001`",`"code`":`"EXPLCrane`",`"description`":`"An example crane`",`"status`":0,`"setupTimeInMinutes`":20,`"qualificationsCodes`": [`"STSOP`"],`"operationalWindow`":{`"shifts`":[]}}"
```

### <img src="svg/put.svg" height="20" style="position: relative; top: 4px;"> /PhysicalResource/UpdateSTSCrane/{code}

```
curl -Method PUT -Uri "http://localhost:5000/PhysicalResource/UpdateSTSCrane/EXPLCrane" -Headers @{accept="text/plain"; "Content-Type"="application/json"} -Body "{`"containersPerHour`":40,`"liftingCapacity`":80,`"servingDockCode`":`"DCK002`",`"code`":`"EXPLCrane`",`"description`":`"An new example crane`",`"status`":0,`"setupTimeInMinutes`":20,`"qualificationsCodes`":[`"STSOP`",`"YACOP`"],`"operationalWindow`":{`"shifts`":[]}}"
```

### <img src="svg/post.svg" height="20" style="position: relative; top: 4px;"> /PhysicalResource/AddYardCrane

```
curl -Method POST -Uri "http://localhost:5000/PhysicalResource/AddYardCrane" -Headers @{accept="text/plain"; "Content-Type"="application/json"} -Body "{`"containersPerHour`":10,`"liftingCapacity`":10,`"code`":`"EXPLYardCrane`",`"description`":`"An example yard crane`",`"status`":1,`"setupTimeInMinutes`":30,`"qualificationsCodes`":[`"YACOP`"],`"operationalWindow`":{`"shifts`":[]}}"
```

### <img src="svg/put.svg" height="20" style="position: relative; top: 4px;"> /PhysicalResource/UpdateYardCrane/{code}

```
curl -Method PUT -Uri "http://localhost:5000/PhysicalResource/UpdateYardCrane/EXPLYardCrane" -Headers @{accept="text/plain"; "Content-Type"="application/json"} -Body "{`"containersPerHour`":10,`"liftingCapacity`":10,`"code`":`"EXPLYardCrane`",`"description`":`"An new yard crane`",`"status`":1,`"setupTimeInMinutes`":30,`"qualificationsCodes`":[`"TRKDR`"],`"operationalWindow`":{`"shifts`":[]}}"
```

### <img src="svg/post.svg" height="20" style="position: relative; top: 4px;"> /PhysicalResource/AddTruck

```
curl -Method POST -Uri "http://localhost:5000/PhysicalResource/AddTruck" -Headers @{accept="text/plain"; "Content-Type"="application/json"} -Body "{`"containersPerTrip`":20,`"averageSpeed`":80,`"maxLoadCapacity`":1,`"code`":`"EXPLTruck`",`"description`":`"An example truck`",`"status`":0,`"setupTimeInMinutes`":0,`"qualificationsCodes`":[`"TRKDR`"],`"operationalWindow`":{`"shifts`":[]}}"
```

### <img src="svg/put.svg" height="20" style="position: relative; top: 4px;"> /PhysicalResource/UpdateTruck/{code}

```
curl -Method PUT -Uri "http://localhost:5000/PhysicalResource/UpdateTruck/EXPLTruck" -Headers @{accept="text/plain"; "Content-Type"="application/json"} -Body "{`"containersPerTrip`":3,`"averageSpeed`":85,`"maxLoadCapacity`":2,`"code`":`"EXPLTruck`",`"description`":`"An new truck`",`"status`":1,`"setupTimeInMinutes`":0,`"qualificationsCodes`":[`"TRKDR`"],`"operationalWindow`":{`"shifts`":[]}}"
```

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /PhysicalResource/{id}

```
curl -Method GET -Uri "http://localhost:5000/PhysicalResource/STS001" -Headers @{accept="text/plain"; "Content-Type"="application/json"}
```

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /PhysicalResource/filter

```
curl -Method GET -Uri "http://localhost:5000/PhysicalResource/filter?code=STS001&description=crane&status=0&type=0&pageNumber=1&pageSize=10" -Headers @{accept="text/plain"; "Content-Type"="application/json"}
```

### <img src="svg/delete.svg" height="20" style="position: relative; top: 4px;"> /PhysicalResource/{code}

```
curl -Method DELETE -Uri "http://localhost:5000/PhysicalResource/EXPLTruck" -Headers @{accept="text/plain"; "Content-Type"="application/json"}
```

## Storage area

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /StorageArea

```
curl -Method GET -Uri "http://localhost:5000/StorageArea" -Headers @{accept="text/plain"; "Content-Type"="application/json"}
```

### <img src="svg/post.svg" height="20" style="position: relative; top: 4px;"> /StorageArea

```
curl -Method POST -Uri "http://localhost:5000/StorageArea" -Headers @{accept="text/plain"; "Content-Type"="application/json"} -Body "{`"nameCode`":`"WH34`",`"location`":`"Section 34`",`"type`":1,`"capacity`":1500,`"currentOccupancy`":0,`"dockServices`":[]}"
```

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /StorageArea/{id}

```
curl -Method GET -Uri "http://localhost:5000/StorageArea/WH34" -Headers @{accept="text/plain"; "Content-Type"="application/json"}
```

### <img src="svg/put.svg" height="20" style="position: relative; top: 4px;"> /StorageArea/{id}

```
curl -Method PUT -Uri "http://localhost:5000/StorageArea/WH34" -Headers @{accept="text/plain"; "Content-Type"="application/json"} -Body "{`"nameCode`":`"WH34`",`"location`":`"Section 34`",`"type`":1,`"capacity`":1500,`"currentOccupancy`":500,`"dockServices`":[]}"
```

## Vessel Visit Notification

### <img src="svg/post.svg" height="20" style="position: relative; top: 4px;"> /VesselVisitNotification

```
curl -Method POST -Uri "http://localhost:5000/VesselVisitNotification" -Headers @{accept="text/plain"; "Content-Type"="application/json"} -Body "{`"notificationId`":`"2025-PORTO-000006`",`"expectedArrival`":`"2025-10-27T15:54:33.638Z`",`"expectedDeparture`":`"2025-10-30T15:54:33.638Z`",`"isCargoHazardous`":false,`"specialRequirements`":null,`"vesselImoNumber`":`"IMO 3815389`",`"submitterId`":908029952}"
```

### <img src="svg/put.svg" height="20" style="position: relative; top: 4px;"> /VesselVisitNotification

```
curl -Method PUT -Uri "http://localhost:5000/VesselVisitNotification/2025-PORTO-000006" -Headers @{accept="text/plain"; "Content-Type"="application/json"} -Body "{`"notificationId`":`"2025-PORTO-000006`",`"expectedArrival`":`"2025-10-27T15:54:33.638Z`",`"expectedDeparture`":`"2025-10-30T15:54:33.638Z`",`"isCargoHazardous`":true,`"specialRequirements`":null,`"crewDetails`":null,`"loadCargoManifest`":[{`"position`":{`"bay`":`"6`",`"row`":`"12`",`"tier`":`"8`"},`"storageAreaCode`":`"YARD1`",`"container`":{`"containerNumber`":`"ABCD1234560`",`"cargoType`":2,`"description`":`"various electronic items`"}}],`"unloadCargoManifest`":[],`"vesselImoNumber`":`"IMO 9703318`",`"submitterId`":733060890}"
```

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /VesselVisitNotification/decisions

```
curl -Method GET -Uri "http://localhost:5000/VesselVisitNotification/decisions?vesselVisitNotificationId=2025-PORTO-000001" -Headers @{accept="text/plain"; "Content-Type"="application/json"}
```

### <img src="svg/post.svg" height="20" style="position: relative; top: 4px;"> /VesselVisitNotification/decisions

```
curl -Method POST -Uri "http://localhost:5000/VesselVisitNotification/decisions?vesselVisitNotificationId=2025-PORTO-000004" -Headers @{accept="text/plain"; "Content-Type"="application/json"} -Body "{`"status`":1,`"reason`":`"Everything checked and ready to receive the vessel`",`"decisionDate`":`"2025-10-26T16:25:31.528Z`",`"assignedDockCode`":`"DCK001`",`"isFinal`":true}"
```

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /VesselVisitNotification/filter

```
curl -Method GET -Uri "http://localhost:5000/VesselVisitNotification/filter?SubmitterCitizenshipId=733060890&status=2&withReason=true&withDockAssigned=true&vessel=IMO%209703318&ETAFrom=2025-10-26&ETATo=2025-10-30&pageNumber=1&pageSize=10" -Headers @{accept="text/plain"; "Content-Type"="application/json"}
```