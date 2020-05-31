// Copyright (c) 2020 Matteo Beltrame

namespace Assets.Scripts.TUtils.SaveSystem
{
    public class SaveObject
    {
        private object data;

        public SaveObject(object data)
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
}