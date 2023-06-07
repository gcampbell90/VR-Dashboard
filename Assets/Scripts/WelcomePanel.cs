using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WelcomePanel : Panel
{
    public TextMeshProUGUI welcomeText;
    public Button startButton;

    private string welcomeMessage = @"Welcome to the VR Data Visualization Dashboard!

Explore your data in a whole new dimension.

Get ready to unlock insights and visualize your datasets in immersive virtual reality. With our VR Data Visualization Dashboard, you can dive into your data like never before. Experience the power of interactive visualizations, explore trends, and gain deeper understanding through spatial analysis.

Features:
- Load datasets from CSV or JSON files
- Customize visualization parameters
- Navigate and interact with 3D visualizations
- Access additional analytics and insights
- Export and share your findings

How to Get Started:
Click the 'Start' button below to begin your data visualization journey. Use the provided controls and follow the on-screen instructions to navigate through the various panels. Enjoy the thrill of uncovering hidden patterns and making data-driven decisions in virtual reality!

Note:
Ensure you have your VR headset properly set up and connected before starting the dashboard.";

    public Action OnStartButtonPressed { get; internal set; }

    //public void Start()
    //{
    //    Initialise();
    //}

    public override void Initialise()
    {
        // Set the welcome message
        welcomeText.text = welcomeMessage;

        // Set up event handler for the start button
        startButton.onClick.AddListener(OnStartButtonClick);
    }

    private void OnStartButtonClick()
    {
        // Hide the welcome panel and proceed to the next panel
        gameObject.SetActive(false);
        OnStartButtonPressed?.Invoke();
        // Call a function to activate the next panel in the dashboard
        // For example, you can call a function like ActivateDatasetSelectionPanel() to proceed to the dataset selection panel
        // You would need to implement this function in the appropriate script for the next panel
        // Example: dashboardManager.ActivateDatasetSelectionPanel();
    }

    //public override void SetInteractable(bool interactable)
    //{
    //    startButton.interactable = interactable;
    //}

}
