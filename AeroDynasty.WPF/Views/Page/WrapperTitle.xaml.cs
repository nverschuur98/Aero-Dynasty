using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AeroDynasty.WPF.Views.Page
{
    public partial class WrapperTitle : UserControl
    {
        #region Dependency Properties

        /// <summary>
        /// Represents the title text displayed in the control.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(WrapperTitle),
                new PropertyMetadata(default(string)));

        /// <summary>
        /// Represents the icon geometry displayed in the control.
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(
                nameof(Icon),
                typeof(Geometry),
                typeof(WrapperTitle),
                new PropertyMetadata(null, OnIconChanged));

        /// <summary>
        /// Represents the visibility state of the icon.
        /// </summary>
        public static readonly DependencyProperty IconVisibilityProperty =
            DependencyProperty.Register(
                nameof(IconVisibility),
                typeof(Visibility),
                typeof(WrapperTitle),
                new PropertyMetadata(Visibility.Collapsed));

        #endregion

        #region Public Properties

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public Geometry Icon
        {
            get => (Geometry)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public Visibility IconVisibility
        {
            get => (Visibility)GetValue(IconVisibilityProperty);
            set => SetValue(IconVisibilityProperty, value);
        }

        #endregion

        #region Constructor
        public WrapperTitle()
        {
            InitializeComponent();
            UpdateIconVisibility(); // Ensure visibility is set on initialization
        }
        #endregion

        #region Private Methods

        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WrapperTitle)d).UpdateIconVisibility();
        }

        private void UpdateIconVisibility()
        {
            IconVisibility = Icon != null ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion
    }
}
