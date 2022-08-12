using MauiAppTesty.BaseView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MauiAppTesty.ViewModels
{
    internal sealed class RandomFactsApiPageViewModel : BaseViewModel
    {
        private string _joke = "Here will be a joke soon...";
        private string _id;
        private string _categories;
        private DateTime _createdAt;
        private DateTime _updatedAt;
        private Uri _iconUrl;
        private Uri _url;

        public ICommand FetchNew { get; set; }
        public string Joke { get => _joke; set => RaiseAndSetIfChanged(ref _joke, value); }
        public string Id { get => _id; set => RaiseAndSetIfChanged(ref _id, value); }
        public string Categories { get => _categories; set => RaiseAndSetIfChanged(ref _categories, value); }
        public DateTime CreatedAt { get => _createdAt; set => RaiseAndSetIfChanged(ref _createdAt, value); }
        public DateTime UpdatedAt { get => _updatedAt; set => RaiseAndSetIfChanged(ref _updatedAt, value); }
        public Uri IconUrl { get => _iconUrl; set => RaiseAndSetIfChanged(ref _iconUrl, value); }
        public Uri Url { get => _url; set => RaiseAndSetIfChanged(ref _url, value); }
    }
}
