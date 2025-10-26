
# User Manual

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