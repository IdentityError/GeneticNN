// Copyright (c) 2020 Matteo Beltrame

using System;
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
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="data"> </param>
        /// <param name="path"> </param>
        /// <returns> SaveObject instance, null on error </returns>
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
        ///   Load data from the specified path
        /// </summary>
        /// <param name="path"> </param>
        /// <returns>
        ///   A SaveObject instance. Use
        ///   <code>saveObject.GetData() </code>
        ///   to retrieve data. If the data is not present a null SaveObject will be returned
        /// </returns>
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

        /// <summary>
        ///   Delete data
        /// </summary>
        /// <param name="path"> </param>
        public static void DeleteObjectData(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        ///   Serialize a string to a file
        /// </summary>
        /// <param name="path"> </param>
        /// <param name="data"> </param>
        /// <param name="append"> </param>
        public static void SerializeToFile(string path, string data, bool append)
        {
            System.IO.Directory.CreateDirectory("data");
            if (append)
            {
                File.AppendAllText(path, data + Environment.NewLine);
            }
            else
            {
                File.WriteAllText(path, data + Environment.NewLine);
            }
        }
    }
}