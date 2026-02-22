using Microsoft.AspNetCore.Http;
using System.Net;

namespace NetMVP.Infrastructure.Helpers;

/// <summary>
/// IP 地址工具类
/// </summary>
public static class IpHelper
{
    /// <summary>
    /// 获取客户端 IP 地址
    /// </summary>
    public static string GetClientIp(HttpContext context)
    {
        var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(ip))
        {
            var ips = ip.Split(',');
            if (ips.Length > 0)
                return ips[0].Trim();
        }

        ip = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(ip))
            return ip;

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    /// <summary>
    /// 判断是否为内网 IP
    /// </summary>
    public static bool IsInternalIp(string ip)
    {
        if (string.IsNullOrEmpty(ip) || ip == "unknown")
            return false;

        if (!IPAddress.TryParse(ip, out var address))
            return false;

        var bytes = address.GetAddressBytes();
        
        // 127.0.0.0/8
        if (bytes[0] == 127)
            return true;

        // 10.0.0.0/8
        if (bytes[0] == 10)
            return true;

        // 172.16.0.0/12
        if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31)
            return true;

        // 192.168.0.0/16
        if (bytes[0] == 192 && bytes[1] == 168)
            return true;

        return false;
    }

    /// <summary>
    /// 获取 IP 位置信息（简化版，实际应调用第三方 API）
    /// </summary>
    public static string GetIpLocation(string ip)
    {
        if (IsInternalIp(ip))
            return "内网IP";

        // 实际项目中应调用第三方 IP 定位服务
        return "未知";
    }
}
