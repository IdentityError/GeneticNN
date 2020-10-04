// Copyright (c) 2020 Matteo Beltrame

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.TUtils.SceneManagement
{
    /// <summary>
    ///   Utilities class to manage the scene management. Attach this component to a GameObject and use the EventTrigger component to
    ///   trigger the functions
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        public void LoadScene(string name)
        {
            int index = SceneUtility.GetBuildIndexByScenePath(name);
            if (index != -1)
            {
                SceneManager.LoadScene(index);
            }
        }

        /// <summary>

        public void LoadSceneAsynchronously(string name)
        {
            SceneUtil.LoadSceneAsynchronously(name);
        }

        public void ReloadCurrentSceneAsynchronously()
        {
            SceneUtil.ReloadCurrentSceneAsynchronously();
        }
    }
}