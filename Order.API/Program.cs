using MassTransit;
using Order.Service;
using Polly;
using Polly.Extensions.Http;
using ServiceBus;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//retry policy

var retryPolicy = HttpPolicyExtensions.HandleTransientHttpError()
    .WaitAndRetryAsync(3, retryAttemp => TimeSpan.FromSeconds(Math.Pow(2, retryAttemp)));

var circuitBreakerpolicy = HttpPolicyExtensions.HandleTransientHttpError().CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));

var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(20));

var combinedPolicy = Policy.WrapAsync(retryPolicy, circuitBreakerpolicy, timeoutPolicy);

builder.Services.AddSingleton<ServiceBus.IBus, ServiceBus.Bus>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddHttpClient<StockService>(options =>
{
    options.BaseAddress = new Uri(builder.Configuration.GetSection("MicroserviceBaseUrl")["Stock"]!);
}).AddPolicyHandler(combinedPolicy);

builder.Services.AddMassTransit(configure =>
{
   
    configure.UsingRabbitMq(configure: (context, cfg) =>
    {
        var busOptions = builder.Configuration.GetSection(key: nameof(BusOptions)).Get<BusOptions>();

        cfg.Host(new Uri(busOptions!.Url));

        //cfg.ConfigureEndpoints(context);
    });

});
builder.Services.Configure<BusOptions>(builder.Configuration.GetSection(key: nameof(BusOptions)));
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
