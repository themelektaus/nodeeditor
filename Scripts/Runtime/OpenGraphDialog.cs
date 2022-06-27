using System;
using UnityEngine;
using UnityEngine.UI;

namespace NodeEditor
{
    public class OpenGraphDialog : MonoBehaviour
    {
        public GraphList graphList;
        public Button cancelButton;

        public event Action onDestroy;

        Animator animator;

        void Awake()
        {
            animator = GetComponent<Animator>();
        }

        void Start()
        {
            animator.Play("In");
        }

        public void Stop()
        {
            animator.Play("Out");
            Destroy(gameObject, .2f);
        }

        void OnDestroy()
        {
            onDestroy?.Invoke();
        }
    }
}