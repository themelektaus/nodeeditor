using UnityEngine;
using UnityEngine.UI;

using TMPro;

using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

using System;
using System.Collections;

namespace NodeEditor
{
    [RequireComponent(typeof(Button))]
    public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        static float lastClick;

        [FormerlySerializedAs("normalImage")]
        public Image iconImage;

        [FormerlySerializedAs("normalText")]
        public TextMeshProUGUI uiText;

        public GameObject rippleParent;

        [Serializable]
        public class Appearance
        {
            public int colorIndex;
            public Sprite icon;
            public string text;
        }
        public Appearance appearance = new();

        [Serializable]
        public class Events
        {
            public UnityEvent click;
            public UnityEvent hover;
        }
        public Events events = new();

        public CanvasGroup canvasGroup { get; private set; }
        public Button button { get; private set; }
        public Image image { get; private set; }

        public bool isHovering { get; private set; }

        public enum RippleUpdateMode { Normal, UnscaledTime }

        void OnValidate()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            button = GetComponent<Button>();
            image = GetComponent<Image>();

            UpdateAppearance();
        }

        public void UpdateAppearance()
        {
            if (iconImage)
            {
                if (appearance.icon)
                {
                    iconImage.enabled = true;
                    iconImage.sprite = appearance.icon;
                }
                else
                {
                    iconImage.enabled = false;
                }
            }

            if (uiText)
            {
                var m = uiText.margin;
                m.x = appearance.icon ? 35 : 0;
                uiText.margin = m;
                uiText.text = appearance.text;
            }

            if (!isActiveAndEnabled)
                return;

            IEnumerator _()
            {
                yield return null;
                UpdateTheme();
            }
            StartCoroutine(_());
        }

        void Awake()
        {
            OnValidate();

            if (rippleParent)
            {
                if (Theme.active && Theme.active.button.rippleShape)
                {
                    rippleParent.SetActive(false);
                    return;
                }

                Destroy(rippleParent);
            }
        }

        void Update()
        {
            UpdateTheme();

            if (!canvasGroup || !button)
                return;

            if (button.interactable)
            {
                canvasGroup.interactable = true;
                canvasGroup.alpha = 1;
                return;
            }

            canvasGroup.interactable = false;
            canvasGroup.alpha = .3f;
        }

        void UpdateTheme()
        {
            if (Theme.active)
                Theme.active.button.ApplyTo(this);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!button.interactable || !isHovering)
                return;

            if (!Theme.active || !Theme.active.button.rippleShape)
                return;

            Theme.active.button.CreateRipple(rippleParent, Input.mousePosition);

            if (lastClick + .2f <= Time.unscaledTime)
            {
                lastClick = Time.unscaledTime;
                IEnumerator _()
                {
                    yield return new WaitForSecondsRealtime(.2f);
                    if (button.interactable)
                        events.click.Invoke();
                }
                StartCoroutine(_());
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            events.hover.Invoke();
            isHovering = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovering = false;
        }
    }
}