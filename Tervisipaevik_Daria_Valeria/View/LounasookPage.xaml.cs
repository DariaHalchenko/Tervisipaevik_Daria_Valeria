using Tervisipaevik_Daria_Valeria.Database;
using Tervisipaevik_Daria_Valeria.Models;

namespace Tervisipaevik_Daria_Valeria.View
{
    public partial class LounasookPage : ContentPage
    {
        private LounasookDatabase database;
        private LounasookClass selectedItem;

        private EntryCell ec_roaNimi, ec_valgud, ec_rasvad, ec_susivesikud, ec_kalorid;
        private DatePicker dp_kuupaev;
        private TimePicker tp_kallaaeg;

        private Button btn_salvesta, btn_kustuta, btn_puhastada, btn_pildista, btn_valifoto;

        private ImageCell ic;
        private TableSection fotoSection;

        private ListView lounasookListView;
        private byte[] fotoBytes;
        private string lisafoto;

        private TableView tableView;

        public LounasookPage()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "tervisepaevik.db");
            database = new LounasookDatabase(dbPath);

            Title = "Lõunasöök";

            // Ввод
            ec_roaNimi = new EntryCell { Label = "Roa nimi", Placeholder = "nt. Supp" };
            ec_valgud = new EntryCell { Label = "Valgud", Placeholder = "g", Keyboard = Keyboard.Numeric };
            ec_rasvad = new EntryCell { Label = "Rasvad", Placeholder = "g", Keyboard = Keyboard.Numeric };
            ec_susivesikud = new EntryCell { Label = "Süsivesikud", Placeholder = "g", Keyboard = Keyboard.Numeric };
            ec_kalorid = new EntryCell { Label = "Kalorid", Placeholder = "kcal", Keyboard = Keyboard.Numeric };

            dp_kuupaev = new DatePicker { Date = DateTime.Now };
            tp_kallaaeg = new TimePicker { Time = TimeSpan.FromHours(12) };

            // Кнопки
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

            // Фото
            ic = new ImageCell
            {
                Text = "Foto nimetus",
                Detail = "Toidupilt"
            };

            fotoSection = new TableSection("Foto");

            // Таблица ввода
            tableView = new TableView
            {
                Intent = TableIntent.Form,
                Root = new TableRoot("Sisesta lõunasöök")
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
                    new TableSection("Foto")
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

            // Список записей
            lounasookListView = new ListView
            {
                ItemTemplate = new DataTemplate(() =>
                {
                    var imageCell = new ImageCell();
                    imageCell.SetBinding(ImageCell.TextProperty, "Roa_nimi");
                    imageCell.SetBinding(ImageCell.DetailProperty, new Binding("Kuupaev", stringFormat: "{0:d}"));
                    imageCell.SetBinding(ImageCell.ImageSourceProperty, "FotoPath");
                    return imageCell;
                }),
                HeightRequest = 250
            };

            lounasookListView.ItemSelected += LounasookListView_ItemSelected;

            Content = new ScrollView
            {
                Content = new StackLayout
                {
                    Padding = 10,
                    Children =
                    {
                        tableView,
                        new Label { Text = "Salvestatud lõunasöögid", FontAttributes = FontAttributes.Bold },
                        lounasookListView
                    }
                }
            };

            LoadData();
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
                await Shell.Current.DisplayAlert("Viga", "Teie seade ei toeta foto tegemist", "OK");
            }
        }

        private async void Btn_valifoto_Clicked(object sender, EventArgs e)
        {
            FileResult foto = await MediaPicker.Default.PickPhotoAsync();
            await SalvestaFoto(foto);
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

                await Shell.Current.DisplayAlert("Edu", "Foto on salvestatud", "OK");
            }
        }

        private void Btn_salvesta_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ec_roaNimi.Text)) return;

            if (selectedItem == null)
                selectedItem = new LounasookClass();

            selectedItem.Roa_nimi = ec_roaNimi.Text;
            selectedItem.Valgud = int.TryParse(ec_valgud.Text, out int valgud) ? valgud : 0;
            selectedItem.Rasvad = int.TryParse(ec_rasvad.Text, out int rasvad) ? rasvad : 0;
            selectedItem.Susivesikud = int.TryParse(ec_susivesikud.Text, out int susivesikud) ? susivesikud : 0;
            selectedItem.Kalorid = int.TryParse(ec_kalorid.Text, out int kalorid) ? kalorid : 0;
            selectedItem.Kuupaev = dp_kuupaev.Date;
            selectedItem.Kallaaeg = tp_kallaaeg.Time;

            if (fotoBytes != null)
                selectedItem.Toidu_foto = fotoBytes;

            database.SaveLounasook(selectedItem);
            ClearForm();
            LoadData();
        }

        private void Btn_kustuta_Clicked(object sender, EventArgs e)
        {
            if (selectedItem != null)
            {
                database.DeleteLounasook(selectedItem.Lounasook_id);
                ClearForm();
                LoadData();
            }
        }

        private void Btn_puhastada_Clicked(object sender, EventArgs e) => ClearForm();

        private void LounasookListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            selectedItem = e.SelectedItem as LounasookClass;
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
                string tempPath = Path.Combine(FileSystem.CacheDirectory, "pilt.jpg");
                File.WriteAllBytes(tempPath, selectedItem.Toidu_foto);
                ic.ImageSource = ImageSource.FromFile(tempPath);

                fotoSection.Clear();
                fotoSection.Add(ic);
            }
            else
            {
                fotoSection.Clear();
            }
        }

        private void LoadData()
        {
            var list = database.GetLounasook().OrderByDescending(x => x.Kuupaev).ToList();

            foreach (var item in list)
            {
                if (item.Toidu_foto != null && item.Toidu_foto.Length > 0)
                {
                    string tempPath = Path.Combine(FileSystem.CacheDirectory, $"img_{item.Lounasook_id}.jpg");
                    File.WriteAllBytes(tempPath, item.Toidu_foto);
                    item.FotoPath = tempPath;
                }
                else
                {
                    item.FotoPath = null;
                }
            }
            lounasookListView.ItemsSource = list;
        }

        private void ClearForm()
        {
            selectedItem = null;
            fotoBytes = null;
            ec_roaNimi.Text = ec_valgud.Text = ec_rasvad.Text = ec_susivesikud.Text = ec_kalorid.Text = string.Empty;
            dp_kuupaev.Date = DateTime.Now;
            tp_kallaaeg.Time = TimeSpan.FromHours(12);
            lounasookListView.SelectedItem = null;
            btn_kustuta.IsVisible = false;
            fotoSection.Clear();
        }
    }
}
