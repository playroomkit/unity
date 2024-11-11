#if UNITY_EDITOR


using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using ParrelSync;
using UnityEditor.Search;

namespace Playroom
{
    [CustomEditor(typeof(PlayroomkitDevManager))]
    public class PrkMockInspector : Editor
    {
        public StyleSheet styleSheet;

        public void OnEnable()
        {
            styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/PlayroomKit/Editor/PlayroomkitDevManagerEditor.uss");
        }

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Column
                }
            };

            if (styleSheet == null)
            {
                Debug.LogError("Style sheet is null");
            }

            root.styleSheets.Add(styleSheet);

            VisualElement mockModeContainer = MockModeContainerCreator();
            VisualElement insertCoinContainer = InsertCoinContainerCreator();
            Button launchPlayerButton = LaunchPlayerButtonCreator();
            VisualElement docsLink = DocumentationLinkCreator();

            // Add elements to root
            root.Add(mockModeContainer);
            root.Add(insertCoinContainer);
            root.Add(launchPlayerButton);
            root.Add(docsLink);

            return root;
        }

        private static void OpenClonesManager()
        {
            ClonesManagerWindow window = (ClonesManagerWindow)EditorWindow.GetWindow(typeof(ClonesManagerWindow));
            window.titleContent = new GUIContent("Clones Manager");
            window.Show();
        }

        private static Button LaunchPlayerButtonCreator()
        {
            var launchPlayerButton = new Button(OpenClonesManager)
            {
                text = "Launch Player"
            };
            launchPlayerButton.AddToClassList("button");
            launchPlayerButton.tooltip =
                "Launch another player by clicking below, wait for the new editor to open, then click 'Clone'.";
            return launchPlayerButton;
        }

        private static VisualElement InsertCoinContainerCreator()
        {
            // Insert Coin Caller with tooltip
            VisualElement insertCoinContainer = new VisualElement();
            insertCoinContainer.AddToClassList("insertCoinContainer");

            var insertCoinLabel = new Label("Insert Coin Caller")
            {
                tooltip =
                    "InsertCoin() must be called in order to connect PlayroomKit server.\n\nChoose the gameObject (with the script) which calls InsertCoin.\n\nRead More in the docs"
            };
            insertCoinLabel.AddToClassList("label");

            var insertCoinTooltipButton = new Button
            {
                text = "?",
                tooltip = ""
            };
            insertCoinTooltipButton.AddToClassList("tooltip-button");

            var insertCoinField = new ObjectField()
            {
                objectType = typeof(GameObject),
                bindingPath = "insertCoinCaller"
            };
            insertCoinField.AddToClassList("insertCoinField");

            insertCoinContainer.Add(insertCoinLabel);
            insertCoinContainer.Add(insertCoinTooltipButton);
            insertCoinContainer.Add(insertCoinField);
            return insertCoinContainer;
        }

        private VisualElement MockModeContainerCreator()
        {
            VisualElement mockModeContainer = new VisualElement();
            mockModeContainer.AddToClassList("mockModeContainer");
            var mockModeLabel = new Label("Mock Mode");
            mockModeLabel.AddToClassList("label");

            // Tooltip icon and box
            var mockModeTooltipButton = new Button(() => { })
            {
                text = "?",
                tooltip =
                    "Mock mode helps you test your game locally.\n\n1. Local (simulated): This mode runs in a local state, not connected to a server.\n\n2. Browser Bridge (live): This mode connects to a live multiplayer server.\n\nRead more in the docs."
            };
            mockModeTooltipButton.AddToClassList("tooltip-button");

            // Create a PopupField for Mock Mode Selector
            var mockModeOptions = new List<string>
            {
                "Local (simulated)",
                "Browser Bridge (live)"
            };

            var mockModeProperty = serializedObject.FindProperty("mockMode");
            var initialIndex = (int)(PlayroomKit.MockModeSelector)mockModeProperty.enumValueIndex;

            var mockModeDropdown = new PopupField<string>(
                mockModeOptions,
                initialIndex
            );
            mockModeDropdown.AddToClassList("dropdown");

            // Register callback to handle changes in the dropdown
            mockModeDropdown.RegisterValueChangedCallback(evt =>
            {
                var selectedString = evt.newValue;
                PlayroomKit.MockModeSelector selectedEnum = PlayroomKit.MockModeSelector.Local;

                if (selectedString == "Local (simulated)")
                {
                    selectedEnum = PlayroomKit.MockModeSelector.Local;
                }
                else if (selectedString == "Browser Bridge (live)")
                {
                    selectedEnum = PlayroomKit.MockModeSelector.Browser;
                }

                mockModeProperty.enumValueIndex = (int)selectedEnum;
                serializedObject.ApplyModifiedProperties();
            });

            mockModeContainer.Add(mockModeLabel);
            mockModeContainer.Add(mockModeTooltipButton);
            mockModeContainer.Add(mockModeDropdown);
            return mockModeContainer;
        }

        private static VisualElement DocumentationLinkCreator()
        {
            var docLink = new Label("Read more in the documentation ↗");
            docLink.AddToClassList("doc-link");
            docLink.tooltip = "https://docs.joinplayroom.com/usage/unity";

            docLink.RegisterCallback<MouseUpEvent>(evt => { Application.OpenURL(docLink.tooltip); });

            return docLink;
        }
    }
}
#endif