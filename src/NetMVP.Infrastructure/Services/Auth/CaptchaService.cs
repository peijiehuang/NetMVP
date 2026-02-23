using NetMVP.Domain.Constants;
using NetMVP.Domain.Interfaces;
using SkiaSharp;

namespace NetMVP.Infrastructure.Services.Auth;

/// <summary>
/// 验证码服务实现
/// </summary>
public class CaptchaService : ICaptchaService
{
    private readonly ICacheService _cacheService;
    private const int Width = 120;
    private const int Height = 40;
    private const int CodeLength = 4;
    private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(5);

    public CaptchaService(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    /// <inheritdoc/>
    public async Task<(string uuid, string image)> GenerateCaptchaAsync(CancellationToken cancellationToken = default)
    {
        // 生成随机验证码
        var code = GenerateRandomCode();
        var uuid = Guid.NewGuid().ToString();

        // 缓存验证码
        await _cacheService.SetAsync($"{CacheConstants.CAPTCHA_CODE_KEY}{uuid}", code, CacheExpiry, cancellationToken);

        // 生成验证码图片
        var imageBase64 = GenerateCaptchaImage(code);

        return (uuid, imageBase64);
    }

    /// <inheritdoc/>
    public async Task<bool> ValidateCaptchaAsync(string uuid, string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(uuid) || string.IsNullOrWhiteSpace(code))
            return false;

        var cacheKey = $"{CacheConstants.CAPTCHA_CODE_KEY}{uuid}";
        var cachedCode = await _cacheService.GetAsync<string>(cacheKey, cancellationToken);

        if (string.IsNullOrWhiteSpace(cachedCode))
            return false;

        // 验证后删除验证码
        await _cacheService.RemoveAsync(cacheKey, cancellationToken);

        return cachedCode.Equals(code, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 生成随机验证码
    /// </summary>
    private static string GenerateRandomCode()
    {
        const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, CodeLength)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    /// <summary>
    /// 生成验证码图片
    /// </summary>
    private static string GenerateCaptchaImage(string code)
    {
        var random = new Random();
        
        using var surface = SKSurface.Create(new SKImageInfo(Width, Height));
        var canvas = surface.Canvas;
        
        // 背景色
        canvas.Clear(SKColors.White);
        
        // 绘制干扰线
        using var linePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1,
            IsAntialias = true
        };
        
        for (int i = 0; i < 5; i++)
        {
            linePaint.Color = GetRandomColor(random);
            var x1 = random.Next(Width);
            var y1 = random.Next(Height);
            var x2 = random.Next(Width);
            var y2 = random.Next(Height);
            canvas.DrawLine(x1, y1, x2, y2, linePaint);
        }
        
        // 绘制验证码文字
        using var textPaint = new SKPaint
        {
            Color = GetRandomColor(random),
            TextSize = 28,
            IsAntialias = true,
            FakeBoldText = true,
            TextAlign = SKTextAlign.Left
        };
        
        // 计算文字位置
        var textWidth = textPaint.MeasureText(code);
        var x = (Width - textWidth) / 2;
        var y = (Height + textPaint.TextSize) / 2;
        
        canvas.DrawText(code, x, y, textPaint);
        
        // 绘制干扰点
        using var pointPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill
        };
        
        for (int i = 0; i < 100; i++)
        {
            pointPaint.Color = GetRandomColor(random);
            var px = random.Next(Width);
            var py = random.Next(Height);
            canvas.DrawCircle(px, py, 1, pointPaint);
        }
        
        // 转换为 Base64
        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        var imageBytes = data.ToArray();
        return $"{Convert.ToBase64String(imageBytes)}";
    }

    /// <summary>
    /// 获取随机颜色
    /// </summary>
    private static SKColor GetRandomColor(Random random)
    {
        return new SKColor(
            (byte)random.Next(256),
            (byte)random.Next(256),
            (byte)random.Next(256)
        );
    }
}
