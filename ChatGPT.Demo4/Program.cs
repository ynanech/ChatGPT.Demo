using ChatGPT.Demo4.Extensions;
using OpenAI.Extensions;
using OpenAI.Interfaces;
using OpenAI.Managers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddOpenAIService(settings =>
{
    settings.ApiKey = "sk-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
});

builder.Services.AddSingleton<IChatGPTKeyService>(
    new ChatGPTKeyService("localhost"));

builder.Services.AddTransient<ChatGPTHttpMessageHandler>();
builder.Services.AddHttpClient<IOpenAIService, OpenAIService>().AddHttpMessageHandler<ChatGPTHttpMessageHandler>();

//注册账单服务
builder.Services.AddSingleton<IChatGPTBillService, ChatGPTBillService>();
//注册后台任务
builder.Services.AddHostedService<ChatGPTBillBackgroundService>();


var app = builder.Build();

var _chatGPTKeyService = app.Services.GetRequiredService<IChatGPTKeyService>();
_chatGPTKeyService.InitAsync().Wait();

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
