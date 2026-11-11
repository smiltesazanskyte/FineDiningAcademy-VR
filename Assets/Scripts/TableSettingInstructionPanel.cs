
using UnityEngine;
using TMPro;
using System.Text;

public class TableSettingInstructionPanel : MonoBehaviour
{
    [Header("UI Text")]
    public TextMeshProUGUI stageTitle;
    public TextMeshProUGUI stageDescription;
    public TextMeshProUGUI itemChecklist;
    public TextMeshProUGUI stageProgress;

    [Header("Stages")]
    public GameObject[] stages;

    private int currentStage = 0;

    private readonly string[] stageNames =
    {
        "Foundation Setting",
        "Additional Cutlery",
        "Bread & Dessert",
        "Glassware & Napkin"
    };

    private readonly string[] stageDescriptions =
    {
        "Place the charger plate and main course cutlery.",
        "Arrange the soup spoon and salad cutlery.",
        "Place the bread plate, butter knife, and dessert cutlery.",
        "Complete the setting with glassware and a linen napkin."
    };

    void Start()
    {
        UpdatePanel();
    }

    void Update()
    {
        UpdatePanel();
    }

    void UpdatePanel()
    {
        if (stages == null || stages.Length == 0)
            return;

        // Find the current active stage.
        // Completed stages remain active, so use the last active stage.
        int activeStage = -1;

        for (int i = 0; i < stages.Length; i++)
        {
            if (stages[i] != null && stages[i].activeSelf)
                activeStage = i;
        }

        if (activeStage == -1)
            return;

        currentStage = activeStage;
        if (stageProgress != null)
        {
            stageProgress.text =
            "STAGE " + (currentStage + 1) + " OF " + stages.Length;
        }

        TablewareSnap[] items =
            stages[currentStage].GetComponentsInChildren<TablewareSnap>(true);

        int completed = 0;
        StringBuilder checklist = new StringBuilder();

        foreach (TablewareSnap item in items)
        {
            if (item.IsSnapped)
                completed++;

            TablewareHoverLabel hover =
                item.GetComponent<TablewareHoverLabel>();

            string itemName =
                hover != null && !string.IsNullOrEmpty(hover.displayName)
                ? hover.displayName
                : item.gameObject.name;

            if (item.IsSnapped)
            {
                checklist.AppendLine(
                    "<color=#A8D5BA>[DONE] " + itemName + "</color>"
                );
            }
            else
            {
                checklist.AppendLine("○ " + itemName);
            }

            checklist.AppendLine();
        }

        // Show completion message after the final stage.
        if (currentStage == stages.Length - 1 &&
            completed == items.Length &&
            items.Length > 0)
        {
            stageTitle.text = "Training Complete!";
            if (stageProgress != null)
            {
                stageProgress.text = "4 OF 4 COMPLETED";
            }
            stageDescription.text =
                "Congratulations! You have successfully completed " +
                "the fine-dining table setting.";

            itemChecklist.text =
                 "<align=center><color=#D6BE91><b>EXCELLENT WORK!</b></color>\n\n" +
                 "14 / 14 Items Completed</align>";

            itemChecklist.alignment = TextAlignmentOptions.Center;
            itemChecklist.lineSpacing = 0;

            return;
        }
        itemChecklist.alignment = TextAlignmentOptions.TopLeft;

        // Update stage information.
        stageTitle.text = stageNames[
            Mathf.Min(currentStage, stageNames.Length - 1)
        ];

        stageDescription.text = stageDescriptions[
            Mathf.Min(currentStage, stageDescriptions.Length - 1)
        ];

        itemChecklist.text = checklist.ToString();
    }
}