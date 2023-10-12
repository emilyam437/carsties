using AuctionService;
using AuctionService.Data;
// using MassTransit;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Npgsql;
// using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<AuctionDbContext>(opt => {
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

try {
DbInitializer.InitDb(app);
} catch(Exception e) {
    Console.WriteLine("There was an error (Program.cs line 39):");
    Console.WriteLine(e);
}

app.Run();
