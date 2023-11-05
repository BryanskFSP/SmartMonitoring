using Microsoft.AspNetCore.Components;

namespace SmartMonitoring.Client.Pages.Admin.Invites;

public partial class InvitePage
{
    [Parameter] public Guid? ID { get; set; }

    public bool IsEdit => ID != null;
    public string PageTitle => IsEdit ? "Изменение приглашения" : "Создание приглашения";
}