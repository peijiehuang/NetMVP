using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetMVP.Domain.Entities;

namespace NetMVP.Infrastructure.Persistence.Configurations;

/// <summary>
/// 角色部门关联配置
/// </summary>
public class SysRoleDeptConfiguration : IEntityTypeConfiguration<SysRoleDept>
{
    public void Configure(EntityTypeBuilder<SysRoleDept> builder)
    {
        builder.ToTable("sys_role_dept");

        builder.HasKey(e => new { e.RoleId, e.DeptId });

        builder.Property(e => e.RoleId).HasColumnName("role_id");
        builder.Property(e => e.DeptId).HasColumnName("dept_id");
    }
}
