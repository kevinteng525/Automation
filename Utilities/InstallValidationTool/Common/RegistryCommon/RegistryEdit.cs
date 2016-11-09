﻿using Microsoft.Win32;

namespace Common.RegistryCommon
{
    public static class RegistryEdit
    {
        public static string ReadHKLM64(string rootKey, string keyName)
        {
            RegistryKey rk = GetHKLMKey64(rootKey);

            if (rk == null)
            {
                return null;
            }

            if (rk.GetValue(keyName) == null)
            {
                return null;
            }

            return rk.GetValue(keyName).ToString();
        }

        public static string ReadHKLM32(string rootKey, string keyName)
        {
            RegistryKey rk = GetHKLMKey32(rootKey);

            if (rk == null)
            {
                return null;
            }

            if (rk.GetValue(keyName) == null)
            {
                return null;
            }

            return rk.GetValue(keyName).ToString();
        }

        public static RegistryKey GetHKLMKey64(string keyName)
        {
            return RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(keyName);
        }

        public static RegistryKey GetHKLMKey32(string keyName)
        {
            return RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(keyName);
        }

        public static RegistryKey GetHKLMKeyDefault(string keyName)
        {
            return Registry.LocalMachine.OpenSubKey(keyName);
        }

        public static string ReadHKCR(string rootKey, string keyName)
        {
            RegistryKey rk = GetClassRootKey(rootKey);

            if (rk == null)
            {
                return null;
            }

            if (rk.GetValue(keyName) == null)
            {
                return null;
            }

            return rk.GetValue(keyName).ToString();
        }

        public static RegistryKey GetClassRootKey(string keyName)
        {
            return Registry.ClassesRoot.OpenSubKey(keyName);
        }
    }
}
