using Tervisipaevik_Daria_Valeria.Database;
using Tervisipaevik_Daria_Valeria.Models;

namespace Tervisipaevik_Daria_Valeria.View;

public partial class TreeningudPage : ContentPage
{
    TreeningudDatabase database;

    ListView treeningudListView;
    Entry e_nimi, e_tuup, e_sammude, e_kalorid;
    DatePicker dp_kuupaev;
    TimePicker tp_kallaaeg;
    Button btn_salvesta, btn_kustuta, clearButton;

    TreeningudClass selectedItem;

    public TreeningudPage()
    {

        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "tervisepaevik.db");
        database = new TreeningudDatabase(dbPath);

        Title = "Treeningud";

        e_nimi = new Entry { Placeholder = "Treeningu nimi" };
        e_tuup = new Entry { Placeholder = "Treeningu tüüp" };
        e_sammude = new Entry { Placeholder = "Sammude arv", Keyboard = Keyboard.Numeric };
        e_kalorid = new Entry { Placeholder = "Kulutatud kalorid", Keyboard = Keyboard.Numeric };
        dp_kuupaev = new DatePicker { Date = DateTime.Now };
        tp_kallaaeg = new TimePicker { Time = TimeSpan.FromHours(12) };

        btn_salvesta = new Button { Text = "Salvesta" };
        btn_kustuta = new Button { Text = "Kustuta", IsVisible = false };
        clearButton = new Button { Text = "Uus sisestus" };

        treeningudListView = new ListView
        {
            ItemTemplate = new DataTemplate(() =>
            {
                var textCell = new TextCell();
                textCell.SetBinding(TextCell.TextProperty, "Treeningu_nimi");
                textCell.SetBinding(TextCell.DetailProperty, new Binding("Kuupaev", stringFormat: "{0:d}"));
                return textCell;
            })
        };

        treeningudListView.ItemSelected += TreeningudListView_ItemSelected;
        btn_salvesta.Clicked += Btn_salvesta_Clicked; ;
        btn_kustuta.Clicked += Btn_kustuta_Clicked; ;
        clearButton.Clicked += ClearButton_Clicked;

        Content = new ScrollView
        {
            Content = new StackLayout
            {
                Padding = 20,
                Children =
                    {
                        e_nimi,
                        e_tuup,
                        e_sammude,
                        e_kalorid,
                        dp_kuupaev,
                        tp_kallaaeg,
                        btn_salvesta,
                        btn_kustuta,
                        clearButton,
                        treeningudListView
                    }
            }
        };

        LoadData();
    }

    private void Btn_salvesta_Clicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e_nimi.Text)) return;

        if (selectedItem == null)
            selectedItem = new TreeningudClass();
        selectedItem.Treeningu_nimi = e_nimi.Text;
        selectedItem.Treeningu_tuup = e_tuup.Text;
        selectedItem.Sammude_Arv = int.TryParse(e_sammude.Text, out int sammud) ? sammud : 0;
        selectedItem.Kulutud_kalorid = int.TryParse(e_kalorid.Text, out int kalorid) ? kalorid : 0;
        selectedItem.Kuupaev = dp_kuupaev.Date;
        selectedItem.Kallaaeg = tp_kallaaeg.Time;

        database.SaveTreeningud(selectedItem);
        ClearForm();
        LoadData();
    }

    private void Btn_kustuta_Clicked(object? sender, EventArgs e)
    {
        if (selectedItem != null)
        {
            database.DeleteTreeningud(selectedItem.Treeningud_id);
            ClearForm();
            LoadData();
        }
    }

    public void LoadData()
    {
        treeningudListView.ItemsSource = database.GetTreeningud().OrderByDescending(t => t.Kuupaev).ToList();
    }

    public void TreeningudListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        selectedItem = e.SelectedItem as TreeningudClass;
        if (selectedItem != null)
        {
            e_nimi.Text = selectedItem.Treeningu_nimi;
            e_tuup.Text = selectedItem.Treeningu_tuup;
            e_sammude.Text = selectedItem.Sammude_Arv.ToString();
            e_kalorid.Text = selectedItem.Kulutud_kalorid.ToString();
            dp_kuupaev.Date = selectedItem.Kuupaev;
            tp_kallaaeg.Time = selectedItem.Kallaaeg;
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
        e_nimi.Text = string.Empty;
        e_tuup.Text = string.Empty;
        e_sammude.Text = string.Empty;
        e_kalorid.Text = string.Empty;
        dp_kuupaev.Date = DateTime.Now;
        tp_kallaaeg.Time = TimeSpan.FromHours(12);
        treeningudListView.SelectedItem = null;
        btn_kustuta.IsVisible = false;
    }
}
