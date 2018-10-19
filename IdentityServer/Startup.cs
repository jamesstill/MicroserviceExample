using System.Security.Cryptography;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        

        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _config;
        private readonly ILoggerFactory _loggerFactory;
        public KeyParameters _keyParameters;

        public Startup(IHostingEnvironment env, IConfiguration config, ILoggerFactory loggerFactory)
        {
            _env = env;
            _config = config;
            _loggerFactory = loggerFactory;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // configure identity server with in-memory stores, keys, clients and scopes
            services
                .AddIdentityServer()
                .AddSigningCredential(CreateSigningCredentials())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();
        }

        private SigningCredentials CreateSigningCredentials()
        {
            return new SigningCredentials(GetSecurityKey(), SecurityAlgorithms.RsaSha256Signature);
        }

        private SecurityKey GetSecurityKey()
        {
            return new RsaSecurityKey(GetRSAParameters());
        }

        private RSAParameters GetRSAParameters()
        {
            /*
                Secret Manager is used for this project. But I did not need to call 
                AddUserSecrets<Startup>() above.
            
                In ASP.NET Core 2.0 or later, the user secrets configuration source is automatically 
                added in development mode when the project calls CreateDefaultBuilder to initialize 
                a new instance of the host with preconfigured defaults. CreateDefaultBuilder calls 
                AddUserSecrets when the EnvironmentName is Development.

                In production the secrets will not be deployed so the app settings would have to be
                set at deploy time.
             */
            var keyParameters = Configuration.GetSection("KeyParameters").Get<KeyParameters>();
            return new RSAParameters
            {
                D = Base64UrlEncoder.DecodeBytes(keyParameters.D),
                DP = Base64UrlEncoder.DecodeBytes(keyParameters.DP),
                DQ = Base64UrlEncoder.DecodeBytes(keyParameters.DQ),
                Exponent = Base64UrlEncoder.DecodeBytes(keyParameters.Exponent),
                InverseQ = Base64UrlEncoder.DecodeBytes(keyParameters.InverseQ),
                Modulus = Base64UrlEncoder.DecodeBytes(keyParameters.Modulus),
                P = Base64UrlEncoder.DecodeBytes(keyParameters.P),
                Q = Base64UrlEncoder.DecodeBytes(keyParameters.Q)
            };
        }
    }
}
