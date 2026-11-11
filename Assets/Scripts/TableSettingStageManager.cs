
using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TableSettingStageManager : MonoBehaviour
{
    [Header("Stages in Order")]
    public GameObject[] stages;

    [Header("Progression Settings")]
    public float stageTransitionDelay = 1.0f;

    private TablewareSnap[][] stageItems;
    private int currentStage = 0;
    private bool transitioning = false;

    void Start()
    {
        stageItems = new TablewareSnap[stages.Length][];

        // Remember every required object before gameplay begins.
        for (int i = 0; i < stages.Length; i++)
        {
            if (stages[i] == null)
                continue;

            stageItems[i] =
                stages[i].GetComponentsInChildren<TablewareSnap>(true);

            Debug.Log(
                stages[i].name + " requires " +
                stageItems[i].Length + " objects."
            );

            stages[i].SetActive(i == 0);
        }
    }

    void Update()
    {
        if (transitioning || currentStage >= stages.Length)
            return;

        if (!StageComplete(currentStage))
            return;

        StartCoroutine(AdvanceStage());
    }

    bool StageComplete(int stageIndex)
    {
        TablewareSnap[] items = stageItems[stageIndex];

        if (items == null || items.Length == 0)
            return false;

        foreach (TablewareSnap item in items)
        {
            if (item == null || !item.IsSnapped)
                return false;

            XRGrabInteractable grab =
                item.GetComponent<XRGrabInteractable>();

            if (grab != null && grab.isSelected)
                return false;

            if (item.snapPoint == null)
                return false;

            if (Vector3.Distance(
                item.transform.position,
                item.snapPoint.position) > 0.01f)
                return false;
        }

        return true;
    }

    IEnumerator AdvanceStage()
    {
        transitioning = true;

        yield return new WaitForSeconds(stageTransitionDelay);

        // Check again after the delay.
        if (!StageComplete(currentStage))
        {
            transitioning = false;
            yield break;
        }

        currentStage++;

        if (currentStage < stages.Length)
        {
            stages[currentStage].SetActive(true);
            Debug.Log("Stage " + (currentStage + 1) + " started!");
        }
        else
        {
            Debug.Log("Congratulations! Table setting complete!");
        }

        transitioning = false;
    }
}