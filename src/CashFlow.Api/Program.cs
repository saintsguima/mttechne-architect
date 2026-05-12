using CashFlow.Application.Abstractions;
using CashFlow.Application.UseCases;
using CashFlow.Infrastructure.BackgroundServices;
using CashFlow.Infrastructure.Data;
using CashFlow.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "Cash Flow API", Version = "v1" }));

builder.Services.AddDbContext<CashFlowDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

builder.Services.AddScoped<ICashEntryRepository, CashEntryRepository>();
builder.Services.AddScoped<IDailyBalanceRepository, DailyBalanceRepository>();
builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<CreateCashEntryUseCase>();
builder.Services.AddScoped<GetDailyBalanceUseCase>();
builder.Services.AddHostedService<OutboxConsolidationWorker>();

builder.Services.AddHealthChecks().AddSqlServer(builder.Configuration.GetConnectionString("SqlServer")!);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();

public partial class Program { }
