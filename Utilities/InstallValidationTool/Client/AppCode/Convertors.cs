using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using RequestLib;

namespace Client.AppCode
{
    public class SelectServerConverters : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        // Not need here
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ServerCollectionICOConverters : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        // Not need here
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CustomizedComponentsConverters : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isChecked = bool.Parse(value.ToString());

            return isChecked ? Visibility.Visible : Visibility.Collapsed;
        }

        // Not need here
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MachineStatusConverters : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (MachineStatus)Enum.Parse(typeof(MachineStatus), value.ToString(), true);

            switch (status)
            {
                case MachineStatus.Unavaliable:
                    return new SolidColorBrush(Colors.Red);

                case MachineStatus.Avaliavle:
                    return new SolidColorBrush(Colors.Yellow);

                case MachineStatus.ServiceInstalled:
                    return new SolidColorBrush(Colors.Green);

                default: 
                    return new SolidColorBrush(Colors.Gray);
            }
        }

        // Not need here
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MachineContextMenuConverters : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (MachineStatus)Enum.Parse(typeof(MachineStatus), value.ToString(), true);

            var menuItem = parameter.ToString();

            switch (menuItem)
            {
                case "Validation":
                    switch (status)
                    {
                        case MachineStatus.ServiceInstalled:
                            return true;

                        default:
                            return false;
                    }

                case "CopyDirectory":
                    switch (status)
                    {
                        case MachineStatus.Avaliavle:
                            return true;

                        case MachineStatus.ServiceInstalled:
                            return true;

                        default:
                            return false;
                    }

                case "Service":
                    switch (status)
                    {
                        case MachineStatus.Unavaliable:
                            return false;

                        case MachineStatus.Avaliavle:
                            return true;

                        case MachineStatus.ServiceInstalled:
                            return true;

                        default:
                            return false;
                    }

                case "ServiceInstall":
                    switch (status)
                    {
                        case MachineStatus.Unavaliable:
                            return false;

                        case MachineStatus.Avaliavle:
                            return true;

                        case MachineStatus.ServiceInstalled:
                            return false;

                        default:
                            return false;
                    }

                case "ServiceUninstall":
                    switch (status)
                    {
                        case MachineStatus.Unavaliable:
                            return false;

                        case MachineStatus.Avaliavle:
                            return false;

                        case MachineStatus.ServiceInstalled:
                            return true;

                        default:
                            return false;
                    }

                case "ServiceReinstall":
                    switch (status)
                    {
                        case MachineStatus.Unavaliable:
                            return false;

                        case MachineStatus.Avaliavle:
                            return false;

                        case MachineStatus.ServiceInstalled:
                            return true;

                        default:
                            return false;
                    }

                case "EditServer":
                    switch (status)
                    {
                        case MachineStatus.Unavaliable:
                            return true;

                        case MachineStatus.Avaliavle:
                            return true;

                        case MachineStatus.ServiceInstalled:
                            return true;

                        default:
                            return false;
                    }

                case "DeleteServer":
                    return true;

                default:
                    return false;
            }
        }

        // Not need here
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class VerifyResultColorConverters : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = (VerifyResult)Enum.Parse(typeof(VerifyResult), value.ToString(), true);

            switch (result)
            {
                case VerifyResult.Pass:
                    return new SolidColorBrush(Colors.Green);

                case VerifyResult.Failed:
                    return new SolidColorBrush(Colors.Red);

                case VerifyResult.Warning:
                    return new SolidColorBrush(Colors.Yellow);

                case VerifyResult.Skip:
                    return new SolidColorBrush(Colors.LightBlue);

                case VerifyResult.Unknow:
                    return new SolidColorBrush(Colors.Gray);

                default:
                    return new SolidColorBrush(Colors.Gray);
            }
        }

        // Not need here
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ShowAdvanceSettingsConverters : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isChecked = bool.Parse(value.ToString());

            return isChecked ? Visibility.Visible : Visibility.Collapsed;
        }

        // Not need here
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
