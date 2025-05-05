using Tervisipaevik_Daria_Valeria.View;

namespace Tervisipaevik_Daria_Valeria;

public partial class Startpage : ContentPage
{
    public List<ContentPage> lehed = new List<ContentPage>() { new TreeningudPage(), new EnesetunnePage(), new MeeldetuletusedPage(), new VeejalgiminePage(), new StartPage1(),
    new OhtusookPage(), new VahepalaPage(), new LounasookPage(), new HommikusookFotoPage(),  new HommikusookPage(),};
    public List<string> tekstid = new List<string> { "Tee lahti TreeningudPage", "Tee lahti EnesetunnePage", "Tee lahti MeeldetuletusedPage", "Tee lahti VeejalgiminePage", 
        "StartPage1", "OhtusookPage", "VahepalaPage", "LounasookPage", "ghd", "HommikusookPage",};
    ScrollView sv;
    VerticalStackLayout vsl;
    public Startpage()
    {
        Title = "Avaleht";
        vsl = new VerticalStackLayout { BackgroundColor = Color.FromArgb("#FFC0CB") };
        for (int i = 0; i < tekstid.Count; i++)
        {
            Button nupp = new Button
            {
                Text = tekstid[i],
                BackgroundColor = Color.FromArgb("#EE82EE"),
                TextColor = Color.FromArgb("#FF00FF"),
                BorderWidth = 10,
                ZIndex = i,
                FontFamily = "Luckymoon 400",
                FontSize = 28
            };
            vsl.Add(nupp);
            nupp.Clicked += Lehte_avamine;
        }
        sv = new ScrollView { Content = vsl };
        Content = sv;

    }

    private async void Lehte_avamine(object? sender, EventArgs e)
    {
        Button btn = (Button)sender;
        await Navigation.PushAsync(lehed[btn.ZIndex]);
    }

    private async void Tagasi_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }
}