using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TUtilsProvider
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
    ///   Returns: first matching Type component in a child that has the specified tag
    /// </summary>
    public static Type GetFirstComponentInChildrenWithTag<Type>(GameObject parent, string tag, bool inactive)
    {
        Transform[] transforms = parent.GetComponentsInChildren<Transform>(inactive);
        for (int i = 1; i < transforms.Length; i++)
        {
            if (transforms[i].tag == tag)
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
    ///   Returns: first matching Type component in a parent that has the specified tag
    /// </summary>
    public static Type GetFirstComponentInParentsWithTag<Type>(GameObject parent, string tag, bool inactive)
    {
        Transform[] transforms = parent.GetComponentsInParent<Transform>(inactive);
        for (int i = 1; i < transforms.Length; i++)
        {
            if (transforms[i].tag == tag)
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
    ///   Returns: first child GameObject that has a matching type component
    /// </summary>
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
    ///   Returns: list of all children GameObjects with the specified tag, parent excluded
    /// </summary>
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
    ///   Returns: list of all parents GameObjects with the specified tag
    /// </summary>
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
    ///   Returns: first occurence of a parent with the selected tag
    /// </summary>
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
    ///   Returns: first occurence of a child with the selected tag
    /// </summary>
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
}