using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Protoype
{
    public class AddFileDialog : Window
    {
        TextBox PathField { get; set; }
        public AddFileDialog()
        {
            AvaloniaXamlLoader.Load(this);

            PathField = this.FindControl<TextBox>("PathField");

            this.AttachDevTools();
        }

        public void OKCommand(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            this.Close(PathField.Text);
        }
        public void AbortCommand(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            this.Close();
        }

        new public event PropertyChangedEventHandler PropertyChanged;

        protected bool RaiseAndSetIfChanged<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                RaisePropertyChanged(propertyName);
                return true;
            }
            return false;
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
