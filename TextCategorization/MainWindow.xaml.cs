using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using DataFormats = System.Windows.DataFormats;
using DragEventArgs = System.Windows.DragEventArgs;
using ListBox = System.Windows.Controls.ListBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace TextCategorization
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static readonly object Locker = new object();

        /*private static string DownloadText(string url)
        {
        Again:
            try
            {
                var client = new WebClient();
                return client.DownloadString(url);
            }
            catch (Exception)
            {
                goto Again;
            }
        }*/

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SourceTb_PreviewDrop(object sender, DragEventArgs e)
        {
            object text = e.Data.GetData(DataFormats.FileDrop);

            SourceTb.Text = ((string[]) text)[0].EndsWith(".txt") ? ((string[]) text)[0] : "";

            if (SourceTb.Text.Length <= 3) return;

            CategorizeBtn.IsEnabled = true;
            CategorizeMenuItem.IsEnabled = true;
        }

        private void AnalizeUrl(object url)
        {
            string data;

            #region Download content
            Again:
            try
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(5));

                Task<string> download = new Task<string>(
                        () => Downloader.DownloadText((string) url));
                download.Start();
                download.Wait(cts.Token);
                data = download.Result;
            }
            catch (Exception)
            {
                if (MessageBox.Show("Server not responding.\n" + 
                                    "Try again?", 
                                    "Error", 
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Error) == MessageBoxResult.Yes)
                {
                    goto Again;
                }
                return;
            }
            #endregion

            if (data == string.Empty)
            {
                MessageBox.Show("Data not found!", 
                                "Error", 
                                MessageBoxButton.YesNo, 
                                MessageBoxImage.Error);
                return;
            }

            #region Formatting data
            data = HtmlRemoval.StripTagsCharArray(data);
            data = HtmlRemoval.StripTagsRegex(data);
            data = data.Trim();
            data = "_" + data.ToLower() + "_";
            data = data.Replace(" ", "_|_");

            string[] split = data.Split('|');
            #endregion

            Dictionary<string, int> nGrams = new Dictionary<string, int>();

            #region Parse data
            foreach (var item in split)
            {
                string itemCopy = item;
                for (int nfrom = 2; nfrom < item.Length - 1; ++nfrom)
                {
                    char[] arr = itemCopy.ToArray();
                    for (int j = nfrom; j <= nfrom; j++)
                    {
                        for (int i = 0; i < arr.Length - j + 1; i++)
                        {
                            string nGram = "";
                            for (int k = 0; k < j; k++)
                                nGram += arr[i + k];
                            if (nGrams.ContainsKey(nGram))
                                nGrams[nGram] = nGrams[nGram] + 1;
                            else
                                nGrams.Add(nGram, 1);
                        }
                    }
                    itemCopy += "_";
                }
            }
            #endregion

            #region Compare profile
            lock (Locker)
            {
                Dictionary<string, int> distances = new Dictionary<string, int>();

                string[] profiles = Directory.GetFiles(".\\Profiles\\");

                foreach (var profile in profiles)
                {
                    int counter = 0;

                    #region Deserialize
                    List<KeyValuePair<string, int>> deserialized = new List<KeyValuePair<string, int>>();
                    try
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        using (FileStream stream = new FileStream(profile, FileMode.OpenOrCreate))
                        {
                            deserialized = (List<KeyValuePair<string, int>>) formatter.Deserialize(stream);
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                        return;
                    }
                    #endregion

                    int outOfPlaceMax = nGrams.Values.Max();
                    bool flag = false;

                    #region Compare
                    foreach (var pItem in deserialized)
                    {
                        if (nGrams.Keys.Contains(pItem.Key))
                        {
                            counter += (ushort) (pItem.Value - nGrams[pItem.Key]);
                        }
                        else counter += outOfPlaceMax;
                    }
                    #endregion

                    distances.Add(profile, counter);
                }
                
                var orderedDistances = distances.OrderBy(p => p.Value).ToList();

                string category = orderedDistances[0].Key;
                category = category.Substring(0, category.LastIndexOf(".", StringComparison.Ordinal));
                category = category.Substring(category.LastIndexOf("\\", StringComparison.Ordinal) + 1);

                #region Change profile
                Dispatcher.Invoke(new MethodInvoker(() =>
                {
                    foreach (TabItem item in ProfilesCtrl.Items)
                    {
                        if (item.Header.ToString() != category) continue;

                        var content = item.Content as ListBox;

                        if (content != null) 
                            content.Items.Add(new ListBoxItem().Content = url);
                    }
                }), null);
                #endregion
            }
            #endregion
        }

        private void FromFile(object file)
        {
            StreamReader input = new StreamReader((string)file);

            const int threadsCount = 3;
            Thread[] threads = new Thread[threadsCount];
            int counter = 0;

            while (!input.EndOfStream)
            {
                var url = input.ReadLine();

                Regex urlRx =
                    new Regex(
                        @"^((http|ftp|https|www)://)([\w+?\.\w+])+([a-zA-Z0-9\~\!\@\#\$\%\^\&\*\(\)_\-\=\+\\\/\?\.\:\;\'\,]*)?$",
                        RegexOptions.IgnoreCase);

                if (url != null &&
                    !urlRx.IsMatch(url))
                {
                    continue;
                }

                #region Threads
                if (counter < threadsCount)
                {
                    threads[counter] = new Thread(AnalizeUrl);
                    threads[counter].Start(url);
                    ++counter;
                }
                else
                {
                    for (int i = 0; i < threadsCount; ++i)
                    {
                        if (threads[i] == null) continue;
                        if (!threads[i].IsAlive)
                        {
                            threads[i] = new Thread(AnalizeUrl);
                            threads[i].Start(url);
                            counter = i;
                            break;
                        }
                        else
                        {
                            threads[i].Join();
                            threads[i] = new Thread(AnalizeUrl);
                            threads[i].Start(url);
                            counter = i;
                            break;
                        }
                    }
                }
                #endregion
            }
            input.Close();

            for (int i = 0; i < threadsCount; ++i)
            {
                if (threads[i] == null) continue;
                if (threads[i].IsAlive)
                    threads[i].Join();
            }
            MessageBox.Show("File categorized!");
        }

        private void CategorizeBtn_Click(object sender, RoutedEventArgs e)
        {
            Thread thread;
            if (!SourceTb.Text.EndsWith(".txt"))
            {
                thread = new Thread(AnalizeUrl);
                thread.Start(SourceTb.Text);
            }
            else
            {
                thread = new Thread(FromFile);
                thread.Start(SourceTb.Text);
            }
        }

        private void SourceTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            CategorizeBtn.IsEnabled = true;
            CategorizeMenuItem.IsEnabled = true;
        }

        #region Menu
        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Multiselect = false;
            openFile.DefaultExt = "*.txt";
            openFile.Filter = "Text File | *.txt";
            openFile.ShowDialog();

            SourceTb.Text = openFile.FileName;

            if (SourceTb.Text.Length <= 3) return;

            CategorizeBtn.IsEnabled = true;
            CategorizeMenuItem.IsEnabled = true;
        }

        private void HelpMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("1) Scan or create profiles.\n" +
                            "2) Open file of paste URL to textbox.\n" +
                            "3) Click \"Categorize\".",
                            "Help",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("TextCategorization\n" +
                            "Version: 1.0\n" +
                            "Autor: Alexandr Halchin",
                            "About",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }

        private void CreateProfileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new CreateProfileWnd().Show();
        }

        private void ScanProfilesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ProfilesCtrl.Items.Clear();

            if (Directory.Exists(".\\Profiles"))
            {
                int profilesCount = Directory.GetFiles(".\\Profiles\\").Length;

                if (profilesCount < 1)
                {
                    MessageBox.Show("Profiles not found!", 
                                    "Error", 
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    return;
                }

                TabItem[] items = new TabItem[profilesCount];

                string[] profiles = Directory.GetFiles(".\\Profiles\\");

                for (int i = 0; i < profilesCount; ++i)
                {
                    profiles[i] = profiles[i].Substring(0, profiles[i].LastIndexOf(".", StringComparison.Ordinal));
                    profiles[i] = profiles[i].Substring(profiles[i].LastIndexOf("\\", StringComparison.Ordinal) + 1);
                }

                for (int i = 0; i < profilesCount; ++i)
                {
                    items[i] = new TabItem
                    {
                        Header = profiles[i],
                        Content = new ListBox()
                    };
                    ProfilesCtrl.Items.Add(items[i]);
                }

                SourceTb.IsEnabled = true;
                FileBtn.IsEnabled = true;
            }
            else
                MessageBox.Show("Profiles directory not exists!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        #endregion
    }
}
