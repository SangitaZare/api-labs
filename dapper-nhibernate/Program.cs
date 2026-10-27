using DapperNHibernateApi.Data;
using DapperNHibernateApi.NHibernateData;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Dapper: a small helper that creates SQL connections
builder.Services.AddSingleton<DapperContext>();

// NHibernate: ISessionFactory (built once) + ISession (one per request)
builder.Services.AddNHibernate(connectionString);

builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
