using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Dependency;
using AbpAspNetCoreDemo.Core.Domain;
using Castle.MicroKernel.Registration;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using NSubstitute;
using Shouldly;
using Xunit;

namespace AbpAspNetCoreDemo.IntegrationTests.Tests;

public class AbpODataEntityControllerTests
{
    private readonly WebApplicationFactory<Startup> _factory;

    private IPermissionChecker _permissionChecker;

    public AbpODataEntityControllerTests()
    {
        _factory = new WebApplicationFactory<Startup>();

        RegisterFakePermissionChecker();
    }

    private void RegisterFakePermissionChecker()
    {
        Startup.IocManager.Value = new IocManager();

        _permissionChecker = Substitute.For<IPermissionChecker>();
        _permissionChecker.IsGrantedAsync(Arg.Any<string>()).Returns(false);
        _permissionChecker.IsGranted(Arg.Any<string>()).Returns(false);

        Startup.IocManager.Value.IocContainer.Register(
            Component.For<IPermissionChecker>().Instance(
                _permissionChecker
            ).IsDefault()
        );
    }

    [Fact]
    public async Task AbpODataEntityController_DontWrapResult_Test()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/odata");
        var response2 = await client.GetAsync("/odata/$metadata");

        // Assert
        response.StatusCode.ShouldBe(Enum.Parse<HttpStatusCode>("200"));
        (await response.Content.ReadAsStringAsync()).ShouldNotContain("__abp");

        response2.StatusCode.ShouldBe(Enum.Parse<HttpStatusCode>("200"));
        (await response2.Content.ReadAsStringAsync()).ShouldNotContain("__abp");
    }

    [Fact]
    public async Task AbpODataEntityController_GetAll_Permission_Test()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/odata/Products");

        // Assert
        response.StatusCode.ShouldBe(Enum.Parse<HttpStatusCode>("500"));

        _permissionChecker.Received().IsGranted(
            Arg.Is<string>(permissionNames => permissionNames == "GetAllProductsPermission")
        );
    }

    [Fact]
    public async Task AbpODataEntityController_Get_Permission_Test()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/odata/Products(1)");

        // Assert
        response.StatusCode.ShouldBe(Enum.Parse<HttpStatusCode>("500"));

        _permissionChecker.Received().IsGranted(
            Arg.Is<string>(permissionNames => permissionNames == "GetProductPermission")
        );
    }

    [Fact]
    public async Task AbpODataEntityController_Create_Permission_Test()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var content = JsonConvert.SerializeObject(new Product("Test product2"));
        var response = await client.PostAsync("/odata/Products",
            new StringContent(content, Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.ShouldBe(Enum.Parse<HttpStatusCode>("500"));

        _permissionChecker.Received().IsGranted(Arg.Is<string>(
            permissionNames => permissionNames == "CreateProductPermission")
        );
    }

    [Fact]
    public async Task AbpODataEntityController_Update_Permission_Test()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var content = JsonConvert.SerializeObject(new Product("Test product2"));
        var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Patch, "/odata/Products(1)")
        {
            Content = new StringContent(content, Encoding.UTF8, "application/json")
        });

        // Assert
        response.StatusCode.ShouldBe(Enum.Parse<HttpStatusCode>("500"));

        _permissionChecker.Received().IsGranted(Arg.Is<string>(
            permissionNames => permissionNames == "UpdateProductPermission")
        );
    }

    [Fact]
    public async Task AbpODataEntityController_Delete_Permission_Test()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync("/odata/Products(1)");

        // Assert
        response.StatusCode.ShouldBe(Enum.Parse<HttpStatusCode>("500"));

        _permissionChecker.Received().IsGranted(Arg.Is<string>(
            permissionNames => permissionNames == "DeleteProductPermission")
        );
    }
}