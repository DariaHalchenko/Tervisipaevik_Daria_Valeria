using Tervisipaevik_Daria_Valeria.Database;
using Tervisipaevik_Daria_Valeria.Models;

namespace Tervisipaevik_Daria_Valeria.View
{
    public partial class HommikusookPage : ContentPage
    {
        private HommikusookDatabase database;
        private HommikusookClass selectedItem;

        private EntryCell ec_roaNimi, ec_valgud, ec_rasvad, ec_susivesikud, ec_kalorid;
        private DatePicker dp_kuupaev;
        private TimePicker tp_kallaaeg;

        private Button btn_salvesta, btn_kustuta, btn_puhastada, btn_pildista, btn_valifoto;

        private ImageCell ic;
        private TableSection fotoSection;

        private ListView hommikusookListView;
        private byte[] fotoBytes;
        private string lisafoto;

        private TableView tableView;

        public HommikusookPage()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "tervisepaevik.db");
            database = new HommikusookDatabase(dbPath);

            Title = "Hommikuöök";

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
            
            var btn_clearDb = new Button { Text = "Очистить базу" };

            btn_salvesta.Clicked += Btn_salvesta_Clicked;
            btn_kustuta.Clicked += Btn_kustuta_Clicked;
            btn_puhastada.Clicked += Btn_puhastada_Clicked;
            btn_pildista.Clicked += Btn_pildista_Clicked;
            btn_valifoto.Clicked += Btn_valifoto_Clicked;

            btn_clearDb.Clicked += (s, e) => { database.ClearHommikusookTable(); ClearForm(); LoadData(); };

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
                Root = new TableRoot("Sisesta hommikuöök")
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

            tableView.Root[2].Add(new ViewCell { View = btn_clearDb });
            // Список записей
            hommikusookListView = new ListView
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

            hommikusookListView.ItemSelected += HommikusookListView_ItemSelected;

            Content = new ScrollView
            {
                Content = new StackLayout
                {
                    Padding = 10,
                    Children =
                    {
                        tableView,
                        new Label { Text = "Salvestatud hommikusöögid", FontAttributes = FontAttributes.Bold },
                        hommikusookListView
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
            try
            {
                if (foto != null)
                {
                    lisafoto = Path.Combine(FileSystem.CacheDirectory, foto.FileName);
                    using Stream sourceStream = await foto.OpenReadAsync();
                    using MemoryStream ms = new MemoryStream();
                    await sourceStream.CopyToAsync(ms);
                    fotoBytes = ms.ToArray();

                    File.WriteAllBytes(lisafoto, fotoBytes);
                    if (File.Exists(lisafoto))
                    {
                        ic.ImageSource = ImageSource.FromFile(lisafoto);
                        lock (fotoSection)
                        {
                            fotoSection.Clear();
                            fotoSection.Add(ic);
                        }
                        await Shell.Current.DisplayAlert("Edu", "Foto on salvestatud", "OK");
                    }
                    else
                    {
                        WriteLog($"File not found: {lisafoto}");
                        await Shell.Current.DisplayAlert("Ошибка", "Не удалось сохранить фото", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog($"Error in SalvestaFoto: {ex.Message}\nStackTrace: {ex.StackTrace}");
                await Shell.Current.DisplayAlert("Ошибка в SalvestaFoto", $"Произошла ошибка: {ex.Message}", "OK");
            }
        }

        private void Btn_salvesta_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ec_roaNimi.Text)) return;

            if (selectedItem == null)
                selectedItem = new HommikusookClass();

            selectedItem.Roa_nimi = ec_roaNimi.Text;
            selectedItem.Valgud = int.TryParse(ec_valgud.Text, out int valgud) ? valgud : 0;
            selectedItem.Rasvad = int.TryParse(ec_rasvad.Text, out int rasvad) ? rasvad : 0;
            selectedItem.Susivesikud = int.TryParse(ec_susivesikud.Text, out int susivesikud) ? susivesikud : 0;
            selectedItem.Kalorid = int.TryParse(ec_kalorid.Text, out int kalorid) ? kalorid : 0;
            selectedItem.Kuupaev = dp_kuupaev.Date;
            selectedItem.Kallaaeg = tp_kallaaeg.Time;

            if (fotoBytes != null)
                selectedItem.Toidu_foto = fotoBytes;

            database.SaveHommikusook(selectedItem);
            ClearForm();
            LoadData();
        }

        private void Btn_kustuta_Clicked(object sender, EventArgs e)
        {
            if (selectedItem != null)
            {
                database.DeleteHommikusook(selectedItem.Hommikusook_id);
                ClearForm();
                LoadData();
            }
        }

        private void Btn_puhastada_Clicked(object sender, EventArgs e)
        {
            ClearForm();    
        }

        private void HommikusookListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                selectedItem = e.SelectedItem as HommikusookClass;
                if (selectedItem == null) return;

                ec_roaNimi.Text = selectedItem.Roa_nimi;
                ec_valgud.Text = selectedItem.Valgud.ToString();
                ec_rasvad.Text = selectedItem.Rasvad.ToString();
                ec_susivesikud.Text = selectedItem.Susivesikud.ToString();
                ec_kalorid.Text = selectedItem.Kalorid.ToString();
                dp_kuupaev.Date = selectedItem.Kuupaev;
                tp_kallaaeg.Time = selectedItem.Kallaaeg;
                btn_kustuta.IsVisible = true;

                lock (fotoSection)
                {
                    fotoSection.Clear();
                    if (selectedItem.Toidu_foto != null && selectedItem.Toidu_foto.Length > 0)
                    {
                        string tempPath = Path.Combine(FileSystem.CacheDirectory, "pilt.jpg");
                        try
                        {
                            File.WriteAllBytes(tempPath, selectedItem.Toidu_foto);
                            if (File.Exists(tempPath))
                            {
                                ic.ImageSource = ImageSource.FromFile(tempPath);
                                fotoSection.Add(ic);
                            }
                            else
                            {
                                WriteLog($"File not found: {tempPath}");
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteLog($"Error writing file {tempPath}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog($"Error in ItemSelected: {ex.Message}\nStackTrace: {ex.StackTrace}");
                Shell.Current.DisplayAlert("Ошибка в ItemSelected", $"Произошла ошибка: {ex.Message}", "OK");
            }
        }

        private void LoadData()
        {
            try
            {
                var list = database.GetHommikusook().OrderByDescending(x => x.Kuupaev).ToList();
                WriteLog($"Loaded {list.Count} records from database");

                // Проверка на дубликаты
                var duplicates = list.GroupBy(x => x.Hommikusook_id)
                                    .Where(g => g.Count() > 1)
                                    .Select(g => g.Key)
                                    .ToList();
                if (duplicates.Any())
                {
                    WriteLog($"Found duplicate IDs: {string.Join(", ", duplicates)}");
                }

                foreach (var item in list)
                {
                    if (item.Toidu_foto != null && item.Toidu_foto.Length > 0)
                    {
                        string tempPath = Path.Combine(FileSystem.CacheDirectory, $"img_{item.Hommikusook_id}.jpg");
                        try
                        {
                            File.WriteAllBytes(tempPath, item.Toidu_foto);
                            if (File.Exists(tempPath))
                            {
                                item.FotoPath = tempPath;
                            }
                            else
                            {
                                item.FotoPath = null;
                                WriteLog($"File not found: {tempPath}");
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteLog($"Error writing file {tempPath}: {ex.Message}");
                            item.FotoPath = null;
                        }
                    }
                    else
                    {
                        item.FotoPath = null;
                    }
                }
                hommikusookListView.ItemsSource = null; // Сброс перед обновлением
                hommikusookListView.ItemsSource = list;
                WriteLog("ListView updated");
            }
            catch (Exception ex)
            {
                WriteLog($"Error in LoadData: {ex.Message}\nStackTrace: {ex.StackTrace}");
                Shell.Current.DisplayAlert("Ошибка в LoadData", $"Произошла ошибка: {ex.Message}", "OK");
            }
        }

        private void WriteLog(string message)
        {
            string logPath = Path.Combine(FileSystem.CacheDirectory, "app_log.txt");
            Console.WriteLine(logPath, $"{DateTime.Now}: {message}\n");
            File.AppendAllText(logPath, $"{DateTime.Now}: {message}\n");
        }

        private void ClearForm()
        {
            try
            {
                selectedItem = null;
                fotoBytes = null;
                ec_roaNimi.Text = ec_valgud.Text = ec_rasvad.Text = ec_susivesikud.Text = ec_kalorid.Text = string.Empty;
                dp_kuupaev.Date = DateTime.Now;
                tp_kallaaeg.Time = TimeSpan.FromHours(12);
                hommikusookListView.SelectedItem = null;
                btn_kustuta.IsVisible = false;

                lock (fotoSection)
                {
                    fotoSection.Clear();
                }
            }
            catch (Exception ex)
            {
                WriteLog($"Error in ClearForm: {ex.Message}\nStackTrace: {ex.StackTrace}");
                Shell.Current.DisplayAlert("Ошибка в ClearForm", $"Произошла ошибка: {ex.Message}", "OK");
            }
        }
    }
}
