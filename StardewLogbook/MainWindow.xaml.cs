using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class MainWindow : Window
    {
        private const string BASE_URL = "https://stardewvalleywiki.com/";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (webView != null && webView.CoreWebView2 != null && e.Key == Key.Enter)
            {
                string searchTerm = txtSearch.Text;

                // Run JavaScript commands real quick
                // TODO: Use JS to get important elements for a quick data dump
                if (searchTerm.Contains("run")) {
                    webView.CoreWebView2.ExecuteScriptAsync($"alert('You are not safe, try an https link')");
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
            }
        }
    }
}
