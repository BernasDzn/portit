<style>
    .http-method { display: inline-block; padding: 0.15rem 0.5rem; border-radius: 4px; color: #fff; font-weight: 700; font-family: monospace; margin-right: 0.5rem; }
    .http-get { background: #2d9cdb; }
    .http-post { background: #27ae60; }
    .http-put { background: #f39c12; }
    .http-delete { background: #e74c3c; }
    .http-patch { background: #8e44ad; }
    .http-method.small { padding: 0.08rem 0.4rem; font-size: 0.8em; }

    .endpoint-path { font-weight: 600; color: #ffffffff; }
</style>
# User Manual

## Vessel

### <span class="http-method http-get">GET</span> <span class="endpoint-path">/Vessel</span>

Should return a list of all vessels in the database.

### <span class="http-method http-post">POST</span> <span class="endpoint-path">/Vessel</span>
```
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

### <span class="http-method http-get">GET</span> <span class="endpoint-path">/Vessel/{imo}</span>

Should return the vessel with IMO Number (eg. IMO 3815389):
```
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

### <span class="http-method http-put">PUT</span> <span class="endpoint-path">/Vessel/{imo}</span>

IMO Number field should be the IMO of the desired vessel (eg. IMO 3815389).

```
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

### <span class="http-method http-get">GET</span> <span class="endpoint-path">/Vessel/filter</span>

Should return a list of vessels that match the filter criteria.

Query Parameters:
- name (string, optional): Filter by vessel name.
- type (string, optional): Filter by vessel type.
- owner (string, optional): Filter by vessel owner.
- pageNumber (int, optional): Page number for pagination (default is 1).
- pageSize (int, optional): Number of items per page (default is 10).

When filtering, after running the previous requests, the "Vessel" name filter should be a good example, returning both the "New Vessel" and "Update Vessel Name" entries.