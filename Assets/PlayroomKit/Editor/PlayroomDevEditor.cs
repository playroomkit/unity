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
        var mockModeTooltipButton = new Button(() => ShowTooltip(mockModeContainer, "Mock Mode helps you test..."))
            { text = "?" };
        mockModeTooltipButton.AddToClassList("tooltip-button");
        var mockModeDropdown = new PopupField<string>(
            new List<string> { "Local (simulated)", "Browser Bridge (live)" },
            0
        );
        mockModeDropdown.AddToClassList("dropdown");


        mockModeContainer.Add(mockModeLabel);
        mockModeContainer.Add(mockModeTooltipButton);
        mockModeContainer.Add(mockModeDropdown);


        // Insert Coin Caller with tooltip
        VisualElement insertCoinContainer = new VisualElement();
        insertCoinContainer.AddToClassList("insertCoinContainer");

        var insertCoinLabel = new Label("Insert Coin Caller");
        insertCoinLabel.AddToClassList("label");

        var insertCoinTooltipButton =
            new Button(() => ShowTooltip(insertCoinContainer, "InsertCoin must be called in order..."))
                { text = "?" };
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

        // launchPlayerButton.tooltip = "Launch another player by clicking below, wait for the new editor to open, then click 'Clone'.";

        launchPlayerButton.tooltip = "Launch another player by clicking below, wait for the new editor to open, then click 'Clone'.";
        launchPlayerButton.AddToClassList("custom-tooltip");
        launchPlayerButton.AddToClassList("button");

        
        // Add elements to root
        root.Add(mockModeContainer);
        root.Add(insertCoinContainer);
        root.Add(launchPlayerButton);

        return root;
    }
    

    
    private void ShowTooltip(VisualElement container, string message)
    {
        // // Check if a tooltip already exists and remove it to avoid duplicates
        // var existingTooltip = container.Q<Label>("tooltip");
        // if (existingTooltip != null)
        // {
        //     container.Remove(existingTooltip);
        // }
        //
        // // Create a new Label to display the tooltip message
        // var tooltip = new Label(message)
        // {
        //     name = "tooltip"
        // };
        //
        // // Apply the helpBox style class
        // tooltip.AddToClassList("helpBox");
        // tooltip.BringToFront();
        // // Add the tooltip to the container
        // container.Add(tooltip);
        //
        // // Optionally, you can remove the tooltip after a certain time
        // // to simulate a disappearing tooltip.
        // container.schedule.Execute(() => { container.Remove(tooltip); }).StartingIn(5000); // Remove after 5 seconds
        Debug.Log("Show Tooltip");
    }
}