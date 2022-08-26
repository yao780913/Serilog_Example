using System;
using System.Windows;
using Microsoft.Extensions.Logging;

namespace WpfApp1;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly ILogger<MainWindow> _logger;
    
    public MainWindow (ILogger<MainWindow> logger)
    {
        _logger = logger;
        _logger.LogInformation("hello world");
            
        InitializeComponent();
    }

    private void Btn1_OnClick(object sender, RoutedEventArgs e)
    {
        Btn1.Content += " click ";
        _logger.LogInformation("button click");
        
        try
        {
            var x = 0;
            var y = 10;

            MessageBox.Show((y/x).ToString());
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            _logger.LogError(exception, "error occured");
        }
    }
}