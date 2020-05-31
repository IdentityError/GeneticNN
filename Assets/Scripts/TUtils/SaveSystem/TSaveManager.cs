// Copyright (c) 2020 Matteo Beltrame

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Assets.Scripts.TUtils.SaveSystem
{
    public static class TSaveManager
    {
        //Can write here the static file names to use in the game
        //I.E
        //public static readonly string PLAYER_DATA = Application.persistentDataPath + "/player_data.data";
        //Calling the methods will look like this:
        //SaveManager.GetInstance().SavePersistentData<T>(data, SaveManager.PLAYER_DATA);

        /// <summary>
        ///   Save a generic type of data in the application persisten data path.
        ///   <para> Returns: a SaveObject instance, null on error </para>
        /// </summary>
        public static SaveObject SavePersistentData<T>(T data, string path)
        {
            SaveObject saveObject = new SaveObject(data);
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);
            formatter.Serialize(stream, data);
            stream.Close();
            return saveObject;
        }

        /// <summary>
        ///   Load a SaveObject that contains the type of data in the selected path.
        ///   <para>
        ///     Return a SaveObject. Use saveObject.GetData() to retrieve data. If the data is not present a null SaveObject will be returned
        ///   </para>
        /// </summary>
        public static SaveObject LoadPersistentData(string path)
        {
            SaveObject saveObject;
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);
                if (stream.Length == 0)
                    return null;
                object data = formatter.Deserialize(stream);
                if (data is EncryptedData)
                {
                    EncryptedData enc = (EncryptedData)data;
                    if (enc.GetDeviceID() != SystemInfo.deviceUniqueIdentifier)
                    {
                        Debug.LogError("Unauthorized to open encrypted file, identifiers not matching, aborting");
                        stream.Close();
                        return null;
                    }
                }
                saveObject = new SaveObject(data);
                stream.Close();
                return saveObject;
            }
            else
            {
                return null;
            }
        }

        public static void DeleteData(string path)
        {
            File.Delete(path);
        }
    }
}