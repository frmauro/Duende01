using Microsoft.AspNetCore.Authentication;
using WebClient.Services.Interfaces;
using WebClient.Services;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();


builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
})
    .AddCookie("Cookies")
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = "https://localhost:5001";

        options.ClientId = "web";
        options.ClientSecret = "secret";
        options.ResponseType = "code";

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("productApi");
        options.Scope.Add("verification");
        //options.Scope.Add("offline_access");
        //options.Scope.Add("color");
        options.Scope.Add("cartApi");
        options.Scope.Add("couponApi");
        //options.ClaimActions.MapJsonKey("email_verified", "email_verified");
        //options.ClaimActions.MapUniqueJsonKey("favorite_color", "favorite_color");

        options.MapInboundClaims = false; // Don't rename claim types

        options.GetClaimsFromUserInfoEndpoint = true;
        options.SaveTokens = true;
    });

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICouponService, CouponService>();

builder.Services.AddUserAccessTokenHttpClient("apiClient", configureClient: client =>
{
    client.BaseAddress = new Uri("https://localhost:6001");
});

builder.Services.AddUserAccessTokenHttpClient("cartApi", configureClient: client =>
{
    client.BaseAddress = new Uri("https://localhost:7229");
});

builder.Services.AddUserAccessTokenHttpClient("couponApi", configureClient: client =>
{
    client.BaseAddress = new Uri("https://localhost:4450");
});


builder.Services.AddOpenIdConnectAccessTokenManagement();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages().RequireAuthorization();

app.Run();
