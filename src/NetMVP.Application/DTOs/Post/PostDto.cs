namespace NetMVP.Application.DTOs.Post;

/// <summary>
/// 岗位 DTO
/// </summary>
public class PostDto
{
    public long PostId { get; set; }
    public string PostCode { get; set; } = string.Empty;
    public string PostName { get; set; } = string.Empty;
    public int PostSort { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Remark { get; set; }
    public DateTime? CreateTime { get; set; }
}
