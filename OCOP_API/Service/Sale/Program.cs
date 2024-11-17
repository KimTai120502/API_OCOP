using sv.Sale;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors((options) => options.AddPolicy("CustomCors", build => { build.WithOrigins("*").AllowAnyHeader().AllowAnyMethod(); }));

#region Register Util Repository
// Register DapperContext 
builder.Services.AddSingleton<sv.Sale.Context.DapperContext>();

// Register Dabase Context Entity Framework 
builder.Services.AddDbContext<sv.Sale.DBModels.SaleContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnectionString"));
});

#endregion

#region Register Main Repository
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUtilsRepository, UtilsRepository>();
builder.Services.AddScoped<IStoreRepository, StoreRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
#endregion

// Thêm cấu hình JSON Serializer để giữ nguyên tên Field không thay đổi sang camelCase
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.AddHttpContextAccessor();

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
