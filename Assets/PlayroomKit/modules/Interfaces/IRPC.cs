using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using SimpleJSON;
using UnityEngine;

namespace Playroom
{
    public partial class PlayroomKit
    {
        public enum RpcMode
        {
            ALL,
            OTHERS,
            HOST
        }

        public interface IRPC
        {
            public void RpcRegister(string name, Action<string, string> rpcRegisterCallback,
                string onResponseReturn = null);

            public void RpcCall(string name, object data, RpcMode mode, Action callbackOnResponse = null);

            // Default Mode
            public void RpcCall(string name, object data, Action callbackOnResponse = null);

        }
    }
}