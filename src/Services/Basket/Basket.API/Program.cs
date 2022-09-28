using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Discount.Grpc.Protos;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

//Redis configuration
builder.Services.AddStackExchangeRedisCache(opt =>
{
    opt.Configuration = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString");
});

//General configuration
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddAutoMapper(typeof(Program));

//Grpc configuration
builder.Services
    .AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>
        (opt => opt.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]));
builder.Services.AddScoped<DiscountGrpcService>();

//Mass transit configuration
builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(builder.Configuration["EventBusSettings:HostAddress"]);
    });
});

//builder.Services.AddMassTransitHostedService();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
