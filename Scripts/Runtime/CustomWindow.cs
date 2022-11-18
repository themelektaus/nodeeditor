using UnityEngine;
using UnityEngine.UI;

namespace NodeEditor
{
    [ExecuteAlways]
    public class CustomWindow : MonoBehaviour
    {
        [SerializeField] Theme theme;
        
        public Image background;
        public Image content;

        void Awake()
        {
            if (theme)
                theme.window.ApplyTo(this);
        }
    }
}