using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using URlShortener;
using URlShortener.Entity;
using URlShortener.Extentions;
using URlShortener.Models;
using URlShortener.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(x =>
{
    x.UseSqlServer(builder.Configuration.GetConnectionString("Defaults"));
});
builder.Services.AddScoped<UrlShortenServices>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigration();
}

app.UseHttpsRedirection();



app.MapGet("api/{code}", async(string code, ApplicationDbContext context) =>
{
    var shortenedUrl = await context.ShortenedUrls.FirstOrDefaultAsync(s => s.Code == code);

    if(shortenedUrl is null)
    {
        return Results.BadRequest("Url Not Found");

    }
    return Results.Redirect(shortenedUrl.LongUrl);
})
.WithName("Shortner")
.WithOpenApi();

app.MapPost("api/shortn", async (ShortenUrlRequest request,
                            UrlShortenServices services,
                            ApplicationDbContext _context,
                            HttpContext context) =>
{
    //check if this valid uri 
    if(!Uri.TryCreate(request.Url , UriKind.Absolute , out _))
    {
        return Results.BadRequest("the specified Url is not vaild");
    }
    // Post Long Url To Create Short Url 
    var code = await services.UniqueCode();

    var shortenedUrl = new ShortenedUrl
    {
        Id = new Guid(),
        LongUrl = request.Url,
        Code = code,
        ShortUrl = $"{context.Request.Scheme}://{context.Request.Host}/api/{code}",
        CreateionOnUtc = DateTime.UtcNow,
    };
    _context.ShortenedUrls.Add(shortenedUrl);
    await _context.SaveChangesAsync();

    return Results.Ok(shortenedUrl.ShortUrl);

})
.WithName("UrlShortner")
.WithOpenApi();

app.Run();


