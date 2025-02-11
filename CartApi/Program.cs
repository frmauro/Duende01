using AutoMapper;
using CartApi.Model.Context;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.OpenApi.Models;
using CartApi.Config;
using CartApi.Repository;
using CartApi.Data.ValueObject;
using CartApi.Messages;
using Microsoft.AspNetCore.Http.HttpResults;
using CartApi.RabbitMQSender;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:5001";
        options.TokenValidationParameters.ValidateAudience = false;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "cartApi");
    });
});

//builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CartApi" });
    c.EnableAnnotations();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Enter 'Bearer' [space] and your token!",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string> ()
        }
    });
});

var connection = builder.Configuration["MySqlConnection:MySqlConnectionString"];
builder.Services.AddDbContext<MySqlContext>(options => options.UseMySql(connection, new MySqlServerVersion(new Version(5, 7))));

IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddSingleton<IRabbitMQSender, RabbitMQSender>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddUserAccessTokenHttpClient("couponApi", configureClient: client =>
{
    client.BaseAddress = new Uri("https://localhost:4450");
});


builder.Services.AddOpenIdConnectAccessTokenManagement();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


//app.MapGet("identity", (ClaimsPrincipal user) => user.Claims.Select(c => new { c.Type, c.Value }))
//    .RequireAuthorization("ApiScope");


app.MapGet("find-cart/{id}", async (ICartRepository repository, string id) =>
{
    var cartVo = await repository.FindCartByUserId(id);
    if (cartVo == null) return Results.NotFound();
    return Results.Ok(cartVo);
}).RequireAuthorization("ApiScope");

app.MapPost("add-cart", async (ICartRepository repository, CartVO vo) =>
{
    var cartVo = await repository.SaveOrUpdateCart(vo);
    if (cartVo == null) return Results.NotFound();
    return Results.Ok(cartVo);
}).RequireAuthorization("ApiScope");


app.MapPut("update-cart", async (ICartRepository repository, CartVO vo) =>
{
    var cartVo = await repository.SaveOrUpdateCart(vo);
    if (cartVo == null) return Results.NotFound();
    return Results.Ok(cartVo);
}).RequireAuthorization("ApiScope");

app.MapDelete("remove-cart/{id}", async (ICartRepository repository, int id) =>
{
    var status = await repository.RemoveFromCart(id);
    if (!status) return Results.NotFound();
    return Results.Ok(status);
}).RequireAuthorization("ApiScope");

app.MapPost("apply-coupon", async (ICartRepository repository, CartVO vo) =>
{
    var isApplied = await repository.ApplyCoupon(vo.CartHeader.UserId, vo.CartHeader.CouponCode);
    if (!isApplied) return Results.NotFound();
    return Results.Ok(isApplied);
}).RequireAuthorization("ApiScope");

app.MapDelete("remove-coupon/{userId}", async (ICartRepository repository, string userId) =>
{
    var isRemoved = await repository.RemoveCoupon(userId);
    if (!isRemoved) return Results.NotFound();
    return Results.Ok(isRemoved);
}).RequireAuthorization("ApiScope");


app.MapPost("checkout", async (ICartRepository repository, ICouponRepository couponRepository, IRabbitMQSender rabbitMQSender, CheckoutHeaderVO vo) =>
{
    if (vo?.UserId == null) return Results.BadRequest();
    var cart = await repository.FindCartByUserId(vo.UserId!);
    if (cart == null) return Results.NotFound();

    if (!string.IsNullOrEmpty(vo.CouponCode))
    {
        var coupon = await couponRepository.GetCoupon(vo.CouponCode);

        if (vo.DiscountTotal != coupon.DiscountAmount)
        {
            return Results.StatusCode(412);
        }
    }

    vo.CartDetails = cart.CartDetails;

    await rabbitMQSender.SendMessageAsync(vo, "checkoutqueue");

    return Results.Ok(vo);
}).RequireAuthorization("ApiScope");


app.Run();

