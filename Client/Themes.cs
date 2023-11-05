using MudBlazor;

namespace SmartMonitoring.Client;

public class Themes
{
    public static MudTheme TerminalTheme = new()
    {
        Palette = new Palette()
        {
            AppbarBackground = new("#00BFFF"),
            Primary = new("#554b46"),
            Secondary = new("#2467e9"),
            Tertiary = new("#19ffff"),
            TextPrimary = new("#000000")
        },
        // Typography = new()
        // {
        //     Default = new()
        //     {
        //         FontFamily = new[] { "Montserrat Alternates", "Roboto" },
        //         FontWeight = 600,
        //         FontSize = "32px",
        //         LineHeight = 39.01
        //     },
        //     H4 = new()
        //     {
        //         FontFamily = new[] { "Montserrat Alternates", "Roboto" },
        //         FontWeight = 700,
        //         FontSize = "64px",
        //         LineHeight = 78.02
        //     }
        // },
        LayoutProperties = new()
        {
            DefaultBorderRadius = "16px"
        }
    };
}