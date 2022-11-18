using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using ContextItem = NodeEditor.ContextMenuContent.ContextItem;
using ContextItemType = NodeEditor.ContextMenuContent.ContextItemType;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace NodeEditor
{
    [RequireComponent(typeof(Animator))]
    public class ContextMenuManager : MonoBehaviour
    {
        static Camera mainCamera => Camera.main;

        // Resources
        public GameObject contextContent;
        public GameObject contextButton;
        public GameObject contextSeparator;

        // Bounds
        [Range(-50, 50)] public int vBorderTop = -10;
        [Range(-50, 50)] public int vBorderBottom = 10;
        [Range(-50, 50)] public int hBorderLeft = 15;
        [Range(-50, 50)] public int hBorderRight = -15;

        Canvas mainCanvas;
        [System.NonSerialized] public Animator contextAnimator;

        Vector3 cursorPos;
        Vector3 contentPos = Vector3.zero;
        Vector3 contextVelocity = Vector3.zero;

        Transform itemParent;

        [HideInInspector] public bool isOn;

        public enum CameraSource { Main, Custom }

        public enum SubMenuBehaviour { Hover, Click }

        void Awake()
        {
            mainCanvas = gameObject.GetComponentInParent<Canvas>();
            contextAnimator = gameObject.GetComponent<Animator>();

            contentPos = new Vector3(vBorderTop, hBorderLeft, 0);
            gameObject.transform.SetAsLastSibling();

            itemParent = transform.Find("Content/Item List").transform;
        }

        void OnEnable()
        {
            contextAnimator.Play("Start");
            Init();
        }

        void Init()
        {
            var scale = contextContent.transform.localScale;
            scale.y = 0;
            contextContent.transform.localScale = scale;
        }

        public void SetContextMenuPosition(Vector2 offset)
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            cursorPos = Input.mousePosition;

#elif ENABLE_INPUT_SYSTEM
            cursorPos = Mouse.current.position.ReadValue();

#endif
            float x = offset.x * Screen.width / 1920;
            float y = offset.y * Screen.height / 1080;

            if (mainCanvas.renderMode == RenderMode.ScreenSpaceCamera || mainCanvas.renderMode == RenderMode.WorldSpace)
                contextContent.transform.position = mainCamera.ScreenToWorldPoint(cursorPos);

            else if (mainCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                contextContent.transform.position = new(cursorPos.x + x, cursorPos.y + y, 0);

            Init();
        }

        public void OpenContextMenu() => Open();

        public void Open()
        {
            Init();
            contextAnimator.Play("Menu In");
            isOn = true;
        }

        public void Open(List<ContextItem> contexItems, Vector2 offset = new())
        {
            Close();

            foreach (Transform child in itemParent)
                Destroy(child.gameObject);

            for (int i = 0; i < contexItems.Count; ++i)
            {
                if (contexItems[i].contextItemType == ContextItemType.Button)
                {
                    var gameObject = Instantiate(contextButton, Vector3.zero, Quaternion.identity);
                    gameObject.transform.SetParent(itemParent, false);

                    var setItemText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
                    var textHelper = contexItems[i].itemText;
                    setItemText.text = textHelper;

                    var goImage = gameObject.transform.Find("Icon");
                    var setItemImage = goImage.GetComponent<Image>();
                    var imageHelper = contexItems[i].itemIcon;
                    setItemImage.sprite = imageHelper;

                    if (!imageHelper)
                        setItemImage.color = new();

                    var itemButton = gameObject.GetComponent<Button>();
                    itemButton.onClick.AddListener(contexItems[i].onClick.Invoke);
                    itemButton.onClick.AddListener(CloseOnClick);
                }
                else if (contexItems[i].contextItemType == ContextItemType.Separator)
                {
                    var gameObject = Instantiate(contextSeparator, Vector3.zero, Quaternion.identity);
                    gameObject.transform.SetParent(itemParent, false);
                }

                StopCoroutine(nameof(ExecuteAfterTime));
                StartCoroutine(nameof(ExecuteAfterTime), .01f);
            }

            SetContextMenuPosition(offset);
            Open();
            SetContextMenuPosition(offset);
        }

        public void CloseOnClick() => Close();

        public void Close()
        {
            if (!isOn)
                return;

            contextAnimator.Play("Menu Out");
            isOn = false;
        }

        IEnumerator ExecuteAfterTime(float time)
        {
            yield return new WaitForSecondsRealtime(time);
            itemParent.gameObject.SetActive(false);
            itemParent.gameObject.SetActive(true);
        }
    }
}