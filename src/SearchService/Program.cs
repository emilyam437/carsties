// using MongoDB.Entities;
// using MongoDB.Driver;
using MassTransit;
using SearchService;
using Polly;
using Polly.Extensions.Http;

//using MongoDB.Bson;   // For BsonDocument
// Add other using directives as needed for your specific types.

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpClient<AuctionServiceHttpClient>().AddPolicyHandler(GetPolicy());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddMassTransit(x => {
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));
    x.UsingRabbitMq((context, cfg) => {
        cfg.ReceiveEndpoint("search-auction-created", e => {
            e.UseMessageRetry(r => r.Interval(5, 5));
            e.ConfigureConsumer<AuctionCreatedConsumer>(context);
        }); // e for endpoint formatter
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(async () =>{
try {
await DbInitializer.InitDb(app);
} catch(Exception e) {
Console.WriteLine(e);
}
});



//await DB.InitAsync("SearchDb", MongoClientSettings.FromConnectionString(builder.Configuration.GetConnectionString("MongoDbConnection")));
// Configure MongoDB.Entities
//await DB.InitAsync("SearchDb", builder.Configuration.GetConnectionString("MongoDbConnection"));

//await DB.InitAsync("SearchDb", "mongodb://localhost");

// await DB.Index<Item>()
//     .Key(x => x.Make, KeyType.Text)
//     .Key(x => x.Model, KeyType.Text)
//     .Key(x => x.Color, KeyType.Text).CreateAsync();

app.Run();

static IAsyncPolicy<HttpResponseMessage> GetPolicy()
=> HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryForeverAsync(_=> TimeSpan.FromSeconds(3));
