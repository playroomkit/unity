#if UNITY_EDITOR
using System;
using System.Collections;
using UnityEngine;

namespace Playroom
{
    public class MockCallbackHandler : MonoBehaviour
    {
        private Action callbackOne = null;
        private Action<string> callback = null;

        private void Start()
        {
            var manager = FindObjectOfType<PlayroomkitDevManager>();
            gameObject.transform.SetParent(manager.gameObject.transform);
        }

        public void SetCallback(Action<string> cb)
        {
            callback = cb;

            CallbackManager.RegisterCallback(cb);
        }

        public void SetCallback(Action cb)
        {
            callbackOne = cb;

            CallbackManager.RegisterCallback(cb);
        }


        public void ExecuteCallback(string result)
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


            CallbackManager.InvokeCallback(gameObject.name);

            Debug.LogWarning($"{gameObject.name} is invoking a callback with data: {result} and will be destroyed");
            // callback?.Invoke(result);
            Destroy(gameObject);
        }

        private IEnumerator ExecuteCallbackCoroutine()
        {
            yield return new WaitForEndOfFrame();

            Debug.LogWarning($"{gameObject.name} is invoking!");

            CallbackManager.InvokeCallback(gameObject.name);
            // callbackOne?.Invoke();
            Destroy(gameObject);
        }
    }
}

#endif