using GibFrame.Extensions;
using GibFrame.Utils.Mathematics;
using UnityEngine;

namespace Assets.Scripts.CustomBehaviour
{
    public class Track : MonoBehaviour
    {
        private Transform finishLine;

        private Transform startPoint;

        private float trackLength;

        public float Length => trackLength;

        public GameObject FinishLine => finishLine.gameObject;

        public Transform StartPoint => startPoint;

        public string Id => gameObject.name;

        private void Awake()
        {
            finishLine = transform.GetFirstGameObjectInChildrenWithTag("FinishLine", true);
            startPoint = transform.GetFirstComponentInChildrenWithTag<Transform>("StartPoint", true);
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
            length -= GMath.Abs((finishLine.localPosition - startPoint.localPosition).magnitude);
            return length;
        }
    }
}