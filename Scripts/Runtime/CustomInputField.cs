using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace NodeEditor
{
    [RequireComponent(typeof(TMP_InputField))]
    [RequireComponent(typeof(Animator))]
    public class CustomInputField : MonoBehaviour
    {
        [Header("Resources")]
        public TMP_InputField inputText;
        public Animator inputFieldAnimator;

        [Header("Settings")]
        public bool processSubmit = false;

        [Header("Events")]
        public UnityEvent onSubmit;

        void Awake()
        {
            if (!inputText)
                inputText = gameObject.GetComponent<TMP_InputField>();

            if (!inputFieldAnimator)
                inputFieldAnimator = gameObject.GetComponent<Animator>();

            inputText.onSelect.AddListener(_ => AnimateIn());
            inputText.onEndEdit.AddListener(_ => AnimateOut());

            UpdateStateInstant();
        }

        void OnEnable()
        {
            if (!inputText)
                return;

            inputText.ForceLabelUpdate();
            UpdateStateInstant();

            if (gameObject.activeInHierarchy)
                StartCoroutine(DisableAnimator());
        }

        void Update()
        {
            if (!processSubmit)
                return;

            if (EventSystem.current.currentSelectedGameObject != inputText.gameObject)
                return;

#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.Return))
                onSubmit.Invoke();
#elif ENABLE_INPUT_SYSTEM
            if (Keyboard.current.enterKey.wasPressedThisFrame)
                onSubmit.Invoke();
#endif
        }

        public void AnimateIn()
        {
            StopCoroutine(DisableAnimator());

            if (!inputFieldAnimator.gameObject.activeInHierarchy)
                return;

            inputFieldAnimator.enabled = true;
            inputFieldAnimator.Play("In");

            StartCoroutine(DisableAnimator());
        }

        public void AnimateOut()
        {
            if (!inputFieldAnimator.gameObject.activeInHierarchy)
                return;

            inputFieldAnimator.enabled = true;

            if (inputText.text.Length == 0)
                inputFieldAnimator.Play("Out");

            StartCoroutine(DisableAnimator());
        }

        public void UpdateState()
        {
            if (inputText.text.Length == 0)
            {
                AnimateOut();
                return;
            }

            AnimateIn();
        }

        public void UpdateStateInstant()
        {
            if (inputText.text.Length == 0)
            {
                inputFieldAnimator.Play("Out", 0, 1);
                return;
            }

            inputFieldAnimator.Play("In", 0, 1);
        }

        IEnumerator DisableAnimator()
        {
            yield return new WaitForSeconds(1);
            inputFieldAnimator.enabled = false;
        }

        public string GetText()
        {
            return inputText.text;
        }

        public void SetText(string text)
        {
            inputText.text = text;
            UpdateStateInstant();
        }
    }
}