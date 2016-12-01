using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using System.Net;
using System.Web.Http;

[assembly: OwinStartupAttribute(typeof(HttpClientWriteStreamFailureOn401.Startup))]

namespace HttpClientWriteStreamFailureOn401
{
    /// <summary>
    /// Start-up class for the self hosting OWIN context.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Entry point for the self hosting OWIN start method. This starts the pipeline.
        /// </summary>
        /// <param name="appBuilder">Used to assign the <see cref="HttpConfiguration"/> necessary for the WebApi.</param>
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();

             // Web API routes
            config.MapHttpAttributeRoutes();

             // Serailizers
            JsonSerializerSettings jsonSetting = new JsonSerializerSettings();

            // forces enums as strings instead of ints.
            jsonSetting.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            jsonSetting.Formatting = Formatting.Indented;
            jsonSetting.ContractResolver = new CamelCasePropertyNamesContractResolver();

            config.Formatters.JsonFormatter.SerializerSettings = jsonSetting;

            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            if (appBuilder.Properties.ContainsKey("System.Net.HttpListener"))
            {
                // used for self hosting
                HttpListener listener = (HttpListener)appBuilder.Properties["System.Net.HttpListener"];
                listener.AuthenticationSchemes = AuthenticationSchemes.IntegratedWindowsAuthentication;
            }

            appBuilder.UseWebApi(config);
        }
    }
}
