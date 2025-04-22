using Tervisipaevik_Daria_Valeria.View;

namespace Tervisipaevik_Daria_Valeria;

public partial class StartPage1 : ContentPage
{
    ImageButton btn_hommikusook, btn_louna, btn_ohtusook, btn_vahepala;
    Label lbl_hommikusook, lbl_louna, lbl_ohtusook, lbl_vahepala;

    public StartPage1()
    {
        lbl_hommikusook = new Label
        {
            Text = "Hommikusöök",
            HorizontalOptions = LayoutOptions.Center
        };
        lbl_louna = new Label
        {
            Text = "Lõuna",
            HorizontalOptions = LayoutOptions.Center
        };
        lbl_ohtusook = new Label
        {
            Text = "Õhtusöök",
            HorizontalOptions = LayoutOptions.Center
        };
        lbl_vahepala = new Label
        {
            Text = "Vahepala",
            HorizontalOptions = LayoutOptions.Center
        };
        btn_hommikusook = new ImageButton
        {
            Source = "hommikusook.jpg",
            WidthRequest = 150,
            HeightRequest = 150
        };
        btn_hommikusook.Clicked += Btn_hommikusook_Clicked;
        btn_louna = new ImageButton
        {
            Source = "louna.jpg",
            WidthRequest = 150,
            HeightRequest = 150
        };
        btn_louna.Clicked += Btn_louna_Clicked;
        btn_ohtusook = new ImageButton
        {
            Source = "ohtusook.jpg",
            WidthRequest = 150,
            HeightRequest = 150
        };
        btn_vahepala = new ImageButton
        {
            Source = "vahepala.jpg",
            WidthRequest = 150,
            HeightRequest = 150
        };
        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, 
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }, 
                new RowDefinition { Height = GridLength.Auto }  
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star }
            }
        };

        grid.Add(btn_hommikusook, 0, 0);
        grid.Add(btn_louna, 1, 0);
        grid.Add(lbl_hommikusook, 0, 1);
        grid.Add(lbl_louna, 1, 1);
        grid.Add(btn_ohtusook, 0, 2);
        grid.Add(btn_vahepala, 1, 2);
        grid.Add(lbl_ohtusook, 0, 3);
        grid.Add(lbl_vahepala, 1, 3);

        Content = new StackLayout
        {
            Padding = new Thickness(20),
            Children =
            {
                grid
            }
        };
    }

    private async void Btn_louna_Clicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new LounasookPage());
    }

    private async void Btn_hommikusook_Clicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new HommikusookPage());
    }
}
