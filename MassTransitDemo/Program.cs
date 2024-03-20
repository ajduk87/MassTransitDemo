using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sample.Components.Consumers;
using Sample.Contracts;
using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
builder.Services.AddMassTransit(cfg =>
{
    //cfg.AddConsumer<SubmitOrderConsumer>();

    //cfg.UsingInMemory((context, busConfigurator) =>
    //{
    //    busConfigurator.ReceiveEndpoint("submit-order", ep =>
    //    {
    //        ep.ConfigureConsumer<SubmitOrderConsumer>(context);
    //    });
    //});

    cfg.UsingRabbitMq((context, busConfigurator) =>
    {
        busConfigurator.Host(new Uri("rabbitmq://localhost/"), h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        // Add additional configuration here as needed
       
    });
});

//builder.Services.AddMassTransitHostedService();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
