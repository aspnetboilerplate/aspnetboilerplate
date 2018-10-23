### Introduction

ASP.NET Boilerplate defines two basic interfaces for Multi-Lingual entity definitions to provide a standard model for translating entities. 

#### IMultiLingualEntity

`IMultiLingualEntity<TTranslation>` interface is used to mark multi lingual entities. The entities marked with `IMultiLingualEntity<TTranslation>` interface must define language-neutral information. The entities marked with `IMultiLingualEntity<TTranslation>` contains a collection of Translations which contains language-dependent information.

A sample multi lingual entity would be;


	public class Product : Entity, IMultiLingualEntity<ProductTranslation>
	{
	    public decimal Price { get; set; }
	
	    public ICollection<ProductTranslation> Translations { get; set; }
	}


#### IEntityTranslation

IEntityTranslation interface is used to mark translation of a Multi-Lingual entity. The entities marked with IEntityTranslation interface must define language dependent information. The entities marked with IEntityTranslation contains Language field which contains a language code for the translation and a reference to Multi-Lingual entity.

A sample multi lingual entity would be;


	public class ProductTranslation : Entity, IEntityTranslation<Product>
	{
	    public string Name { get; set; }
	
	    public Product Core { get; set; }
	
	    public int CoreId { get; set; }
	
	    public string Language { get; set; }
	}

 #### CreateMultiLingualMap 

When listing Multi-Lingual entities on a user interface, most of the time, only one translation of a Multi-Lingual entity which is in user's current language will be displayed to user.

For this purpose, ABP defines CreateMultiLingualMap extension method to map a Multi-Lingual entity and one of it's Translation to an appropriate Dto class using **AutoMapper**. 

By using CreateMultiLingualMap extension method, only one record from Translations collection of a Multi-Lingual entity will be mapped to target Dto class. This extension method finds the translation with selected UI language first. If there is no translation with selected UI language, then extension method searches for the default language setting (see  [Setting-Management](Setting-Management#setting-scope.md)) and uses the translation in default language. If extension method couldn't find any translation in current UI language or default language, it uses one of the existing translations. 

A sample Dto class for sample Product entity above would be;


	public class ProductListDto
	{
	    // Mapped from Product.Price
	    public decimal Price { get; set; }
	
	    // Mapped from ProductTranslation.Name
	    public string Name { get; set; }
	}


And it's mapping configuration is;


	Configuration.Modules.AbpAutoMapper().Configurators.Add(configuration =>
	{
	    CustomDtoMapper.CreateMappings(configuration, new MultiLingualMapContext(
	        IocManager.Resolve<ISettingManager>()
	    ));
	});
	
	internal static class CustomDtoMapper
	{
	    public static void CreateMappings(IMapperConfigurationExpression configuration, MultiLingualMapContext context)
	    {
	        configuration.CreateMultiLingualMap<Product, ProductTranslation, ProductListDto>(context);
	    }
	}


SettingManager is required to find default language setting when mapping a multi lingual entity to a Dto class. 

In some cases like editing a multi lingual entity on the UI, all translations may be needed in the Dto class. In such cases, the Dto classes can be defined like below and [Object-To-Object-Mapping](Object-To-Object-Mapping.md) can be used.


	[AutoMap(typeof(Product))]
	public class ProductDto
	{
	    public decimal Price { get; set; }
	
	    public List<ProductTranslationDto> Translations {get; set;}
	}



	[AutoMap(typeof(ProductTranslation))]
	public class ProductTranslationDto
	{
	    public string Name { get; set; }
	}



CreateMultiLingualMap extension method returns an object of type CreateMultiLingualMapResult which contains **EntityMap** and  **TranslationMap** fields. These fields can be used to customize multi lingual mapping. A sample usage would be;

```c#
configuration.CreateMultiLingualMap<Order, OrderTranslation, OrderListDto>(context)
    .EntityMap.ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count));
```

### Crud Operations

#### Creating a MultiLingual Entity with Translation(s) 

A Dto class like the below one can be used for creating a Multi-Lingual entity with it's translations.


	[AutoMap(typeof(Product))]
	public class ProductDto
	{
	    public decimal Price { get; set; }
	
	    public ICollection<ProductTranslationDto> Translations { get; set; }
	}

After defining such a Dto class, we can use it in our application service to create a Multi-Lingual entity.


	public class ProductAppService : ApplicationService, IProductAppService
	{
	    private readonly IRepository<Product> _productRepository;
	
	    public ProductAppService(IRepository<Product> productRepository)
	    {
	        _productRepository = productRepository;
	    }
	
	    public async Task CreateProduct(ProductDto input)
	    {
	        var product = ObjectMapper.Map<Product>(input);
	        await _productRepository.InsertAsync(product);
	    }
	}

#### Updating a Multi-Lingual Entity with Translation(s)

We can use similar Dto class for updating our Multi-Lingual entity. A sample application service method for update operation can be defined like below;


	public async Task UpdateProduct(ProductDto input)
	{
	    var product = await _productRepository.GetAllIncluding(p => p.Translations)
	        .FirstOrDefaultAsync(p => p.Id == input.Id);
	
	    product.Translations.Clear();
	
	    ObjectMapper.Map(input, product);
	}

##### Note for EntityFramework 6.x

For EntityFramework 6.x, all the translations must be deleted from database manually because Entity Framework 6.x doesn't delete related data. Instead, EntityFramework 6.x tries to set CoreId of each Translation entity to null which fails. So, a sample code like the below one might be used to delete translations of a Multi-Lingual entity for EntityFramework 6.x.


	foreach (var translation in product.Translations.ToList())
	{
	    await _productTranslationRepository.DeleteAsync(translation);
	    product.Translations.Remove(translation);
	}

