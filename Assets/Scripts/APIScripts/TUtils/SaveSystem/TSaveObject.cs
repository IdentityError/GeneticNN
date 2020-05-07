public class TSaveObject
{
    private object data;

    public TSaveObject(object data)
    {
        this.data = data;
    }

    /// <summary>
    ///   Returns: casted Type data
    /// </summary>
    public T GetData<T>()
    {
        return (T)data;
    }
}