using System.ComponentModel;
using System.Security.Cryptography;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DotnetDramaMeter.ViewModel;

public partial class MainViewModel : ObservableObject
{
    private readonly List<Level> _levels = new()
    {
        new Level("LOW", 0..44, Color.Parse("#1CAF54"), Colors.White),
        new Level("MODERATE", 45..89, Color.Parse("#FCCD35"), Colors.Black),
        new Level("HIGH", 90..134, Color.Parse("#F27E32"), Colors.White),
        new Level("EXTREME", 135..180, Color.Parse("#DA232C"), Colors.White)
    };

    private readonly Drama waiting = new(0, new Level("Waiting...", Range.All, Colors.Grey, Colors.WhiteSmoke));

    public MainViewModel()
    {
        // starting point
        Drama = new Drama(90, new Level("Start", Range.All, Colors.Black, Colors.White));
    }

    [ObservableProperty] 
    [AlsoNotifyChangeFor(nameof(IsNotBusy))]
    private bool isBusy;

    public bool IsNotBusy => !IsBusy;

    [ObservableProperty] 
    private Drama drama;

    [ICommand]
    async Task RandomDrama()
    {
        if (IsNotBusy)
        {
            var value = RandomNumberGenerator.GetInt32(0, 180);
            // change the value
            Drama = Drama with 
            {
                Value = value,
                Level = _levels.First(l => value >= l.Range.Start.Value && value <= l.Range.End.Value)
            };
        }
    }
}

public record Drama(int Value, Level Level)
{
    /// <summary>
    /// Goes from -90 degrees (left) to 90 degrees (right)
    /// </summary>
    public int Rotation => Value - 90;
}

public record Level(string Text, Range Range, Color BackgroundColor, Color TextColor)
{
    public Color ScreenBackgroundColor =>
        BackgroundColor
            .WithAlpha(0.3f)
            .WithSaturation(0.5f);
}