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
        CreateMap<TelegramUserEditModel, TelegramUserEntity>().ReverseMap();
        CreateMap<TelegramUserViewModel, TelegramUserEntity>().ReverseMap();
        
        CreateMap<AdminViewModel, AdminEditModel>().ReverseMap();
        CreateMap<AdminEditModel, AdminEntity>().ReverseMap();
        CreateMap<AdminViewModel, AdminEntity>().ReverseMap();

        CreateMap<InviteViewModel, InviteEditModel>().ReverseMap();
        CreateMap<InviteEditModel, InviteEntity>().ReverseMap();
        CreateMap<InviteViewModel, InviteEntity>().ReverseMap();
        
        CreateMap<DataBaseViewModel, DataBaseEditModel>().ReverseMap();
        CreateMap<DataBaseEditModel, DataBaseEntity>().ReverseMap();
        CreateMap<DataBaseViewModel, DataBaseEntity>().ReverseMap();
        
        CreateMap<OrganizationViewModel, OrganizationEditModel>().ReverseMap();
        CreateMap<OrganizationEditModel, OrganizationEntity>().ReverseMap();
        CreateMap<OrganizationViewModel, OrganizationEntity>().ReverseMap();
        
        CreateMap<DataBaseViewModel, DataBaseEditModel>().ReverseMap();
        CreateMap<DataBaseEditModel, DataBaseEntity>().ReverseMap();
        CreateMap<DataBaseViewModel, DataBaseEntity>().ReverseMap();
    }
}