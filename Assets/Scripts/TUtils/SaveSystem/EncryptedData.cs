// Copyright (c) 2020 Matteo Beltrame

using UnityEngine;

namespace Assets.Scripts.TUtils.SaveSystem
{
    [System.Serializable]
    public class EncryptedData
    {
        protected string deviceId;

        protected EncryptedData()
        {
            deviceId = deviceId == null ? SystemInfo.deviceUniqueIdentifier : deviceId;
        }

        internal string GetDeviceID()
        {
            return deviceId;
        }
    }
}