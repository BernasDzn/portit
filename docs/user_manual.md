# Developer Manual
The developer manual attempts to guide you through the backend's endpoints by enumerating them, showing of their functionality, and suggesting inputs.

The suggested inputs are for testing purposes only. They assume the `Bootstrap` script was ran once and no additional data was added.

## Staff

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /Staff

> Gets all staff members in the system.

> When we execute, we should see 3 staff that we bootstraped:
<br>**João Pedro**, **Carlos Santos** and **Maria Silva**

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

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /Staff/filter

> Filters staffs by any field.

> Let's try filtering all Staffs with **"Faria"** in their names and the **Truck Driver** qualification.
<br>Since **Rodrigo Faria Pinto** is the only staff that meets there requirements, only he should be returned.

```
Name: Faria

QualificationCodes: TRKDR
```

### <img src="svg/delete.svg" height="20" style="position: relative; top: 4px;"> /Staff/{mecanographicNumber}

> Acts as a soft delete, in domain terms, it **deactivates** the staff member.
<br>The company now wants to deactivate the staff **Rodrigo Faria Pinto**:

```json
TESTMEC01
```

## Dock

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /Dock

> Gets all docks in the system.

> When we execute, we should see 3 docks that we bootstraped:
<br>**DCK001 (Dock A)**, **DCK002 (Dock B)** and **DCK003 (Dock C)**

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

This includes all information about the dock, including its type details.

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

## Vessel

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /Vessel

Should return a list of all vessels in the database.

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

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /Vessel/{imo}

Should return the vessel with IMO Number (eg. IMO 3815389):
```json
{
  "name": "Maersk Triple E",
  "imoNumber": "IMO 3815389",
  "type": {
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
  "owner": {
    "name": "Global Shipping Co.",
    "altNames": [
      "GSC",
      "Global Ship"
    ],
    "taxNumber": "PT123456789",
    "address": {
      "id": "6d845b6e-26f8-4a1f-9532-beccf8113d37",
      "street": "123 Ocean Drive",
      "city": "Maritime City",
      "zipCode": "90210",
      "country": "USA"
    },
    "representatives": [
      {
        "name": "Kayley Begbie",
        "citizenshipId": 319982093,
        "emailAddress": "kbegbie1@spotify.com",
        "phone": "6382283741"
      },
      {
        "name": "Patricio Sharply",
        "citizenshipId": 908029952,
        "emailAddress": "psharply0@yolasite.com",
        "phone": "6947302134"
      }
    ]
  },
  "physicalCharacteristics": {
    "length": 370,
    "depth": 16,
    "draft": 13
  }
}
```

This includes all information about the vessel, including its type and owner details.

### <img src="svg/put.svg" height="20" style="position: relative; top: 4px;"> /Vessel/{imo}

IMO Number field should be the IMO of the desired vessel (eg. IMO 3815389).

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

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /Vessel/filter

Should return a list of vessels that match the filter criteria.

Query Parameters:
- name (string, optional): Filter by vessel name.
- type (string, optional): Filter by vessel type.
- owner (string, optional): Filter by vessel owner.
- pageNumber (int, optional): Page number for pagination (default is 1).
- pageSize (int, optional): Number of items per page (default is 10).

When filtering, after running the previous requests, the "Vessel" name filter should be a good example, returning both the "New Vessel" and "Update Vessel Name" entries.

## Vessel Type

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /VesselType

> Gets all vessel types in the system.

> When we execute, we should see 5 vessel types that we bootstraped:
<br>**Panamax**, **Post-Panamax**, **Ultra Large Container Vessel (ULCV)**, **Handymax** and **Capesize**

### <img src="svg/post.svg" height="20" style="position: relative; top: 4px;"> /VesselType

> Vessel Type with name Large Vessel<br>
Description is "A really large vessel type",
has max number of:
<br>Rows: 20
<br>Bays: 15
<br>Tiers: 10
<br>The physical characteristics:
<br>Length: 200
<br>Depth: 30
<br>Draft: 15
<br>**The capacity does not need to be specified since it is calculated with rows,bays and tiers**

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

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /VesselType/{name}

Should return the vessel type with name (eg. Panamax):
```json
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
```

This includes all information about the vessel type, including the capacity that is calculated automatically with other information.

### <img src="svg/put.svg" height="20" style="position: relative; top: 4px;"> /VesselType/{name}

> Vessel Type with name Large Vessel (Created previously)<br>
Description is actually **Bigger than usual vessel type**<br>
Has max number of:
<br>Rows: **50**
<br>Bays: **45**
<br>Tiers: **40**
<br>The physical characteristics:
<br>Length: **800**
<br>Depth: **150**
<br>Draft: **75**

```json
{
  "name": "Large Vessel",
  "description": "Bigger than usual vessel type",
  "maxNumberOfRows": 50,
  "maxNumberOfBays": 45,
  "maxNumberOfTiers": 40,
  "physicalCharacteristics": {
    "length": 800,
    "depth": 150,
    "draft": 75
  }
}
```

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /VesselType/filter

> Filters vessel types by any field.

Query Parameters:
- name (string, optional): Filter by vessel type name.
- description (string, optional): Filter by vessel type description.
- pageNumber (int, optional): Page number for pagination (default is 1).
- pageSize (int, optional): Number of items per page (default is 10).

> Let's try filtering all vessel types with **"Panamax"** in their names and **"Panama Canal"** in their description.

Since **Panamax** is the only vessel type that meets the requirements, it is the only one that's returned.

## Qualifications
### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /Qualification

Should return a list of all qualifications in the database.

### <img src="svg/post.svg" height="20" style="position: relative; top: 4px;"> /Qualification
```json
{
  "idCode": "EXQL",
  "qualificationName": "example qualification"
}
```

### <img src="svg/put.svg" height="20" style="position: relative; top: 4px;"> /Qualification/{id}
```json
{
  "idCode": "EXQL",
  "qualificationName": "new name"
}
```

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /Qualification/{id}
Should return the qualification with the given Id (eg. STSOP):
```json
{
  "idCode": "STSOP",
  "qualificationName": "STS Crane Operator"
}
```

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /Qualification/filter
Should return a list of qualifications that match the filter criteria.

Query Parameters:
- code (string, optional): Filter by qualification code.
- name (string, optional): Filter by qualification name.
- pageNumber (int, optional): Page number for pagination (default is 1).
- pageSize (int, optional): Number of items per page (default is 10).

## Physical Resources
### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /PhysicalResource

Should return a list of all Physical Resources in the database.

### <img src="svg/post.svg" height="20" style="position: relative; top: 4px;"> /PhysicalResource/AddSTSCrane
```json
{
  "containersPerHour": 10,
  "liftingCapacity": 10,
  "servingDockCode": "DCK001",
  "code": "EXPLCrane",
  "description": "An example crane",
  "status": 0,
  "setupTimeInMinutes": 20,
  "qualificationsCodes": [
    "STSOP"
  ],
  "operationalWindow": {
    "shifts": [
        {
          "day": 1,
          "startTime": "06:00:00",
          "endTime": "22:00:00"
        },
        {
          "day": 2,
          "startTime": "06:00:00",
          "endTime": "22:00:00"
        },
        {
          "day": 3,
          "startTime": "06:00:00",
          "endTime": "22:00:00"
        }
    ]
  }
}
```

### <img src="svg/put.svg" height="20" style="position: relative; top: 4px;"> /PhysicalResource/UpdateSTSCrane/{code}
```json

{
  "containersPerHour": 40,
  "liftingCapacity": 80,
  "servingDockCode": "DCK002",
  "code": "EXPLCrane",
  "description": "An new example crane",
  "status": 0,
  "setupTimeInMinutes": 20,
  "qualificationsCodes": [
    "STSOP",
    "YACOP"
  ],
  "operationalWindow": {
    "shifts": [
        {
          "day": 1,
          "startTime": "06:00:00",
          "endTime": "22:00:00"
        },
        {
          "day": 2,
          "startTime": "06:00:00",
          "endTime": "22:00:00"
        },
        {
          "day": 3,
          "startTime": "06:00:00",
          "endTime": "22:00:00"
        },
        {
          "day": 4,
          "startTime": "23:00:00",
          "endTime": "23:59:00"
        }
    ]
  }
}
```

### <img src="svg/post.svg" height="20" style="position: relative; top: 4px;"> /PhysicalResource/AddYardCrane
```json
{
  "containersPerHour": 10,
  "liftingCapacity": 10,
  "code": "EXPLYardCrane",
  "description": "An example yard crane",
  "status": 1,
  "setupTimeInMinutes": 30,
  "qualificationsCodes": [
    "YACOP"
  ],
  "operationalWindow": {
    "shifts": [
    ]
  }
}
```

### <img src="svg/put.svg" height="20" style="position: relative; top: 4px;"> /PhysicalResource/UpdateYardCrane/{code}
```json
{
  "containersPerHour": 10,
  "liftingCapacity": 10,
  "code": "EXPLYardCrane",
  "description": "An new yard crane",
  "status": 1,
  "setupTimeInMinutes": 30,
  "qualificationsCodes": [
    "TRKDR"
  ],
  "operationalWindow": {
    "shifts": [
        {
          "day": 3,
          "startTime": "06:00:00",
          "endTime": "22:00:00"
        },
        {
          "day": 4,
          "startTime": "23:00:00",
          "endTime": "23:59:00"
        }
    ]
  }
}
```

### <img src="svg/post.svg" height="20" style="position: relative; top: 4px;"> /PhysicalResource/AddTruck
```json
{
  "containersPerTrip": 20,
  "averageSpeed": 80,
  "maxLoadCapacity": 1,
  "code": "EXPLTruck",
  "description": "An example truck",
  "status": 0,
  "setupTimeInMinutes": 0,
  "qualificationsCodes": [
    "TRKDR"
  ],
  "operationalWindow": {
    "shifts": [
      {
        "day": 0,
        "startTime": "0:0",
        "endTime": "23:59"
      }
    ]
  }
}
```

### <img src="svg/put.svg" height="20" style="position: relative; top: 4px;"> /PhysicalResource/UpdateTruck/{code}
```json
{
  "containersPerTrip": 3,
  "averageSpeed": 85,
  "maxLoadCapacity": 2,
  "code": "EXPLTruck",
  "description": "An new truck",
  "status": 1,
  "setupTimeInMinutes": 0,
  "qualificationsCodes": [
    "TRKDR"
  ],
  "operationalWindow": {
    "shifts": [
    ]
  }
}
```

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /PhysicalResource/{id}
Should return the Physical Resource with the given code (eg. STS001):
```json
{
  "containersPerHour": 30,
  "liftingCapacity": 40,
  "servingDock": {
    "code": "DCK001",
    "name": "Dock A",
    "location": "North Harbor",
    "physicalCharacteristics": {
      "length": 500,
      "depth": 35,
      "draft": 20
    },
    "supportedVesselTypes": [
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
      },
      {
        "name": "Handymax",
        "description": "Medium-sized bulk carriers",
        "maxNumberOfRows": 15,
        "maxNumberOfBays": 8,
        "maxNumberOfTiers": 4,
        "capacity": 480,
        "physicalCharacteristics": {
          "length": 250,
          "depth": 12,
          "draft": 10
        }
      }
    ]
  },
  "code": "STS001",
  "description": "STS Crane 1",
  "status": 0,
  "setupTimeInMinutes": 30,
  "qualifications": [
    {
      "idCode": "STSOP",
      "qualificationName": "STS Crane Operator"
    }
  ],
  "operationalWindow": {
    "shifts": [
      {
        "day": 1,
        "startTime": "00:00:00",
        "endTime": "23:59:59.9999999"
      },
      {
        "day": 2,
        "startTime": "00:00:00",
        "endTime": "23:59:59.9999999"
      },
      {
        "day": 3,
        "startTime": "00:00:00",
        "endTime": "23:59:59.9999999"
      },
      {
        "day": 4,
        "startTime": "00:00:00",
        "endTime": "23:59:59.9999999"
      },
      {
        "day": 5,
        "startTime": "00:00:00",
        "endTime": "23:59:59.9999999"
      },
      {
        "day": 6,
        "startTime": "00:00:00",
        "endTime": "23:59:59.9999999"
      },
      {
        "day": 0,
        "startTime": "00:00:00",
        "endTime": "23:59:59.9999999"
      }
    ]
  }
}
```

### <img src="svg/get.svg" height="20" style="position: relative; top: 4px;"> /PhysicalResource/filter
Should return a list of Physical Resources that match the filter criteria.

Query Parameters:
- code (string, optional): Filter by resource code.
- description (string, optional): Filter by resource description.
- status (int, optional): Filter by resource status. (0 = Available; 1 = Maintenance; 2 = Out of service)
- type (int, optional): Filter by resource type. (0 = STSCrane; 1 = YardCrane; 2 = Truck)
- pageNumber (int, optional): Page number for pagination (default is 1).
- pageSize (int, optional): Number of items per page (default is 10).

### <img src="svg/delete.svg" height="20" style="position: relative; top: 4px;"> /PhysicalResource/{code}

Deactivate the Physical Resources of the given code.

## Vessel Visit Notification

### <img src="svg/post.svg" height="20" style="position: relative; top: 4px;"> /VesselVisitNotification
```json
{
  "notificationId": "2025-PORTO-000006",
  "expectedArrival": "2025-10-27T15:54:33.638Z",
  "expectedDeparture": "2025-10-30T15:54:33.638Z",
  "isCargoHazardous": false,
  "specialRequirements": null,
  "crewDetails": null,
  "loadCargoManifest": [],
  "unloadCargoManifest": [],
  "vesselImoNumber": "IMO 9703318",
  "submitterId": 733060890
}
```

Creates a new Vessel Visit Notification for the vessel with IMO Number IMO 9703318 (Ever Given). The expected arrival is on October 27, 2025, and the expected departure is on October 30, 2025. The notification indicates that there is no hazardous cargo and no special requirements. The load and unload cargo manifests are empty, and the submitter has the ID 733060890.
