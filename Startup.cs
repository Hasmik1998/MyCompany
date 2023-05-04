using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Owin;
using MyCompany.Data;
using MyCompany.Utilities;
using Microsoft.Owin;
using MyCompany.Repository;

namespace MyCompany
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
            });
            services.AddDbContext<CompanyDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<CompanyDbContext>()
                .AddDefaultTokenProviders();

            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<CompanyDbContext>();
            services.AddSwaggerGen();

            services.AddScoped<IRepository, CompanyRepository>();

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddAuthentication().AddCookie(options => { options.LoginPath = "/Pages/Login"; });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("OnlyAdminAccess", policy => policy.RequireRole("Admin"));
            });
            services.AddSession();
            services.AddMvc()
                //.AddRazorPagesOptions(options =>
                //{
                //    options.Conventions.AuthorizePage("/Pages/Login", "OnlyAdminAccess");
                //})
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = true;
            });
            app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseSession();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
            CreateRoles(serviceProvider).Wait();
        }
        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            //initializing custom roles   
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            string[] roleNames = { "Admin", "User", "SimpleUser"};
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database: Question 1  
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            IdentityUser admin = await UserManager.FindByEmailAsync(Constants.ADMIN_MAIL);
           
            if (admin == null)
            {
                admin = new IdentityUser()
                {
                    UserName = Constants.ADMIN_MAIL,
                    Email = Constants.ADMIN_MAIL,
                };
                await UserManager.CreateAsync(admin, Constants.ADMIN_PASSWORD);
                
            }
            
            //IdentityUser user1 = new IdentityUser()
            //{
            //    UserName = "a@a.com",
            //    Email = "a@a.com",
            //};
            //await UserManager.CreateAsync(user1, "Aa,1234");

            await UserManager.AddToRoleAsync(admin, "Admin");
            //await UserManager.AddToRoleAsync(user1, "User");
        }

    }
}
