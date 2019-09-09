using System;
using System.IO;
using System.Reflection;
using CrawlerService.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace CrawlerService {

	public class Startup {

		private const string ReactClientCorsOptions = "_react_client_";

		public Startup(IConfiguration configuration) {
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		/// <summary>
		/// This method gets called by the runtime. Use this method to add services to the container.
		/// </summary>
		public void ConfigureServices(IServiceCollection services) {
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
			services.AddCors(options => {
				options.AddPolicy(ReactClientCorsOptions,
				builder => {
					builder.WithOrigins("http://localhost:3000");
				});
			});
			// Add business dependencies.
			services.AddSingleton<IContentProvider, WebContentProvider>();
			services.AddSingleton<ICrawlerStrategy, FaultTolerantCrawler>();
			// services.AddSingleton<ICrawlerStrategy, DFSCrawler>();
			// Configure swagger middleware
			services.AddSwaggerGen(c => {
				c.SwaggerDoc("v1", new Info {
					Title = "SwaggerDemo",
					Version = "v1",
					Description = "Crawler service API",
					Contact = new Contact {
						Name = "Renier Marichal",
						Url = "https://github.com/rrmarichal"
					}
				});
				var xmlfile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlpath = Path.Combine(AppContext.BaseDirectory, xmlfile);
				c.IncludeXmlComments(xmlpath);
			});
		}

		/// <summary>
		/// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		/// </summary>
		public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
			if (env.IsDevelopment()) { }
			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
			}
			else {
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
				app.UseExceptionHandler("/Error");
			}
			app.UseCors(ReactClientCorsOptions);
			app.UseHttpsRedirection();
			app.UseMvc();
			app.UseSwagger();
			app.UseSwaggerUI(c => {
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "SwaggerDemo v1");
			});
		}

	}
	
}
