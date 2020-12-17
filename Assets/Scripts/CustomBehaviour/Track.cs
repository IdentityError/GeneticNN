using TUtils.Utils;
using TUtils.Utils.Mathematics;
using UnityEngine;

namespace Assets.Scripts.CustomBehaviour
{
    public class Track : MonoBehaviour
    {
        private GameObject finishLine;

        private Transform startPoint;

        private float trackLength;

        /// <summary>
        ///   Get the length of this track
        /// </summary>
        /// <returns> </returns>
        public float Length()
        {
            return trackLength;
        }

        /// <summary>
        ///   Get the finish line gameObject
        /// </summary>
        /// <returns> </returns>
        public GameObject GetFinishLine()
        {
            return finishLine;
        }

        /// <summary>
        ///   Get the start point of the track
        /// </summary>
        /// <returns> </returns>
        public Transform GetStartPoint()
        {
            return startPoint;
        }

        /// <summary>
        ///   Get this track's id (name)
        /// </summary>
        /// <returns> </returns>
        public string GetId()
        {
            return gameObject.name;
        }

        private void Awake()
        {
            finishLine = UnityUtilsProvider.GetFirstGameObjectInChildrenWithTag(gameObject, "FinishLine", true);
            startPoint = UnityUtilsProvider.GetFirstComponentInChildrenWithTag<Transform>(gameObject, "StartPoint", true);
            trackLength = CalculateTrackLength();
        }

        private float CalculateTrackLength()
        {
            float length = 0;
            foreach (Transform child in transform)
            {
                if (child.tag.Equals("TrackPart"))
                {
                    length += child.localScale.z * 31;
                }
            }
            length -= TMath.Abs((finishLine.transform.localPosition - startPoint.localPosition).magnitude);
            return length;
        }
    }
}