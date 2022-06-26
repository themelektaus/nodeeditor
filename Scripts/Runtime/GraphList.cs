using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NodeEditor
{
    public class GraphList : MonoBehaviour
    {
        [SerializeField] GraphListItem itemPrefab;
        [SerializeField] RectTransform listTransform;

        [SerializeField] UnityEngine.UI.Button createButton;

        public UnityEvent<string> onOpen;

        readonly List<GraphListItem> items = new();

        void Awake()
        {
            createButton.onClick.AddListener(() =>
            {
                Global.Prefs.CreateGraph("New Graph");
                Refresh();
            });
        }

        void OnEnable()
        {
            Refresh();
        }

        void OnDisable()
        {
            Clear();
        }

        void Clear()
        {
            items.Clear();
            foreach (Transform child in listTransform)
                Destroy(child.gameObject);
        }

        void Refresh()
        {
            Clear();

            var guids = LoadGraphGUIDs();

            var activeGuid = Global.Prefs.GetActiveGraphGUID();

            foreach (var guid in guids.items)
            {
                var _guid = guid;
                
                var itemGameObject = Instantiate(itemPrefab.gameObject, listTransform);
                
                var newItem = itemGameObject.GetComponent<GraphListItem>();
                newItem.inputField.inputText.text = Global.Prefs.GetGraphTitle(guid);
                newItem.inputField.inputText.onValueChanged.AddListener((UnityAction<string>) (title =>
                {
                    Global.Prefs.SetGraphTitle(guid, title);
                }));
                newItem.inputField.UpdateStateInstant();

                if (guid == activeGuid)
                {
                    newItem.activateButton.gameObject.SetActive(false);
                }
                else
                {
                    newItem.activateButton.onClick.AddListener(() =>
                    {
                        Global.Prefs.SetActiveGraphGUID(guid);
                        Refresh();
                    });
                }

                newItem.openButton.onClick.AddListener(() =>
                {
                    onOpen.Invoke(_guid);
                });

                newItem.deleteButton.onClick.AddListener(() =>
                {
                    Global.Prefs.DeleteGraph(guid);
                    guids = LoadGraphGUIDs();
                    Refresh();
                });

                items.Add(newItem);
            }
        }

        StringList LoadGraphGUIDs()
        {
            var guids = Global.Prefs.GetGraphGUIDs();
            if (guids.items.Count == 0)
            {
                var guid = Global.Prefs.CreateGraph("Hello World!", Global.HELLO_WORLD_GRAPH_DATA_JSON);
                guids.items.Add(guid);
                Global.Prefs.SetActiveGraphGUID(guid);
                Global.Prefs.SetGraphGUIDs(guids);
            }
            return guids;
        }
    }
}