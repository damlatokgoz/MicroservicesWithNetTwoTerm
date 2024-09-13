using MassTransit;
using Microsoft.AspNetCore.OutputCaching;
using ServiceBus;
using Stock.Service.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ServiceBus.IBus, ServiceBus.Bus>();
builder.Services.AddHostedService<OrderCreatedEventConsumerBackgroundService>();


builder.Services.Configure<BusOptions>(builder.Configuration.GetSection(key: nameof(BusOptions)));

builder.Services.AddMassTransit(configure =>
{
    configure.AddConsumer<OrderCreatedEventConsumer>();

    configure.UsingRabbitMq(configure: (context, cfg) =>
    {
        cfg.UseMessageRetry(configure: r => r.Interval(5, TimeSpan.FromSeconds(5)));


        cfg.UseInMemoryOutbox(context);

        var busOptions = builder.Configuration.GetSection(key: nameof(BusOptions)).Get<BusOptions>();

        cfg.Host(new Uri(busOptions!.Url));

        //create order created event queue endpoint
        cfg.ReceiveEndpoint(BusConstants.StockCreatedEventQueueMassTransit, e => 
        { 
            e.Consumer<OrderCreatedEventConsumer>(context);
        });


        //cfg.ConfigureEndpoints(context);
    });

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
