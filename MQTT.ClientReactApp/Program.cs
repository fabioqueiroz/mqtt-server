using Duende.Bff.Yarp;
using Microsoft.AspNetCore.Authentication;
using MQTT.CentralServer.Entities.Options;
using MQTT.CentralServer.Services.Interfaces;
using MQTT.CentralServer.Services.Token;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services
    .AddBff()
    .AddRemoteApis();

builder.Services
    .AddReverseProxy()
    .AddBffExtensions();

builder.Services.AddHttpClient<IJwtTokenService, JwtTokenService>(c =>
        c.BaseAddress = new Uri(builder.Configuration["ApiConfigs:BackChannelApi:Uri"]));

builder.Services.Configure<ApiConfigs>(builder.Configuration.GetSection(nameof(ApiConfigs)));

//LOCAL
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "cookie";
    options.DefaultChallengeScheme = "oidc";
    options.DefaultSignOutScheme = "oidc";
})
.AddCookie("cookie", options =>
{
    options.Cookie.Name = "__Host-bff";
    options.Cookie.SameSite = SameSiteMode.Strict;
})
.AddOpenIdConnect("oidc", options =>
{
    options.Authority = "https://localhost:5001";
    options.ClientId = "interactive.bff";
    options.ClientSecret = "Imnzr4HWbjnk0GRy7wRxKw==";
    options.ResponseType = "code";
    options.ResponseMode = "query";

    options.GetClaimsFromUserInfoEndpoint = true;
    options.MapInboundClaims = false;
    options.SaveTokens = true;

    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("api");
    options.Scope.Add("offline_access");
    options.Scope.Add("roles");
    //options.Scope.Add("client.scope");
    options.ClaimActions.MapUniqueJsonKey("role", "role");
    options.TokenValidationParameters = new()
    {
        NameClaimType = "name",
        RoleClaimType = "role"
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseBff();
app.UseAuthorization();
app.MapBffManagementEndpoints();

app.MapControllers()
    .RequireAuthorization()
    .AsBffApiEndpoint();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();
