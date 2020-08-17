// Copyright (c) 2020 Matteo Beltrame

namespace Assets.Scripts.TUtils.SceneManagement
{
    /// <summary>
    ///   Static class to save the index of the scene to load asynchronously
    /// </summary>
    public static class AsyncLoadIndexSaver
    {
        private static int indexToPreload;

        public static int GetSceneIndexToPreload()
        {
            return indexToPreload;
        }

        public static void SetIndexToPreload(int index)
        {
            indexToPreload = index;
        }
    }
}