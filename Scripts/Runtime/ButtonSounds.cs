using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;

namespace NodeEditor
{
    [RequireComponent(typeof(Button))]
    public class ButtonSounds : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        [SerializeField] AudioSource soundSource;
        [SerializeField] AudioClip hoverSound;
        [SerializeField] AudioClip clickSound;

        Button button;
        bool isHovering;

        void Awake()
        {
            button = GetComponent<Button>();
        }

        public void OnPointerDown(PointerEventData _)
        {
            if (!button.interactable || !isHovering)
                return;

            if (clickSound)
                soundSource.PlayOneShot(clickSound, .8f);
        }

        public void OnPointerEnter(PointerEventData _)
        {
            if (button.interactable && hoverSound)
                soundSource.PlayOneShot(hoverSound, .7f);

            isHovering = true;
        }

        public void OnPointerExit(PointerEventData _)
        {
            isHovering = false;
        }
    }
}