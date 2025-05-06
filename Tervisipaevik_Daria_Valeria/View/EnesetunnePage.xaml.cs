using Tervisipaevik_Daria_Valeria.Database;
using Tervisipaevik_Daria_Valeria.Models;
using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Tervisipaevik_Daria_Valeria.View
{
    public partial class EnesetunnePage : ContentPage
    {
        EnesetunneDatabase database;

        ListView enesetunneListView;
        DatePicker dp_kuupaev;
        Button btn_salvesta, btn_kustuta, btn_selge;

        StackLayout sl_tuju, sl_energia;
        int selectedTuju = 0;
        int selectedEnergia = 0;

        EnesetunneClass selectedItem;

        public EnesetunnePage()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tervisepaevik.db");
            database = new EnesetunneDatabase(dbPath);

            Title = "Enesetunne";

            dp_kuupaev = new DatePicker { Date = DateTime.Now };

            btn_salvesta = new Button { Text = "Salvesta" };
            btn_kustuta = new Button { Text = "Kustuta", IsVisible = false };
            btn_selge = new Button { Text = "Uus sisestus" };

            // TUJU (эмоции)
            sl_tuju = new StackLayout { Orientation = StackOrientation.Horizontal };
            for (int i = 1; i <= 5; i++)
            {
                var img = new Image
                {
                    Source = $"tuju{i}.PNG",
                    HeightRequest = 70,
                    WidthRequest = 70,
                };
                int level = i;
                var tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) =>
                {
                    selectedTuju = level;
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
                    Source = "energia.jpg",
                    HeightRequest = 70,
                    WidthRequest = 70
                };
                int level = i;
                var tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) =>
                {
                    selectedEnergia = level;
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
                    WidthRequest = 50
                };
                tujuImage.SetBinding(Image.SourceProperty, "TujuImageSource");  

                var energiaImage = new Image
                {
                    HeightRequest = 50,
                    WidthRequest = 50
                };
                energiaImage.SetBinding(Image.SourceProperty, "EnergiaImageSource");  

                var lbl_kuupaev = new Label
                {
                    FontSize = 14,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(10, 0)
                };
                lbl_kuupaev.SetBinding(Label.TextProperty, new Binding("Kuupaev", stringFormat: "{0:dd.MM.yyyy}"));

                return new ViewCell
                {
                    View = new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Spacing = 10,
                        Padding = 10,
                        Children = { tujuImage, energiaImage, lbl_kuupaev }
                    }
                };
            });

            enesetunneListView.ItemSelected += EnesetunneListView_ItemSelected;
            btn_salvesta.Clicked += Btn_salvesta_Clicked;
            btn_kustuta.Clicked += Btn_kustuta_Clicked;
            btn_selge.Clicked += Btn_selge_Clicked;

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
                        new Label { Text = "Energia:" },
                        sl_energia,
                        btn_salvesta,
                        btn_kustuta,
                        btn_selge,
                        enesetunneListView
                    }
                }
            };
            NaitaAndmeid();
        }

        private void Btn_selge_Clicked(object? sender, EventArgs e)
        {
            SelgeForm();
        }

        private void Btn_salvesta_Clicked(object? sender, EventArgs e)
        {
            if (selectedTuju == 0 || selectedEnergia == 0) return;

            if (selectedItem == null)
                selectedItem = new EnesetunneClass();

            selectedItem.Tuju = selectedTuju;
            selectedItem.Energia = selectedEnergia;
            selectedItem.Kuupaev = dp_kuupaev.Date;

            database.SaveEnesetunne(selectedItem);
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
    }
}