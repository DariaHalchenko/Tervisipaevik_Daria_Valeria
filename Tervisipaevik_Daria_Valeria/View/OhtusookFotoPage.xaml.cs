using Tervisipaevik_Daria_Valeria.Database;
using Tervisipaevik_Daria_Valeria.Models;

namespace Tervisipaevik_Daria_Valeria.View
{
    public partial class OhtusookFotoPage : ContentPage
    {
        private OhtusookDatabase database;
        private Button btn_lisa;
        private Grid grid;

        public OhtusookFotoPage()
        {
            Title = "Toidufotod";

            btn_lisa = new Button
            {
                Text = "Lisa",
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            btn_lisa.Clicked += Btn_lisa_Clicked;

            grid = new Grid
            {
                Padding = 10,
                RowSpacing = 10,
                ColumnSpacing = 10
            };

            for (int i = 0; i < 3; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            }

            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tervisepaevik.db");
            database = new OhtusookDatabase(dbPath);

            var imageList = new List<OhtusookClass>(database.GetOhtusook()
                .Where(x => x.Toidu_foto != null && x.Toidu_foto.Length > 0));

            int rida = 0;
            int veerg = 0;

            foreach (var item in imageList)
            {
                if (veerg == 0)
                {
                    grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                }

                string tempFilePath = Path.Combine(FileSystem.CacheDirectory, $"image_{item.Ohtusook_id}.jpg");
                File.WriteAllBytes(tempFilePath, item.Toidu_foto);

                var image = new Image
                {
                    Source = ImageSource.FromFile(tempFilePath),
                    HeightRequest = 100,
                    WidthRequest = 100,
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

                grid.Children.Add(image);
                Grid.SetRow(image, rida);
                Grid.SetColumn(image, veerg);

                veerg++;
                if (veerg == 3)
                {
                    veerg = 0;
                    rida++;
                }
            }

            var vsl = new VerticalStackLayout
            {
                Padding = 10,
                Spacing = 10,
                Children = { btn_lisa, grid }
            };

            Content = new ScrollView
            {
                Content = vsl
            };
        }

        private async void Btn_lisa_Clicked(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(new OhtusookPage());
        }
    }
}
