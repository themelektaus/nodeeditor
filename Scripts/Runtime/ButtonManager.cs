using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.EventSystems;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace NodeEditor
{
    [ExecuteAlways]
    [RequireComponent(typeof(Button))]
    public class ButtonManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, ISelectHandler, IDeselectHandler
    {
        // Content
        public string buttonText = "BUTTON";
        public UnityEvent clickEvent;
        public UnityEvent hoverEvent;
        public AudioClip hoverSound;
        public AudioClip clickSound;

        // Resources
        public TextMeshProUGUI normalText;
        public TextMeshProUGUI highlightedText;
        public AudioSource soundSource;
        public GameObject rippleParent;

        // Settings
        [Range(0.25f, 15)] public float fadingMultiplier = 8;

        // Ripple
        public RippleUpdateMode rippleUpdateMode = RippleUpdateMode.UnscaledTime;
        public Sprite rippleShape;
        [Range(0.1f, 5)] public float rippleSpeed = 2.4f;
        [Range(0.5f, 25)] public float rippleMaxSize = 6;
        public Color rippleStartColor = new(1, 1, 1, .1568628f);
        public Color rippleTransitionColor = new(1, 1, 1, 0);

        Button button;
        bool isPointerOn;

        float currentNormalValue;
        float currenthighlightedValue;
        CanvasGroup normalCanvasGroup;
        CanvasGroup highlightedCanvasGroup;

        public enum AnimationSolution { Animator, Script }
        public enum RippleUpdateMode { Normal, UnscaledTime }

        void OnEnable()
        {
            if (!normalCanvasGroup)
                return;

            if (!highlightedCanvasGroup)
                return;

            normalCanvasGroup.alpha = 1;
            highlightedCanvasGroup.alpha = 0;
        }

        void OnDisable()
        {
            foreach (Transform child in rippleParent.transform)
                Destroy(child.gameObject);
        }

        void Awake()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UpdateUI();
                return;
            }
#endif

            normalCanvasGroup = transform.Find("Normal").GetComponent<CanvasGroup>();
            highlightedCanvasGroup = transform.Find("Highlighted").GetComponent<CanvasGroup>();

            Destroy(GetComponent<Animator>());

            button = gameObject.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                if (clickSound)
                    soundSource.PlayOneShot(clickSound);
                clickEvent.Invoke();
            });

            UpdateUI();

            if (rippleParent)
            {
                if (rippleShape)
                    rippleParent.SetActive(false);
                else
                    Destroy(rippleParent);
            }
        }

        public void UpdateUI()
        {
            if (normalText)
                normalText.text = buttonText;

            if (highlightedText)
                highlightedText.text = buttonText;
        }

        public void CreateRipple(Vector2 position)
        {
            if (!rippleParent)
                return;

            rippleParent.SetActive(true);

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

            if (button.interactable)
                StartCoroutine(nameof(FadeIn));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isPointerOn = false;

            if (button.interactable)
                StartCoroutine(nameof(FadeOut));
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (button.interactable)
                StartCoroutine(nameof(FadeIn));
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (button.interactable)
                StartCoroutine(nameof(FadeOut));
        }

        IEnumerator FadeIn()
        {
            StopCoroutine(nameof(FadeOut));

            currentNormalValue = normalCanvasGroup.alpha;
            currenthighlightedValue = highlightedCanvasGroup.alpha;

            while (currenthighlightedValue <= 1)
            {
                currentNormalValue -= Time.unscaledDeltaTime * fadingMultiplier;
                normalCanvasGroup.alpha = currentNormalValue;

                currenthighlightedValue += Time.unscaledDeltaTime * fadingMultiplier;
                highlightedCanvasGroup.alpha = currenthighlightedValue;

                if (normalCanvasGroup.alpha >= 1)
                    StopCoroutine(nameof(FadeIn));

                yield return null;
            }
        }

        IEnumerator FadeOut()
        {
            StopCoroutine(nameof(FadeIn));

            currentNormalValue = normalCanvasGroup.alpha;
            currenthighlightedValue = highlightedCanvasGroup.alpha;

            while (currentNormalValue >= 0)
            {
                currentNormalValue += Time.unscaledDeltaTime * fadingMultiplier;
                normalCanvasGroup.alpha = currentNormalValue;

                currenthighlightedValue -= Time.unscaledDeltaTime * fadingMultiplier;
                highlightedCanvasGroup.alpha = currenthighlightedValue;

                if (highlightedCanvasGroup.alpha <= 0)
                    StopCoroutine(nameof(FadeOut));

                yield return null;
            }
        }
    }
}