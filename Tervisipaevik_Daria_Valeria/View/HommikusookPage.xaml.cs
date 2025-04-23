using Tervisipaevik_Daria_Valeria.Database;
using Tervisipaevik_Daria_Valeria.Models;

namespace Tervisipaevik_Daria_Valeria.View;

public partial class HommikusookPage : ContentPage
{
    HommikusookDatabase database;

    ListView hommikusookListView;
    Entry e_roaNimi, e_valgud, e_rasvad, e_susivesikud, e_kalorid;
    DatePicker dp_kuupaev;
    TimePicker tp_kallaaeg;
    Button btn_salvesta, btn_kustuta, clearButton;

    HommikusookClass selectedItem;
    public HommikusookPage()
    {
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "tervisepaevik.db");
        database = new HommikusookDatabase(dbPath);

        Title = "Hommikusook";

        e_roaNimi = new Entry { Placeholder = "Roa nimi" };
        e_valgud = new Entry { Placeholder = "Valgud", Keyboard = Keyboard.Numeric };
        e_rasvad = new Entry { Placeholder = "Rasvad", Keyboard = Keyboard.Numeric };
        e_susivesikud = new Entry { Placeholder = "Süsivesikud", Keyboard = Keyboard.Numeric };
        e_kalorid = new Entry { Placeholder = "Kalorid", Keyboard = Keyboard.Numeric };
        dp_kuupaev = new DatePicker { Date = DateTime.Now };
        tp_kallaaeg = new TimePicker { Time = TimeSpan.FromHours(12) };

        btn_salvesta = new Button { Text = "Salvesta" };
        btn_kustuta = new Button { Text = "Kustuta", IsVisible = false };
        clearButton = new Button { Text = "Uus sisestus" };

        hommikusookListView = new ListView
        {
            ItemTemplate = new DataTemplate(() =>
            {
                var textCell = new TextCell();
                textCell.SetBinding(TextCell.TextProperty, "Roa_nimi");
                textCell.SetBinding(TextCell.DetailProperty, new Binding("Kuupaev", stringFormat: "{0:d}"));
                return textCell;
            })
        };

        hommikusookListView.ItemSelected += HommikusookListView_ItemSelected;
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
                        e_roaNimi,
                        e_valgud,
                        e_rasvad,
                        e_susivesikud,
                        e_kalorid,
                        dp_kuupaev,
                        tp_kallaaeg,
                        btn_salvesta,
                        btn_kustuta,
                        clearButton,
                        hommikusookListView
                    }
            }
        };

        LoadData();

    }

    private void ClearButton_Clicked(object? sender, EventArgs e)
    {
        ClearForm();
    }

    private void Btn_kustuta_Clicked(object? sender, EventArgs e)
    {
        if (selectedItem != null)
        {
            database.DeleteHommikusook(selectedItem.Hommikusook_id);
            ClearForm();
            LoadData();
        }
    }

    private void Btn_salvesta_Clicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e_roaNimi.Text)) return;

        if (selectedItem == null)
            selectedItem = new HommikusookClass();
        selectedItem.Roa_nimi = e_roaNimi.Text;
        selectedItem.Valgud = int.TryParse(e_valgud.Text, out int valgud) ? valgud : 0;
        selectedItem.Rasvad = int.TryParse(e_rasvad.Text, out int rasvad) ? rasvad : 0;
        selectedItem.Susivesikud = int.TryParse(e_susivesikud.Text, out int susivesikud) ? susivesikud : 0;
        selectedItem.Kalorid = int.TryParse(e_kalorid.Text, out int kalorid) ? kalorid : 0;
        selectedItem.Kuupaev = dp_kuupaev.Date;
        selectedItem.Kallaaeg = tp_kallaaeg.Time;

        database.SaveHommikusook(selectedItem);
        ClearForm();
        LoadData();
    }

    private void HommikusookListView_ItemSelected(object? sender, SelectedItemChangedEventArgs e)
    {
        selectedItem = e.SelectedItem as HommikusookClass;
        if (selectedItem != null)
        {
            e_roaNimi.Text = selectedItem.Roa_nimi;
            e_valgud.Text = selectedItem.Valgud.ToString();
            e_rasvad.Text = selectedItem.Rasvad.ToString();
            e_susivesikud.Text = selectedItem.Susivesikud.ToString();
            e_kalorid.Text = selectedItem.Kalorid.ToString();
            dp_kuupaev.Date = selectedItem.Kuupaev;
            tp_kallaaeg.Time = selectedItem.Kallaaeg;
            btn_kustuta.IsVisible = true;
        }
    }

    public void LoadData()
    {
        hommikusookListView.ItemsSource = database.GetHommikusook().OrderByDescending(t => t.Kuupaev).ToList();
    }

    public void ClearForm()
    {
        selectedItem = null;
        e_roaNimi.Text = string.Empty;
        e_valgud.Text = string.Empty;
        e_rasvad.Text = string.Empty;
        e_susivesikud.Text = string.Empty;
        e_kalorid.Text = string.Empty;
        dp_kuupaev.Date = DateTime.Now;
        tp_kallaaeg.Time = TimeSpan.FromHours(12);
        hommikusookListView.SelectedItem = null;
        btn_kustuta.IsVisible = false;
    }
}