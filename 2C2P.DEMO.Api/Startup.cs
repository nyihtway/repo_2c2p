using _2C2P.DEMO.Api.AutofacModules;
using _2C2P.DEMO.Infrastructure.AutoMapper;
using _2C2P.DEMO.Infrastructure.Interfaces;
using _2C2P.DEMO.Infrastructure.Repositories;
using Autofac;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace _2C2P.DEMO.Api
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks()
               .AddMongoDb(MongoClientSettings.FromUrl(new MongoUrl(Configuration.GetSection("MongoDBConnection:ConnectionString").Value)));

            services.AddControllers()
                 .AddJsonOptions(options => options.JsonSerializerOptions.IgnoreNullValues = true);
            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddHttpClient()
                .AddHttpContextAccessor();

            services.AddValidatorsFromAssembly(typeof(Startup).Assembly);

            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Version = "v1",
                    Title = "TRANSACTION API",
                    Description = "2C2P Transaction Demo"
                });
            });

            services.AddMediatR(typeof(Startup).Assembly);

            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.AllowNullCollections = true;
                cfg.AddProfile<MappingProfile>();
            });

            services.AddSingleton(mappingConfig.CreateMapper());

        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var clientSettings = MongoClientSettings.FromUrl(new MongoUrl(Configuration.GetSection("MongoDBConnection:ConnectionString").Value));
            
            builder.RegisterModule(new InfrastructureModule(new MongoClient(clientSettings),
                       Configuration.GetSection("MongoDBConnection:Database").Value));
            builder.RegisterModule(new MediatorModule());

            builder.RegisterModule(new BsonModule());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseSwagger();

            app.UseSwaggerUI(
            options =>
            {
                options.SwaggerEndpoint($"/swagger/v1/swagger.json", "TRANSACTION API");

            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
