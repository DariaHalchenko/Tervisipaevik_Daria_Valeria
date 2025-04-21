using Tervisipaevik_Daria_Valeria.Database;
using Tervisipaevik_Daria_Valeria.Models;

namespace Tervisipaevik_Daria_Valeria.View;

public partial class ToidukorradPage : ContentPage
{
    ToidukorradDatabase database;

    ListView toidukorradListView;
    Entry e_roaNimi, e_soogiAeg, e_kalorid;
    DatePicker dp_kuupaev;
    TimePicker tp_kallaaeg;
    Button btn_salvesta, btn_kustuta, clearButton;

    ToidukorradClass selectedItem;

    public ToidukorradPage()
	{
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "tervisepaevik.db");
        database = new ToidukorradDatabase(dbPath);

        Title = "Toidukorrad";

        e_roaNimi = new Entry { Placeholder = "Roa nimi" };
        e_soogiAeg = new Entry { Placeholder = "Söögi aeg" };
        e_kalorid = new Entry { Placeholder = "Kalorid", Keyboard = Keyboard.Numeric };
        dp_kuupaev = new DatePicker { Date = DateTime.Now };
        tp_kallaaeg = new TimePicker { Time = TimeSpan.FromHours(12) };

        btn_salvesta = new Button { Text = "Salvesta" };
        btn_kustuta = new Button { Text = "Kustuta", IsVisible = false };
        clearButton = new Button { Text = "Uus sisestus" };

        toidukorradListView = new ListView
        {
            ItemTemplate = new DataTemplate(() =>
            {
                var textCell = new TextCell();
                textCell.SetBinding(TextCell.TextProperty, "Roa_nimi");
                textCell.SetBinding(TextCell.DetailProperty, new Binding("Kuupaev", stringFormat: "{0:d}"));
                return textCell;
            })
        };
        toidukorradListView.ItemSelected += ToidukorradListView_ItemSelected;
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
                        e_soogiAeg,
                        e_kalorid,
                        dp_kuupaev,
                        tp_kallaaeg,
                        btn_salvesta,
                        btn_kustuta,
                        clearButton,
                        toidukorradListView
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
            database.DeleteToidukorrad(selectedItem.Toidukorrad_id);
            ClearForm();
            LoadData();
        }
    }

    private void Btn_salvesta_Clicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e_roaNimi.Text)) return;

        if (selectedItem == null)
            selectedItem = new ToidukorradClass();
        selectedItem.Roa_nimi = e_roaNimi.Text;
        selectedItem.Soogi_aeg = e_soogiAeg.Text;
        selectedItem.Kalorid = int.TryParse(e_kalorid.Text, out int kalorid) ? kalorid : 0;
        selectedItem.Kuupaev = dp_kuupaev.Date;
        selectedItem.Kallaaeg = tp_kallaaeg.Time;

        database.SaveToidukorrad(selectedItem);
        ClearForm();
        LoadData();
    }

    private void ToidukorradListView_ItemSelected(object? sender, SelectedItemChangedEventArgs e)
    {
        selectedItem = e.SelectedItem as ToidukorradClass;
        if (selectedItem != null)
        {
            e_roaNimi.Text = selectedItem.Roa_nimi;
            e_soogiAeg.Text = selectedItem.Soogi_aeg;
            e_kalorid.Text = selectedItem.Kalorid.ToString();
            dp_kuupaev.Date = selectedItem.Kuupaev;
            tp_kallaaeg.Time = selectedItem.Kallaaeg;
            btn_kustuta.IsVisible = true;
        }
    }

    public void LoadData()
    {
        toidukorradListView.ItemsSource = database.GetToidukorrad().OrderByDescending(t => t.Kuupaev).ToList();
    }

    public void ClearForm()
    {
        selectedItem = null;
        e_roaNimi.Text = string.Empty;
        e_soogiAeg.Text = string.Empty;
        e_kalorid.Text = string.Empty;
        dp_kuupaev.Date = DateTime.Now;
        tp_kallaaeg.Time = TimeSpan.FromHours(12);
        toidukorradListView.SelectedItem = null;
        btn_kustuta.IsVisible = false;
    }
}