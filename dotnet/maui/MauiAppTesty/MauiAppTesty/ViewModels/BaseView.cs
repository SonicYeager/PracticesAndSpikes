using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiAppTesty.ViewModels;

public class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    protected bool RaiseAndSetIfChanged<TValue>(ref TValue field, TValue value, [CallerMemberName] string propertyName = null)
    {
        if (!EqualityComparer<TValue>.Default.Equals(field, value))
        {
            field = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        return false;
    }

    private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}