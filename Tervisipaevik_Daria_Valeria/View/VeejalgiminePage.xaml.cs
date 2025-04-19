using Tervisipaevik_Daria_Valeria.Models;
using Tervisipaevik_Daria_Valeria.Database;

namespace Tervisipaevik_Daria_Valeria.View
{
    public partial class VeejalgiminePage : ContentPage
    {
        VeejalgimineDatabase database;

        Entry kogusEntry;
        DatePicker kuupaevPicker;
        Switch aktiivneSwitch;
        Button salvestaButton, kustutaButton, uusSisestusButton;
        ListView veejalgimineListView;

        VeejalgimineClass selectedItem;

        public VeejalgiminePage()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "tervisepaevik.db");
            database = new VeejalgimineDatabase(dbPath);

            Title = "Vee jälgimine";

            kogusEntry = new Entry { Placeholder = "Joodud vee kogus (ml)", Keyboard = Keyboard.Numeric };
            kuupaevPicker = new DatePicker { Date = DateTime.Now };
            aktiivneSwitch = new Switch { IsToggled = true };

            salvestaButton = new Button { Text = "Salvesta" };
            kustutaButton = new Button { Text = "Kustuta", IsVisible = false };
            uusSisestusButton = new Button { Text = "Uus sisestus" };

            veejalgimineListView = new ListView
            {
                ItemTemplate = new DataTemplate(() =>
                {
                    var textCell = new TextCell();
                    textCell.SetBinding(TextCell.TextProperty, new Binding("Kogus", stringFormat: "Kogus: {0} ml"));
                    textCell.SetBinding(TextCell.DetailProperty, new Binding("Kuupaev", stringFormat: "{0:d}"));
                    return textCell;
                })
            };

            veejalgimineListView.ItemSelected += VeejalgimineListView_ItemSelected;
            salvestaButton.Clicked += SalvestaButton_Clicked;
            kustutaButton.Clicked += KustutaButton_Clicked;
            uusSisestusButton.Clicked += UusSisestusButton_Clicked;

            Content = new ScrollView
            {
                Content = new StackLayout
                {
                    Padding = 20,
                    Children =
                    {
                        new Label { Text = "Kuupäev" },
                        kuupaevPicker,
                        new Label { Text = "Kogus (ml)" },
                        kogusEntry,
                        new Label { Text = "Aktiivne" },
                        aktiivneSwitch,
                        salvestaButton,
                        kustutaButton,
                        uusSisestusButton,
                        veejalgimineListView
                    }
                }
            };

            LoadData();
        }

        private void SalvestaButton_Clicked(object sender, EventArgs e)
        {
            if (!int.TryParse(kogusEntry.Text, out int kogus)) return;

            if (selectedItem == null)
                selectedItem = new VeejalgimineClass();

            selectedItem.Kuupaev = kuupaevPicker.Date;
            selectedItem.Kogus = kogus;
            selectedItem.Aktiivne = aktiivneSwitch.IsToggled;

            database.SaveVeejalgimine(selectedItem);
            ClearForm();
            LoadData();
        }

        private void KustutaButton_Clicked(object sender, EventArgs e)
        {
            if (selectedItem != null)
            {
                database.DeleteVeejalgimine(selectedItem.Veejalgimine_id);
                ClearForm();
                LoadData();
            }
        }

        private void VeejalgimineListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            selectedItem = e.SelectedItem as VeejalgimineClass;
            if (selectedItem != null)
            {
                kuupaevPicker.Date = selectedItem.Kuupaev;
                kogusEntry.Text = selectedItem.Kogus.ToString();
                aktiivneSwitch.IsToggled = selectedItem.Aktiivne;
                kustutaButton.IsVisible = true;
            }
        }

        private void UusSisestusButton_Clicked(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            selectedItem = null;
            kuupaevPicker.Date = DateTime.Now;
            kogusEntry.Text = string.Empty;
            aktiivneSwitch.IsToggled = true;
            veejalgimineListView.SelectedItem = null;
            kustutaButton.IsVisible = false;
        }

        private void LoadData()
        {
            veejalgimineListView.ItemsSource = database.GetVeejalgimine()
                .OrderByDescending(v => v.Kuupaev)
                .ToList();
        }
    }
}
