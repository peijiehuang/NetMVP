using AutoMapper;
using NetMVP.Application.Common.Utils;
using NetMVP.Application.DTOs.Config;
using NetMVP.Application.DTOs.Dept;
using NetMVP.Application.DTOs.Dict;
using NetMVP.Application.DTOs.Gen;
using NetMVP.Application.DTOs.LoginInfo;
using NetMVP.Application.DTOs.Menu;
using NetMVP.Application.DTOs.Notice;
using NetMVP.Application.DTOs.OperLog;
using NetMVP.Domain.Constants;
using NetMVP.Application.DTOs.Post;
using NetMVP.Application.DTOs.Profile;
using NetMVP.Application.DTOs.Role;
using NetMVP.Application.DTOs.User;
using NetMVP.Domain.Entities;
using System.Text;

namespace NetMVP.Application.Mappings;

/// <summary>
/// AutoMapper 映射配置
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // 用户映射
        CreateMap<SysUser, UserDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailValue ?? string.Empty))
            .ForMember(dest => dest.Phonenumber, opt => opt.MapFrom(src => src.PhoneNumberValue ?? string.Empty))
            .ForMember(dest => dest.Sex, opt => opt.MapFrom(src => src.Sex))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Dept, opt => opt.Ignore());

        // 角色映射
        CreateMap<SysRole, RoleDto>()
            .ForMember(dest => dest.DataScope, opt => opt.MapFrom(src => src.DataScope))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.MenuIds, opt => opt.Ignore())
            .ForMember(dest => dest.DeptIds, opt => opt.Ignore());

        // 菜单映射
        CreateMap<SysMenu, MenuDto>()
            .ForMember(dest => dest.IsFrame, opt => opt.MapFrom(src => src.IsFrame.ToString()))
            .ForMember(dest => dest.IsCache, opt => opt.MapFrom(src => src.IsCache.ToString()))
            .ForMember(dest => dest.MenuType, opt => opt.MapFrom(src => 
                src.MenuType == UserConstants.TYPE_DIR ? "M" : 
                src.MenuType == UserConstants.TYPE_MENU ? "C" : "F"))
            .ForMember(dest => dest.Visible, opt => opt.MapFrom(src => src.Visible))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Children, opt => opt.Ignore());

        // 部门映射
        CreateMap<SysDept, DeptDto>()
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneValue))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailValue))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Children, opt => opt.Ignore());

        // 岗位映射
        CreateMap<SysPost, PostDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
        CreateMap<SysPost, ExportPostDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

        // 字典类型映射
        CreateMap<SysDictType, DictTypeDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

        // 字典数据映射
        CreateMap<SysDictData, DictDataDto>()
            .ForMember(dest => dest.IsDefault, opt => opt.MapFrom(src => src.IsDefault))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

        // 参数配置映射
        CreateMap<SysConfig, ConfigDto>()
            .ForMember(dest => dest.ConfigType, opt => opt.MapFrom(src => src.ConfigType.ToString()));
        CreateMap<CreateConfigDto, SysConfig>()
            .ForMember(dest => dest.ConfigType, opt => opt.Ignore());
        CreateMap<UpdateConfigDto, SysConfig>()
            .ForMember(dest => dest.ConfigType, opt => opt.Ignore());

        // 通知公告映射
        CreateMap<SysNotice, NoticeDto>()
            .ForMember(dest => dest.NoticeType, opt => opt.MapFrom(src => src.NoticeType))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.NoticeContent, opt => opt.MapFrom(src => 
                src.NoticeContent != null ? Encoding.UTF8.GetString(src.NoticeContent) : null));

        // 操作日志映射
        CreateMap<SysOperLog, OperLogDto>()
            .ForMember(dest => dest.OperIp, opt => opt.MapFrom(src => src.OperIpValue));

        // 登录日志映射
        CreateMap<SysLoginInfo, LoginInfoDto>()
            .ForMember(dest => dest.IpAddr, opt => opt.MapFrom(src => src.IpAddrValue));

        // 代码生成表映射
        CreateMap<GenTable, GenTableDto>();
        CreateMap<GenTableColumn, GenTableColumnDto>();

        // 个人中心映射
        CreateMap<SysUser, ProfileDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailValue))
            .ForMember(dest => dest.Phonenumber, opt => opt.MapFrom(src => src.PhoneNumberValue))
            .ForMember(dest => dest.Sex, opt => opt.MapFrom(src => src.Sex))
            .ForMember(dest => dest.DeptName, opt => opt.Ignore())
            .ForMember(dest => dest.PostIds, opt => opt.Ignore())
            .ForMember(dest => dest.RoleIds, opt => opt.Ignore());
    }
}


