
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TablewareHoverLabel : MonoBehaviour
{
    [Header("Label Settings")]
    public string displayName;
    public Vector3 labelOffset = new Vector3(0, 0.25f, 0);

    private XRGrabInteractable grab;
    private GameObject labelObject;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();

        // Create a world-space Canvas.
        labelObject = new GameObject("HoverLabel");
        labelObject.transform.SetParent(null);
        labelObject.transform.position = transform.position + labelOffset;

        Canvas canvas = labelObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        RectTransform canvasRect =
            labelObject.GetComponent<RectTransform>();

        canvasRect.sizeDelta = new Vector2(260, 65);
        canvasRect.localScale = Vector3.one * 0.0015f;

        // Dark background.
        GameObject background = new GameObject("Background");
        background.transform.SetParent(labelObject.transform, false);

        Image image = background.AddComponent<Image>();
        image.color = new Color(0.08f, 0.09f, 0.11f, 0.92f);
        image.raycastTarget = false;

        RectTransform bgRect =
            background.GetComponent<RectTransform>();

        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // White TextMeshPro label.
        GameObject textObject = new GameObject("LabelText");
        textObject.transform.SetParent(background.transform, false);

        TextMeshProUGUI text =
            textObject.AddComponent<TextMeshProUGUI>();

        text.text = string.IsNullOrEmpty(displayName)
            ? gameObject.name
            : displayName;

        text.fontSize = 32;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        text.enableWordWrapping = false;
        text.raycastTarget = false;

        RectTransform textRect =
            textObject.GetComponent<RectTransform>();

        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10, 5);
        textRect.offsetMax = new Vector2(-10, -5);

        // Hidden until hovered.
        labelObject.SetActive(false);
    }

    void OnEnable()
    {
        if (grab != null)
        {
            grab.hoverEntered.AddListener(ShowLabel);
            grab.hoverExited.AddListener(HideLabel);
        }
    }

    void OnDisable()
    {
        if (grab != null)
        {
            grab.hoverEntered.RemoveListener(ShowLabel);
            grab.hoverExited.RemoveListener(HideLabel);
        }

        if (labelObject != null)
            labelObject.SetActive(false);
    }

    void ShowLabel(HoverEnterEventArgs args)
    {
        if (labelObject != null)
            labelObject.SetActive(true);
    }

    void HideLabel(HoverExitEventArgs args)
    {
        if (labelObject != null)
            labelObject.SetActive(false);
    }

    
    void LateUpdate()
    {
        if (labelObject == null)
            return;

        // Keep the label above the object.
        labelObject.transform.position =
        transform.position + labelOffset;

        // Always face the player's camera.
        Camera cam = Camera.main;

        if (cam != null)
        {
            labelObject.transform.rotation =
            cam.transform.rotation;
        }
    }

    void OnDestroy()
    {   
    if (labelObject != null)
        Destroy(labelObject);
    }
}