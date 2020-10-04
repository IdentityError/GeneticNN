// Copyright (c) 2020 Matteo Beltrame

using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.TUtils.Utils
{
    public class TUtilsUI
    {
        /// <summary>
        ///   Coroutine that fills an image in a specified time. (Image has to be set to filled image)
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="image"> </param>
        /// <param name="duration"> </param>
        /// <returns> IEnumerator reference </returns>
        public static IEnumerator FillImage(MonoBehaviour context, Image image, float duration)
        {
            IEnumerator coroutine = FillImage_C(image, duration);
            context.StartCoroutine(coroutine);
            return coroutine;
        }

        private static IEnumerator FillImage_C(Image image, float duration)
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
        ///   Coroutine that fills an image in a specified time and calls a specified delegate at the end of the coroutine (Image has to be
        ///   set to filled image)
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="context"> </param>
        /// <param name="image"> </param>
        /// <param name="duration"> </param>
        /// <param name="funcToCall"> </param>
        /// <param name="funcParameter"> </param>
        /// <returns> IEnumerator reference </returns>
        public static IEnumerator FillImage<T>(MonoBehaviour context, Image image, float duration, Action<T> funcToCall, T funcParameter)
        {
            IEnumerator coroutine = FillImage_C<T>(image, duration, funcToCall, funcParameter);
            context.StartCoroutine(coroutine);
            return coroutine;
        }

        private static IEnumerator FillImage_C<T>(Image image, float duration, Action<T> funcToCall, T funcParameter)
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
                funcToCall(funcParameter);
            }
        }

        /// <summary>
        ///   Coroutine that unfills an image in a specified time. (Image has to be set to filled image)
        /// </summary>
        /// <param name="context"> </param>
        /// <param name="image"> </param>
        /// <param name="duration"> </param>
        /// <returns> IEnumerator reference </returns>
        public static IEnumerator UnfillImage(MonoBehaviour context, Image image, float duration)
        {
            IEnumerator coroutine = UnfillImage_C(image, duration);
            context.StartCoroutine(coroutine);
            return coroutine;
        }

        private static IEnumerator UnfillImage_C(Image image, float duration)
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
        ///   Coroutine that unfills an image in a specified time and calls a specified delegate at the end of the coroutine (Image has to
        ///   be set to filled image)
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="context"> </param>
        /// <param name="image"> </param>
        /// <param name="duration"> </param>
        /// <param name="funcToCall"> </param>
        /// <param name="funcParameter"> </param>
        /// <returns> IEnumerator reference </returns>
        public static IEnumerator UnfillImage<T>(MonoBehaviour context, Image image, float duration, Action<T> funcToCall, T funcParameter)
        {
            IEnumerator coroutine = UnfillImage_C<T>(image, duration, funcToCall, funcParameter);
            context.StartCoroutine(coroutine);
            return coroutine;
        }

        private static IEnumerator UnfillImage_C<T>(Image image, float duration, Action<T> funcToCall, T funcParameter)
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
                funcToCall(funcParameter);
            }
        }

        /// <summary>
        ///   Draw a line from a point to another
        /// </summary>
        public static void DrawSpriteLine(Vector3 from, Vector3 to, Image lineImage)
        {
            DrawSpriteLine_A(from, to, 1F, lineImage, null);
        }

        /// <summary>
        ///   Draw a line from a point to another
        /// </summary>
        public static void DrawSpriteLine(Vector3 from, Vector3 to, float thickness, Image lineImage)
        {
            DrawSpriteLine_A(from, to, thickness, lineImage, null);
        }

        /// <summary>
        ///   Draw a line from a point to another
        /// </summary>
        public static void DrawSpriteLine(Vector3 from, Vector3 to, Image lineImage, Transform parent)
        {
            DrawSpriteLine_A(from, to, 1F, lineImage, parent);
        }

        /// <summary>
        ///   Draw a line from a point to another
        /// </summary>
        public static void DrawSpriteLine(Vector3 from, Vector3 to, float thickness, Image lineImage, Transform parent)
        {
            DrawSpriteLine_A(from, to, thickness, lineImage, parent);
        }

        private static void DrawSpriteLine_A(Vector3 from, Vector3 to, float thickness, Image lineImage, Transform parent)
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

        /// <summary>
        /// </summary>
        /// <param name="seconds"> </param>
        /// <returns> The formatted string in the form: <b> H:M:S </b> </returns>
        public static string GetTimeStringFromSeconds(float seconds)
        {
            int hours = 0;
            int minutes = 0;
            while (seconds / 3600 >= 1)
            {
                hours++;
                seconds -= 3600;
            }
            while (seconds / 60 >= 1)
            {
                minutes++;
                seconds -= 60;
            }
            return new StringBuilder().Append(hours.ToString())
                                      .Append("h : ")
                                      .Append(minutes.ToString())
                                      .Append("m : ")
                                      .Append(seconds.ToString("0.0"))
                                      .Append("s").ToString();
        }
    }
}