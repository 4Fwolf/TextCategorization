using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace TextCategorization
{
    /// <summary>
    /// Interaction logic for CreateProfileWnd.xaml
    /// </summary>
    public partial class CreateProfileWnd
    {
        private static string _profile;
        private static readonly object Locker = new object();
        private static int _valid;
        private static int _invalid;

       /* private static string DownloadText(string url)
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

        private static void AnalizeUrl(object url)
        {
            string profileName;

            #region Profiles directory
            lock (Locker)
            {
                if (!Directory.Exists("Profiles"))
                    Directory.CreateDirectory("Profiles");

                profileName = _profile;
            }
            #endregion

            string data;

            #region Download content
            try
            {
                CancellationTokenSource cancellationToken = new CancellationTokenSource();
                cancellationToken.CancelAfter(TimeSpan.FromSeconds(5));

                Task<string> download = new Task<string>(
                        () => Downloader.DownloadText((string)url)
                    );
                download.Start();
                download.Wait(cancellationToken.Token);

                data = download.Result;
            }
            catch (Exception)
            {
                lock (Locker)
                {
                    ++_invalid;
                }
                return;
            }
            #endregion

            if (data == string.Empty)
            {
                lock (Locker)
                {
                    ++_invalid;
                }
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

            #region Deserialize existing profile
            lock (Locker)
            {
                if (File.Exists(".\\Profiles\\" + profileName + ".dat"))
                {
                    List<KeyValuePair<string, int>> deserialized;
                    try
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        using (FileStream stream = new FileStream(".\\Profiles\\" + profileName + ".dat", FileMode.OpenOrCreate))
                        {
                            deserialized = (List<KeyValuePair<string, int>>)formatter.Deserialize(stream);
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                        return;
                    }
                    foreach (var item in deserialized)
                    {
                        if (nGrams.ContainsKey(item.Key))
                            nGrams[item.Key] = nGrams[item.Key] + 1;
                        else
                            nGrams.Add(item.Key, 1);
                    }
                }
            }
            #endregion

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

            #region Serialize profile
            var orderedNGrams = nGrams.OrderByDescending(p => p.Value).ToList();

            lock (Locker)
            {
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    using (FileStream stream = new FileStream(".\\Profiles\\" + profileName + ".dat", FileMode.OpenOrCreate))
                    {
                        formatter.Serialize(stream, orderedNGrams);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return;
                }
            }
            #endregion

            lock (Locker)
            {
                ++_valid;
            }
        }

        public CreateProfileWnd()
        {
            InitializeComponent();
        }

        private void ChangeFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                Multiselect = false,
                DefaultExt = "*.txt",
                Filter = "Text File | *.txt"
            };
            openFile.ShowDialog();

            InpFileTb.Text = openFile.FileName;
            try
            {
                string linksCount = LinksLb.Content.ToString();
                linksCount = linksCount.Substring(0, linksCount.IndexOf(" ", StringComparison.Ordinal) + 1);
                linksCount += File.ReadAllLines(openFile.FileName).Length.ToString();
                LinksLb.Content = linksCount;
            }
            catch (Exception)
            {
                return;
            }
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
                    ++_invalid;
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

                Dispatcher.Invoke(new MethodInvoker(() =>
                {
                    ++CreatingProgress.Value;
                }), null);
            }
            input.Close();

            for (int i = 0; i < threadsCount; ++i)
            {
                if (threads[i] == null) continue;
                if (threads[i].IsAlive)
                    threads[i].Join();
            }

            #region Rezults
            Dispatcher.Invoke(new MethodInvoker(() =>
            {
                string analized = AnalizedLb.Content.ToString();
                analized = analized.Substring(0, analized.LastIndexOf(" ", StringComparison.Ordinal) + 1);
                analized += _valid.ToString();
                AnalizedLb.Content = analized;

                _invalid = (int) CreatingProgress.Maximum - _valid;
                analized = InvalidLinksLb.Content.ToString();
                analized = analized.Substring(0, analized.LastIndexOf(" ", StringComparison.Ordinal) + 1);
                analized += _invalid;
                InvalidLinksLb.Content = analized;

                CreatingProgress.Value = _valid + _invalid;
            }), null);
            #endregion

            MessageBox.Show("Profile created!");

            #region Reset
            _valid = 0;
            _invalid = 0;
            Dispatcher.Invoke(new MethodInvoker(() =>
            {
                CreateBtn.IsEnabled = true;
                ProfileNameTb.IsEnabled = true;
                ChangeFileBtn.IsEnabled = true;
                CreatingProgress.Value = 0;

                string analized = AnalizedLb.Content.ToString();
                analized = analized.Substring(0, analized.LastIndexOf(" ", StringComparison.Ordinal) + 1);
                analized += "0";
                AnalizedLb.Content = analized;

                _invalid = (int)CreatingProgress.Maximum - _valid;
                analized = InvalidLinksLb.Content.ToString();
                analized = analized.Substring(0, analized.LastIndexOf(" ", StringComparison.Ordinal) + 1);
                analized += "0";
                InvalidLinksLb.Content = analized;
            }), null);
            #endregion
        }

        private void CreateBtn_Click(object sender, RoutedEventArgs e)
        {
            CreateBtn.IsEnabled = false;
            ProfileNameTb.IsEnabled = false;
            ChangeFileBtn.IsEnabled = false;

            string file = InpFileTb.Text;

            #region Errors check

            if (ProfileNameTb.Text.Trim().StartsWith(" ") ||
                ProfileNameTb.Text.Trim().Length < 3)
            {
                MessageBox.Show("Uncorrect profile name!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CreateBtn.IsEnabled = true;
                ProfileNameTb.IsEnabled = true;
                ChangeFileBtn.IsEnabled = true;
                return;
            }

            if (file.Trim().Length < 5)
            {
                MessageBox.Show("Missing intput file!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CreateBtn.IsEnabled = true;
                ProfileNameTb.IsEnabled = true;
                ChangeFileBtn.IsEnabled = true;
                return;
            }

            if (Uri.CheckSchemeName(file.Trim()))
            {
                MessageBox.Show("Missing intput file!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CreateBtn.IsEnabled = true;
                ProfileNameTb.IsEnabled = true;
                ChangeFileBtn.IsEnabled = true;
                return;
            }

            if (!File.Exists(file.Trim()))
            {
                MessageBox.Show("Error, file not exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CreateBtn.IsEnabled = true;
                ProfileNameTb.IsEnabled = true;
                ChangeFileBtn.IsEnabled = true;
                return;
            }

            #endregion

            _profile = ProfileNameTb.Text.Trim();

            CreatingProgress.Maximum = File.ReadAllLines(file).Length;

            Thread thread = new Thread(FromFile);
            thread.Start(file);
        }
    }
}
