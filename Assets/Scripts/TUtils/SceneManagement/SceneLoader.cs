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
        /// <summary>
        ///   Load a scene
        /// </summary>
        public void LoadScene(string name)
        {
            int index = SceneUtility.GetBuildIndexByScenePath(name);
            if (index != -1)
            {
                SceneManager.LoadScene(index);
            }
        }

        /// <summary>
        ///   Load a scene asynchronously
        /// </summary>
        public void LoadSceneAsynchronously(string name)
        {
            SceneUtil.LoadSceneAsynchronously(name);
        }

        /// <summary>
        ///   Reload the current active scene asynchronously
        /// </summary>
        public void ReloadCurrentSceneAsynchronously()
        {
            SceneUtil.ReloadCurrentSceneAsynchronously();
        }
    }
}