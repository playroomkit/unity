#if UNITY_EDITOR
using System;
using UBB;
using UnityEngine;

namespace Playroom
{
    public class BrowserMockRPC : PlayroomKit.IRPC
    {
        public void RpcRegister(string name, Action<string, string> rpcRegisterCallback, string onResponseReturn = null)
        {
            string callbackKey = $"RpcEvent_{name}";
            GameObject callbackObject = new GameObject(callbackKey);
            Debug.Log(callbackKey);

            MockCallbackInvoker invoker = callbackObject.AddComponent<MockCallbackInvoker>();
            invoker.SetCallback(rpcRegisterCallback, callbackKey);
            CallBacksHandlerMock.Instance.RegisterCallbackObject(callbackKey, callbackObject, "ExecuteCallback");
            
            UnityBrowserBridge.Instance.CallJs("RpcRegister", null, null, false, name, callbackKey, onResponseReturn);
        }

        public void RpcCall(string name, object data, PlayroomKit.RpcMode mode, Action callbackOnResponse = null)
        {
            UnityBrowserBridge.Instance.CallJs("RpcCall", null, null, false, name, data.ToString(), mode.ToString());
        }

        public void RpcCall(string name, object data, Action callbackOnResponse = null)
        {
            RpcCall(name, data, PlayroomKit.RpcMode.ALL, callbackOnResponse);
        }
    }
}
#endif