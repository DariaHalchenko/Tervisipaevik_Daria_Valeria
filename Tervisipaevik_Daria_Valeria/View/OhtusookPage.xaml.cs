using System.Globalization;
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
    private Image img;

    private TableView tableview;
    private TableSection fotoSection;

    private Button btn_salvesta, btn_kustuta, btn_puhastada, btn_pildista, btn_valifoto;

    public OhtusookPage()
    {
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tervisepaevik.db");
        database = new OhtusookDatabase(dbPath);

        Title = "Ohtusöök";

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

        img = new Image();

        fotoSection = new TableSection("Foto");

        tableview = new TableView
        {
            Intent = TableIntent.Form,
            Root = new TableRoot("Sisesta Ohtusöök")
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

        Content = new ScrollView
        {
            Content = new StackLayout
            {
                Padding = 10,
                Children =
                {
                    tableview
                }
            }
        };
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

            img.Source = ImageSource.FromFile(lisafoto);

            fotoSection.Clear();
            var imageViewCell = new ViewCell
            {
                View = img
            };
            fotoSection.Add(imageViewCell);

            await Shell.Current.DisplayAlert("Edu", "Foto on edukalt salvestatud", "OK");
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
    }

    private void Btn_kustuta_Clicked(object? sender, EventArgs e)
    {
        if (selectedItem != null)
        {
            database.DeleteOhtusook(selectedItem.Ohtusook_id);
            ClearForm();
        }
    }

    private void ClearForm()
    {
        selectedItem = null;
        fotoBytes = null;
        ec_roaNimi.Text = ec_valgud.Text = ec_rasvad.Text = ec_susivesikud.Text = ec_kalorid.Text = string.Empty;
        dp_kuupaev.Date = DateTime.Now;
        tp_kallaaeg.Time = TimeSpan.FromHours(8);
        btn_kustuta.IsVisible = false;
        fotoSection.Clear();
    }

    public class ByteArrayToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte[] bytes && bytes.Length > 0)
                return ImageSource.FromStream(() => new MemoryStream(bytes));
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}