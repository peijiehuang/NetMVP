using NetMVP.Domain.Common;
using NetMVP.Domain.Enums;
using NetMVP.Domain.ValueObjects;

namespace NetMVP.Domain.Entities;

/// <summary>
/// 用户实体
/// </summary>
public class SysUser : BaseEntity
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 部门ID
    /// </summary>
    public long? DeptId { get; set; }

    /// <summary>
    /// 用户账号
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 用户昵称
    /// </summary>
    public string NickName { get; set; } = string.Empty;

    /// <summary>
    /// 用户类型（00系统用户）
    /// </summary>
    public string UserType { get; set; } = "00";

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? EmailValue { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string? PhoneNumberValue { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    public Gender Gender { get; set; } = Gender.Unknown;

    /// <summary>
    /// 头像
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 状态
    /// </summary>
    public UserStatus Status { get; set; } = UserStatus.Normal;

    /// <summary>
    /// 删除标志
    /// </summary>
    public DelFlag DelFlag { get; set; } = DelFlag.Exist;

    /// <summary>
    /// 最后登录IP
    /// </summary>
    public string? LoginIpValue { get; set; }

    /// <summary>
    /// 最后登录时间
    /// </summary>
    public DateTime? LoginDate { get; set; }

    /// <summary>
    /// 密码最后更新时间
    /// </summary>
    public DateTime? PwdUpdateDate { get; set; }

    /// <summary>
    /// 用户角色关联
    /// </summary>
    public List<SysUserRole> UserRoles { get; set; } = new();

    /// <summary>
    /// 用户岗位关联
    /// </summary>
    public List<SysUserPost> UserPosts { get; set; } = new();

    /// <summary>
    /// 邮箱（值对象）
    /// </summary>
    public Email? Email => string.IsNullOrWhiteSpace(EmailValue) ? null : ValueObjects.Email.Create(EmailValue);

    /// <summary>
    /// 手机号（值对象）
    /// </summary>
    public PhoneNumber? PhoneNumber => string.IsNullOrWhiteSpace(PhoneNumberValue) ? null : ValueObjects.PhoneNumber.Create(PhoneNumberValue);

    /// <summary>
    /// 登录IP（值对象）
    /// </summary>
    public IpAddress? LoginIp => string.IsNullOrWhiteSpace(LoginIpValue) ? null : IpAddress.Create(LoginIpValue);

    /// <summary>
    /// 设置密码
    /// </summary>
    public void SetPassword(string plainPassword)
    {
        var password = UserPassword.CreateFromPlainText(plainPassword);
        Password = password.HashedPassword;
        PwdUpdateDate = DateTime.Now;
    }

    /// <summary>
    /// 验证密码
    /// </summary>
    public bool VerifyPassword(string plainPassword)
    {
        var password = UserPassword.CreateFromHash(Password);
        return password.Verify(plainPassword);
    }

    /// <summary>
    /// 是否为管理员
    /// </summary>
    public bool IsAdmin()
    {
        return UserId == 1;
    }

    /// <summary>
    /// 更新登录信息
    /// </summary>
    public void UpdateLoginInfo(string ipAddress)
    {
        LoginIpValue = ipAddress;
        LoginDate = DateTime.Now;
    }
}
