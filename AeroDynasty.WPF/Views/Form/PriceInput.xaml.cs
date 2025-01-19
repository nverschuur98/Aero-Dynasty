using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AeroDynasty.WPF.Views.Form
{
    /// <summary>
    /// Interaction logic for PriceInput.xaml
    /// </summary>
    public partial class PriceInput : UserControl
    {
        // Dependency properties
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(
                nameof(Label),
                typeof(string),
                typeof(PriceInput),
                new PropertyMetadata(null));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(double),
                typeof(PriceInput),
                new FrameworkPropertyMetadata(
                    0.0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        // Public properties
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Constructor
        public PriceInput()
        {
            InitializeComponent();
        }

        // Handle TextBox input and convert it to a numeric value
        private void PriceInput_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow numbers, backspace, and decimal points
            e.Handled = !IsValidInput(sender as TextBox, e.Text);
        }

        // Method to validate input (only allow numeric input, and one decimal point)
        private bool IsValidInput(TextBox textBox, string input)
        {
            // Get the current text of the TextBox
            string currentText = textBox.Text + input;
            decimal parsedValue;

            // Try parsing as a valid decimal currency value
            return decimal.TryParse(currentText, NumberStyles.Currency, CultureInfo.CurrentCulture, out parsedValue);
        }
    }
}
