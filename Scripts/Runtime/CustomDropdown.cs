﻿using System;
using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace NodeEditor
{
    public class CustomDropdown : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {
        [FormerlySerializedAs("themeMain")]
        public Image main;

        public GameObject triggerObject;
        public TextMeshProUGUI selectedText;
        public Image selectedImage;
        public Transform itemParent;
        public GameObject itemObject;
        public GameObject scrollbar;
        public CustomInputField inputField;
        public VerticalLayoutGroup itemList;
        public Transform listParent;
        public AudioSource soundSource;

        [HideInInspector] public Transform currentListParent;

        [Range(1, 50)] public int itemPaddingTop = 8;
        [Range(1, 50)] public int itemPaddingBottom = 8;
        [Range(1, 50)] public int itemPaddingLeft = 8;
        [Range(1, 50)] public int itemPaddingRight = 25;
        public int selectedItemIndex = 0;

        [Range(1, 25)] public float transitionSmoothness = 10;
        [Range(1, 25)] public float sizeSmoothness = 15;
        public float panelSize = 200;

        RectTransform listRect;
        CanvasGroup listCG;
        bool isInTransition = false;
        float closeOn;

        public List<Item> dropdownItems = new();

        Func<List<string>> getDynamicItems;

        [Serializable]
        public class DropdownEvent : UnityEvent<int> { }
        [Space(8)] public DropdownEvent dropdownEvent;

        public AudioClip hoverSound;
        public AudioClip clickSound;

        [HideInInspector] public bool isOn;
        [HideInInspector] public int index = 0;
        [HideInInspector] public int siblingIndex = 0;

        EventTrigger triggerEvent;

        [SerializeField] Sprite uncheckedIcon;
        [SerializeField] Sprite checkedIcon;

        Viewport viewport;
        CanvasGroup inputFieldCanvasGroup;

        [Serializable]
        public class Item
        {
            public string itemName = "Dropdown Item";
            public Sprite itemIcon;
            [HideInInspector] public int itemIndex;
            public UnityEvent OnItemSelection = new();
        }

        void OnEnable()
        {
            listCG = gameObject.GetComponentInChildren<CanvasGroup>();
            listCG.alpha = 0;
            listCG.interactable = false;
            listCG.blocksRaycasts = false;

            if (inputFieldCanvasGroup)
            {
                inputFieldCanvasGroup.alpha = 0;
                inputFieldCanvasGroup.interactable = false;
                inputFieldCanvasGroup.blocksRaycasts = false;
            }

            closeOn = gameObject.GetComponent<RectTransform>().sizeDelta.y;

            listRect = listCG.GetComponent<RectTransform>();
            listRect.sizeDelta = new(listRect.sizeDelta.x, closeOn);
        }

        void Awake()
        {
            viewport = GetComponentInParent<Viewport>();

            if (inputField)
                inputFieldCanvasGroup = inputField.GetComponent<CanvasGroup>();

            try
            {
                SetupDropdown();

                currentListParent = transform.parent;

                if (triggerObject)
                {
                    triggerEvent = triggerObject.AddComponent<EventTrigger>();

                    EventTrigger.Entry entry;

                    entry = new() { eventID = EventTriggerType.Scroll };
                    entry.callback.AddListener(e => Animate());
                    triggerEvent.triggers.Add(entry);

                    entry = new() { eventID = EventTriggerType.PointerClick };
                    entry.callback.AddListener(e => Animate());
                    triggerEvent.triggers.Add(entry);
                }
            }
            catch
            {
                Debug.LogError("<b>[Dropdown]</b> Cannot initalize the object due to missing resources.", this);
            }
        }

        void UpdateItems()
        {
            if (inputField)
            {
                var text = inputField.inputText.text;
                dropdownItems.Clear();
                dropdownItems.Add(new() { itemName = text });

                if (getDynamicItems is not null)
                    foreach (var item in getDynamicItems())
                        if (text != item && !dropdownItems.Select(x => x.itemName).Contains(item))
                            dropdownItems.Add(new() { itemName = item });

                dropdownItems = dropdownItems.OrderBy(x => x.itemName).ToList();
                selectedItemIndex = dropdownItems.FindIndex(x => x.itemName == text);
            }

            for (int i = itemParent.childCount - 1; i >= 0; --i)
                DestroyImmediate(itemParent.GetChild(i).gameObject);

            for (int i = 0; i < dropdownItems.Count; i++)
            {
                var itemGameObject = Instantiate(itemObject);
                itemGameObject.transform.SetParent(itemParent, false);

                var setItemText = itemGameObject.GetComponentInChildren<TextMeshProUGUI>();
                setItemText.text = dropdownItems[i].itemName;

                dropdownItems[i].itemIndex = i;

                var mainItem = dropdownItems[i];

                if (itemGameObject.TryGetComponent(out Button itemButton))
                {
                    itemButton.onClick.AddListener(() =>
                    {
                        if (inputField)
                            inputField.inputText.text = mainItem.itemName;

                        Animate();

                        ChangeDropdownInfo(mainItem.itemIndex);
                        dropdownEvent.Invoke(mainItem.itemIndex);

                        mainItem.OnItemSelection.Invoke();
                    });
                }
            }

            if (selectedImage)
                selectedImage.gameObject.SetActive(false);

            try
            {
                selectedText.text = dropdownItems[selectedItemIndex].itemName;
                selectedImage.sprite = checkedIcon ? checkedIcon : dropdownItems[selectedItemIndex].itemIcon;
                selectedImage.gameObject.SetActive(selectedImage.sprite);
            }
            catch
            {
                selectedText.text = "";
            }

            currentListParent = transform.parent;
        }

        void Update()
        {
            if (!isInTransition)
                return;

            var alpha = Time.unscaledDeltaTime * transitionSmoothness;
            var size = Time.unscaledDeltaTime * sizeSmoothness;

            if (isOn)
            {
                listCG.alpha += alpha;
                if (inputFieldCanvasGroup)
                    inputFieldCanvasGroup.alpha += alpha;

                listRect.sizeDelta = Vector2.Lerp(listRect.sizeDelta, new(listRect.sizeDelta.x, panelSize), size);
                if (listRect.sizeDelta.y >= panelSize - .1f && listCG.alpha >= 1)
                    isInTransition = false;
                return;
            }

            listCG.alpha -= alpha;
            if (inputFieldCanvasGroup)
                inputFieldCanvasGroup.alpha -= alpha;

            listRect.sizeDelta = Vector2.Lerp(listRect.sizeDelta, new(listRect.sizeDelta.x, closeOn), size);

            if (listRect.sizeDelta.y <= closeOn + .1f && listCG.alpha <= 0)
            {
                isInTransition = false;
                enabled = false;
            }
        }

        public void SetupDropdown()
        {
            transform.SetAsLastSibling();

            if (!itemList)
                itemList = itemParent.GetComponent<VerticalLayoutGroup>();

            UpdateItemLayout();
            UpdateItems();
        }

        void ScrollTo(RectTransform itemTransform)
        {
            var itemParentY = listRect.InverseTransformPoint(itemParent.position).y;
            var itemTransformY = listRect.InverseTransformPoint(itemTransform.position).y;
            (itemParent as RectTransform).anchoredPosition = new(0, itemParentY - itemTransformY - itemTransform.sizeDelta.y);
        }

        void UpdateIcons()
        {
            var scrollRect = GetComponentInChildren<ScrollRect>();
            if (scrollRect)
                scrollRect.velocity = Vector2.zero;

            var images = itemParent
                .GetComponentsInChildren<Image>(true)
                .Where(x => x.name == "Icon")
                .ToList();

            for (int i = 0; i < dropdownItems.Count; i++)
            {
                images[i].sprite = dropdownItems[i].itemIcon;

                if (checkedIcon && selectedItemIndex == i)
                {
                    images[i].sprite = checkedIcon;
                    ScrollTo(images[i].transform.parent as RectTransform);

                    if (inputField)
                    {
                        inputField.inputText.text = dropdownItems[i].itemName;
                        inputField.inputText.Select();
                        inputField.UpdateState();
                    }
                }
                else if (uncheckedIcon && selectedItemIndex != i)
                {
                    images[i].sprite = uncheckedIcon;
                }

                images[i].gameObject.SetActive(images[i].sprite);
            }
        }

        public void ChangeDropdownInfo(int itemIndex)
        {
            if (selectedImage)
                selectedImage.sprite = checkedIcon ? checkedIcon : dropdownItems[itemIndex].itemIcon;

            if (selectedText)
                selectedText.text = dropdownItems[itemIndex].itemName;

            if (clickSound)
                soundSource.PlayOneShot(clickSound);

            selectedItemIndex = itemIndex;
        }

        public void Open()
        {
            if (isOn)
                return;

            Animate();
        }

        public void Animate()
        {
            if (!isOn)
            {
                if (viewport)
                    viewport.zoomEnabled = false;

                isOn = true;
                isInTransition = true;
                enabled = true;

                listCG.blocksRaycasts = true;
                listCG.interactable = true;

                if (inputFieldCanvasGroup)
                {
                    inputFieldCanvasGroup.blocksRaycasts = true;
                    inputFieldCanvasGroup.interactable = true;
                }
            }
            else
            {
                if (viewport)
                    viewport.zoomEnabled = true;

                isOn = false;
                isInTransition = true;
                enabled = true;

                listCG.blocksRaycasts = false;
                listCG.interactable = false;

                if (inputFieldCanvasGroup)
                {
                    inputFieldCanvasGroup.blocksRaycasts = false;
                    inputFieldCanvasGroup.interactable = false;
                }
            }

            if (isOn)
            {
                if (inputField)
                    UpdateItems();

                UpdateIcons();

                triggerObject.SetActive(true);
            }
            else
            {
                if (inputField)
                    selectedText.text = inputField.inputText.text;

                triggerObject.SetActive(false);
            }

            transform.SetAsLastSibling();
        }

        public void CreateNewItem(string title, Sprite icon)
        {
            CreateNewItemFast(title, icon);
            SetupDropdown();
        }

        public void CreateNewItemFast(string title, Sprite icon)
        {
            dropdownItems.Add(new Item
            {
                itemName = title,
                itemIcon = icon
            });
        }

        public void RemoveItem(string itemTitle)
        {
            dropdownItems.RemoveAll(x => x.itemName == itemTitle);
            SetupDropdown();
        }

        public void AddNewItem()
        {
            dropdownItems.Add(new());
        }

        public void UpdateItemLayout()
        {
            if (!itemList)
                return;

            itemList.padding.top = itemPaddingTop;
            itemList.padding.bottom = itemPaddingBottom;
            itemList.padding.left = itemPaddingLeft;
            itemList.padding.right = itemPaddingRight;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (clickSound)
                soundSource.PlayOneShot(clickSound);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (hoverSound)
                soundSource.PlayOneShot(hoverSound);
        }

        public string GetText()
        {
            if (inputField)
                return inputField.inputText.text;

            return selectedText.text;
        }

        public void SetText(string text)
        {
            if (inputField)
                inputField.inputText.text = text;

            selectedText.text = text;
        }

        public void SetText(string text, Func<List<string>> getDynamicItems)
        {
            SetText(text);
            this.getDynamicItems = getDynamicItems;
            SetupDropdown();
        }

        public int GetIndex()
        {
            return selectedItemIndex;
        }

        public void SetIndex(int index)
        {
            selectedItemIndex = index;
            SetupDropdown();
        }
    }
}