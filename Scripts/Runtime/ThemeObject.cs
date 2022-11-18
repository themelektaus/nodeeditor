using UnityEngine;
using UnityEngine.UI;

namespace NodeEditor
{
    [ExecuteAlways]
    public class ThemeObject : MonoBehaviour
    {
        public enum Type
        {
            None,
            Window,
            Dropdown,
            DropdownItem,
            ScrollView,
            ContextMenu,
            ContextMenuSeparator
        }

        [SerializeField] Type type;

        public Object[] objects;

        void Awake()
        {
            if (type == Type.None)
                return;

            var theme = Theme.active;

            if (type == Type.Window)
            {
                (objects[0] as Image).color = theme.window.titlebarColor;
                (objects[1] as Image).color = theme.window.backgroundColor;
                return;
            }

            if (type == Type.Dropdown)
            {
                (objects[0] as Image).color = theme.button.baseColors[0];
                return;
            }

            if (type == Type.DropdownItem)
            {
                (objects[0] as Image).color = theme.window.titlebarColor;
                return;
            }

            if (type == Type.ScrollView)
            {
                (objects[0] as Image).color = theme.scrollView.backgroundColor;
                return;
            }

            if (type == Type.ContextMenu)
            {
                (objects[0] as Image).color = theme.scrollView.backgroundColor;
                return;
            }

            if (type == Type.ContextMenuSeparator)
            {
                (objects[0] as Image).color = theme.button.baseColors[0];
                return;
            }
        }
    }
}