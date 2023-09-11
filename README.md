# IpPools2Vmess

转换动态代理为V2ray配置

`config.json`

```json
{
  "V2ray": {
    "TemplatePath": "template.json", //v2ray配置模板
    "OutputPath": "op/config.json" //v2ray配置输出路径
  },
  "Pool": {
    "ProxySource": "TXT格式的API，或者本地文件路径",
    "ProxyType": "Socks", //协议类型 http/https/socks
    "MaxProxyCount": 10 //一次最多取多少个代理
  }
}
```