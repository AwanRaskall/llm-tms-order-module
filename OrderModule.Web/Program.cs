using OrderModule.Application.Features.OrderExtractorService;
using OrderModule.Application.ExternalServices.OpenRouter;
using OrderModule.Application.Interfaces;
using OrderModule.Application.Features.OrderExtractorService.Utils;
using OrderModule.RavenDB.Connection;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using OrderModule.Application.Features.Configuration;
using OrderModule.Application.Features.ShipmentRequests;


// Register Windows encodings (needed for MsgReader and .msg files)
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);


var builder = WebApplication.CreateBuilder(args);

// -- MVC + JSON settings --
// PropertyNamingPolicy = null -> PascalCase is in JSON answers
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Disable camelCase - leave PascalCase as in C# models
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// OrderExtractor Services
builder.Services.AddScoped<MessageProcessor>();
builder.Services.AddScoped<OpenRouterService>();
builder.Services.AddScoped<IHttpServiceOpenRouter, HttpServiceOpenRouter>();
builder.Services.AddScoped<Normalizer>();


// -- RavenDB --
// Singleton: one DocumentStore for the entire duration of the application
builder.Services.AddSingleton<IDocumentStore>(
    _ => RavenDbStore.Initialize(builder.Configuration));

// Scoped: a new session for each HTTP request
builder.Services.AddScoped<IDocumentSession>(sp =>
    sp.GetRequiredService<IDocumentStore>().OpenSession());


// -- Shipment Request services --
builder.Services.AddScoped<ShipmentRequestService>();
builder.Services.AddScoped<ShipmentRequestReadService>();


// -- Configuration page services --
builder.Services.AddScoped<ConfigurationReadService>();
builder.Services.AddScoped<ConfigurationService>();


var app = builder.Build();

// Initialize RavenDB at startup, not on the first request. 
// Now the delay will occur only when the application starts (once),
// rather than each time the user navigates to the Config page.
_ = app.Services.GetRequiredService<IDocumentStore>();

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
