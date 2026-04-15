using manage_my_hairsaloon.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register mock repositories
builder.Services.AddSingleton<MockUserRepository>();
builder.Services.AddSingleton<MockHairSalonRepository>();
builder.Services.AddSingleton<MockStaffRepository>();
builder.Services.AddSingleton<MockServiceRepository>();
builder.Services.AddSingleton<MockReservationRepository>();
builder.Services.AddSingleton<MockReviewRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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
