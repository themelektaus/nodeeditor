using UnityEngine;
using UnityEngine.UI;

using TMPro;

using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

using Foldout;

namespace NodeEditor
{
    [ExecuteAlways]
    [RequireComponent(typeof(Button))]
    public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        [FormerlySerializedAs("normalImage")]
        public Image iconImage;

        [FormerlySerializedAs("normalText")]
        public TextMeshProUGUI uiText;

        public GameObject rippleParent;



        [Foldout("Appearance")]

        [FormerlySerializedAs("buttonColorIndex")]
        public int colorIndex;

        [FormerlySerializedAs("buttonIcon")]
        public Sprite icon;

        [FormerlySerializedAs("buttonText")]
        public string text = "";



        [Foldout("Events")]

        public UnityEvent clickEvent;
        public UnityEvent hoverEvent;



        [Foldout("Audio")]

        public AudioSource soundSource;
        public AudioClip hoverSound;
        public AudioClip clickSound;



        public CanvasGroup canvasGroup { get; private set; }
        public Button button { get; private set; }
        public Image image { get; private set; }

        public bool isHovering { get; private set; }

        public enum RippleUpdateMode { Normal, UnscaledTime }

        void Awake()
        {
            canvasGroup  = GetComponent<CanvasGroup>();
            button = GetComponent<Button>();
            image = GetComponent<Image>();

            if (iconImage)
                iconImage.sprite = icon;

            if (uiText)
                uiText.text = text;

            if (Theme.active)
                Theme.active.button.ApplyTo(this);

#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif

            button.onClick.AddListener(() =>
            {
                if (clickSound)
                    soundSource.PlayOneShot(clickSound);

                clickEvent.Invoke();
            });

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

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!button.interactable || !isHovering)
                return;

            if (!Theme.active || !Theme.active.button.rippleShape)
                return;

            Theme.active.button.CreateRipple(rippleParent, Input.mousePosition);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (button.interactable && hoverSound)
                soundSource.PlayOneShot(hoverSound);

            hoverEvent.Invoke();
            isHovering = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovering = false;
        }
    }
}