using MauiAppTesty.BaseView;
using System.Windows.Input;

namespace MauiAppTesty.ViewModels
{
    public class SettingsPageViewModel : BaseViewModel
    {
        private string _theme;
        private IEnumerable<string> _themes;
        private ICommand _saveChanges;
        
        public string Theme { get => _theme; set => RaiseAndSetIfChanged(ref _theme, value); }
        public IEnumerable<string> Themes { get => _themes; set => RaiseAndSetIfChanged(ref _themes, value); }
        public ICommand SaveChanges { get => _saveChanges; set => RaiseAndSetIfChanged(ref _saveChanges, value); }
    }
}
