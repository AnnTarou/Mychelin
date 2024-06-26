﻿using Microsoft.EntityFrameworkCore;
using Mychelin.Data;
namespace Mychelin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<MychelinContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("MychelinContext") ?? throw new InvalidOperationException("Connection string 'MychelinContext' not found.")));

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // セッションの追加
            builder.Services.AddSession(options=>options.IdleTimeout=TimeSpan.FromHours(1));

            // IHttpContextAccessorをサービスとして登録
            builder.Services.AddHttpContextAccessor();

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

            // セッションの追加
            app.UseSession();

            app.Run();
        }
    }
}
