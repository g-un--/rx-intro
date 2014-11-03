using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
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

namespace RxSearchBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var textChanged = Observable
                .FromEventPattern(searchTextBox, "TextChanged")
                .Select(_ => searchTextBox.Text.Trim());

            var input = textChanged
                .Where(text => !string.IsNullOrEmpty(text))
                .Throttle(TimeSpan.FromSeconds(1))
                .DistinctUntilChanged();

            var subscription = input
                .SelectMany(searchInput => GetFromMovieDb(searchInput).TakeUntil(input))
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(result => searchResults.Text = result);
        }

        IObservable<string> GetFromMovieDb(string input)
        {
            var searchMovieDb = "http://api.themoviedb.org/3/search/movie?query={0}&&api_key=6ce0ef5b176501f8c07c634dfa933cff";
            var httpClient = new HttpClient();

            var searchString = string.Join("+", input.Split(" ".ToCharArray()));
            var searchUrl = string.Format(searchMovieDb, searchString);

            return Observable.FromAsync<string>(() => httpClient.GetStringAsync(searchUrl));
        }
    }
}
