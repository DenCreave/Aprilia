using System;
using Aprilia.Engines.Glance.Content;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform;
using Avalonia.Platform.Storage;

namespace Aprilia.Views.Pages;

public partial class IntroPageView : UserControl
{
    public IntroPageView()
    {
        InitializeComponent();
    }
    private async void OnDownloadClicked(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        try
        {
            // Tell the OS/Browser to prompt the user for a download location
            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Download CV",
                SuggestedFileName = "Julius_Deer_CV.pdf",
                DefaultExtension = ".pdf"
            });

            // If the user cancels the download, do nothing
            if (file == null) return;
            
            var resourceUri = new Uri("avares://Aprilia/Assets/Metia/Julius_Deer_CV.pdf");
            using var assetStream = AssetLoader.Open(resourceUri);

            // stream the data directly into the user's chosen download file
            await using var destinationStream = await file.OpenWriteAsync();
            await assetStream.CopyToAsync(destinationStream);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to download PDF: {ex.Message}");
        }
    }
    
    private async void OnImageTapped(object? sender, TappedEventArgs e)
    {
        // Grab the data context and execute the RelayCommand!
        if (DataContext is IntroContent content)
        {
            await content.ImageTapped();
        }
    }
}