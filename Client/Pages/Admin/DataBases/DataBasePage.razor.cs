using Microsoft.AspNetCore.Components;

namespace SmartMonitoring.Client.Pages.Admin.DataBases;

public partial class DataBasePage
{
    [Parameter] public Guid? ID { get; set; }

    public bool IsEdit => ID != null;
    public string PageTitle => IsEdit ? "Изменение базы данных" : "Создание базы данных";
}