using Azure.Data.Tables;
using Azure.Storage.Queues;
using Microsoft.Extensions.Azure;
using Mvc.StorageAccount.Demo.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration["AzureStorage:ConnectionString"]; 

// Add services to the container.
builder.Services.AddAzureClients(builder =>
{
    builder.AddBlobServiceClient(connectionString);
    builder.AddQueueServiceClient(connectionString)
           .ConfigureOptions(c =>
           {
               c.MessageEncoding = QueueMessageEncoding.Base64;
           });
    builder.AddTableServiceClient(connectionString);
});

builder.Services.AddAzureClients(b =>
{
    b.AddClient<QueueClient, QueueClientOptions>((_, _, _) =>
    {
        return new QueueClient(connectionString,
            builder.Configuration["AzureStorage:QueueName"],
            new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            });
    });

    b.AddClient<TableClient, TableClientOptions>((_, _, _) =>
    {
        return new TableClient(connectionString, builder.Configuration["AzureStorage:TableStorage"]);
    });
});

builder.Services.AddScoped<ITableStorageService, TableStorageService>();
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
builder.Services.AddScoped<IQueueService, QueueService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
