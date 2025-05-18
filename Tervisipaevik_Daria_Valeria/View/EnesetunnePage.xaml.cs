using Tervisipaevik_Daria_Valeria.Database;
using Tervisipaevik_Daria_Valeria.Models;
using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using static Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific.VisualElement;

namespace Tervisipaevik_Daria_Valeria.View
{
    public partial class EnesetunnePage : ContentPage
    {
        EnesetunneDatabase database;

        ListView enesetunneListView;
        DatePicker dp_kuupaev;
        Button btn_salvesta, btn_kustuta, btn_puhastata;

        StackLayout sl_tuju, sl_energia;
        int selectedTuju = 0;
        int selectedEnergia = 0;

        EnesetunneClass selectedItem;

        public EnesetunnePage()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tervisepaevik.db");
            database = new EnesetunneDatabase(dbPath);

            Title = "Enesetunne";

            dp_kuupaev = new DatePicker 
            { 
                Date = DateTime.Now,
                BackgroundColor = Colors.LightBlue,
                TextColor = Colors.White,
                FontSize = 16,
                HorizontalOptions = LayoutOptions.Center
            };

            dp_kuupaev.DateSelected += (s, e) =>
            {
                dp_kuupaev.ScaleTo(1.1, 300, Easing.CubicInOut);
                dp_kuupaev.ScaleTo(1.0, 300, Easing.CubicInOut);
            };
            btn_salvesta = new Button { Text = "Salvesta" };
            btn_kustuta = new Button { Text = "Kustuta", IsVisible = false };
            btn_puhastata = new Button { Text = "Uus sisestus" };

            // TUJU (эмоции)
            sl_tuju = new StackLayout { Orientation = StackOrientation.Horizontal };
            for (int i = 1; i <= 5; i++)
            {
                var img = new Image
                {
                    Source = $"tuju{i}.PNG",
                    BackgroundColor = Colors.Transparent,
                    HeightRequest = 70,
                    WidthRequest = 70,
                };
                int level = i;
                var tap = new TapGestureRecognizer();
                tap.Tapped += async (s, e) =>
                {
                    selectedTuju = level;
                    var image = s as Image;
                    await image.ScaleTo(1.2, 100, Easing.CubicInOut);
                    await image.ScaleTo(1.0, 100, Easing.CubicInOut);
                };
                img.GestureRecognizers.Add(tap);
                sl_tuju.Children.Add(img);
            }

            // ENERGIA (огоньки)
            sl_energia = new StackLayout { Orientation = StackOrientation.Horizontal };
            for (int i = 1; i <= 5; i++)
            {
                var img = new Image
                {
                    Source = "energia.png",
                    BackgroundColor = Colors.Transparent,
                    HeightRequest = 70,
                    WidthRequest = 70
                };
                int level = i;
                var tap = new TapGestureRecognizer();
                tap.Tapped += async (s, e) =>
                {
                    selectedEnergia = level;

                    var image = s as Image;
                    await image.ScaleTo(1.2, 100, Easing.CubicInOut);
                    await image.ScaleTo(1.0, 100, Easing.CubicInOut);
                };
                img.GestureRecognizers.Add(tap);
                sl_energia.Children.Add(img);
            }

            enesetunneListView = new ListView();

            enesetunneListView.ItemTemplate = new DataTemplate(() =>
            {
                var tujuImage = new Image
                {
                    HeightRequest = 50,
                    WidthRequest = 50,
                    Aspect = Aspect.Fill,
                    BackgroundColor = Colors.Transparent, 
         
                };
                tujuImage.SetBinding(Image.SourceProperty, "TujuImageSource");

                var energiaImage = new Image
                {
                    HeightRequest = 50,
                    WidthRequest = 50,
                    Aspect = Aspect.Fill,
                    BackgroundColor = Colors.Transparent,

                };
                energiaImage.SetBinding(Image.SourceProperty, "EnergiaImageSource");

                var lbl_kuupaev = new Label
                {
                    FontSize = 14,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(10, 0),
                    TextColor = Colors.DarkSlateGray // Цвет текста
                };
                lbl_kuupaev.SetBinding(Label.TextProperty, new Binding("Kuupaev", stringFormat: "{0:dd.MM.yyyy}"));

                var sudaImage = new Image
                {
                    Source = "suda.jpg",
                    WidthRequest = 30,
                    HeightRequest = 30,
                    VerticalOptions = LayoutOptions.Center
                };
                sudaImage.SetBinding(IsVisibleProperty, new Binding("Energia", BindingMode.Default, converter: new EnergiaToHeartVisibilityConverter()));

                sudaImage.Loaded += async (s, e) =>
                {
                    while (true)
                    {
                        await sudaImage.ScaleTo(1.2, 500, Easing.SinInOut);
                        await sudaImage.ScaleTo(1.0, 500, Easing.SinInOut);

                    }
                };

                var cell = new ViewCell
                {
                    View = new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Spacing = 15,
                        Padding = 15,
                        Children = { tujuImage, energiaImage, lbl_kuupaev, sudaImage }
                    }
                };

                return cell;
            });

            enesetunneListView.ItemSelected += EnesetunneListView_ItemSelected;
            btn_salvesta.Clicked += Btn_salvesta_Clicked;
            btn_kustuta.Clicked += Btn_kustuta_Clicked;
            btn_puhastata.Clicked += Btn_puhastata_Clicked;

            Content = new ScrollView
            {
                Content = new StackLayout
                {
                    Padding = 20,
                    Children =
                    {
                        new Label { Text = "Kuupäev:" },
                        dp_kuupaev,
                        new Label { Text = "Tuju:" },
                        sl_tuju,
                        new Label { Text = "Energia:", Margin = new Thickness(0, 20, 0, 0) },
                        sl_energia,
                        btn_salvesta,
                        btn_kustuta,
                        btn_puhastata,
                        enesetunneListView
                    }
                }
            };
            NaitaAndmeid();
        }
        private void Btn_puhastata_Clicked(object? sender, EventArgs e)
        {
            SelgeForm();
        }

        private async void Btn_salvesta_Clicked(object? sender, EventArgs e)
        {
            if (selectedTuju == 0 || selectedEnergia == 0)
                return;

            if (selectedItem == null)
                selectedItem = new EnesetunneClass();

            selectedItem.Tuju = selectedTuju;
            selectedItem.Energia = selectedEnergia;
            selectedItem.Kuupaev = dp_kuupaev.Date;

            database.SaveEnesetunne(selectedItem);

            string sonum = "";

            if (selectedTuju >= 4 && selectedEnergia >= 4)
                sonum = "Super! Tundub, et sul on suurepärane päev! 😄⚡";
            else if (selectedTuju <= 2 && selectedEnergia <= 2)
                sonum = "Püüa puhata ja hoolitse enda eest täna.";
            else if (selectedTuju >= 4 && selectedEnergia <= 2)
                sonum = "Hea tuju, aga oled väsinud – aeg hellitada end ☕🎶";
            else if (selectedTuju <= 2 && selectedEnergia >= 4)
                sonum = "Sul on energiat, aga tuju vajab turgutust. Proovi midagi toredat teha! 🚶🎨";

            if (!string.IsNullOrEmpty(sonum))
                await DisplayAlert("Sõnum sulle", sonum, "OK");

            SelgeForm();
            NaitaAndmeid();
        }

        private void Btn_kustuta_Clicked(object? sender, EventArgs e)
        {
            if (selectedItem != null)
            {
                database.DeleteEnesetunne(selectedItem.Enesetunne_id);
                SelgeForm();
                NaitaAndmeid();
            }
        }

        public void NaitaAndmeid()
        {
            enesetunneListView.ItemsSource = database.GetEnesetunne()
                .OrderByDescending(e => e.Kuupaev)
                .ToList();
        }

        public void EnesetunneListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            selectedItem = e.SelectedItem as EnesetunneClass;
            if (selectedItem != null)
            {
                selectedTuju = selectedItem.Tuju;
                selectedEnergia = selectedItem.Energia;
                dp_kuupaev.Date = selectedItem.Kuupaev;
                btn_kustuta.IsVisible = true;
            }
        }

        public void SelgeForm()
        {
            selectedItem = null;
            selectedTuju = 0;
            selectedEnergia = 0;
            dp_kuupaev.Date = DateTime.Now;
            enesetunneListView.SelectedItem = null;
            btn_kustuta.IsVisible = false;
        }
        public class EnergiaToHeartVisibilityConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                int energia = (int)value;
                return energia >= 2; 
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}