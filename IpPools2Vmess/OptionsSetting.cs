using System.Security.Principal;
using System.Text.Json.Serialization;

namespace IpPools2Vmess;

public sealed record OptionsSetting
{
    public V2rayConfig? V2ray { get; set; }
    public PoolConfig? Pool { get; set; }

    [JsonConstructor]
    public OptionsSetting()
    {
    }
}

public sealed record V2rayConfig
{
    public string? TemplatePath { get; set; }
    public string? OutputPath { get; set; }

    [JsonConstructor]
    public V2rayConfig()
    {
    }
}

public sealed record PoolConfig
{
    public string? ProxySource { get; set; }
    public EProxyType ProxyType { get; set; }
    public int MaxProxyCount { get; set; } = 10;

    [JsonConstructor]
    public PoolConfig()
    {
    }
}

public enum EProxyType
{
    Http,
    Https,
    Socks
}