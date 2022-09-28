using EventBus.Messages.Common;
using MassTransit;
using Ordering.API.EventBusConsumer;
using Ordering.API.Extensions;
using Ordering.Application.Extensions;
using Ordering.Infrastructure.Extensions;
using Ordering.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

//Mass transit and RabbitMQ configuration
builder.Services.AddMassTransit(massTransitConfig =>
{
    massTransitConfig.AddConsumer<BasketCheckoutConsumer>();

    massTransitConfig.UsingRabbitMq((context, rabbitMQConfig) =>
    {
        rabbitMQConfig.Host(builder.Configuration["EventBusSettings:HostAddress"]);
        rabbitMQConfig.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueue, endpointConfig =>
        {
            endpointConfig.ConfigureConsumer<BasketCheckoutConsumer>(context);
        });
    });
});

//General configuration
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<BasketCheckoutConsumer>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MigrateDatabase<OrderContext>((context, services) =>
{
    var logger = services.GetService<ILogger<OrderContextSeed>>();
    //Commenting data seed since it doesn't meet table structure
    //OrderContextSeed.SeedAsync(context, logger).Wait();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
