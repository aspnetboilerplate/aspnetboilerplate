# Using MassTransit with RabbitMQ in ASP.NET Boilerplate

MassTransit is a free, open-source distributed application framework for .NET. MassTransit makes it easy to create applications and services that leverage message-based, loosely-coupled asynchronous communication for higher availability, reliability, and scalability.

You can check [MassTransit documentation](https://masstransit-project.com/) and [GitHub repository](https://github.com/MassTransit/MassTransit) for more information.

## Install RabbitMQ

We will use RabbitMQ with MassTransit. So, first thing we will do is installing RabbitMQ. Go to [installation documentation](https://www.rabbitmq.com/install-windows.html) for Windows or select your operating system on the related page if you are not using Windows. Then, install the RabbitMQ to your computer.

## Creating a project

In order to integrate MassTransit, let's create a new project on [https://aspnetboilerplate.com/Templates](https://aspnetboilerplate.com/Templates) page. After creating the project, run the project by following the suggested getting started document which will be displayed to you after project creation. In order to use sample code in this article, you can use `MassTransitSample` as the project name.

### Create Publisher project

In order to publish messages, we will create a separate web project. To do that, add an empty web project to your solution named `MassTransitSample.Web.Publisher`.

After that, add packages below to created project;

```xml
    <PackageReference Include="MassTransit.RabbitMQ" Version="8.0.6" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.2.3" />
```

Then, change the content of `Program.cs` file in the publisher project as shown below;

```csharp
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(mass =>
{
    mass.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
```

This will basically integrate Swagger and MassTransit to the publisher project.

In order to publish a message, add line below to your `Program.cs` right after `app.UseHttpsRedirection();`;

```csharp
app.MapPost("create-order", async (IBus bus, OrderDto order) =>
{
    await bus.Publish(order);
});
```

#### Create OrderDto

In our message, we will use a [DTO](https://aspnetboilerplate.com/Pages/Documents/Data-Transfer-Objects) class to send our message. So, let's first create `Orders`  folder, then `Dto` folder under Orders folder in `MassTransitSample.Application` project and then create `OrderDto` class as shown below under `Orders/Dto` folder;

```csharp
public class OrderDto
{
    public string OrderName { get; set; }

    public decimal Price { get; set; }
}
```

Finally, add a reference to `MassTransitSample.Application` in `MassTransitSample.Web.Publisher` project, so we can use `OrderDto` in publisher project.

### Configure MVC project

We managed to publish messages from our Publisher project. So, now we need to configure our consumer project to receive and handle messsages. In this sample, we will configure `MassTransitSample.Web.Mvc` project but, you can apply same configuration to `MassTransitSample.Web.Host` or any other web project you will add to your solution.

Add MassTransit NuGet package to your MVC project;

```xml
<ItemGroup>
    <PackageReference Include="MassTransit.RabbitMQ" Version="8.0.10" />
</ItemGroup>
```

Then, create a consumer to consume messages sent from Publisher project as shown below;

```csharp
using System.Threading.Tasks;
using MassTransit;
using MassTransitSample.Orders.Dto;
using Microsoft.Extensions.Logging;

namespace MassTransitSample.Web.Orders
{
    public class OrderConsumer : IConsumer<OrderDto>
    {
        private readonly ILogger<OrderDto> _logger;

        public OrderConsumer(ILogger<OrderDto> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderDto> context)
        {
            _logger.LogInformation("Received order {code} with price {price}",
                context.Message.OrderName,
                context.Message.Price
            );

            await Task.CompletedTask;
        }
    }
}
```

Since ASP.NET Boilerplate uses Castle.Windsor for dependency injection, we will configure MassTransit in a different way then the suggested approach in its default documentation. 

In order to inject any service to our consumer classes, we need to do this registration in `PostInitialize` method so that it will be executed after ASP.NET Boilerplate completes registration of classes to dependency injection. To do this, go to `MassTransitSampleWebMvcModule` and override `PostInitialize` method and change its content as shown below;

```csharp
public override void PostInitialize()
{
	IocManager.IocContainer.Register(Component.For<OrderConsumer>().LifestyleTransient());
	
	var busControl = Bus.Factory.CreateUsingRabbitMq(config =>
	{
		config.Host(new Uri("rabbitmq://localhost/"), host =>
		{
			host.Username("guest");
			host.Password("guest");
		});

		config.ReceiveEndpoint(queueName: "repro-service", endpoint =>
		{
			endpoint.Handler<OrderDto>(async context =>
			{
				using (var consumer = IocManager.ResolveAsDisposable<OrderConsumer>(typeof(OrderConsumer)))
				{
					await consumer.Object.Consume(context);
				}
			});
		});
	});

	IocManager.IocContainer.Register(Component.For<IBus, IBusControl>().Instance(busControl));

	busControl.Start();
}
```

After all, run the `MassTransit.Publisher` app and `MassTransit.Web.Mvc` app. Then, open /swagger URL on publisher app and send a message. This message will be handled by `OrderConsumer` in MVC project.

## Source Code

You can access the sample project on [https://github.com/aspnetboilerplate/aspnetboilerplate-samples/tree/master/MassTransitSample](https://github.com/aspnetboilerplate/aspnetboilerplate-samples/tree/master/MassTransitSample).