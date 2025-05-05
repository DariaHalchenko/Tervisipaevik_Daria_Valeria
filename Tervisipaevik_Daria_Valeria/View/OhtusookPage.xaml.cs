using Tervisipaevik_Daria_Valeria.Database;
using Tervisipaevik_Daria_Valeria.Models;

namespace Tervisipaevik_Daria_Valeria.View;

public partial class OhtusookPage : ContentPage
{
    private string lisafoto;
    private byte[] fotoBytes;
    private OhtusookDatabase database;
    private OhtusookClass selectedItem;

    private EntryCell ec_roaNimi, ec_valgud, ec_rasvad, ec_susivesikud, ec_kalorid;
    private DatePicker dp_kuupaev;
    private TimePicker tp_kallaaeg;
    private ImageCell ic;

    private TableView tableview;
    private TableSection fotoSection;
    private ListView ohtusookListView;

    private Button btn_salvesta, btn_kustuta, btn_puhastada, btn_pildista, btn_valifoto;

    public OhtusookPage()
    {
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "tervisepaevik.db");
        database = new OhtusookDatabase(dbPath);

        Title = "Ohtusook";

        ec_roaNimi = new EntryCell { Label = "Roa nimi", Placeholder = "nt. Puder" };
        ec_valgud = new EntryCell { Label = "Valgud", Placeholder = "g", Keyboard = Keyboard.Numeric };
        ec_rasvad = new EntryCell { Label = "Rasvad", Placeholder = "g", Keyboard = Keyboard.Numeric };
        ec_susivesikud = new EntryCell { Label = "Süsivesikud", Placeholder = "g", Keyboard = Keyboard.Numeric };
        ec_kalorid = new EntryCell { Label = "Kalorid", Placeholder = "kcal", Keyboard = Keyboard.Numeric };

        dp_kuupaev = new DatePicker { Date = DateTime.Now };
        tp_kallaaeg = new TimePicker { Time = TimeSpan.FromHours(8) };

        btn_salvesta = new Button { Text = "Salvesta" };
        btn_kustuta = new Button { Text = "Kustuta", IsVisible = false };
        btn_puhastada = new Button { Text = "Uus sisestus" };
        btn_pildista = new Button { Text = "Tee foto" };
        btn_valifoto = new Button { Text = "Vali foto" };



        btn_salvesta.Clicked += Btn_salvesta_Clicked;
        btn_kustuta.Clicked += Btn_kustuta_Clicked;
        btn_puhastada.Clicked += Btn_puhastada_Clicked;
        btn_pildista.Clicked += Btn_pildista_Clicked;
        btn_valifoto.Clicked += Btn_valifoto_Clicked;


        ic = new ImageCell
        {
            Text = "Foto nimetus",
            Detail = "Foto kirjeldus"
        };

        fotoSection = new TableSection("Foto");

        tableview = new TableView
        {
            Intent = TableIntent.Form,
            Root = new TableRoot("Sisesta Ohtusook")
                {
                    new TableSection("Üldandmed")
                    {
                        new ViewCell { View = dp_kuupaev },
                        new ViewCell { View = tp_kallaaeg },
                        ec_roaNimi,
                        ec_valgud,
                        ec_rasvad,
                        ec_susivesikud,
                        ec_kalorid
                    },
                    fotoSection,
                    new TableSection("Tegevused")
                    {
                        new ViewCell
                        {
                            View = new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                HorizontalOptions = LayoutOptions.Center,
                                Children = { btn_salvesta, btn_kustuta, btn_puhastada }
                            }
                        }
                    },
                    new TableSection("FOTO")
                    {
                        new ViewCell
                        {
                            View = new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                HorizontalOptions = LayoutOptions.Center,
                                Children = { btn_valifoto, btn_pildista }
                            }
                        }
                    }
                }
        };

        ohtusookListView = new ListView
        {
            ItemTemplate = new DataTemplate(() =>
            {
                var textCell = new TextCell();
                textCell.SetBinding(TextCell.TextProperty, "Roa_nimi");
                textCell.SetBinding(TextCell.DetailProperty, new Binding("Kuupaev", stringFormat: "{0:d}"));
                return textCell;
            }),
            HeightRequest = 250
        };

        ohtusookListView.ItemSelected += VahepalaListView_ItemSelected;

        Content = new ScrollView
        {
            Content = new StackLayout
            {
                Padding = 10,
                Children =
                    {
                        tableview,
                        new Label { Text = "Salvestatud Ohtusook", FontAttributes = FontAttributes.Bold },
                        ohtusookListView
                    }
            }
        };

        LoadData();
    }


    private void Btn_puhastada_Clicked(object sender, EventArgs e) => ClearForm();

    private async void Btn_valifoto_Clicked(object sender, EventArgs e)
    {
        FileResult foto = await MediaPicker.Default.PickPhotoAsync();
        await SalvestaFoto(foto);
    }

    private async void Btn_pildista_Clicked(object sender, EventArgs e)
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            FileResult foto = await MediaPicker.Default.CapturePhotoAsync();
            await SalvestaFoto(foto);
        }
        else
        {
            await Shell.Current.DisplayAlert("Viga", "Teie seade ei ole toetatud", "Ok");
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
            ic.ImageSource = ImageSource.FromFile(lisafoto);

            fotoSection.Clear();
            fotoSection.Add(ic);

            await Shell.Current.DisplayAlert("Edu", "Foto on edukalt salvestatud", "Ok");
        }
    }

    private void Btn_salvesta_Clicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ec_roaNimi.Text)) return;

        if (selectedItem == null)
            selectedItem = new OhtusookClass();

        selectedItem.Roa_nimi = ec_roaNimi.Text;
        selectedItem.Valgud = int.TryParse(ec_valgud.Text, out var valgud) ? valgud : 0;
        selectedItem.Rasvad = int.TryParse(ec_rasvad.Text, out var rasvad) ? rasvad : 0;
        selectedItem.Susivesikud = int.TryParse(ec_susivesikud.Text, out var susivesikud) ? susivesikud : 0;
        selectedItem.Kalorid = int.TryParse(ec_kalorid.Text, out var kalorid) ? kalorid : 0;
        selectedItem.Kuupaev = dp_kuupaev.Date;
        selectedItem.Kallaaeg = tp_kallaaeg.Time;

        if (fotoBytes != null)
            selectedItem.Toidu_foto = fotoBytes;

        database.SaveOhtusook(selectedItem);
        ClearForm();
        LoadData();
    }

    private void Btn_kustuta_Clicked(object? sender, EventArgs e)
    {
        if (selectedItem != null)
        {
            database.DeleteOhtusook(selectedItem.Ohtusook_id);
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
        selectedItem = e.SelectedItem as OhtusookClass;
        if (selectedItem != null)
        {
            selectedItem = e.SelectedItem as OhtusookClass;
            if (selectedItem == null) return;

            ec_roaNimi.Text = selectedItem.Roa_nimi;
            ec_valgud.Text = selectedItem.Valgud.ToString();
            ec_rasvad.Text = selectedItem.Rasvad.ToString();
            ec_susivesikud.Text = selectedItem.Susivesikud.ToString();
            ec_kalorid.Text = selectedItem.Kalorid.ToString();
            dp_kuupaev.Date = selectedItem.Kuupaev;
            tp_kallaaeg.Time = selectedItem.Kallaaeg;
            btn_kustuta.IsVisible = true;

            if (selectedItem.Toidu_foto != null && selectedItem.Toidu_foto.Length > 0)
            {
                string tempFilePath = Path.Combine(FileSystem.CacheDirectory, "temp_selected_image.jpg");
                File.WriteAllBytes(tempFilePath, selectedItem.Toidu_foto);
                ic.ImageSource = ImageSource.FromFile(tempFilePath);

                fotoSection.Clear();
                fotoSection.Add(ic);
            }
            else
            {
                fotoSection.Clear();
            }
        }
    }

    public void LoadData()
    {
        ohtusookListView.ItemsSource = database.GetOhtusook().OrderByDescending(x => x.Kuupaev).ToList();
    }

    public void ClearForm()
    {
        selectedItem = null;
        fotoBytes = null;
        ec_roaNimi.Text = ec_valgud.Text = ec_rasvad.Text = ec_susivesikud.Text = ec_kalorid.Text = string.Empty;
        dp_kuupaev.Date = DateTime.Now;
        tp_kallaaeg.Time = TimeSpan.FromHours(8);
        ohtusookListView.SelectedItem = null;
        btn_kustuta.IsVisible = false;
        fotoSection.Clear();
    }
}
