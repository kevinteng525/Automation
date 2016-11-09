using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Xml.Linq;
using Client.AppCode;

namespace Client
{
    public class AppContext : INotifyPropertyChanged
    {
        private readonly System.Timers.Timer refreshCollectionTimer;

        private readonly System.Timers.Timer refreshMathineTimer;

        #region properties

        private ServerCollection currentCollection;

        public ServerCollection CurrentCollection
        {
            get { return currentCollection; }
            set
            {
                currentCollection = value;
                NotifyPropertyChanged("CurrentCollection");
            }
        }

        private Machine currentMachine;

        public Machine CurrentMachine
        {
            get { return currentMachine; }
            set
            {
                currentMachine = value;
                NotifyPropertyChanged("CurrentMachine");
            }
        }

        private ObservableCollection<ServerCollection> collections;

        public ObservableCollection<ServerCollection> Collections
        {
            get { return collections; }
            set
            {
                collections = value;
                NotifyPropertyChanged("Collections");
            }
        }

        #endregion

        private void NotifyPropertyChanged(String propertyName)
        {
            if (null != PropertyChanged)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public AppContext()
        {
            try
            {
                LoadMachines();
            }
            catch (Exception)
            {
                MessageBox.Show("Fail to load machines.xml", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);

                Application.Current.Shutdown();
            }

            // init timer
            refreshCollectionTimer = new System.Timers.Timer(1000 * 10) { Enabled = true };
            refreshCollectionTimer.Start();
            refreshCollectionTimer.Elapsed += RefreshCollection;

            refreshMathineTimer = new System.Timers.Timer(1000 * 5) { Enabled = true };
            refreshMathineTimer.Start();
            refreshMathineTimer.Elapsed += RefreshMachine;

            // Refresh Info In Backgroud for performance
            RefreshAllMachinesSync();
        }

        // Machine.xml
        public void LoadMachines()
        {
            Collections = new ObservableCollection<ServerCollection>();

            var machinesRoot = XElement.Load("machines.xml");

            foreach (var collectionRoot in machinesRoot.Elements("Collection"))
            {
                var collection = new ServerCollection
                {
                    DisplayName = collectionRoot.Attribute("name").Value
                };

                foreach (var machineRoot in collectionRoot.Elements("Machine"))
                {
                    collection.Machines.Add
                    (
                        new Machine
                        {
                            DisplayName = machineRoot.Attribute("name").Value,
                            ServerIP = machineRoot.Attribute("ip").Value
                        }
                    );
                }

                Collections.Add(collection);
            }
        }

        public void SaveMachines()
        {
            var machinesRoot = new XElement("Mathines");

            foreach (var collection in Collections)
            {
                var collectionRoot = new XElement("Collection");
                collectionRoot.Add(new XAttribute("name", collection.DisplayName));

                foreach (var machine in collection.Machines)
                {
                    var machineRoot = new XElement("Machine");
                    machineRoot.Add(new XAttribute("name", machine.DisplayName));
                    machineRoot.Add(new XAttribute("ip", machine.ServerIP));

                    collectionRoot.Add(machineRoot);
                }

                machinesRoot.Add(collectionRoot);
            }

            try
            {
                machinesRoot.Save("machines.xml");
            }
            catch (Exception)
            {
                throw new Exception("Failed to save changes to local machine");
            }
        }

        // Refresh Method
        #region Refresh Machines Status

        public void RefreshAllMachine()
        {
            foreach (ServerCollection collection in Collections)
            {
                RefreshCollectionSync(collection);
            }
        }

        public void RefreshCollection(ServerCollection collection)
        {
            if (collection != null)
            {
                foreach (Machine machine in collection.Machines)
                {
                    RefreshMachineSync(machine);
                }
            }
        }

        public  void RefreshMachine(Machine machine)
        {
            if (machine != null)
            {
                machine.UpdateMachineStatus();
            }
        }

        // Timer Method
        private void RefreshCollection(object sender, EventArgs e)
        {
            RefreshCollection(CurrentCollection);
        }

        private void RefreshMachine(object sender, EventArgs e)
        {
            RefreshMachine(CurrentMachine);
        }

        // Background worker job
        private void bgWorkRefreshAll_DoWork(object sender, DoWorkEventArgs e)
        {
            RefreshAllMachine();
        }

        private void bgWorkRefreshCollection_DoWork(object sender, DoWorkEventArgs e)
        {
            var collection = e.Argument as ServerCollection;

            if (collection != null)
            {
                RefreshCollection(collection);
            }
        }

        private void bgWorkRefreshtMachine_DoWork(object sender, DoWorkEventArgs e)
        {
            var machine = e.Argument as Machine;

            if (machine != null)
            {
                RefreshMachine(machine);
            }
        }

        private void bkWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        // Sync
        public void RefreshAllMachinesSync()
        {
            var bgWorkRefreshAll = new BackgroundWorker();
            bgWorkRefreshAll.RunWorkerAsync();
            bgWorkRefreshAll.DoWork += bgWorkRefreshAll_DoWork;
            bgWorkRefreshAll.RunWorkerCompleted += bkWorker_RunWorkerCompleted;
        }

        public void RefreshCollectionSync(ServerCollection collection)
        {
            var bgWorkRefreshCollection = new BackgroundWorker();
            bgWorkRefreshCollection.RunWorkerAsync(collection);
            bgWorkRefreshCollection.DoWork += bgWorkRefreshCollection_DoWork;
            bgWorkRefreshCollection.RunWorkerCompleted += bkWorker_RunWorkerCompleted;
        }

        public void RefreshMachineSync(Machine machine)
        {
            var bgWorkRefreshMachine = new BackgroundWorker();
            bgWorkRefreshMachine.RunWorkerAsync(machine);
            bgWorkRefreshMachine.DoWork += bgWorkRefreshtMachine_DoWork;
            bgWorkRefreshMachine.RunWorkerCompleted += bkWorker_RunWorkerCompleted;
        }

        #endregion
    }
}
