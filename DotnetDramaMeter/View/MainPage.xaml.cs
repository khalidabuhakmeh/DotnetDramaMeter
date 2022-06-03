using System.ComponentModel;
using DotnetDramaMeter.ViewModel;
using Microsoft.Maui.Platform;

namespace DotnetDramaMeter.View;

public partial class MainPage
{
    private readonly IVibration vibration;

    public MainPage(MainViewModel viewModel, IVibration vibration)
    {
        this.vibration = vibration;
        InitializeComponent();
        BindingContext = viewModel;
        viewModel.PropertyChanged += OnPropertyChanged;
    }

    private async void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(MainViewModel.Drama)
            && BindingContext is MainViewModel { IsNotBusy: true } vm)
        {
            vm.IsBusy = true;
            RandomButton.Style = (Style)Application.Current?.Resources["DisabledButton"];

            // rotate to -90 (left)
            DisplayFrame.RotateXTo(-360, 500, Easing.BounceIn)
                .ContinueWith(_ => DisplayFrame.RotationX = 0);
            
            await Needle.RotateTo(-120, easing: Easing.SpringIn);
            await Needle.RotateTo(vm.Drama.Rotation, easing: Easing.SpringOut);
            
            await DisplayFrame.ScaleTo(1.2, 100, easing: Easing.SpringIn);
            await DisplayFrame.ScaleTo(1, 100, easing: Easing.SpringOut);

            RandomButton.Style = null;
            
            vm.IsBusy = false;
            
            vibration.Vibrate(TimeSpan.FromMilliseconds(500));
        }
    }
}

public static class ViewExtensions
{
    public static Task<bool> ColorTo(this VisualElement self, Color fromColor, Color toColor, Action<Color> callback, uint length = 250, Easing easing = null)
    {
        Color Transform(double t) => 
            Color.FromRgba(fromColor.Red + t * (toColor.Red - fromColor.Red), fromColor.Green + t * (toColor.Green - fromColor.Green), fromColor.Blue + t * (toColor.Blue - fromColor.Blue), fromColor.Alpha + t * (toColor.Alpha - fromColor.Alpha));

        return ColorAnimation(self, "ColorTo", Transform, callback, length, easing);
    }

    public static void CancelAnimation(this VisualElement self)
    {
        self.AbortAnimation("ColorTo");
    }

    static Task<bool> ColorAnimation(VisualElement element, string name, Func<double, Color> transform, Action<Color> callback, uint length, Easing easing)
    {
        easing = easing ?? Easing.Linear;
        var taskCompletionSource = new TaskCompletionSource<bool>();
        element.Animate<Color>(name, transform, callback, 16, length, easing, (v, c) => taskCompletionSource.SetResult(c));
        return taskCompletionSource.Task;
    }
}