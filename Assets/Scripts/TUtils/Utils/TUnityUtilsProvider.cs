// Copyright (c) 2020 Matteo Beltrame

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.TUtils.Utils
{
    public class TUnityUtilsProvider
    {
        /// <summary>
        ///   <para> Type: UTILS </para>
        ///   Start a scene with a specified delay
        /// </summary>
        public static void StartSceneWithDelay(MonoBehaviour context, int index, float delay)
        {
            context.StartCoroutine(StartSceneWithDelay_C(index, delay));
        }

        private static IEnumerator StartSceneWithDelay_C(int index, float delay)
        {
            yield return new WaitForSeconds(delay);
            SceneManager.LoadScene(index);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="Type"> </typeparam>
        /// <param name="parent"> </param>
        /// <param name="tag"> </param>
        /// <param name="inactive"> </param>
        /// <returns> First matching Type component in a child that has the specified tag </returns>
        public static Type GetFirstComponentInChildrenWithTag<Type>(GameObject parent, string tag, bool inactive)
        {
            Transform[] transforms = parent.GetComponentsInChildren<Transform>(inactive);

            for (int i = 1; i < transforms.Length; i++)
            {
                if (transforms[i].tag.Equals(tag))
                {
                    Type component = transforms[i].gameObject.GetComponent<Type>();
                    if (component != null)
                    {
                        return component;
                    }
                }
            }
            return default(Type);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="Type"> </typeparam>
        /// <param name="parent"> </param>
        /// <param name="tag"> </param>
        /// <param name="inactive"> </param>
        /// <returns> First matching Type component in a parent that has the specified tag </returns>
        public static Type GetFirstComponentInParentsWithTag<Type>(GameObject parent, string tag, bool inactive)
        {
            Transform[] transforms = parent.GetComponentsInParent<Transform>(inactive);
            for (int i = 1; i < transforms.Length; i++)
            {
                if (transforms[i].tag.Equals(tag))
                {
                    Type component = transforms[i].gameObject.GetComponent<Type>();
                    if (component != null)
                    {
                        return component;
                    }
                }
            }
            return default(Type);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="Type"> </typeparam>
        /// <param name="parent"> </param>
        /// <returns> First child GameObject that has a matching type component </returns>
        public static GameObject GetFirstChildWithComponent<Type>(GameObject parent)
        {
            Type parentComponent = parent.GetComponent<Type>(), childComponent;
            foreach (Transform transform in parent.transform)
            {
                childComponent = transform.GetComponent<Type>();
                if (childComponent != null && !childComponent.Equals(parentComponent))
                {
                    return transform.gameObject;
                }
            }
            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="parent"> </param>
        /// <param name="tag"> </param>
        /// <param name="inactive"> </param>
        /// <returns> List of all children GameObjects with the specified tag, parent excluded </returns>
        public static List<GameObject> GetGameObjectsInChildrenWithTag(GameObject parent, string tag, bool inactive)
        {
            List<GameObject> objs = new List<GameObject>();
            Transform[] transforms = parent.GetComponentsInChildren<Transform>(inactive);
            for (int i = 1; i < transforms.Length; i++)
            {
                if (transforms[i].tag == tag)
                {
                    objs.Add(transforms[i].gameObject);
                }
            }
            return objs;
        }

        /// <summary>
        /// </summary>
        /// <param name="parent"> </param>
        /// <param name="tag"> </param>
        /// <param name="inactive"> </param>
        /// <returns> List of all parents GameObjects with the specified tag </returns>
        public static List<GameObject> GetGameObjectsInParentsWithTag(GameObject parent, string tag, bool inactive)
        {
            List<GameObject> objs = new List<GameObject>();
            Transform[] transforms = parent.GetComponentsInParent<Transform>(inactive);
            for (int i = 0; i < transforms.Length; i++)
            {
                if (transforms[i].tag == tag)
                {
                    objs.Add(transforms[i].gameObject);
                }
            }
            return objs;
        }

        /// <summary>
        /// </summary>
        /// <param name="parent"> </param>
        /// <param name="tag"> </param>
        /// <param name="inactive"> </param>
        /// <returns> First occurence of a parent with the selected tag </returns>
        public static GameObject GetFirstGameObjectInParentWithTag(GameObject parent, string tag, bool inactive)
        {
            Transform[] transforms = parent.GetComponentsInParent<Transform>(inactive);
            for (int i = 0; i < transforms.Length; i++)
            {
                if (transforms[i].tag == tag)
                {
                    return transforms[i].gameObject;
                }
            }
            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="parent"> </param>
        /// <param name="tag"> </param>
        /// <param name="inactive"> </param>
        /// <returns> First occurence of a child with the selected tag </returns>
        public static GameObject GetFirstGameObjectInChildrenWithTag(GameObject parent, string tag, bool inactive)
        {
            Transform[] transforms = parent.GetComponentsInChildren<Transform>(inactive);
            for (int i = 0; i < transforms.Length; i++)
            {
                if (transforms[i].tag == tag)
                {
                    return transforms[i].gameObject;
                }
            }
            return null;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="Type"> </typeparam>
        /// <param name="parent"> </param>
        /// <param name="name"> </param>
        /// <param name="inactive"> </param>
        /// <returns> First Component retrieved from the first occurrece of a child with a matching name </returns>
        public static Type GetFirstComponentInChildrenWithName<Type>(GameObject parent, string name, bool inactive)
        {
            Transform[] transforms = parent.GetComponentsInChildren<Transform>(inactive);
            for (int i = 0; i < transforms.Length; i++)
            {
                if (transforms[i].name.Equals(name))
                {
                    Type component = transforms[i].gameObject.GetComponent<Type>();
                    if (component != null)
                    {
                        return component;
                    }
                }
            }
            return default(Type);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="Type"> </typeparam>
        /// <param name="parent"> </param>
        /// <param name="name"> </param>
        /// <param name="inactive"> </param>
        /// <returns> First Component retrieved from the first occurrece of a parent with a matching name </returns>
        public static Type GetFirstComponentInParentWithName<Type>(GameObject parent, string name, bool inactive)
        {
            Transform[] transforms = parent.GetComponentsInParent<Transform>(inactive);
            for (int i = 0; i < transforms.Length; i++)
            {
                if (transforms[i].name.Equals(name))
                {
                    Type component = transforms[i].gameObject.GetComponent<Type>();
                    if (component != null)
                    {
                        return component;
                    }
                }
            }
            return default(Type);
        }

        /// <summary>
        ///   Toggle the visibility of a GameObject and all of his children
        /// </summary>
        public static void MakeGameObjectVisible(GameObject obj, bool state)
        {
            if (obj == null) return;
            Transform[] transforms = obj.GetComponentsInChildren<Transform>();
            Renderer renderer;
            int length = transforms.Length;
            for (int i = 0; i < length; i++)
            {
                renderer = transforms[i].gameObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.enabled = state;
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <returns> Whether the mouse or a touch are overlapping a game object </returns>
        public static bool IsAnyPointerOverGameObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
    }
}