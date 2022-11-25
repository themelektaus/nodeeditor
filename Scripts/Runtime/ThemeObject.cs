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
                if (TryGet(0, out Image titlebarImage))
                    titlebarImage.color = theme.window.titlebarColor;

                if (TryGet(1, out Image backgroundImage))
                    backgroundImage.color = theme.window.backgroundColor;

                return;
            }

            if (type == Type.Dropdown)
            {
                if (TryGet(0, out Image image))
                    image.color = theme.button.baseColors[0];

                return;
            }

            if (type == Type.DropdownItem)
            {
                if (TryGet(0, out Image image))
                    image.color = theme.window.titlebarColor;

                return;
            }

            if (type == Type.ScrollView)
            {
                if (TryGet(0, out Image image))
                    image.color = theme.scrollView.backgroundColor;

                return;
            }

            if (type == Type.ContextMenu)
            {
                if (TryGet(0, out Image image))
                    image.color = theme.scrollView.backgroundColor;

                return;
            }

            if (type == Type.ContextMenuSeparator)
            {
                if (TryGet(0, out Image image))
                    image.color = theme.button.baseColors[0];

                return;
            }
        }

        bool TryGet<T>(int index, out T @object)
        {
            if (index < objects.Length)
            {
                if (objects[index] is T result)
                {
                    @object = result;
                    return true;
                }
            }
            @object = default;
            return false;
        }
    }
}