using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UBB;
using Unity.VisualScripting;
using UnityEngine;

namespace Playroom
{
    /// <summary>
    /// This file contains the new  mock-mode which uses browser-driver to run Playroom's within the editor!.
    /// </summary>
    public partial class PlayroomKit
    {
        private static void MockInsertCoinBrowser(InitOptions options, Action onLaunchCallBack)
        {
            isPlayRoomInitialized = true;

            Debug.Log("Coin Inserted!");

            string optionsJson = null;
            if (options != null) optionsJson = SerializeInitOptions(options);

            // Debug.Log(aGameObject.name);

            UnityBrowserBridge.Instance.ExecuteJS(
                $"await InsertCoin({optionsJson}, '{onLaunchCallBack.GetMethodInfo().Name}', 'Manager')");
        }
    }
}