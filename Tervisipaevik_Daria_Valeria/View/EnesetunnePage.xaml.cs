using Tervisipaevik_Daria_Valeria.Database;
using Tervisipaevik_Daria_Valeria.Models;

namespace Tervisipaevik_Daria_Valeria.View;

public partial class EnesetunnePage : ContentPage
{
    EnesetunneDatabase database;

    ListView enesetunneListView;
    DatePicker dp_kuupaev;
    Button btn_salvesta, btn_kustuta, clearButton;

    StackLayout tujuLayout, energiaLayout;
    int selectedTuju = 0;
    int selectedEnergia = 0;

    EnesetunneClass selectedItem;

    public EnesetunnePage()
    {
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "tervisepaevik.db");
        database = new EnesetunneDatabase(dbPath);

        Title = "Enesetunne";

        dp_kuupaev = new DatePicker { Date = DateTime.Now };

        btn_salvesta = new Button { Text = "Salvesta" };
        btn_kustuta = new Button { Text = "Kustuta", IsVisible = false };
        clearButton = new Button { Text = "Uus sisestus" };

        // TUJU (эмоции)
        tujuLayout = new StackLayout { Orientation = StackOrientation.Horizontal };
        for (int i = 1; i <= 5; i++)
        {
            var img = new Image
            {
                Source = $"tuju{i}.jpg",
                HeightRequest = 40,
                WidthRequest = 40
            };
            int level = i;
            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) =>
            {
                selectedTuju = level;
                HighlightSelectedTuju();
            };
            img.GestureRecognizers.Add(tap);
            tujuLayout.Children.Add(img);
        }

        // ENERGIA (огоньки)
        energiaLayout = new StackLayout { Orientation = StackOrientation.Horizontal };
        for (int i = 1; i <= 5; i++)
        {
            var img = new Image
            {
                Source = "energia.jpg", 
                HeightRequest = 40,
                WidthRequest = 40,
                Opacity = 0.3
            };
            int level = i;
            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) =>
            {
                selectedEnergia = level;
                HighlightSelectedEnergia();
            };
            img.GestureRecognizers.Add(tap);
            energiaLayout.Children.Add(img);
        }


        enesetunneListView = new ListView
        {
            ItemTemplate = new DataTemplate(() =>
            {
                var textCell = new TextCell();
                textCell.SetBinding(TextCell.TextProperty, new Binding("Tuju", stringFormat: "Tuju: {0}"));
                textCell.SetBinding(TextCell.DetailProperty, new Binding("Kuupaev", stringFormat: "{0:d}"));
                return textCell;
            })
        };

        enesetunneListView.ItemSelected += EnesetunneListView_ItemSelected;
        btn_salvesta.Clicked += Btn_salvesta_Clicked;
        btn_kustuta.Clicked += Btn_kustuta_Clicked;
        clearButton.Clicked += ClearButton_Clicked;

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
                    tujuLayout,
                    new Label { Text = "Energia:" },
                    energiaLayout,
                    btn_salvesta,
                    btn_kustuta,
                    clearButton,
                    enesetunneListView
                }
            }
        };

        LoadData();
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
        ClearForm();
        LoadData();
    }

    private void Btn_kustuta_Clicked(object? sender, EventArgs e)
    {
        if (selectedItem != null)
        {
            database.DeleteEnesetunne(selectedItem.Enesetunne_id);
            ClearForm();
            LoadData();
        }
    }

    public void LoadData()
    {
        enesetunneListView.ItemsSource = database.GetEnesetunne().OrderByDescending(e => e.Kuupaev).ToList();
    }

    public void EnesetunneListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        selectedItem = e.SelectedItem as EnesetunneClass;
        if (selectedItem != null)
        {
            selectedTuju = selectedItem.Tuju;
            selectedEnergia = selectedItem.Energia;
            dp_kuupaev.Date = selectedItem.Kuupaev;
            HighlightSelectedTuju();
            HighlightSelectedEnergia();
            btn_kustuta.IsVisible = true;
        }
    }

    public void ClearButton_Clicked(object sender, EventArgs e)
    {
        ClearForm();
    }

    public void ClearForm()
    {
        selectedItem = null;
        selectedTuju = 0;
        selectedEnergia = 0;
        dp_kuupaev.Date = DateTime.Now;
        enesetunneListView.SelectedItem = null;
        btn_kustuta.IsVisible = false;
        HighlightSelectedTuju();
        HighlightSelectedEnergia();
    }

    void HighlightSelectedTuju()
    {
        for (int i = 0; i < tujuLayout.Children.Count; i++)
        {
            ((Image)tujuLayout.Children[i]).Opacity = (i + 1 == selectedTuju) ? 1 : 0.5;
        }
    }

    void HighlightSelectedEnergia()
    {
        for (int i = 0; i < energiaLayout.Children.Count; i++)
        {
            ((Image)energiaLayout.Children[i]).Opacity = (i + 1 == selectedEnergia) ? 1 : 0.5;
        }
    }
}
