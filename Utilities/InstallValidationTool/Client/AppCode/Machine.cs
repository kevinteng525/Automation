using System;
using System.Collections.Generic;
using System.ComponentModel;
using Common.SystemCommon;
using RequestLib.Installation;
using RequestLib.Requests;

namespace Client.AppCode
{
    public class Machine : INotifyPropertyChanged
    {
        private object updateLocker = new object();

        private bool isUpdating = false;

        #region Properties

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

        private string domainName;

        public string DomainName
        {
            get
            {
                return domainName;
            }
            set
            {
                domainName = value;

                NotifyPropertyChanged("DomainName");
            }
        }

        private string serverName;

        public string ServerName
        {
            get
            {
                return serverName;
            }
            set
            {
                serverName = value;

                NotifyPropertyChanged("ServerName");
            }
        }

        private string serverIP;

        public string ServerIP
        {
            get
            {
                return serverIP;
            }
            set
            {
                serverIP = value;

                NotifyPropertyChanged("ServerIP");
            }
        }

        private string osName;

        public string OSName
        {
            get
            {
                return osName;
            }
            set
            {
                osName = value;

                NotifyPropertyChanged("OSName");
            }
        }

        private string osVersion;

        public string OSVersion
        {
            get
            {
                return osVersion;
            }
            set
            {
                osVersion = value;

                NotifyPropertyChanged("OSVersion");
            }
        }

        private string programFilesX86;

        public string ProgramFilesX86
        {
            get
            {
                return programFilesX86;
            }
            set
            {
                programFilesX86 = value;

                NotifyPropertyChanged("ProgramFilesX86");
            }
        }

        private string programData;

        public string ProgramData
        {
            get
            {
                return programData;
            }
            set
            {
                programData = value;

                NotifyPropertyChanged("ProgramData");
            }
        }

        private string apData;

        public string AppData
        {
            get
            {
                return apData;
            }
            set
            {
                apData = value;

                NotifyPropertyChanged("AppData");
            }
        }

        private string is64BitOS;

        public string Is64BitOS
        {
            get
            {
                return is64BitOS;
            }
            set
            {
                is64BitOS = value;

                NotifyPropertyChanged("Is64BitOS");
            }
        }

        private string serviceVersion;

        public string ServiceVersion
        {
            get
            {
                return serviceVersion;
            }
            set
            {
                serviceVersion = value;

                NotifyPropertyChanged("ServiceVersion");
            }
        }

        private MachineStatus status { get; set; }

        public MachineStatus Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;

                NotifyPropertyChanged("Status");
            }
        }

        private void NotifyPropertyChanged(String propertyName)
        {
            if (null != PropertyChanged)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public Machine()
        {
            DisplayName = "New Server";

            ServerIP = "127.0.0.1";

            Status = MachineStatus.Unavaliable;

            ClearMachineInfo();
        }

        private void ClearMachineInfo()
        {
            DomainName = "UNKNOW";

            ServerName = "UNKNOW";

            OSName = "UNKNOW";

            OSVersion = "UNKNOW";

            Is64BitOS = "UNKNOW";

            ProgramFilesX86 = string.Empty;

            ProgramData = string.Empty;

            AppData = string.Empty;

            ServiceVersion = string.Empty;
        }

        private void UpdateNetworkStatus()
        {
            if (ValidationService.IsValidationServiceAvaliable(ServerIP, 5000, 2000))
            {
                Status = MachineStatus.ServiceInstalled;

                return;
            }

            bool? isAvaliable = ServerStatus.IsServerAvailiable(ServerIP);

            if (isAvaliable != null)
            {
                Status = isAvaliable.Value ? MachineStatus.Avaliavle : MachineStatus.Unavaliable;
                return;
            }

            Status = MachineStatus.Unknow;
        }

        private void UpdateMachineInfo()
        {
            Dictionary<string, string> serverStatusDic = new StatusRequest(serverIP, 5000).GetStatus();

            foreach (var pair in serverStatusDic)
            {
                switch (pair.Key.ToUpper())
                {
                    case "DOMAINNAME":
                        DomainName = pair.Value;
                        break;
                    case "SERVERNAME":
                        ServerName = pair.Value;
                        break;
                    case "OSVERSION":
                        OSVersion = pair.Value;
                        break;
                    case "OSNAME":
                        OSName = pair.Value;
                        break;
                    case "IS64BITOS":
                        Is64BitOS = pair.Value;
                        break;
                    case "SERVICEVERSION":
                        ServiceVersion = pair.Value;
                        break;
                    case "PROGRAMFILESX86":
                        ProgramFilesX86 = pair.Value;
                        break;
                    case "PROGRAMDATA":
                        ProgramData = pair.Value;
                        break;
                    case "APPDATA":
                        AppData = pair.Value;
                        break;
                }
            }
        }

        public void UpdateMachineStatus()
        {
            if (!isUpdating)
            {
                lock (updateLocker)
                {
                    isUpdating = true;

                    try
                    {
                        UpdateNetworkStatus();

                        if (Status == MachineStatus.ServiceInstalled)
                        {
                            UpdateMachineInfo();
                        }
                        else
                        {
                            // if service is not install, clear the machine info
                            ClearMachineInfo();
                        }
                    }
                    finally
                    {
                        isUpdating = false;
                    }
                }
            }
        }
    }

    public enum MachineStatus
    {
        Unknow,
        Unavaliable,
        Avaliavle,
        ServiceInstalled,
    }
}
