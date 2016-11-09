using System.Windows;
using Client.AppCode;

namespace Client.Windows
{
    /// <summary>
    /// Interaction logic for MachineWindow.xaml
    /// </summary>
    public partial class MachineWindow
    {
        private readonly Machine currentMachine;

        private readonly string operation;

        public MachineWindow(string operation)
        {
            InitializeComponent();

            this.operation = operation;

            if (operation == "NEW")
            {
                currentMachine = new Machine();

                this.Height = 180;
                OkCancelBar.Visibility = Visibility.Visible;
            }
            else if (operation == "EDIT")
            {
                currentMachine = App.Context.CurrentMachine;

                this.Height = 120;
                OkCancelBar.Visibility = Visibility.Collapsed;
            }

            this.DataContext = currentMachine;
        }

        private void confrim_Click(object sender, RoutedEventArgs e)
        {
            if (operation == "NEW")
            {
                App.Context.CurrentCollection.Machines.Add(currentMachine);
            }

            this.Close();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
