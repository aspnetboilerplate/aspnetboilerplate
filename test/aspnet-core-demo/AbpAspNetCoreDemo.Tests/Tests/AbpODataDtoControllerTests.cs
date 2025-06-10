using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Domain.Repositories;
using AbpAspNetCoreDemo.Core.Application.Dtos;
using AbpAspNetCoreDemo.Core.Domain;
using Castle.MicroKernel.Registration;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using NSubstitute;
using Shouldly;
using Xunit;

namespace AbpAspNetCoreDemo.IntegrationTests.Tests;

public class AbpODataDtoControllerTests
{
    private readonly WebApplicationFactory<Startup> _factory;

    private IPermissionChecker _permissionChecker;

    public AbpODataDtoControllerTests()
    {
        _factory = new WebApplicationFactory<Startup>();

        RegisterFakePermissionChecker();
    }

    private void RegisterFakePermissionChecker()
    {
        Startup.IocManager.Value = new IocManager();

        _permissionChecker = Substitute.For<IPermissionChecker>();
        _permissionChecker.IsGrantedAsync(Arg.Any<string>()).Returns(true);
        _permissionChecker.IsGranted(Arg.Any<string>()).Returns(true);

        Startup.IocManager.Value.IocContainer.Register(
            Component.For<IPermissionChecker>().Instance(
                _permissionChecker
            ).IsDefault()
        );
    }

    [Fact(Skip = "OData does not support System.Text.Json.")]
    public async Task AbpODataDtoController_GetAll_Permission_Test()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/odata/ProductsDto");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var responseBody = await response.Content.ReadAsStringAsync();
        responseBody.ShouldBe("[{\"name\":\"Test product\",\"price\":100,\"id\":1}]");
    }

    [Fact(Skip = "OData does not support System.Text.Json.")]
    public async Task AbpODataDtoController_Get_Test()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/odata/ProductsDto(1)");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var responseBody = await response.Content.ReadAsStringAsync();
        responseBody.ShouldBe("{\"name\":\"Test product\",\"price\":100,\"id\":1}");
    }

    [Fact(Skip = "OData does not support System.Text.Json.")]
    public async Task AbpODataDtoController_Create_Test()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var content = JsonConvert.SerializeObject(new Product("Test product2"));
        var response = await client.PostAsync("/odata/ProductsDto",
            new StringContent(content, Encoding.UTF8, "application/json"));

        // Assert
        var responseBody2 = await response.Content.ReadAsStringAsync();
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        var createResponse = await client.GetAsync("/odata/ProductsDto(2)");
        createResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var responseBody = await createResponse.Content.ReadAsStringAsync();
        responseBody.ShouldBe("{\"name\":\"Test product2\",\"price\":0,\"id\":2}");
    }

    [Fact(Skip = "OData does not support System.Text.Json.")]
    public async Task AbpODataDtoController_Update_Test()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var content = JsonConvert.SerializeObject(new ProductCreateInput
        {
            Name = "Test product2",
            Price = 150
        });

        var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Patch, "/odata/ProductsDto(1)")
        {
            Content = new StringContent(content, Encoding.UTF8, "application/json")
        });

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var createResponse = await client.GetAsync("/odata/ProductsDto(1)");
        createResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var responseBody = await createResponse.Content.ReadAsStringAsync();
        responseBody.ShouldBe("{\"name\":\"Test product2\",\"price\":150,\"id\":1}");
    }

    [Fact(Skip = "OData does not support System.Text.Json.")]
    public async Task AbpODataDtoController_Delete_Test()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var deleteResponse = await client.DeleteAsync("/odata/ProductsDto(1)");

        // Assert
        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var response = await client.GetAsync("/odata/ProductsDto");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var responseBody = await response.Content.ReadAsStringAsync();
        responseBody.ShouldBe("[]");

    }
}