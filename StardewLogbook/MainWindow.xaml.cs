using StardewLogbook.Controllers;
using StardewLogbook.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StardewLogbook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private const string BASE_URL = "https://stardewvalleywiki.com/";
        private const string RECALL_HISTORY_KEYWORD = "*sim*";
        private readonly string HIST_OWNER;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        private ObservableCollection<string> _searchHistory = new ObservableCollection<string>();
        public ObservableCollection<string> SearchHistory
        {
            get => _searchHistory;
            set {
                _searchHistory = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            HIST_OWNER = $"{Environment.MachineName}\\{Environment.UserName}";
            MongoDBController.Init(owner: HIST_OWNER);
            foreach (var term in MongoDBController.Read("Rabren-Home-DB", "stardew_search_history")) {
                SearchHistory.Add(term.Content);
            }
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (webView != null && webView.CoreWebView2 != null && (((sender as string) == RECALL_HISTORY_KEYWORD) || (e.Key == Key.Enter)))
            {
                string searchTerm = txtSearch.Text;

                // Run JavaScript commands real quick
                // TODO: Use JS to get important elements for a quick data dump
                if (searchTerm.StartsWith("run ")) {
                    _ = webView.CoreWebView2.ExecuteScriptAsync($"alert('You are not safe, try an https link')");
                    if ((sender as string) != RECALL_HISTORY_KEYWORD)
                    {
                        SearchHistory.Add($"🔧 Script: {searchTerm}");
                    }

                    return;
                }

                // Fix URL formatting for mulitword search (gem node => gem_Node)
                if (searchTerm.Contains(" ")) {
                    string[] tokens = searchTerm.Split(" ");
                    string firstTerm = tokens[0];
                    string[] terms = tokens[1..];
                    searchTerm = firstTerm;
                    foreach (string term in terms) {
                        searchTerm += $"_{term.Substring(0, 1).ToUpper()}{term[1..]}";
                    }
                }

                webView.CoreWebView2.Navigate(BASE_URL + searchTerm);
                if ((sender as string) != RECALL_HISTORY_KEYWORD)
                {
                    SearchHistory.Add($"💬 Search: {searchTerm}");
                }
            }
        }

        protected void SearchTermDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string term = ((ListViewItem)sender).Content as string;
            txtSearch.Text = term.Split(":")[1].Trim();
            txtSearch.Focus();
            txtSearch_KeyDown(RECALL_HISTORY_KEYWORD, null);
        }

        private void wndMain_Closing(object sender, CancelEventArgs e)
        {
            var terms = new List<SearchTerm>();
            foreach(var term in SearchHistory) {
                terms.Add(new(){
                    TermType = term.StartsWith("🔧") ? SearchTermTypes.Script : SearchTermTypes.General,
                    Content = term,
                    Owner = HIST_OWNER,
                    Timestamp = DateTime.UtcNow
                });
            }
            MongoDBController.Write("Rabren-Home-DB", "stardew_search_history", terms);
        }

        private void lstvHistory_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            int termIndex = (sender as ListView).SelectedIndex;
            if (termIndex < 0) return;

            if (e.Key == Key.Delete)
            {
                SearchHistory.RemoveAt(termIndex);
            }
        }
    }
}
