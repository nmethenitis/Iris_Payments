using IrisPayments.Data;
using IrisPayments.Data.Interfaces;
using IrisPayments.Data.Services;
using IrisPayments.Helpers.Interfaces;
using IrisPayments.Helpers.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<IrisPaymentsContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("IrisPaymentsContext")));
builder.Services.AddTransient<IPaymentCode, PaymentCodeService>();
builder.Services.AddTransient<IPaymentLogs, PaymentLogsService>();
builder.Services.AddTransient<IPaymentHelper, PaymentHelperService>();
builder.Services.AddTransient<IPaymentLogsHelper, PaymentLogsHelperService>();
builder.Services.AddTransient<IEmailSender, EmailSenderService>();
builder.Services.AddTransient<IPaymentCodeHelper, PaymentCodeHelperService>();
builder.Services.AddScoped<AuthorizationFilterService>();
builder.Services.AddScoped<IApiKeyValidator, ApiKeyValidatorService>();
builder.Services.AddScoped<IBasicAuthenticator, BasicAuthenticatorService>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
