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

namespace AeroDynasty.WPF.Views.Menu
{
    /// <summary>
    /// Interaction logic for MenuButton.xaml
    /// </summary>
    public partial class MenuButton : UserControl
    {
        public static readonly DependencyProperty CommandProperty = 
            DependencyProperty.Register(
                "Command", 
                typeof(ICommand), 
                typeof(MenuButton), 
                new PropertyMetadata(null));

        public static readonly DependencyProperty LabelProperty = 
            DependencyProperty.Register(
                "Label", 
                typeof(string), 
                typeof(MenuButton), 
                new PropertyMetadata(null));

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register(
                "IsEnabled", 
                typeof(bool), 
                typeof(MenuButton), 
                new PropertyMetadata(true));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }

        public MenuButton()
        {
            InitializeComponent();
        }
    }
}
