using AutoMapper;
using SmartMonitoring.Shared.EditModels;
using SmartMonitoring.Shared.ViewModels;

namespace SmartMonitoring.Shared;

public class Mappings: Profile
{
    public Mappings()
    {
        CreateMap<AdminViewModel, AdminEditModel>().ReverseMap();
        CreateMap<DataBaseViewModel, DataBaseEditModel>().ReverseMap();
        CreateMap<InviteViewModel, InviteEditModel>().ReverseMap();
        CreateMap<OrganizationViewModel, OrganizationEditModel>().ReverseMap();
        CreateMap<TelegramUserViewModel, TelegramUserEditModel>().ReverseMap();
    }
}