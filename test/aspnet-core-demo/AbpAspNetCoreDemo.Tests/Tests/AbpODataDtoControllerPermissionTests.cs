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
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NSubstitute;
using Shouldly;
using Xunit;

namespace AbpAspNetCoreDemo.IntegrationTests.Tests;

public class AbpODataDtoControllerPermissionTests
{
    private readonly WebApplicationFactory<Startup> _factory;

    private IPermissionChecker _permissionChecker;

    public AbpODataDtoControllerPermissionTests()
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
    public async Task AbpODataDtoController_GetAll_Permission_Test()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/odata/ProductsDto");

        // Assert
        response.StatusCode.ShouldBe(Enum.Parse<HttpStatusCode>("500"));

        _permissionChecker.Received().IsGranted(
            Arg.Is<string>(permissionNames => permissionNames == "GetAllProductsPermission")
        );
    }

    [Fact]
    public async Task AbpODataDtoController_Get_Permission_Test()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/odata/ProductsDto(1)");

        // Assert
        response.StatusCode.ShouldBe(Enum.Parse<HttpStatusCode>("500"));

        _permissionChecker.Received().IsGranted(
            Arg.Is<string>(permissionNames => permissionNames == "GetProductPermission")
        );
    }

    [Fact]
    public async Task AbpODataDtoController_Create_Permission_Test()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var content = JsonConvert.SerializeObject(new Product("Test product2"));
        var response = await client.PostAsync("/odata/ProductsDto",
            new StringContent(content, Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.ShouldBe(Enum.Parse<HttpStatusCode>("500"));

        _permissionChecker.Received().IsGranted(Arg.Is<string>(
            permissionNames => permissionNames == "CreateProductPermission")
        );
    }

    [Fact]
    public async Task AbpODataDtoController_Update_Permission_Test()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var content = JsonConvert.SerializeObject(new Product("Test product2"));
        var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Patch, "/odata/ProductsDto(1)")
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
    public async Task AbpODataDtoController_Delete_Permission_Test()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync("/odata/ProductsDto(1)");

        // Assert
        response.StatusCode.ShouldBe(Enum.Parse<HttpStatusCode>("500"));

        _permissionChecker.Received().IsGranted(Arg.Is<string>(
            permissionNames => permissionNames == "DeleteProductPermission")
        );
    }
}