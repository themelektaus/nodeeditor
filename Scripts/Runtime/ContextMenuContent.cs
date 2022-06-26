using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NodeEditor
{
    public class ContextMenuContent : MonoBehaviour, IPointerDownHandler
    {
        public List<ContextItem> contexItems = new();

        Transform itemParent;
        ContextMenuManager contextManager;
        Image setItemImage;
        TextMeshProUGUI setItemText;
        Sprite imageHelper;
        string textHelper;

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
            itemParent = contextManager.transform.Find("Content/Item List").transform;
            
            foreach (Transform child in itemParent)
                Destroy(child.gameObject);
        }

        void ProcessClick()
        {
            foreach (Transform child in itemParent)
                Destroy(child.gameObject);

            for (int i = 0; i < contexItems.Count; ++i)
            {
                if (contexItems[i].contextItemType == ContextItemType.Button)
                {
                    var gameObject = Instantiate(contextManager.contextButton, Vector3.zero, Quaternion.identity);
                    gameObject.transform.SetParent(itemParent, false);

                    setItemText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
                    textHelper = contexItems[i].itemText;
                    setItemText.text = textHelper;

                    var goImage = gameObject.transform.Find("Icon");
                    setItemImage = goImage.GetComponent<Image>();
                    imageHelper = contexItems[i].itemIcon;
                    setItemImage.sprite = imageHelper;

                    if (!imageHelper)
                        setItemImage.color = new();

                    var itemButton = gameObject.GetComponent<Button>();
                    itemButton.onClick.AddListener(contexItems[i].onClick.Invoke);
                    itemButton.onClick.AddListener(CloseOnClick);
                }
                else if (contexItems[i].contextItemType == ContextItemType.Separator)
                {
                    var gameObject = Instantiate(contextManager.contextSeparator, Vector3.zero, Quaternion.identity);
                    gameObject.transform.SetParent(itemParent, false);
                }

                StopCoroutine(nameof(ExecuteAfterTime));
                StartCoroutine(nameof(ExecuteAfterTime), .01f);
            }

            contextManager.SetContextMenuPosition();
            contextManager.contextAnimator.Play("Menu In");
            contextManager.isOn = true;
            contextManager.SetContextMenuPosition();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (contextManager.isOn)
            {
                contextManager.contextAnimator.Play("Menu Out");
                contextManager.isOn = false;
                return;
            }

            if (eventData.button == PointerEventData.InputButton.Right && !contextManager.isOn)
                ProcessClick();
        }

        IEnumerator ExecuteAfterTime(float time)
        {
            yield return new WaitForSecondsRealtime(time);
            itemParent.gameObject.SetActive(false);
            itemParent.gameObject.SetActive(true);
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