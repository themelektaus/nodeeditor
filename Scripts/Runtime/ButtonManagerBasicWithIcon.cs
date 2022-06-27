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
        // Content
        public Sprite buttonIcon;
        public string buttonText = "BUTTON";
        public UnityEvent clickEvent;
        public UnityEvent hoverEvent;
        public AudioClip hoverSound;
        public AudioClip clickSound;

        // Resources
        public Image normalImage;
        public TextMeshProUGUI normalText;
        public AudioSource soundSource;
        public GameObject rippleParent;

        // Ripple
        public RippleUpdateMode rippleUpdateMode = RippleUpdateMode.UnscaledTime;
        public Sprite rippleShape;
        [Range(0.1f, 5)] public float rippleSpeed = 2.4f;
        [Range(0.5f, 25)] public float rippleMaxSize = 6;
        public Color rippleStartColor = new(1, 1, 1, .1568628f);
        public Color rippleTransitionColor = new(1, 1, 1, 0);

        Button button;
        bool isPointerOn;

        public enum RippleUpdateMode { Normal, UnscaledTime }

        void Awake()
        {
            button = gameObject.GetComponent<Button>();

            UpdateUI();

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
                if (rippleShape)
                    rippleParent.SetActive(false);
                else
                    Destroy(rippleParent);
            }
        }

        void UpdateUI()
        {
            if (normalImage)
                normalImage.sprite = buttonIcon;

            if (normalText)
                normalText.text = buttonText;
        }

        void CreateRipple(Vector2 position)
        {
            if (!rippleParent)
                return;

            rippleParent.SetActive(true);
            rippleParent.transform.SetAsFirstSibling();

            var rippleGameObject = new GameObject("Ripple");
            rippleGameObject.AddComponent<Image>().sprite = rippleShape;
            rippleGameObject.transform.SetParent(rippleParent.transform);
            rippleGameObject.transform.position = position;

            var ripple = rippleGameObject.AddComponent<Ripple>();
            ripple.speed = rippleSpeed;
            ripple.maxSize = rippleMaxSize;
            ripple.startColor = rippleStartColor;
            ripple.transitionColor = rippleTransitionColor;
            ripple.unscaledTime = rippleUpdateMode == RippleUpdateMode.UnscaledTime;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!rippleShape || !isPointerOn)
                return;

#if ENABLE_LEGACY_INPUT_MANAGER
            CreateRipple(Input.mousePosition);
#elif ENABLE_INPUT_SYSTEM
            CreateRipple(Mouse.current.position.ReadValue());
#endif
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (button.interactable && hoverSound)
                soundSource.PlayOneShot(hoverSound);

            hoverEvent.Invoke();
            isPointerOn = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isPointerOn = false;
        }
    }
}