using WebAPIwithMongoDB.Context;
using WebAPIwithMongoDB.Entities;
using WebAPIwithMongoDB.Options;
using WebAPIwithMongoDB.Repositories;
using WebAPIwithMongoDB.Repositories.Interface;
using WebAPIwithMongoDB.Repositories.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();  // Đăng ký dịch vụ controller
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection(nameof(MongoSettings)));

builder.Services.AddScoped<IMongoDbContext, MongoDbContext>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPositionRepository, PositionRepository>();
builder.Services.AddScoped<IPermissionRepository, PermistionRepository>();
builder.Services.AddScoped<IPermissionOfAPositionRepository, PermissionOfAPositionRepository>();
builder.Services.AddScoped<ITeachGroupRepository, TeachGroupRepository>();
builder.Services.AddScoped<ICriteriaGroupRepository, CriteriaGroupRepository>();
builder.Services.AddScoped<ICriteriaRepository, CriteriaRepository>();
builder.Services.AddScoped<ICriteriaOfGroupRepository, CriteriaOfGroupRepository>();
builder.Services.AddScoped<IEvaluateRepository, EvaluateRepository>();
builder.Services.AddScoped<IRankRepository, RankRepository>();
builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<IPermissionRequestsRepository, PermissionRequestsRepository>();


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
