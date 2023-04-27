var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSession();
builder.Services.AddDbContext<MyHostel_Admin.Models.MyHostelContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseSession();

app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
