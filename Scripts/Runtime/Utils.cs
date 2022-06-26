using UnityEngine;

namespace NodeEditor
{
    public static class Utils
    {
        public static bool Approximately(Vector2 a, Vector2 b)
        {
            return Mathf.Approximately(a.x, b.x)
                && Mathf.Approximately(a.y, b.y);
        }

        public static void SetRect(RectTransform rectTransform, Vector3 from, Vector3 to)
        {
            var sizeDelta = to - from;
            sizeDelta.x = Mathf.Abs(sizeDelta.x);
            sizeDelta.y = Mathf.Abs(sizeDelta.y);

            rectTransform.position = from + (to - from) / 2;
            rectTransform.sizeDelta = sizeDelta / rectTransform.lossyScale.x;
        }
    }
}