using Tervisipaevik_Daria_Valeria.Models;
using Tervisipaevik_Daria_Valeria.Database;

namespace Tervisipaevik_Daria_Valeria.View
{
    public partial class MeeldetuletusedPage : ContentPage
    {
        MeeldetuletusedDatabase database;

        Entry tuupEntry;
        DatePicker kuupaevPicker;
        TimePicker kellaaegPicker;
        Switch aktiivneSwitch;
        Button salvestaButton, kustutaButton, uusSisestusButton;
        ListView meeldetuletusedListView;

        MeeldetuletusedClass selectedItem;

        public MeeldetuletusedPage()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "tervisepaevik.db");
            database = new MeeldetuletusedDatabase(dbPath);

            Title = "Meeldetuletused";

            tuupEntry = new Entry { Placeholder = "Meeldetuletuse tüüp (nt. joomine, trenn)" };
            kuupaevPicker = new DatePicker { Date = DateTime.Now };
            kellaaegPicker = new TimePicker { Time = TimeSpan.FromHours(12) };
            aktiivneSwitch = new Switch { IsToggled = true };

            salvestaButton = new Button { Text = "Salvesta" };
            kustutaButton = new Button { Text = "Kustuta", IsVisible = false };
            uusSisestusButton = new Button { Text = "Uus sisestus" };

            meeldetuletusedListView = new ListView
            {
                ItemTemplate = new DataTemplate(() =>
                {
                    var textCell = new TextCell();
                    textCell.SetBinding(TextCell.TextProperty, "tuup");
                    textCell.SetBinding(TextCell.DetailProperty, new Binding("Kuupaev", stringFormat: "{0:d} {1:hh\\:mm}"));
                    return textCell;
                })
            };

            meeldetuletusedListView.ItemSelected += MeeldetuletusedListView_ItemSelected;
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
                        new Label { Text = "Tüüp" },
                        tuupEntry,
                        new Label { Text = "Kuupäev" },
                        kuupaevPicker,
                        new Label { Text = "Kellaaeg" },
                        kellaaegPicker,
                        new Label { Text = "Aktiivne" },
                        aktiivneSwitch,
                        salvestaButton,
                        kustutaButton,
                        uusSisestusButton,
                        meeldetuletusedListView
                    }
                }
            };

            LoadData();
        }

        private void SalvestaButton_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tuupEntry.Text)) return;

            if (selectedItem == null)
                selectedItem = new MeeldetuletusedClass();

            selectedItem.tuup = tuupEntry.Text;
            selectedItem.Kuupaev = kuupaevPicker.Date;
            selectedItem.Kallaaeg = kellaaegPicker.Time;
            selectedItem.Aktiivne = aktiivneSwitch.IsToggled;

            database.SaveMeeldetuletused(selectedItem);
            ClearForm();
            LoadData();
        }

        private void KustutaButton_Clicked(object sender, EventArgs e)
        {
            if (selectedItem != null)
            {
                database.DeleteMeeldetuletused(selectedItem.Meeldetuletused_id);
                ClearForm();
                LoadData();
            }
        }

        private void MeeldetuletusedListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            selectedItem = e.SelectedItem as MeeldetuletusedClass;
            if (selectedItem != null)
            {
                tuupEntry.Text = selectedItem.tuup;
                kuupaevPicker.Date = selectedItem.Kuupaev;
                kellaaegPicker.Time = selectedItem.Kallaaeg;
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
            tuupEntry.Text = string.Empty;
            kuupaevPicker.Date = DateTime.Now;
            kellaaegPicker.Time = TimeSpan.FromHours(12);
            aktiivneSwitch.IsToggled = true;
            meeldetuletusedListView.SelectedItem = null;
            kustutaButton.IsVisible = false;
        }

        private void LoadData()
        {
            meeldetuletusedListView.ItemsSource = database.GetMeeldetuletused()
                .OrderByDescending(m => m.Kuupaev)
                .ToList();
        }
    }
}
