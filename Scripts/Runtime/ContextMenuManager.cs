using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace NodeEditor
{
    [RequireComponent(typeof(Animator))]
    public class ContextMenuManager : MonoBehaviour
    {
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

        RectTransform contextRect;
        RectTransform contentRect;

        [HideInInspector] public bool isOn;

        public enum CameraSource { Main, Custom }

        public enum SubMenuBehaviour { Hover, Click }

        void Awake()
        {
            mainCanvas = gameObject.GetComponentInParent<Canvas>();
            contextAnimator = gameObject.GetComponent<Animator>();
            
            contextRect = gameObject.GetComponent<RectTransform>();
            contentRect = contextContent.GetComponent<RectTransform>();
            contentPos = new Vector3(vBorderTop, hBorderLeft, 0);
            gameObject.transform.SetAsLastSibling();
        }

        void OnEnable()
        {
            contextAnimator.Play("Start");
            Init();
        }

        public void SetContextMenuPosition()
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            cursorPos = Input.mousePosition;
#elif ENABLE_INPUT_SYSTEM
            cursorPos = Mouse.current.position.ReadValue();
#endif
            contentRect.pivot = new(.5f, .5f);

            if (mainCanvas.renderMode == RenderMode.ScreenSpaceCamera || mainCanvas.renderMode == RenderMode.WorldSpace)
            {
                contextRect.position = Camera.main.ScreenToWorldPoint(cursorPos);
                contextRect.localPosition = new Vector3(contextRect.localPosition.x, contextRect.localPosition.y, 0);
                contextContent.transform.localPosition = Vector3.SmoothDamp(contextContent.transform.localPosition, contentPos, ref contextVelocity, 0);
            }
            else if (mainCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                contextRect.position = cursorPos;
                contextContent.transform.position = new Vector3(cursorPos.x + contentPos.x, cursorPos.y + contentPos.y, 0);
            }

            Init();
        }

        void Init()
        {
            var scale = contextContent.transform.localScale;
            scale.y = 0;
            contextContent.transform.localScale = scale;
        }

        public void Open()
        {
            Init();
            contextAnimator.Play("Menu In");
            isOn = true;
        }

        public void Close()
        {
            contextAnimator.Play("Menu Out");
            isOn = false;
        }

        public void OpenContextMenu() => Open();
        public void CloseOnClick() => Close();
    }
}