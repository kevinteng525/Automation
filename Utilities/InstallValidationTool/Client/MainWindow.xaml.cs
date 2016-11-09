using System.Windows;
using System.Windows.Controls;
using Client.AppCode;
using Client.Windows;
using Common.FileCommon;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly AppContext context = App.Context;

        public MainWindow()
        {
            InitializeComponent();

            root.DataContext = context;
        }

        #region Collection

        private void listCollections_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listCollections.SelectedIndex >= 0)
            {
                context.CurrentCollection = context.Collections[listCollections.SelectedIndex];
                context.RefreshCollectionSync(context.CurrentCollection);
            }
            else
            {
                context.CurrentCollection = null;
            }
        }

        private void CollectionOperation_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;

            if (menuItem != null)
            {
                switch (menuItem.Header.ToString())
                {
                    case "Add Collection":
                        var add = new CollectionWindow("NEW") { Owner = this };
                        add.ShowDialog();
                        context.RefreshMachineSync(context.CurrentMachine);
                        break;

                    case "Edit Collection":
                        var edit = new CollectionWindow("EDIT") { Owner = this };
                        edit.ShowDialog();
                        context.RefreshMachineSync(context.CurrentMachine);
                        break;

                    case "Delete Collection":
                        if (MessageBox.Show("Delete this Collection?", "Delete", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                        {
                            context.Collections.Remove(context.CurrentCollection);
                        }
                        break;
                }

                App.Context.SaveMachines();
            }
        }

        #endregion

        #region Machine

        private void listMachines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listMachines.SelectedIndex >= 0)
            {
                context.CurrentMachine = context.CurrentCollection.Machines[listMachines.SelectedIndex];
                context.RefreshMachineSync(context.CurrentMachine);
            }
            else
            {
                context.CurrentMachine = null;
            }
        }

        private void CopyDirectory_Click(object sender, RoutedEventArgs e)
        {
            if (context.CurrentMachine.Status == MachineStatus.Avaliavle || context.CurrentMachine.Status == MachineStatus.ServiceInstalled)
            {
                var copyWindow = new CopyBuildWindow { Owner = this };
                copyWindow.Show();
            }
            else
            {
                MessageBox.Show("Wait for the target server avaliable", "Invalid Operation", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void MachineOperation_Click(object sender, RoutedEventArgs e)
        {
            if (App.Context.CurrentCollection == null)
            {
                MessageBox.Show("Please select a server collection", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning) ;

                return;
            }

            var menuItem = sender as MenuItem;

            if (menuItem != null)
            {
                switch (menuItem.Header.ToString())
                {
                    case "Add Server":
                        var add = new MachineWindow("NEW") { Owner = this };
                        add.ShowDialog();
                        context.RefreshMachine(context.CurrentMachine);
                        break;

                    case "Edit Server":
                        var edit = new MachineWindow("EDIT") { Owner = this };
                        edit.ShowDialog();
                        context.RefreshMachine(context.CurrentMachine);
                        break;

                    case "Delete Server":
                        if (MessageBox.Show("Delete this Machine?", "Delete", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                        {
                            context.CurrentCollection.Machines.Remove(context.CurrentMachine);
                        }
                        break;
                }

                App.Context.SaveMachines();
            }
        }

        private void InsatllService_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;

            if (menuItem != null)
            {
                var install = new InstallServiceWindow(menuItem.Header.ToString()) { Owner = this };
                install.ShowDialog();

                context.RefreshMachine(context.CurrentMachine);
            }
        }

        private void MachineVerify_Click(object sender, RoutedEventArgs e)
        {
            if (context.CurrentMachine.Status != MachineStatus.ServiceInstalled)
            {
                MessageBox.Show("Wait for the validation service ready in target server", "Invalid Operation", MessageBoxButton.OK, MessageBoxImage.Warning);

                return;
            }

            if (context.CurrentMachine.ServiceVersion != App.RequestServiceVersion)
            {
                MessageBox.Show(string.Format("Request ValidationService {0} on Target Machine!", App.RequestServiceVersion), "Version Error", MessageBoxButton.OK, MessageBoxImage.Warning);

                return;
            }

            ValidaionWindow validation = new ValidaionWindow { Owner = this };
            validation.ShowDialog();
        }

        #endregion

        private void Image_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (FileHelper.IsExistsFile(@"Documents\Validation Tool User Guide.pdf"))
            {
                System.Diagnostics.Process.Start(@"Documents\Validation Tool User Guide.pdf");
            }
        }
    }
}
