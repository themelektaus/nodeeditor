using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;

namespace NodeEditor
{
    [RequireComponent(typeof(Button))]
    public class ButtonSounds : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        public AudioSource soundSource;
        public AudioClip hoverSound;
        public AudioClip clickSound;

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
                soundSource.PlayOneShot(hoverSound, .8f);

            isHovering = true;
        }

        public void OnPointerExit(PointerEventData _)
        {
            isHovering = false;
        }
    }
}