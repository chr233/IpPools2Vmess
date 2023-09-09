using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using SteamAuthenticatorOnline.Manager.Services;

namespace IpPools2Vmess
{
    internal class CoreService
    {
        private readonly OptionsSetting _optionsSetting;
        private readonly HttpService _httpService;

        public CoreService(IOptions<OptionsSetting> options, HttpService httpService)
        {
            _optionsSetting = options.Value;
            _httpService = httpService;
        }

        public async Task Start()
        {
            var pool = _optionsSetting.Pool ?? throw new ValidationException("Pool 配置不能为 null");
            var source = pool.ProxySource ?? throw new ValidationException("ProxySource 不能为 null");

            var v2ray = _optionsSetting.V2ray ?? throw new ValidationException("V2ray 配置不能为 null");
            var templatePath = v2ray.TemplatePath ?? throw new ValidationException("TemplatePath 配置不能为 null");
            var outputPath = v2ray.OutputPath ?? throw new ValidationException("OutputPath 配置不能为 null");

            string response;
            if (File.Exists(source))
            {
                using var stream = File.OpenText(source);
                response = await stream.ReadToEndAsync();
            }
            else
            {
                try
                {
                    var uri = new Uri(source);
                    response = await _httpService.GetProxyFromUrl(uri);
                }
                catch (Exception)
                {
                    throw new ValidationException("ProxySource 需要为本地文件路径, 或者代理网址");
                }
            }

            var lines = response.Split('\n');

            if (!lines.Any() || !lines[0].Contains(':'))
            {
                Console.WriteLine(response);
                throw new ValidationException("无效代理响应, 请检查 ProxySource");
            }

            var max = pool.MaxProxyCount;
            var count = 0;

            var sb = new StringBuilder();
            foreach (var line in lines)
            {
                if (!Uri.TryCreate($"http://{line}", UriKind.Absolute, out var uri))
                {
                    Console.WriteLine("{0} 无法识别为 Url, 跳过", line);
                    continue;
                }

                if (sb.Length > 0)
                {
                    sb.AppendLine(",");
                }
                sb.Append('{');
                sb.Append(string.Format(Static.Config, uri.Host, uri.Port));
                sb.Append('}');

                if (++count >= max)
                {
                    break;
                }
            }

            if (!File.Exists(templatePath))
            {
                throw new ValidationException("TemplatePath 路径不存在");
            }

            using var fs = File.OpenText(templatePath);
            var template = await fs.ReadToEndAsync();
            fs.Close();

            var proxyType = pool.ProxyType.ToString().ToLower();

            var outputDir = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(outputDir) && !string.IsNullOrEmpty(outputDir))
            {
                Directory.CreateDirectory(outputDir);
                Console.WriteLine("输出路径不存在, 创建文件夹成功");
            }

            template = template.Replace("$$PROTOCOL$$", proxyType).Replace("$$SERVICES$$", sb.ToString());

            var exists = File.Exists(outputPath);
            using var fs2 = File.Open(outputPath, exists ? FileMode.Truncate : FileMode.Create);
            using var sw = new StreamWriter(fs2);
            sw.Write(template);
            sw.Close();
            fs.Close();

            Console.WriteLine("输出配置成功共 {0} 个 IP", count);
        }
    }
}
