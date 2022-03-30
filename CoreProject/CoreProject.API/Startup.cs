using AutoMapper;
using CoreProject.BusinessLayer.Infrastructure;
using CoreProject.BusinessLayer.Services;
using CoreProject.DataLayer.CacheService;
using CoreProject.DataLayer.CacheService.Redis;
using CoreProject.DataLayer.DataContext;
using CoreProject.DataLayer.Infrastructure;
using CoreProject.DataLayer.Repository;
using CoreProject.Entities.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Text.Json.Serialization;

namespace CoreProject.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            #region json
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            }); 
            #endregion

            #region Datalayerda ki DatabaseContexte yönlendirdim CodeFirst ile migration için 
            services.AddDbContext<DatabaseContext>(x => x.UseSqlServer(Configuration.GetConnectionString("Baglanti"), x => x.MigrationsAssembly("CoreProject.DataLayer")));
            #endregion


            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


            services.AddCors(o => o.AddPolicy("myclients", builder =>
            {
                builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
            }));

            services.AddControllers(options =>
            {
                options.ReturnHttpNotAcceptable = true;
            });

            #region swagger
            services.AddSwaggerGen(c =>
                {
                    c.IncludeXmlComments(string.Format(@"{0}\CoreProject.xml", System.AppDomain.CurrentDomain.BaseDirectory));
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "CoreProject - Dapper - WebApi",
                    });
                });
            #endregion

            #region redis
            services.AddSingleton<RedisServer>();
            services.AddSingleton<ICacheService, RedisCacheService>();
            #endregion

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICustomerService ,CustomerService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddSingleton(typeof( IBaseService<>), typeof( BaseService<>));
            services.AddSingleton(typeof(IBaseRepository<>), typeof(BaseRepository<>));




            // services.AddScoped<ICustomerService>(provider => new CustomerService( provider.GetService<ICacheService>(),provider.GetService<IUnitOfWork>() ));

            //services.AddSingleton<ICustomerService, CustomerService>(sp =>
            //{
            //    var redisService = sp.GetRequiredService<ICacheService>();
            //    var unitofwork = sp.GetRequiredService<IUnitOfWork>();
            //    return new CustomerService( redisService,unitofwork);
            //});

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("myclients");
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CoreProject");
            });





        }
    }
}


