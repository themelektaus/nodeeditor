using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace NodeEditor
{
    [ExecuteAlways]
    [RequireComponent(typeof(Button))]
    public class ButtonManagerBasicWithIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        [SerializeField] Theme theme;

        // Content
        public Sprite buttonIcon;
        public string buttonText = "BUTTON";
        public int buttonColorIndex;
        public UnityEvent clickEvent;
        public UnityEvent hoverEvent;
        public AudioClip hoverSound;
        public AudioClip clickSound;

        // Resources
        public Image normalImage;
        public TextMeshProUGUI normalText;
        public AudioSource soundSource;
        public GameObject rippleParent;

        CanvasGroup canvasGroup;
        public Button button { get; private set; }
        public Image image { get; private set; }

        bool hover;

        public enum RippleUpdateMode { Normal, UnscaledTime }

        void Awake()
        {
            canvasGroup  = GetComponent<CanvasGroup>();
            button = GetComponent<Button>();
            image = GetComponent<Image>();

            if (normalImage)
                normalImage.sprite = buttonIcon;

            if (normalText)
                normalText.text = buttonText;

            if (theme)
                theme.button.ApplyTo(this);

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
                if (theme && theme.button.rippleShape)
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
            if (!button.interactable || !hover)
                return;

            if (!theme || theme.button.rippleShape)
                return;

#if ENABLE_LEGACY_INPUT_MANAGER
            theme.button.CreateRipple(rippleParent, Input.mousePosition);
#elif ENABLE_INPUT_SYSTEM
            theme.button.CreateRipple(rippleParent, Mouse.current.position.ReadValue());
#endif
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (button.interactable && hoverSound)
                soundSource.PlayOneShot(hoverSound);

            hoverEvent.Invoke();
            hover = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hover = false;
        }
    }
}