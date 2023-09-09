using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using SteamAuthenticatorOnline.Manager.Services;

namespace IpPools2Vmess
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("config.json", optional: false, reloadOnChange: true)
                .Build();

            services.Configure<OptionsSetting>(configuration);

            services.AddSingleton<CoreService>();
            services.AddSingleton<HttpService>();

            using var serviceProvider = services.BuildServiceProvider();
            var core = serviceProvider.GetRequiredService<CoreService>();

            try
            {
                await core.Start();
                Console.WriteLine("执行成功");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("执行失败");
                Console.ReadKey();
            }
        }
    }
}