using ApexWebFileMgr.core.Services.DbCallService;
using ApexWebFileMgr.core.Services.FileMgrService;
using ApexWebFileMgr.DB.Dapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IFileManagerService, FileManagerService>();
builder.Services.AddScoped<IDbCallService, DbCallService>();
builder.Services.AddScoped<IDapperService, DapperService>();

builder.Services.AddCors(
    p => p.AddPolicy(
        "corsapp",
        builder =>
        {
            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
    )
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("corsapp");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
