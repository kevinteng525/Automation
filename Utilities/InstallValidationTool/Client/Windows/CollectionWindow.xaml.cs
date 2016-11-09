using System.Windows;
using Client.AppCode;

namespace Client.Windows
{
    /// <summary>
    /// Interaction logic for CollectionWindow.xaml
    /// </summary>
    public partial class CollectionWindow
    {
        private readonly ServerCollection currentCollection;

        private readonly string operation;

        public CollectionWindow(string operation)
        {
            InitializeComponent();

            this.operation = operation;

            if (operation == "NEW")
            {
                currentCollection = new ServerCollection();

                this.Height = 150;
                OkCancelBar.Visibility = Visibility.Visible;
            }
            else if (operation == "EDIT")
            {
                currentCollection = App.Context.CurrentCollection;

                this.Height = 90;
                OkCancelBar.Visibility = Visibility.Collapsed;
            }

            this.DataContext = currentCollection;

            this.txtDisplayName.Focus();
        }

        private void confrim_Click(object sender, RoutedEventArgs e)
        {
            if (operation == "NEW")
            {
                App.Context.Collections.Add(currentCollection);
            }

            this.Close();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
