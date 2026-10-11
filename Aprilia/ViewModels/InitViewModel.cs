using Aprilia.Services;

namespace Aprilia.ViewModels;

public partial class InitViewModel(NavigationService navigator) : ViewModelBase
{
    public NavigationService Navigator { get; } = navigator;
}