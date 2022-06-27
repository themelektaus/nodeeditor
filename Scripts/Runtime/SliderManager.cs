using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

namespace NodeEditor
{
    [RequireComponent(typeof(Slider))]
    public class SliderManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // Resources
        public Slider mainSlider;
        public TextMeshProUGUI valueText;
        public TextMeshProUGUI popupValueText;

        // Settings
        public string suffix = "";
        public bool showValue = true;
        public bool showPopupValue = false;
        public bool useRoundValue = false;

        // Events
        [System.Serializable] public class SliderEvent : UnityEvent<float> { }
        [SerializeField] public SliderEvent onValueChanged = new();
        [Space(8)] public SliderEvent sliderEvent;

        // Other Variables
        [HideInInspector] public Animator sliderAnimator;

        void Awake()
        {
            mainSlider.onValueChanged.AddListener(delegate
            {
                sliderEvent.Invoke(mainSlider.value);
                UpdateUI();
            });

            sliderAnimator = gameObject.GetComponent<Animator>();

            sliderEvent.Invoke(mainSlider.value);

            UpdateUI();
        }

        public void UpdateUI()
        {
            if (useRoundValue)
            {
                if (valueText)
                    valueText.text = Mathf.Round(mainSlider.value * 1.0f).ToString() + suffix;

                if (popupValueText)
                    popupValueText.text = Mathf.Round(mainSlider.value * 1.0f).ToString() + suffix;

                return;
            }

            if (valueText)
                valueText.text = mainSlider.value.ToString("F1") + suffix;

            if (popupValueText)
                popupValueText.text = mainSlider.value.ToString("F1") + suffix;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!showPopupValue)
                return;

            if (sliderAnimator)
                sliderAnimator.Play("Value In");
            else
                popupValueText.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!showPopupValue)
                return;

            if (sliderAnimator)
                sliderAnimator.Play("Value Out");
            else
                popupValueText.gameObject.SetActive(false);
        }
    }
}