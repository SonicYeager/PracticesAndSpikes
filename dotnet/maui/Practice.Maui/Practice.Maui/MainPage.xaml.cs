namespace Practice.Maui;

public sealed partial class MainPage : ContentPage
{
    /// <summary>
    /// 
    /// </summary>
    private int _count = 0;

    public MainPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// This method is triggered when the counter button is clicked.
    /// It increments the counter and updates the button text with the number of clicks.
    /// Also, it announces the updated text for the semantic screen reader.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An EventArgs that contains the event data.</param>
    private void OnCounterClicked(object sender, EventArgs e)
    {
        _count++;

        CounterBtn.Text = _count == 1 ? $"Clicked {_count} time" : $"Clicked {_count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);
    }
}