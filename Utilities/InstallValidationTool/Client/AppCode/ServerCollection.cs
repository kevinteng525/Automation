using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Client.AppCode
{
    public class ServerCollection : INotifyPropertyChanged
    {
        private string displayName;

        public string DisplayName
        {
            get
            {
                return displayName;
            }
            set
            {
                displayName = value;
                NotifyPropertyChanged("DisplayName");
            }
        }

        private ObservableCollection<Machine> machines;

        public ObservableCollection<Machine> Machines
        {
            get
            {
                return machines;
            }
            set
            {
                machines = value;
                NotifyPropertyChanged("Machines");
            }
        }

        public ServerCollection()
        {
            Machines = new ObservableCollection<Machine>();
        }

        private void NotifyPropertyChanged(String propertyName)
        {
            if (null != PropertyChanged)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
