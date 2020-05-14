using UnityEngine;

namespace Assets.Scripts.CustomBehaviour
{
    public class Track : MonoBehaviour
    {
        private GameObject finishLine;

        private Transform startPoint;

        private float trackLength;

        public float Length()
        {
            return trackLength;
        }

        private void Awake()
        {
            finishLine = TUnityUtilsProvider.GetFirstGameObjectInChildrenWithTag(gameObject, "FinishLine", true);
            startPoint = TUnityUtilsProvider.GetFirstComponentInChildrenWithTag<Transform>(gameObject, "StartPoint", true);
            trackLength = CalculateTrackLength();
        }

        private float CalculateTrackLength()
        {
            float length = 0;
            foreach (Transform child in transform)
            {
                if (child.tag.Equals("TrackPart"))
                {
                    length += child.localScale.z * 30;
                }
            }
            length -= TMath.Abs((finishLine.transform.localPosition - startPoint.localPosition).magnitude);
            return length;
        }

        public GameObject GetFinishLine()
        {
            return finishLine;
        }

        public Transform GetStartPoint()
        {
            return startPoint;
        }

        public string GetId()
        {
            return gameObject.name;
        }
    }
}