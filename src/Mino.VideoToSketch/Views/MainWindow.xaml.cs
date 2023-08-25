using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Mino.VideoToSketch.ViewModels;

namespace Mino.VideoToSketch.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Wpf.Ui.Controls.UiWindow // FluentWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            Wpf.Ui.Appearance.Theme.Apply(
                Wpf.Ui.Appearance.ThemeType.Dark,     // Theme type
                Wpf.Ui.Appearance.BackgroundType.Mica, // Background type
                true);

            this.DataContext = new MainViewModel();

            this.Loaded += (sender, args) =>
            {
                Wpf.Ui.Appearance.Watcher.Watch(
                    this,                                  // Window class
                    Wpf.Ui.Appearance.BackgroundType.Mica, // Background type
                    true                                   // Whether to change accents automatically
                );
            };

            this.Closed += (sender, args) =>
            {
                if (this.DataContext is MainViewModel main)
                {
                    main.OnClosed();
                }
            };
        }
    }
}
