# US2201 - Register and update vessel types

## 1. User Story Description *(from project statement)*
> As a Port Authority Officer, I want to create and update vessel types, so that vessels can be classified consistently and their operational constraints are properly defined.

## 2. Customer Specifications and Clarifications

<details><summary>From the specifications document</summary>
<p>

> Data needed include: name, description, capacity, and operational constraints (e.g.: maximum number of rows, bays, and tiers).
> 
> Vessel types must be available for reference when registering vessel records.
> 
> Vessel types must be searchable and filterable by name and description.

</p>
</details>

<details><summary>From the client clarifications</summary>
<p>

> **Q:** When you state "Vessel types must include attributes such as name, description, capacity, and operational constraints (e.g.: maximum number of rows, bays, and tiers).", is the description something general and always the same for each vessel type ? Or is it unique to each record ?
<br> **A:** I'm not sure I understood the question.
The "description" is the description of the vessel type being registered.
A different vessel type has, naturally, a different description. But, this doesn't mean that descriptions of vessel types are unique.
What is the doubt, here?

> **Q:** Can there be more than one vessel type with the same name?
<br> **A:** No! Names are unique.

> **Q:** As stated in a previous question to the client, your response to the question "Can there be more than one vessel type with the same name?" was "No! Names are unique". Does this mean we can use the name as a business identifier for the Vessel Type or is there another concept to identify it?
<br> **A:** Definitely, yes!

> **Q:** In many user stories there is the need to search and/or filter data. For example, in US 221 one of the acceptance criteria is: "Vessel types must be searchable and filterable by name and description." Does this mean that we need to be able to input a, for example, vessel type name "Tanker" and then get "Gas Tanker", "Oil Tanker" if those are vessel types that exist in the system? Or should we need to input BOTH the name AND the description to get some results? And if we need to input only a description then should it only return direct matches (only return the vessel types that perfectly, word for word match the given filtration description) or would the user input a word or two and we would need to show all vessel types containing that word in their description? We don't quite understand how the user wants to use these functions so it would be nice to see an example
<br> **A:** Your example is perfect.
While searching for text fields, the better approach is returning partial match. E.g. searching by "tanker" returns all records whose description contains such word on a case insensitive case.
However, as a user it would be nice if I could refine the search by setting the kind of operator to be applied (e.g. equals, contains).

> **Q:** The assignment clearly indicates that physical information about docks such as their length and depth is important and needs to be stored: However, no such specification exists for vessels, or even vessel types. Is a vessel's physical size determined (or at least limited) by its vessel type, or can vessels have dimensions different from a predefined standard?
<br> **A:** Well caught!
You may capture the physical characteristics (e.g. length, beam, draft) of both:
(i) the vessel type (max dimensions); and
(ii) the vessel, whose dimensions may not exceed the ones of the corresponding vessel type.


> **Q:** The vessel type defines the dimensions, capacity and cargo disposition of all vessels of that type or is some variation possible within the same type?
<br> **A:** For simplicity, you should consider that no variation is possible.

> **Q:** Which are the max dimensions and cargo disposition for each type?
<br> **A:** Such dimensions are defined by the user, i.e., the Port Authority Officer.
Some examples of vessel types are mentioned of the system description document.
Moreover, it states that "The type of vessel determines the maximum number of rows, bays, and tiers, and therefore its maximum TEU capacity.".

</p>
</details> 

## 3. Business Value
> This feature is important because it ensures vessels are classified consistently, allowing accurate planning, compliance with operational constraints, and efficient reference when managing vessel records.

## 4. Definition of Done
- Vessel types attributes are defined.
- Vessel types are available for reference everytime a vessel record is being created.
- Vessel types are searchable.
- Vessel types can be filtered by name and description
- Feature tested and reviewed.


