using OrderModule.Application.Features.OrderExtractorService;
using OrderModule.Application.ExternalServices.OpenRouter;
using OrderModule.Application.Interfaces;
using OrderModule.Application.Features.OrderExtractorService.Utils;

// Register Windows encodings (needed for MsgReader and .msg files)
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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
