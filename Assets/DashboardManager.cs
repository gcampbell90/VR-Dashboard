using UnityEngine;

public class DashboardManager : MonoBehaviour
{
    public WelcomePanel welcomePanel;
    public DatasetSelectionPanel datasetSelectionPanel;
    public DataConfigurationPanel dataConfigurationPanel;
    public Scatterplot dataVisPanelTest;

    private void Start()
    {
        InitializeDashboard();

        datasetSelectionPanel.gameObject.SetActive(false);
        dataConfigurationPanel.gameObject.SetActive(false);
        dataVisPanelTest.gameObject.SetActive(false);
    }

    private void InitializeDashboard()
    {
        // Step 1: Initialize the WelcomePanel immediately
        welcomePanel.Initialise();

        // Step 2: Deactivate the DatasetSelectionPanel initially
        //datasetSelectionPanel.gameObject.SetActive(false);

        // Step 3: Activate the DatasetSelectionPanel when the Start button is pressed
        welcomePanel.OnStartButtonPressed += ActivateDatasetSelectionPanel;

        // Step 4: Deactivate the DataConfigurationPanel initially
        //dataConfigurationPanel.gameObject.SetActive(false);

        // Step 4 (continued): Activate the DataConfigurationPanel when the Continue button is pressed
        datasetSelectionPanel.OnContinueButtonPressed += ActivateDataConfigurationPanel;

        //step 5 on vis button press, set labels in axis
        dataConfigurationPanel.OnVisualisationButtonPress += ActivateVisualisationPanel;
    }

    private void ActivateDatasetSelectionPanel()
    {
        Debug.Log("Activating Datasetselection panel");
        welcomePanel.OnStartButtonPressed -= ActivateDatasetSelectionPanel; // Unsubscribe from the event
        welcomePanel.gameObject.SetActive(false);
        datasetSelectionPanel.gameObject.SetActive(true);
        datasetSelectionPanel.Initialise();
    }

    private void ActivateDataConfigurationPanel()
    {
        Debug.Log("Activating Data config panel");

        datasetSelectionPanel.OnContinueButtonPressed -= ActivateDataConfigurationPanel; // Unsubscribe from the event
        dataConfigurationPanel.gameObject.SetActive(true);
        dataConfigurationPanel.Initialise();
    }
    private void ActivateVisualisationPanel()
    {
        Debug.Log("Activating Data vis panel");
        dataVisPanelTest.gameObject.SetActive(true);
        //dataVisPanelTest.CreateScatterplot(dataConfigurationPanel.xAxisData, dataConfigurationPanel.yAxisData, dataConfigurationPanel.zAxisData);
        dataVisPanelTest.CreateScatterplot(dataConfigurationPanel.engagementDates, dataConfigurationPanel.bands, dataConfigurationPanel.trusts);
    }
}