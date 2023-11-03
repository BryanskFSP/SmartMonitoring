using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using SmartMonitoring.Server.Entities;
using SmartMonitoring.Shared.EditModels;
using SmartMonitoring.Shared.ViewModels;

namespace SmartMonitoring.Server;

public class Mappings: Profile
{
    public Mappings()
    {
        CreateMap<TelegramUserViewModel, TelegramUserEditModel>().ReverseMap();
        CreateMap<TelegramUserEditModel, TelegramUserEntity>();
        CreateMap<TelegramUserViewModel, TelegramUserEntity>();
        
        CreateMap<AdminViewModel, AdminEditModel>().ReverseMap();
        CreateMap<AdminEditModel, AdminEntity>();
        CreateMap<AdminViewModel, AdminEntity>();

        CreateMap<InviteViewModel, InviteEditModel>().ReverseMap();
        CreateMap<InviteEditModel, InviteEntity>();
        CreateMap<InviteViewModel, InviteEntity>();
        
        CreateMap<DataBaseViewModel, DataBaseEditModel>().ReverseMap();
        CreateMap<DataBaseEditModel, DataBaseEntity>();
        CreateMap<DataBaseViewModel, DataBaseEntity>();
        
        CreateMap<OrganizationViewModel, OrganizationEditModel>().ReverseMap();
        CreateMap<OrganizationEditModel, OrganizationEntity>();
        CreateMap<OrganizationViewModel, OrganizationEntity>();
        
        CreateMap<DataBaseViewModel, DataBaseEditModel>().ReverseMap();
        CreateMap<DataBaseEditModel, DataBaseEntity>();
        CreateMap<DataBaseViewModel, DataBaseEntity>();
    }
}