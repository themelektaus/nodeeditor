using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace NodeEditor
{
    public class ContextMenuContent : MonoBehaviour, IPointerDownHandler
    {
        public List<ContextItem> contexItems = new();

        ContextMenuManager contextManager;
        
        [System.Serializable]
        public class ContextItem
        {
            [Header("Information")]
            public string itemText = "Item Text";
            public Sprite itemIcon;
            public ContextItemType contextItemType;

            [Header("Events")]
            public UnityEvent onClick;
        }

        public enum ContextItemType { Button, Separator }

        void Awake()
        {
            contextManager = FindObjectOfType<ContextMenuManager>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            contextManager.Close();
            if (eventData.button == PointerEventData.InputButton.Right)
                contextManager.Open(contexItems);
        }

        public void CloseOnClick()
        {
            contextManager.isOn = false;

            if (!contextManager.contextAnimator.isActiveAndEnabled)
                return;

            contextManager.contextAnimator.Play("Menu Out");
        }
    }
}