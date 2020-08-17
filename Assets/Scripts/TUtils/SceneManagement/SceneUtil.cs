// Copyright (c) 2020 Matteo Beltrame

using UnityEngine.SceneManagement;

namespace Assets.Scripts.TUtils.SceneManagement
{
    public static class SceneUtil
    {
        public const string ASYNCLOADER_SCENE_NAME = "AsyncLoader";

        //Can put here the names of the scenes in the project
        // I.E
        // public static const string SHOP_SCENE_NAME = "ShopeScene";
        //

        public static void LoadSceneAsynchronously(string name)
        {
            int index = SceneUtility.GetBuildIndexByScenePath(name);
            if (index != -1)
            {
                int asyncSceneIndex = SceneUtility.GetBuildIndexByScenePath(ASYNCLOADER_SCENE_NAME);
                AsyncLoadIndexSaver.SetIndexToPreload(index);
                SceneManager.LoadScene(asyncSceneIndex);
            }
        }

        public static void ReloadCurrentSceneAsynchronously()
        {
            int index = SceneManager.GetActiveScene().buildIndex;
            if (index != -1)
            {
                int asyncSceneIndex = SceneUtility.GetBuildIndexByScenePath(ASYNCLOADER_SCENE_NAME);
                AsyncLoadIndexSaver.SetIndexToPreload(index);
                SceneManager.LoadScene(asyncSceneIndex);
            }
        }
    }
}