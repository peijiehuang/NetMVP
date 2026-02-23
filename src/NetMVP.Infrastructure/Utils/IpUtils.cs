using Microsoft.AspNetCore.Http;

namespace NetMVP.Infrastructure.Utils;

/// <summary>
/// IP地址工具类
/// </summary>
public static class IpUtils
{
    /// <summary>
    /// 获取客户端IP地址
    /// </summary>
    public static string GetIpAddress(HttpContext? httpContext)
    {
        if (httpContext == null)
        {
            return "unknown";
        }

        var request = httpContext.Request;
        
        // 尝试从各种代理头获取真实IP
        var ip = request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (string.IsNullOrEmpty(ip) || "unknown".Equals(ip, StringComparison.OrdinalIgnoreCase))
        {
            ip = request.Headers["Proxy-Client-IP"].FirstOrDefault();
        }
        if (string.IsNullOrEmpty(ip) || "unknown".Equals(ip, StringComparison.OrdinalIgnoreCase))
        {
            ip = request.Headers["WL-Proxy-Client-IP"].FirstOrDefault();
        }
        if (string.IsNullOrEmpty(ip) || "unknown".Equals(ip, StringComparison.OrdinalIgnoreCase))
        {
            ip = request.Headers["X-Real-IP"].FirstOrDefault();
        }
        if (string.IsNullOrEmpty(ip) || "unknown".Equals(ip, StringComparison.OrdinalIgnoreCase))
        {
            ip = httpContext.Connection.RemoteIpAddress?.ToString();
        }

        // 处理IPv6本地地址
        if ("::1".Equals(ip) || "0:0:0:0:0:0:0:1".Equals(ip))
        {
            ip = "127.0.0.1";
        }

        // 处理多级反向代理的情况
        if (!string.IsNullOrEmpty(ip) && ip.Contains(','))
        {
            var ips = ip.Split(',');
            foreach (var subIp in ips)
            {
                var trimmedIp = subIp.Trim();
                if (!string.IsNullOrEmpty(trimmedIp) && !"unknown".Equals(trimmedIp, StringComparison.OrdinalIgnoreCase))
                {
                    ip = trimmedIp;
                    break;
                }
            }
        }

        return string.IsNullOrEmpty(ip) ? "127.0.0.1" : ip;
    }

    /// <summary>
    /// 判断是否为内网IP
    /// </summary>
    public static bool IsInternalIp(string ip)
    {
        if (string.IsNullOrEmpty(ip) || "127.0.0.1".Equals(ip) || "localhost".Equals(ip, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var parts = ip.Split('.');
        if (parts.Length != 4)
        {
            return false;
        }

        if (!byte.TryParse(parts[0], out var b0) || !byte.TryParse(parts[1], out var b1))
        {
            return false;
        }

        // 10.x.x.x/8
        if (b0 == 10)
        {
            return true;
        }

        // 172.16.x.x/12
        if (b0 == 172 && b1 >= 16 && b1 <= 31)
        {
            return true;
        }

        // 192.168.x.x/16
        if (b0 == 192 && b1 == 168)
        {
            return true;
        }

        return false;
    }
}
