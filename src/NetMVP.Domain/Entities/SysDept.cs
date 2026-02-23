using NetMVP.Domain.Common;
using NetMVP.Domain.Constants;
using NetMVP.Domain.ValueObjects;

namespace NetMVP.Domain.Entities;

/// <summary>
/// 部门实体
/// </summary>
public class SysDept : BaseEntity
{
    /// <summary>
    /// 部门ID
    /// </summary>
    public long DeptId { get; set; }

    /// <summary>
    /// 父部门ID
    /// </summary>
    public long ParentId { get; set; }

    /// <summary>
    /// 祖级列表
    /// </summary>
    public string Ancestors { get; set; } = string.Empty;

    /// <summary>
    /// 部门名称
    /// </summary>
    public string DeptName { get; set; } = string.Empty;

    /// <summary>
    /// 显示顺序
    /// </summary>
    public int OrderNum { get; set; }

    /// <summary>
    /// 负责人
    /// </summary>
    public string? Leader { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    public string? PhoneValue { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? EmailValue { get; set; }

    /// <summary>
    /// 状态（0正常 1停用）
    /// </summary>
    public string Status { get; set; } = UserConstants.NORMAL;

    /// <summary>
    /// 删除标志（0存在 2删除）
    /// </summary>
    public string DelFlag { get; set; } = UserConstants.DEL_FLAG_EXIST;

    /// <summary>
    /// 子部门
    /// </summary>
    public List<SysDept> Children { get; set; } = new();

    /// <summary>
    /// 联系电话（值对象）
    /// </summary>
    public PhoneNumber? Phone => string.IsNullOrWhiteSpace(PhoneValue) ? null : PhoneNumber.Create(PhoneValue);

    /// <summary>
    /// 邮箱（值对象）
    /// </summary>
    public Email? Email => string.IsNullOrWhiteSpace(EmailValue) ? null : ValueObjects.Email.Create(EmailValue);
}
