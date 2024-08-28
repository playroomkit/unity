using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Playroom;

[CustomEditor(typeof(PlayroomkitDevManager))]
public class PrkMockInspector : Editor
{
    public StyleSheet styleSheet;

    public override VisualElement CreateInspectorGUI()
    {
        // Create root container
        VisualElement root = new VisualElement();
        root.style.flexDirection = FlexDirection.Column;

        root.styleSheets.Add(styleSheet);

        // Mock Mode Dropdown with tooltip
        VisualElement mockModeContainer = new VisualElement();
        mockModeContainer.AddToClassList("mockModeContainer");
        var mockModeLabel = new Label("Mock Mode");
        mockModeLabel.AddToClassList("label");

        // Tooltip icon and box
        var mockModeTooltipButton = new Button(() => { })
        {
            text = "?",
            tooltip = "Mock mode helps you test your game locally.\n\n1. Local (simulated): This mode runs in a local state, not connected to a server.\n\n2. Browser Bridge (live): This mode connects to a live multiplayer server.\n\nRead more in the docs."
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
            PlayroomKit.MockModeSelector selectedEnum = PlayroomKit.MockModeSelector.Local; // Default value

            if (selectedString == "Local (simulated)")
            {
                selectedEnum = PlayroomKit.MockModeSelector.Local;
            }
            else if (selectedString == "Browser Bridge (live)")
            {
                selectedEnum = PlayroomKit.MockModeSelector.BrowserBridge;
            }

            mockModeProperty.enumValueIndex = (int)selectedEnum;
            serializedObject.ApplyModifiedProperties();
        });

        mockModeContainer.Add(mockModeLabel);
        mockModeContainer.Add(mockModeTooltipButton);
        mockModeContainer.Add(mockModeDropdown);

        // Insert Coin Caller with tooltip
        VisualElement insertCoinContainer = new VisualElement();
        insertCoinContainer.AddToClassList("insertCoinContainer");

        var insertCoinLabel = new Label("Insert Coin Caller");
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

        // Launch Player Button
        var launchPlayerButton = new Button(() => Debug.Log("Player Launched"))
        {
            text = "Launch Player"
        };
        launchPlayerButton.AddToClassList("button");
        launchPlayerButton.tooltip = "Launch another player by clicking below, wait for the new editor to open, then click 'Clone'.";

        // Add elements to root
        root.Add(mockModeContainer);
        root.Add(insertCoinContainer);
        root.Add(launchPlayerButton);

        return root;
    }
}
