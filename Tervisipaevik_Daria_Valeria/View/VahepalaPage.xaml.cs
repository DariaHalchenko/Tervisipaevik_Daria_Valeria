using Tervisipaevik_Daria_Valeria.Database;
using Tervisipaevik_Daria_Valeria.Models;

namespace Tervisipaevik_Daria_Valeria.View;

public partial class VahepalaPage : ContentPage
{
    VahepalaDatabase database;

    ListView vahepalaListView;
    Entry e_roaNimi, e_valgud, e_rasvad, e_susivesikud, e_kalorid;
    DatePicker dp_kuupaev;
    TimePicker tp_kallaaeg;
    Button btn_salvesta, btn_kustuta, clearButton;
    Button btn_pildista, btn_valifoto;

    ImageCell ic;
    StackLayout fotoSection;

    VahepalaClass selectedItem;
    byte[] fotoBytes;
    string lisafoto;

    public VahepalaPage()
    {
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "tervisepaevik.db");
        database = new VahepalaDatabase(dbPath);

        Title = "Vahepala";

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

        btn_pildista = new Button { Text = "Tee foto" };
        btn_valifoto = new Button { Text = "Vali foto" };

        ic = new ImageCell { Text = "Foto", Detail = "Toidupilt" };
        fotoSection = new StackLayout();

        btn_salvesta.Clicked += Btn_salvesta_Clicked;
        btn_kustuta.Clicked += Btn_kustuta_Clicked;
        clearButton.Clicked += ClearButton_Clicked;
        btn_pildista.Clicked += Btn_pildista_Clicked;
        btn_valifoto.Clicked += Btn_valifoto_Clicked;

        vahepalaListView = new ListView
        {
            ItemTemplate = new DataTemplate(() =>
            {
                var textCell = new TextCell();
                textCell.SetBinding(TextCell.TextProperty, "Roa_nimi");
                textCell.SetBinding(TextCell.DetailProperty, new Binding("Kuupaev", stringFormat: "{0:d}"));
                return textCell;
            })
        };

        vahepalaListView.ItemSelected += VahepalaListView_ItemSelected;

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
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children = { btn_valifoto, btn_pildista }
                    },
                    fotoSection,
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children = { btn_salvesta, btn_kustuta, clearButton }
                    },
                    new Label { Text = "Salvestatud vahepalad", FontAttributes = FontAttributes.Bold },
                    vahepalaListView
                }
            }
        };

        LoadData();
    }

    private async void Btn_valifoto_Clicked(object sender, EventArgs e)
    {
        var foto = await MediaPicker.Default.PickPhotoAsync();
        await SalvestaFoto(foto);
    }

    private async void Btn_pildista_Clicked(object sender, EventArgs e)
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            var foto = await MediaPicker.Default.CapturePhotoAsync();
            await SalvestaFoto(foto);
        }
        else
        {
            await DisplayAlert("Viga", "Seade ei toeta pildistamist", "OK");
        }
    }

    private async Task SalvestaFoto(FileResult foto)
    {
        if (foto != null)
        {
            lisafoto = Path.Combine(FileSystem.CacheDirectory, foto.FileName);

            using Stream sourceStream = await foto.OpenReadAsync();
            using MemoryStream ms = new MemoryStream();
            await sourceStream.CopyToAsync(ms);
            fotoBytes = ms.ToArray();

            File.WriteAllBytes(lisafoto, fotoBytes);
            var img = new Image { Source = ImageSource.FromFile(lisafoto), HeightRequest = 200 };

            fotoSection.Children.Clear();
            fotoSection.Children.Add(img);

            await DisplayAlert("Foto", "Foto salvestatud", "OK");
        }
    }

    private void Btn_salvesta_Clicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e_roaNimi.Text)) return;

        if (selectedItem == null)
            selectedItem = new VahepalaClass();

        selectedItem.Roa_nimi = e_roaNimi.Text;
        selectedItem.Valgud = int.TryParse(e_valgud.Text, out int valgud) ? valgud : 0;
        selectedItem.Rasvad = int.TryParse(e_rasvad.Text, out int rasvad) ? rasvad : 0;
        selectedItem.Susivesikud = int.TryParse(e_susivesikud.Text, out int susivesikud) ? susivesikud : 0;
        selectedItem.Kalorid = int.TryParse(e_kalorid.Text, out int kalorid) ? kalorid : 0;
        selectedItem.Kuupaev = dp_kuupaev.Date;
        selectedItem.Kallaaeg = tp_kallaaeg.Time;

        if (fotoBytes != null)
            selectedItem.Toidu_foto = fotoBytes;

        database.SaveVahepala(selectedItem);
        ClearForm();
        LoadData();
    }

    private void Btn_kustuta_Clicked(object? sender, EventArgs e)
    {
        if (selectedItem != null)
        {
            database.DeleteVahepala(selectedItem.Vahepala_id);
            ClearForm();
            LoadData();
        }
    }

    private void ClearButton_Clicked(object? sender, EventArgs e)
    {
        ClearForm();
    }

    private void VahepalaListView_ItemSelected(object? sender, SelectedItemChangedEventArgs e)
    {
        selectedItem = e.SelectedItem as VahepalaClass;
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

            if (selectedItem.Toidu_foto != null && selectedItem.Toidu_foto.Length > 0)
            {
                string tempPath = Path.Combine(FileSystem.CacheDirectory, "temp_vahepala.jpg");
                File.WriteAllBytes(tempPath, selectedItem.Toidu_foto);
                var img = new Image { Source = ImageSource.FromFile(tempPath), HeightRequest = 200 };
                fotoSection.Children.Clear();
                fotoSection.Children.Add(img);
            }
            else
            {
                fotoSection.Children.Clear();
            }
        }
    }

    public void LoadData()
    {
        vahepalaListView.ItemsSource = database.GetVahepala().OrderByDescending(t => t.Kuupaev).ToList();
    }

    public void ClearForm()
    {
        selectedItem = null;
        fotoBytes = null;
        e_roaNimi.Text = e_valgud.Text = e_rasvad.Text = e_susivesikud.Text = e_kalorid.Text = string.Empty;
        dp_kuupaev.Date = DateTime.Now;
        tp_kallaaeg.Time = TimeSpan.FromHours(12);
        vahepalaListView.SelectedItem = null;
        btn_kustuta.IsVisible = false;
        fotoSection.Children.Clear();
    }
}
