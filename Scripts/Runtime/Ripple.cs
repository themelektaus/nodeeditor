using UnityEngine;
using UnityEngine.UI;

namespace NodeEditor
{
    public class Ripple : MonoBehaviour
    {
        public bool unscaledTime;
        public float speed;
        public float maxSize;
        public Color startColor;
        public Color transitionColor;

        Image colorImage;

        void Start()
        {
            transform.localScale = Vector3.zero;

            colorImage = GetComponent<Image>();
            colorImage.raycastTarget = false;
            colorImage.color = startColor;
        }

        void Update()
        {
            var deltaTime = unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

            transform.localScale = Vector3.Lerp(transform.localScale, new(maxSize, maxSize, maxSize), deltaTime * speed);
            colorImage.color = Color.Lerp(colorImage.color, transitionColor, deltaTime * speed);

            if (transform.localScale.x >= maxSize * 0.998)
            {
                if (transform.parent.childCount == 1)
                    transform.parent.gameObject.SetActive(false);
                Destroy(gameObject);
            }
        }
    }
}