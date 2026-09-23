#if UNITY_EDITOR
using System;
using System.Collections;
using UnityEngine;

namespace Playroom
{
    public class MockCallbackInvoker : MonoBehaviour
    {
        private void Start()
        {
            var manager = FindObjectOfType<PlayroomkitDevManager>();
            gameObject.transform.SetParent(manager.gameObject.transform);
        }

        public void SetCallback(Action cb, string key)
        {
            CallbackManager.RegisterCallback(cb, key);
        }

        public void SetCallback(Action<string> cb, string key)
        {
            CallbackManager.RegisterCallback(cb, key);
        }

        public void SetCallback(Action<string, string> cb, string key)
        {
            CallbackManager.RegisterCallback(cb, key);
        }


        public void ExecuteCallback(string result)
        {
            StartCoroutine(ExecuteCallbackCoroutine(result));
        }

        public void ExecuteCallback(string[] result)
        {
            StartCoroutine(ExecuteCallbackCoroutine(result));
        }

        public void ExecuteCallback()
        {
            StartCoroutine(ExecuteCallbackCoroutine());
        }

        private IEnumerator ExecuteCallbackCoroutine(string result)
        {
            yield return new WaitForEndOfFrame();

            CallbackManager.InvokeCallback(gameObject.name, result);
            Destroy(gameObject);
        }

        private IEnumerator ExecuteCallbackCoroutine()
        {
            yield return new WaitForEndOfFrame();
            CallbackManager.InvokeCallback(gameObject.name);
            Destroy(gameObject);
        }

        // Used by rpc
        private IEnumerator ExecuteCallbackCoroutine(string[] result)
        {
            yield return new WaitForEndOfFrame();
            CallbackManager.InvokeCallback(gameObject.name, result[0], result[1]);
        }
    }
}

#endif