using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRateLimiter(LimitOpt => 
{
    LimitOpt.AddFixedWindowLimiter("limit", opt =>
    {
        opt.PermitLimit = 1;
        opt.Window = TimeSpan.FromSeconds(10);
        opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 2;
    });
    //LimitOpt.RejectionStatusCode = StatusCodes.Status405MethodNotAllowed;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRateLimiter();
List<string> drinksList = new List<string>() { "Black Coffee", "Lattee","Mocha","Irish Coffee"};

app.MapGet("/Drinks", () => drinksList.ToList()).RequireRateLimiting("limit");
app.MapGet("/Date",() =>DateTime.Now).RequireRateLimiting("limit");
app.MapGet("/Counts",()=>drinksList.Count()).RequireRateLimiting("limit");
app.Run();
