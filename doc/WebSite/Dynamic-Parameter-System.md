## Introduction

**Dynamic Property System** is a system that allows you to add and manage new properties on entity objects at runtime without any code changes. With this system, you can define dynamic properties on entity objects and perform operations on these objects easily. For example, it can be used for cities, counties, gender, status codes etc. 

### Dynamic Property Definition

First of all you need to define which entities can have that feature and allowed *input types* * can a dynamic property has. 

```csharp
public class MyDynamicEntityPropertyDefinitionProvider : DynamicEntityPropertyDefinitionProvider
{
     public override void SetDynamicEntityProperties(IDynamicEntityPropertyDefinitionContext context)
     {
         //input types dynamic properties can have
         context.Manager.AddAllowedInputType<SingleLineStringInputType>();
         context.Manager.AddAllowedInputType<CheckboxInputType>();
         context.Manager.AddAllowedInputType<ComboboxInputType>();
         
         //entities that can have dynamic property
         context.Manager.AddEntity<Country>(); 
         context.Manager.AddEntity<Blog, long>(); 
     }
}
```

*InputType: A UI input type for the feature. This can be defined, and then used while creating an automatic feature screen. For example: [SingleLineStringInputType](https://github.com/aspnetboilerplate/aspnetboilerplate/blob/dev/src/Abp/UI/Inputs/SingleLineStringInputType.cs), [ComboboxInputType](https://github.com/aspnetboilerplate/aspnetboilerplate/blob/dev/src/Abp/UI/Inputs/ComboboxInputType.cs), [CheckboxInputType](https://github.com/aspnetboilerplate/aspnetboilerplate/blob/dev/src/Abp/UI/Inputs/CheckboxInputType.cs)*

```csharp
public class DynamicEntityPropertyTestModule : AbpModule
{
    public override void PreInitialize()
    {        Configuration.DynamicEntityProperties.Providers.Add<MyDynamicEntityPropertyDefinitionProvider>();
    }
}
```



### Dynamic Property

It is a property that you can use in other entities. 

Creating a dynamic property.

```csharp
var cityProperty = new DynamicProperty
{
    PropertyName = "City",//Property name. 
    InputType = InputTypeBase.GetName<ComboboxInputType>(),
    Permission = "App.Permission.DynamicProperty.City",
    TenantId = AbpSession.TenantId
};
var dynamicPropertyManager = Resolve<IDynamicPropertyManager>();
dynamicPropertyManager.Add(cityProperty);
```

***DynamicProperty.cs***

<table>
    <thead>
    	<tr>
            <th>Property</th>
            <th>Summary</th>
        </tr>
    </thead>
    <tbody>
    	<tr>
            <td>PropertyName*  <span style="font-style: italic;">(string)</span></td>
            <td>Unique name of the dynamic property</td>
        </tr>
         <tr>
            <td>Input Type*  <span style="font-style: italic;">(string)</span></td>
            <td>Input type name of the dynamic property.(Input type should be provided in definition.)</td>
        </tr>  
         <tr>
            <td>Permission  <span style="font-style: italic;">(string)</span></td>
            <td>Required permission to manage anything about that property <br><span style="font-style: italic;">(<code>DynamicPropertyValue</code>, <code>DynamicEntityProperty</code>, <code>DynamicEntityPropertyValue</code>)</span></td>
        </tr> 
         <tr>
            <td>TenantId  <span style="font-style: italic;">(int?)</span></td>
            <td>Tenant's unique identifier <br/> <span style="font-style: italic;">(For example you can use (IAbpSession).TenantId to get current tenant's id)</span></td>
        </tr>  
    </tbody>
</table>




You can use [**IDynamicPropertyManager**](https://github.com/aspnetboilerplate/aspnetboilerplate/blob/dev/src/Abp/DynamicEntityProperties/IDynamicPropertyManager.cs) to manage dynamic property. (It uses cache)

```csharp
public interface IDynamicPropertyManager
{
    DynamicProperty Get(int id);

    Task<DynamicProperty> GetAsync(int id);

    DynamicProperty Get(string propertyName);

    Task<DynamicProperty> GetAsync(string propertyName);

    void Add(DynamicProperty dynamicProperty);

    Task AddAsync(DynamicProperty dynamicProperty);

    void Update(DynamicProperty dynamicProperty);

    Task UpdateAsync(DynamicProperty dynamicProperty);

    void Delete(int id);

    Task DeleteAsync(int id);
}
```



### Dynamic Property Value

If your dynamic property's input types need values to select (for example `ComboboxInputType`), you can store the values that your dynamic properties can have here. 

Adding a value to your dynamic property.

```csharp
var istanbul = new new DynamicPropertyValue(cityProperty, "Istanbul", AbpSession.TenantId);
var london = new new DynamicPropertyValue(cityProperty, "London", AbpSession.TenantId);

var _dynamicPropertyValueManager = Resolve<IDynamicPropertyValueManager>();
_dynamicPropertyValueManager.Add(istanbul); 
_dynamicPropertyValueManager.Add(london);
```

***DynamicPropertyValue.cs***

<table>
    <thead>
    	<tr>
            <th>Property</th>
            <th>Summary</th>
        </tr>
    </thead>
    <tbody>
    	<tr>
            <td>DynamicPropertyId*  <span style="font-style: italic;">(int)</span></td>
            <td>Unique indentifier of DynamicProperty</td>
        </tr>
         <tr>
            <td>Value*  <span style="font-style: italic;">(string)</span></td>
            <td>Value</td>
        </tr>  
         <tr>
            <td>TenantId  <span style="font-style: italic;">(int?)</span></td>
            <td>Tenant's unique identifier <br/> <span style="font-style: italic;">(For example you can use (IAbpSession).TenantId to get current tenant's id)</span></td>
        </tr>            
    </tbody>
</table>



You can use [**IDynamicPropertyValueManager**](https://github.com/aspnetboilerplate/aspnetboilerplate/blob/dev/src/Abp/DynamicEntityProperties/IDynamicPropertyValueManager.cs) to manage dynamic property values. (It checks permissions)

```csharp
public interface IDynamicPropertyValueManager
{
    DynamicPropertyValue Get(int id);

    Task<DynamicPropertyValue> GetAsync(int id);

    List<DynamicPropertyValue> GetAllValuesOfDynamicProperty(int dynamicPropertyId);

    Task<List<DynamicPropertyValue>> GetAllValuesOfDynamicPropertyAsync(int dynamicPropertyId);

    void Add(DynamicPropertyValue dynamicPropertyValue);

    Task AddAsync(DynamicPropertyValue dynamicPropertyValue);

    void Update(DynamicPropertyValue dynamicPropertyValue);

    Task UpdateAsync(DynamicPropertyValue dynamicPropertyValue);

    void Delete(int id);

    Task DeleteAsync(int id);

    void CleanValues(int dynamicPropertyId);

    Task CleanValuesAsync(int dynamicPropertyId);
}
```



### Dynamic Entity Properties

Dynamic property definitions of an entity. It stores which dynamic properties an entity have. 

Adding New Property to Entity

```csharp
var _dynamicEntityPropertyManager = Resolve<IDynamicEntityPropertyManager>();
var cityDynamicPropertyOfCountry = _dynamicEntityPropertyManager.Add<Country>(cityProperty, tenantId: 1);//add cityProperty to Country entity
```

***DynamicEntityProperty.cs***

<table>
    <thead>
    	<tr>
            <th>Property</th>
            <th>Summary</th>
        </tr>
    </thead>
    <tbody>
    	<tr>
            <td>DynamicPropertyId*  <span style="font-style: italic;">(int)</span></td>
            <td>Unique indentifier of DynamicProperty</td>
        </tr>
         <tr>
            <td>EntityFullName*  <span style="font-style: italic;">(string)</span></td>
            <td>Full name of type of entity <br/> <span style="font-style: italic;">(For example: "<span style="color:blue">typeof</span>(<span style="color:lightblue">MyEntity</span>).FullName")</span></td>
        </tr>  
         <tr>
            <td>TenantId  <span style="font-style: italic;">(int?)</span></td>
            <td>Tenant's unique identifier <br/> <span style="font-style: italic;">(For example you can use (IAbpSession).TenantId to get current tenant's id)</span></td>
        </tr>            
    </tbody>
</table>



You can use [**IDynamicEntityPropertyManager**](https://github.com/aspnetboilerplate/aspnetboilerplate/blob/dev/src/Abp/DynamicEntityProperties/IDynamicEntityPropertyManager.cs) to manage entities dynamic properties. (It uses cache and checks required permissions.) See also: [DynamicEntityPropertyManagerExtensions](https://github.com/aspnetboilerplate/aspnetboilerplate/blob/dev/src/Abp/DynamicEntityProperties/Extensions/DynamicEntityPropertyManagerExtensions.cs)

```csharp
public interface IDynamicEntityPropertyManager
{
    DynamicEntityProperty Get(int id);

    Task<DynamicEntityProperty> GetAsync(int id);

    List<DynamicEntityProperty> GetAll(string entityFullName);

    Task<List<DynamicEntityProperty>> GetAllAsync(string entityFullName);

    List<DynamicEntityProperty> GetAll();

    Task<List<DynamicEntityProperty>> GetAllAsync();

    void Add(DynamicEntityProperty dynamicEntityProperty);

    Task AddAsync(DynamicEntityProperty dynamicEntityProperty);

    void Update(DynamicEntityProperty dynamicEntityProperty);

    Task UpdateAsync(DynamicEntityProperty dynamicEntityProperty);

    void Delete(int id);

    Task DeleteAsync(int id);
}
```



### Entity Dynamic Property Values

The values your that your entity rows have. 

Adding value to entities dynamic property

```csharp
var valueOfRow1 = new DynamicEntityPropertyValue(cityDynamicPropertyOfCountry, EntityId: "1", value: "Istanbul", tenantId: AbpSession.TenantId);
var valueOfRow12 = new DynamicEntityPropertyValue(cityDynamicPropertyOfCountry, EntityId: "1", value: "London", tenantId: AbpSession.TenantId);//can have multiple values
var valueOfRow2 = new DynamicEntityPropertyValue(cityDynamicPropertyOfCountry, EntityId: "2", value: "London", tenantId: AbpSession.TenantId);

var _dynamicEntityPropertyValueManager = Resolve<IDynamicEntityPropertyValueManager>();
_dynamicEntityPropertyValueManager.Add(valueOfRow1);
_dynamicEntityPropertyValueManager.Add(valueOfRow12);
_dynamicEntityPropertyValueManager.Add(valueOfRow2);
```

Get values of entity property

```csharp
var _dynamicEntityPropertyValueManager = Resolve<IDynamicEntityPropertyValueManager>();
var allValues = _dynamicEntityPropertyValueManager.GetValues<Country>(EntityId: "1");
var cityValues = _dynamicEntityPropertyValueManager.GetValues<Country>(EntityId: "1", cityProperty);
```



***DynamicEntityPropertyValue.cs***

<table>
    <thead>
    	<tr>
            <th>Property</th>
            <th>Summary</th>
        </tr>
    </thead>
    <tbody>
    	<tr>
            <td>DynamicEntityPropertyId*  <span style="font-style: italic;">(int)</span></td>
            <td>Unique indentifier of DynamicEntityPropertyId</td>
        </tr>
         <tr>
            <td>EntityId*  <span style="font-style: italic;">(string)</span></td>
            <td>Unique idenfier of entity row</td>
        </tr>  
         <tr>
            <td>Value*  <span style="font-style: italic;">(string)</span></td>
            <td>Value</td>
        </tr> 
         <tr>
            <td>TenantId  <span style="font-style: italic;">(int?)</span></td>
            <td>Tenant's unique identifier <br/> <span style="font-style: italic;">(For example you can use (IAbpSession).TenantId to get current tenant's id)</span></td>
        </tr>            
    </tbody>
</table>



You can use [**IDynamicEntityPropertyValueManager**](https://github.com/aspnetboilerplate/aspnetboilerplate/blob/dev/src/Abp/DynamicEntityProperties/IDynamicEntityPropertyValueManager.cs) to manage entities dynamic properties. (It uses cache and checks required permissions.) See also: [DynamicEntityPropertyValueManagerExtensions](https://github.com/aspnetboilerplate/aspnetboilerplate/blob/dev/src/Abp/DynamicEntityProperties/Extensions/DynamicEntityPropertyValueManagerExtensions.cs)

```csharp
public interface IDynamicEntityPropertyValueManager
{
    DynamicEntityPropertyValue Get(int id);

    Task<DynamicEntityPropertyValue> GetAsync(int id);

    void Add(DynamicEntityPropertyValue dynamicEntityPropertyValue);

    Task AddAsync(DynamicEntityPropertyValue dynamicEntityPropertyValue);

    void Update(DynamicEntityPropertyValue dynamicEntityPropertyValue);

    Task UpdateAsync(DynamicEntityPropertyValue dynamicEntityPropertyValue);

    void Delete(int id);

    Task DeleteAsync(int id);

    List<DynamicEntityPropertyValue> GetValues(int dynamicEntityPropertyId, string EntityId);

    Task<List<DynamicEntityPropertyValue>> GetValuesAsync(int dynamicEntityPropertyId, string EntityId);

    List<DynamicEntityPropertyValue> GetValues(string entityFullName, string EntityId);

    Task<List<DynamicEntityPropertyValue>> GetValuesAsync(string entityFullName, string EntityId);

    List<DynamicEntityPropertyValue> GetValues(string entityFullName, string EntityId, int dynamicPropertyId);

    Task<List<DynamicEntityPropertyValue>> GetValuesAsync(string entityFullName, string EntityId, int dynamicPropertyId);

    List<DynamicEntityPropertyValue> GetValues(string entityFullName, string EntityId, string propertyName);

    Task<List<DynamicEntityPropertyValue>> GetValuesAsync(string entityFullName, string EntityId, string propertyName);

    void CleanValues(int dynamicEntityPropertyId, string EntityId);

    Task CleanValuesAsync(int dynamicEntityPropertyId, string EntityId);
}
```

