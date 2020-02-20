using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SharedUtilities
{
    private static SharedUtilities instance = null;
    /// <summary>
    ///   Returns: the SharedUtilities singleton instance
    /// </summary>
    public static SharedUtilities GetInstance()
    {
        if (instance == null)
        {
            instance = new SharedUtilities();
        }

        return instance;
    }

    private SharedUtilities()
    {
    }

    /// <summary>
    ///   <para> Type: UTILS </para>
    ///   Start a scene with a specified delay
    /// </summary>
    public void StartSceneWithDelay(MonoBehaviour context, int index, float delay)
    {
        context.StartCoroutine(StartSceneWithDelay_C(index, delay));
    }

    private IEnumerator StartSceneWithDelay_C(int index, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(index);
    }

    /// <summary>
    ///   <para> Type: UTILS </para>
    ///   Returns: first matching Type component in children with tag
    /// </summary>
    public Type GetFirstComponentInChildrenWithTag<Type>(GameObject parent, string tag)
    {
        Transform[] transforms = parent.GetComponentsInChildren<Transform>(true);
        for (int i = 1; i < transforms.Length; i++)
        {
            if (transforms[i].tag == tag)
            {
                return transforms[i].gameObject.GetComponent<Type>();
            }
        }
        return default(Type);
    }

    /// <summary>
    ///   <para> Type: UTILS </para>
    ///   Returns: first matching Type component in parent with tag
    /// </summary>
    public Type GetFirstComponentInParentWithTag<Type>(GameObject parent, string tag)
    {
        Transform[] transforms = parent.GetComponentsInParent<Transform>(true);
        for (int i = 1; i < transforms.Length; i++)
        {
            if (transforms[i].tag == tag)
            {
                return transforms[i].gameObject.GetComponent<Type>();
            }
        }
        return default(Type);
    }

    /// <summary>
    ///   <para> Type: UTILS </para>
    ///   Returns: first child GameObject with a matching Type component
    /// </summary>
    public GameObject GetFirstChildrenWithComponent<Type>(GameObject parent)
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
    ///   <para> Type: UTILS </para>
    ///   Returns: list of all children GameObjects with the specified tag, parent excluded
    /// </summary>
    public List<GameObject> GetGameObjectsInChildrenWithTag(GameObject parent, string tag)
    {
        List<GameObject> objs = new List<GameObject>();
        Transform[] transforms = parent.GetComponentsInChildren<Transform>(true);
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
    ///   <para> Type: UI </para>
    ///   <para>
    ///     Coroutine that fills an image in a specified time. (Image has to be set to filled image)
    ///   </para>
    ///   Returns: the IEnumerator's reference
    /// </summary>
    public IEnumerator FillImage(MonoBehaviour context, Image image, float duration)
    {
        IEnumerator coroutine = FillImage_C(image, duration);
        context.StartCoroutine(coroutine);
        return coroutine;
    }

    private IEnumerator FillImage_C(Image image, float duration)
    {
        image.fillAmount = 0f;
        float stride = Time.fixedDeltaTime / duration;
        while (image.fillAmount + stride < 1)
        {
            image.fillAmount += stride;
            yield return new WaitForFixedUpdate();
        }
        image.fillAmount = 1f;
    }

    /// <summary>
    ///   <para> Type: UI </para>
    ///   <para>
    ///     Coroutine that fills an image in a specified time and calls a specified delegate at the
    ///     end of the coroutine (Image has to be set to filled image)
    ///   </para>
    ///   Returns: the IEnumerator's reference
    /// </summary>
    public IEnumerator FillImage<T>(MonoBehaviour context, Image image, float duration, Action<T> funcToCall, T funcParameters)
    {
        IEnumerator coroutine = FillImage_C<T>(image, duration, funcToCall, funcParameters);
        context.StartCoroutine(coroutine);
        return coroutine;
    }

    private IEnumerator FillImage_C<T>(Image image, float duration, Action<T> funcToCall, T funcParameters)
    {
        image.fillAmount = 0f;
        float stride = Time.fixedDeltaTime / duration;
        while (image.fillAmount + stride < 1)
        {
            yield return new WaitForFixedUpdate();
            image.fillAmount += stride;
        }
        image.fillAmount = 1f;
        if (funcToCall != null)
        {
            funcToCall(funcParameters);
        }
    }

    /// <summary>
    ///   <para> Type: UI </para>
    ///   <para>
    ///     Coroutine that unfills an image in a specified time. (Image has to be set to filled image)
    ///   </para>
    ///   Returns: the IEnumerator's reference
    /// </summary>
    public IEnumerator UnfillImage(MonoBehaviour context, Image image, float duration)
    {
        IEnumerator coroutine = UnfillImage_C(image, duration);
        context.StartCoroutine(coroutine);
        return coroutine;
    }

    private IEnumerator UnfillImage_C(Image image, float duration)
    {
        image.fillAmount = 1f;
        float stride = Time.fixedDeltaTime / duration;
        while (image.fillAmount - stride > 0)
        {
            image.fillAmount -= stride;
            yield return new WaitForFixedUpdate();
        }
        image.fillAmount = 0f;
    }

    /// <summary>
    ///   <para> Type: UI </para>
    ///   <para>
    ///     Coroutine that unfills an image in a specified time and calls a specified delegate at
    ///     the end of the coroutine (Image has to be set to filled image)
    ///   </para>
    ///   Returns: the IEnumerator's reference
    /// </summary>
    public IEnumerator UnfillImage<T>(MonoBehaviour context, Image image, float duration, Action<T> funcToCall, T funcParameters)
    {
        IEnumerator coroutine = UnfillImage_C<T>(image, duration, funcToCall, funcParameters);
        context.StartCoroutine(coroutine);
        return coroutine;
    }

    private IEnumerator UnfillImage_C<T>(Image image, float duration, Action<T> funcToCall, T funcParameters)
    {
        image.fillAmount = 1f;
        float stride = Time.fixedDeltaTime / duration;
        while (image.fillAmount - stride > 0)
        {
            yield return new WaitForFixedUpdate();
            image.fillAmount -= stride;
        }
        image.fillAmount = 0f;
        if (funcToCall != null)
        {
            funcToCall(funcParameters);
        }
    }

    /// <summary>
    ///   <para> Type: UTILS </para>
    ///   Toggle the visibility of a GameObject and all of his childrens
    /// </summary>
    public void MakeGameObjectVisible(GameObject obj, bool state)
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
    ///   <para> Type: UTILS </para>
    ///   Return: formatted string H:M:S calculated from seconds passed
    /// </summary>
    public string GetTimeStringFromSeconds(float _seconds)
    {
        int hours = 0;
        int minutes = 0;
        float seconds = 0;
        while (_seconds / 3600 >= 1)
        {
            hours++;
            _seconds -= 3600;
        }
        while (_seconds / 60 >= 1)
        {
            minutes++;
            _seconds -= 60;
        }
        seconds = _seconds;
        return hours.ToString() + "h " + minutes.ToString() + "m " + seconds.ToString("0.0") + "s";
    }
}