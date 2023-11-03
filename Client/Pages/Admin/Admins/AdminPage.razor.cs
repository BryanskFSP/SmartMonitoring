using Microsoft.AspNetCore.Components;

namespace SmartMonitoring.Client.Pages.Admin.Admins;

public partial class AdminPage
{
    [Parameter] public Guid? ID { get; set; }


    public bool IsEdit => ID != null;
    public string PageTitle => IsEdit ? "Изменение админа" : "Создание админа";
}