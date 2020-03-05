﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TUIUtils
{
    private static TUIUtils instance = null;

    /// <summary>
    ///   Returns: the SharedUtilities singleton instance
    /// </summary>
    public static TUIUtils GetInstance()
    {
        if (instance == null)
        {
            instance = new TUIUtils();
        }

        return instance;
    }

    private TUIUtils()
    {
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
    ///   Draw a line from a point to another
    /// </summary>
    public void DrawSpriteLine(Vector3 from, Vector3 to, Image lineImage)
    {
        DrawSpriteLine_A(from, to, 1F, lineImage, null);
    }

    /// <summary>
    ///   Draw a line from a point to another
    /// </summary>
    public void DrawSpriteLine(Vector3 from, Vector3 to, float thickness, Image lineImage)
    {
        DrawSpriteLine_A(from, to, thickness, lineImage, null);
    }

    /// <summary>
    ///   Draw a line from a point to another
    /// </summary>
    public void DrawSpriteLine(Vector3 from, Vector3 to, Image lineImage, Transform parent)
    {
        DrawSpriteLine_A(from, to, 1F, lineImage, parent);
    }

    /// <summary>
    ///   Draw a line from a point to another
    /// </summary>
    public void DrawSpriteLine(Vector3 from, Vector3 to, float thickness, Image lineImage, Transform parent)
    {
        DrawSpriteLine_A(from, to, thickness, lineImage, parent);
    }

    private void DrawSpriteLine_A(Vector3 from, Vector3 to, float thickness, Image lineImage, Transform parent)
    {
        Image linkRef = UnityEngine.Object.Instantiate(lineImage);
        if (parent != null)
        {
            linkRef.transform.SetParent(parent);
        }

        Vector3 differenceVector = to - from;
        linkRef.rectTransform.sizeDelta = new Vector2(differenceVector.magnitude, thickness);
        linkRef.rectTransform.pivot = new Vector2(0, 0.5f);
        linkRef.rectTransform.position = from;
        float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
        linkRef.rectTransform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}