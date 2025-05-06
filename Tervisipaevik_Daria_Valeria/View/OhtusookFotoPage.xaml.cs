using Tervisipaevik_Daria_Valeria.Database;
using Tervisipaevik_Daria_Valeria.Models;

namespace Tervisipaevik_Daria_Valeria.View;

public partial class OhtusookFotoPage : ContentPage
{
    private OhtusookDatabase database;
    Button btn_lisa;
    public OhtusookFotoPage()
    {
        Title = "Toidufotod";
        btn_lisa = new Button
        {
            Text = "Lisa"
        };
        btn_lisa.Clicked += Btn_lisa_Clicked;

        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tervisepaevik.db");
        database = new OhtusookDatabase(dbPath);

        var imageList = new List<OhtusookClass>(database.GetOhtusook()
            .Where(x => x.Toidu_foto != null && x.Toidu_foto.Length > 0));

        var sl = new StackLayout
        {
            Padding = 10,
            Spacing = 10
        };
        sl.Children.Add(btn_lisa);

        foreach (var item in imageList)
        {
            string tempFilePath = Path.Combine(FileSystem.CacheDirectory, $"image_{item.Ohtusook_id}.jpg");
            File.WriteAllBytes(tempFilePath, item.Toidu_foto);

            var image = new Image
            {
                Source = ImageSource.FromFile(tempFilePath),
                HeightRequest = 200,
                Aspect = Aspect.AspectFill
            };

            var tap = new TapGestureRecognizer();
            tap.Tapped += async (s, e) =>
            {
                string info = $"Roa nimi: {item.Roa_nimi}\n" +
                              $"Kuupäev: {item.Kuupaev:dd.MM.yyyy}\n" +
                              $"Kellaaeg: {item.Kallaaeg:hh\\:mm}\n" +
                              $"Valgud: {item.Valgud} g\n" +
                              $"Rasvad: {item.Rasvad} g\n" +
                              $"Süsivesikud: {item.Susivesikud} g\n" +
                              $"Kalorid: {item.Kalorid} kcal";

                await Shell.Current.DisplayAlert("Toiduandmed", info, "OK");
            };

            image.GestureRecognizers.Add(tap);
            sl.Children.Add(image);
        }
        Content = new ScrollView { Content = sl };
    }

    private async void Btn_lisa_Clicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new OhtusookPage());
    }
}