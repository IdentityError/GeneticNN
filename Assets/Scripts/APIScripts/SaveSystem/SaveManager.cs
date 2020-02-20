using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager
{
    private static SaveManager instance = null;

    /// <summary>
    ///   Returns: the SaveManager singleton instance
    /// </summary>
    public static SaveManager GetInstance()
    {
        if (instance == null)
            instance = new SaveManager();
        return instance;
    }

    //Can write here the static file names to use in the game
    public static readonly string FITTEST_DATA = Application.persistentDataPath + "/fittest.data";
    //I.E
    //public static readonly string PLAYER_DATA = Application.persistentDataPath + "/player_data.data";
    //Calling the methods will look like this:
    //SaveManager.GetInstance().SavePersistentData<T>(data, SaveManager.PLAYER_DATA);

    /// <summary>
    ///   Save a generic type of data in the application persisten data path.
    ///   <para> Returns: a SaveObject instance, null on error </para>
    /// </summary>
    public SaveObject SavePersistentData<T>(T data, string path)
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
    ///   <para> Returns: a SaveObject instance, null on error </para>
    /// </summary>
    public SaveObject LoadPersistentData(string path)
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            if (stream.Length == 0)
                return null;
            object data = formatter.Deserialize(stream);
            SaveObject saveObject = new SaveObject(data);
            stream.Close();
            return saveObject;
        }
        else
        {
            return null;
        }
    }
}