using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ShoppingApi.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingApi.Controllers
{
    public class AdminController : ControllerBase
    {
        private readonly IOptions<ConfigurationForMapper> _options;
        private readonly IServiceCollection services;
        IConfigurationRoot _configRoot;

        public AdminController(IConfiguration config, IOptions<ConfigurationForMapper> options/*, IServiceCollection services*/)
        {
           
            _configRoot = (IConfigurationRoot)  config;
            _options = options;
            //this.services = services;
        }

        [HttpGet("admin/config")]
        public ActionResult GetConfig()
        {
           
            return Ok(_configRoot.Providers.Select(p => p.ToString()).ToList());
        } 

        [HttpGet("admin/markup")]
        public ActionResult GetMarkup()
        {
            
            return Ok($"{_options.Value.greeting}:  {_options.Value.markUp:P}");
        }

        [HttpGet("admin/services")]
        public ActionResult GetServices()
        {
            return Ok(services.Select(s => new { Interface = s.ServiceType.Name, Implementation = s.ImplementationType.Name }));
        }
    }
}
