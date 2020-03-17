## Introduction

**Dynamic Parameter System** is a system that allows you to add and manage new parameters on entity objects at runtime without any code changes. With this system, you can define dynamic parameters on entity objects and perform operations on these objects easily. For example, it can be used for cities, counties, gender, status codes etc. 

### Dynamic Parameter Definition

First of all you need to define which entities can have that feature and allowed *input types* * can a dynamic parameter has. 

```csharp
public class MyDynamicEntityParameterDefinitionProvider : DynamicEntityParameterDefinitionProvider
{
     public override void SetDynamicEntityParameters(IDynamicEntityParameterDefinitionContext context)
     {
         //input types dynamic parameters can have
         context.Manager.AddAllowedInputType<SingleLineStringInputType>();
         context.Manager.AddAllowedInputType<CheckboxInputType>();
         context.Manager.AddAllowedInputType<ComboboxInputType>();
         
         //entities that can have dynamic parameter
         context.Manager.AddEntity<Country>(); 
         context.Manager.AddEntity<Blog, long>(); 
     }
}
```

*InputType: A UI input type for the feature. This can be defined, and then used while creating an automatic feature screen. For example: [SingleLineStringInputType](https://github.com/aspnetboilerplate/aspnetboilerplate/blob/dev/src/Abp/UI/Inputs/SingleLineStringInputType.cs), [ComboboxInputType](https://github.com/aspnetboilerplate/aspnetboilerplate/blob/dev/src/Abp/UI/Inputs/ComboboxInputType.cs), [CheckboxInputType](https://github.com/aspnetboilerplate/aspnetboilerplate/blob/dev/src/Abp/UI/Inputs/CheckboxInputType.cs)*

```csharp
public class DynamicEntityParametersTestModule : AbpModule
{
    public override void PreInitialize()
    {        Configuration.DynamicEntityParameters.Providers.Add<MyDynamicEntityParameterDefinitionProvider>();
    }
}
```



### Dynamic Parameter

It is a parameter that you can use in other entities. 

Creating a dynamic parameter.

```csharp
var cityParameter = new DynamicParameter
{
    ParameterName = "City",//Parameter name. 
    InputType = InputTypeBase.GetName<ComboboxInputType>(),
    Permission = "App.Permission.DynamicParameter.City",
    TenantId = AbpSession.TenantId
};
var dynamicParameterManager = Resolve<IDynamicParameterManager>();
dynamicParameterManager.Add(cityParameter);
```

***DynamicParameter.cs***

<table>
    <thead>
    	<tr>
            <th>Property</th>
            <th>Summary</th>
        </tr>
    </thead>
    <tbody>
    	<tr>
            <td>ParameterName*  <span style="font-style: italic;">(string)</span></td>
            <td>Unique name of the dynamic parameter</td>
        </tr>
         <tr>
            <td>Input Type*  <span style="font-style: italic;">(string)</span></td>
            <td>Input type name of the dynamic parameter.(Input type should be provided in definition.)</td>
        </tr>  
         <tr>
            <td>Permission  <span style="font-style: italic;">(string)</span></td>
            <td>Required permission to manage anything about that parameter <br><span style="font-style: italic;">(<code>DynamicParameterValue</code>, <code>EntityDynamicParameter</code>, <code>EntityDynamicParameterValue</code>)</span></td>
        </tr> 
         <tr>
            <td>TenantId  <span style="font-style: italic;">(int?)</span></td>
            <td>Tenant's unique identifier <br/> <span style="font-style: italic;">(For example you can use (IAbpSession).TenantId to get current tenant's id)</span></td>
        </tr>  
    </tbody>
</table>



You can use [**IDynamicParameterManager**](https://github.com/aspnetboilerplate/aspnetboilerplate/blob/pr/5307/src/Abp/DynamicEntityParameters/IDynamicParameterManager.cs) to manage dynamic parameter. (It uses cache)

```csharp
public interface IDynamicParameterManager
{
    DynamicParameter Get(int id);

    Task<DynamicParameter> GetAsync(int id);

    DynamicParameter Get(string parameterName);

    Task<DynamicParameter> GetAsync(string parameterName);

    void Add(DynamicParameter dynamicParameter);

    Task AddAsync(DynamicParameter dynamicParameter);

    void Update(DynamicParameter dynamicParameter);

    Task UpdateAsync(DynamicParameter dynamicParameter);

    void Delete(int id);

    Task DeleteAsync(int id);
}
```



### Dynamic Parameter Value

If your dynamic parameter's input types need values to select (for example `ComboboxInputType`), you can store the values that your dynamic parameters can have here. 

Adding a value to your dynamic parameter.

```csharp
var istanbul = new new DynamicParameterValue(cityParameter, "Istanbul", AbpSession.TenantId);
var london = new new DynamicParameterValue(cityParameter, "London", AbpSession.TenantId);

var _dynamicParameterValueManager = Resolve<IDynamicParameterValueManager>();
_dynamicParameterValueManager.Add(istanbul); 
_dynamicParameterValueManager.Add(london);
```

***DynamicParameterValue.cs***

<table>
    <thead>
    	<tr>
            <th>Property</th>
            <th>Summary</th>
        </tr>
    </thead>
    <tbody>
    	<tr>
            <td>DynamicParameterId*  <span style="font-style: italic;">(int)</span></td>
            <td>Unique indentifier of DynamicParameter</td>
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



You can use [**IDynamicParameterValueManager**](https://github.com/aspnetboilerplate/aspnetboilerplate/blob/pr/5307/src/Abp/DynamicEntityParameters/IDynamicParameterValueManager.cs) to manage dynamic parameter values. (It checks permissions)

```csharp
public interface IDynamicParameterValueManager
{
    DynamicParameterValue Get(int id);

    Task<DynamicParameterValue> GetAsync(int id);

    List<DynamicParameterValue> GetAllValuesOfDynamicParameter(int dynamicParameterId);

    Task<List<DynamicParameterValue>> GetAllValuesOfDynamicParameterAsync(int dynamicParameterId);

    void Add(DynamicParameterValue dynamicParameterValue);

    Task AddAsync(DynamicParameterValue dynamicParameterValue);

    void Update(DynamicParameterValue dynamicParameterValue);

    Task UpdateAsync(DynamicParameterValue dynamicParameterValue);

    void Delete(int id);

    Task DeleteAsync(int id);

    void CleanValues(int dynamicParameterId);

    Task CleanValuesAsync(int dynamicParameterId);
}
```



### Entity Dynamic Parameters

Dynamic parameter definitions of an entity. It stores which dynamic parameters an entity have. 

Adding New Parameter to Entity

```csharp
var _entityDynamicParameterManager = Resolve<IEntityDynamicParameterManager>();
var cityDynamicParameterOfCountry = _entityDynamicParameterManager.Add<Country>(cityParameter, tenantId: 1);//add cityParameter to Country entity
```

***EntityDynamicParameter.cs***

<table>
    <thead>
    	<tr>
            <th>Property</th>
            <th>Summary</th>
        </tr>
    </thead>
    <tbody>
    	<tr>
            <td>DynamicParameterId*  <span style="font-style: italic;">(int)</span></td>
            <td>Unique indentifier of DynamicParameter</td>
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



You can use [**IEntityDynamicParameterManager**](https://github.com/aspnetboilerplate/aspnetboilerplate/blob/pr/5307/src/Abp/DynamicEntityParameters/IEntityDynamicParameterManager.cs) to manage entities dynamic parameters. (It uses cache and checks required permissions.) See also: [EntityDynamicParameterManagerExtensions](https://github.com/aspnetboilerplate/aspnetboilerplate/blob/pr/5307/src/Abp/DynamicEntityParameters/Extensions/EntityDynamicParameterManagerExtensions.cs)

```csharp
public interface IEntityDynamicParameterManager
{
    EntityDynamicParameter Get(int id);

    Task<EntityDynamicParameter> GetAsync(int id);

    List<EntityDynamicParameter> GetAll(string entityFullName);

    Task<List<EntityDynamicParameter>> GetAllAsync(string entityFullName);

    List<EntityDynamicParameter> GetAll();

    Task<List<EntityDynamicParameter>> GetAllAsync();

    void Add(EntityDynamicParameter entityDynamicParameter);

    Task AddAsync(EntityDynamicParameter entityDynamicParameter);

    void Update(EntityDynamicParameter entityDynamicParameter);

    Task UpdateAsync(EntityDynamicParameter entityDynamicParameter);

    void Delete(int id);

    Task DeleteAsync(int id);
}
```



### Entity Dynamic Parameter Values

The values your that your entity rows have. 

Adding value to entities dynamic parameter

```csharp
var valueOfRow1 = new EntityDynamicParameterValue(cityDynamicParameterOfCountry, EntityId: "1", value: "Istanbul", tenantId: AbpSession.TenantId);
var valueOfRow12 = new EntityDynamicParameterValue(cityDynamicParameterOfCountry, EntityId: "1", value: "London", tenantId: AbpSession.TenantId);//can have multiple values
var valueOfRow2 = new EntityDynamicParameterValue(cityDynamicParameterOfCountry, EntityId: "2", value: "London", tenantId: AbpSession.TenantId);

var _entityDynamicParameterValueManager = Resolve<IEntityDynamicParameterValueManager>();
_entityDynamicParameterValueManager.Add(valueOfRow1);
_entityDynamicParameterValueManager.Add(valueOfRow12);
_entityDynamicParameterValueManager.Add(valueOfRow2);
```

Get values of entity parameter

```csharp
var _entityDynamicParameterValueManager = Resolve<IEntityDynamicParameterValueManager>();
var allValues = _entityDynamicParameterValueManager.GetValues<Country>(EntityId: "1");
var cityValues = _entityDynamicParameterValueManager.GetValues<Country>(EntityId: "1", cityParameter);
```



***EntityDynamicParameterValue.cs***

<table>
    <thead>
    	<tr>
            <th>Property</th>
            <th>Summary</th>
        </tr>
    </thead>
    <tbody>
    	<tr>
            <td>EntityDynamicParameterId*  <span style="font-style: italic;">(int)</span></td>
            <td>Unique indentifier of EntityDynamicParameterId</td>
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



You can use [**IEntityDynamicParameterValueManager**](https://github.com/aspnetboilerplate/aspnetboilerplate/blob/pr/5307/src/Abp/DynamicEntityParameters/IEntityDynamicParameterValueManager.cs) to manage entities dynamic parameters. (It uses cache and checks required permissions.) See also: [EntityDynamicParameterValueManagerExtensions](https://github.com/aspnetboilerplate/aspnetboilerplate/blob/pr/5307/src/Abp/DynamicEntityParameters/Extensions/EntityDynamicParameterValueManagerExtensions.cs)

```csharp
public interface IEntityDynamicParameterValueManager
{
    EntityDynamicParameterValue Get(int id);

    Task<EntityDynamicParameterValue> GetAsync(int id);

    void Add(EntityDynamicParameterValue entityDynamicParameterValue);

    Task AddAsync(EntityDynamicParameterValue entityDynamicParameterValue);

    void Update(EntityDynamicParameterValue entityDynamicParameterValue);

    Task UpdateAsync(EntityDynamicParameterValue entityDynamicParameterValue);

    void Delete(int id);

    Task DeleteAsync(int id);

    List<EntityDynamicParameterValue> GetValues(int entityDynamicParameterId, string EntityId);

    Task<List<EntityDynamicParameterValue>> GetValuesAsync(int entityDynamicParameterId, string EntityId);

    List<EntityDynamicParameterValue> GetValues(string entityFullName, string EntityId);

    Task<List<EntityDynamicParameterValue>> GetValuesAsync(string entityFullName, string EntityId);

    List<EntityDynamicParameterValue> GetValues(string entityFullName, string EntityId, int dynamicParameterId);

    Task<List<EntityDynamicParameterValue>> GetValuesAsync(string entityFullName, string EntityId, int dynamicParameterId);

    List<EntityDynamicParameterValue> GetValues(string entityFullName, string EntityId, string parameterName);

    Task<List<EntityDynamicParameterValue>> GetValuesAsync(string entityFullName, string EntityId, string parameterName);

    void CleanValues(int entityDynamicParameterId, string EntityId);

    Task CleanValuesAsync(int entityDynamicParameterId, string EntityId);
}
```

