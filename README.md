# SimpleMapper

SimpleMapper is a lightweight object-to-object mapping library which helps you map the properties of one object (e.g a domain entity) to another (e.g a DTO or view model), without writing a ton of tedious mapping code.

## Usage

### Mapping single objects

We'll use the following classes for all our examples.
```csharp
public class Entity
{
    public int ID { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }	
    public string EntityOnlyField { get; set; }
}

public class Model
{
    public int ID { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ModelOnlyField { get; set; }
}
```
If your source and destination types have identically named properties, SimpleMapper will map those properties for you automatically. 
```csharp
var entity = new Entity {
    ID = 1,
    Title = "MY TEST ENTITY",
    Description = "DESCRIPTION GOES HERE",
    EntityOnlyField = "ONLY IN ENTITY"
};

// Map the entity to a new instance of Model
var model = entity.MapTo<Model>();

// model.ID = 1
// model.Title = "MY TEST ENTITY"
// model.Description = "DESCRIPTION GOES HERE"
// model.ModelOnlyField = null
```
If you already have an instance of the destination type which you wish to map to, you can do that too. SimpleMapper will leave any unique properties in the destination instance alone:
```csharp
var entity = new Entity {
    ID = 1,
    Title = "MY TEST ENTITY",
    Description = "DESCRIPTION GOES HERE",
    EntityOnlyField = "ONLY IN ENTITY"
};

var model = new Model {
    ID = 2,
    Title = "MY TEST ENTITY",
    Description = "DESCRIPTION GOES HERE",
    ModelOnlyField = "ONLY IN MODEL"
};

// Map the entity to the existing instance of Model
var model = entity.MapTo(model);

// model.ID = 1
// model.Title = "MY TEST ENTITY"
// model.Description = "DESCRIPTION GOES HERE"
// model.ModelOnlyField = "ONLY IN MODEL"
```
If your source and destination types have properties which are _not_ identically named, you don't want to map all of the properties, or you want to perform some kind of transformation on property values during the mapping process, you can set up a custom mapping. 

In this case, you must explicitly specify what you want to happen for each property (like you would with manually-written mapping code), but the advantage here is that you only ever have to do it in one place.

In an ASP.NET MVC project, the best place to set up custom mappings is in the `Application_Start` method of your Global.asax file:
```csharp
protected void Application_Start()
{
    // All your other application setup code here

    Mapper.AddMapping<Entity, Model>((source, destination) => {
        destination.ID = source.ID;
        // Note we leave out Title because we don't want it mapped
        destination.Description = source.Description + ", USED CUSTOM MAPPER";
        destination.ModelOnlyField = source.EntityOnlyField;
    });
}
```
Once your mappings are defined, usage is exacly the same:
```csharp
var entity = new Entity {
    ID = 1,
    Title = "MY TEST ENTITY",
    Description = "DESCRIPTION GOES HERE",
    EntityOnlyField = "ONLY IN ENTITY"
};

// Map the entity to a new instance of Model
var model = entity.MapTo<Model>();

// model.ID = 1
// model.Title = null
// model.Description = "DESCRIPTION GOES HERE, USED CUSTOM MAPPER"
// model.ModelOnlyField = "ONLY IN ENTITY"
```
### Mapping lists of objects

SimpleMapper also contains an extension method for mapping lists:
```csharp
var listOfModels = listOfEntities.MapToList<Model>();
```
There is currently no facility to map to an existing list.
