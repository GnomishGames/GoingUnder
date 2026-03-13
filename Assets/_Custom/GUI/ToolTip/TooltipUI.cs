using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TooltipUI : MonoBehaviour
{
    static TooltipUI instance;

    public static TooltipUI Instance
    {
        get
        {
            if (instance == null)
            {
                TooltipUI[] tooltips = Resources.FindObjectsOfTypeAll<TooltipUI>();

                foreach (TooltipUI tooltip in tooltips)
                {
                    if (!tooltip.gameObject.scene.IsValid())
                    {
                        continue;
                    }

                    instance = tooltip;
                    instance.EnsureInitialized();
                    break;
                }
            }

            return instance;
        }
    }

    static readonly Vector3 MouseOffset = new Vector3(10f, 75f, 0f);

    public GameObject panel;
    public Transform container;
    public GameObject linePrefab;
    Transform originalParent;

    //drag layer reference
    Transform dragLayer;
    bool isInitialized;

    void Awake()
    {
        instance = this;
        EnsureInitialized();

        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    void Update()
    {
        if (panel != null && panel.activeSelf)
        {
            UpdatePosition();
        }
    }

    public static void ShowTooltip(ItemSO item)
    {
        TooltipUI tooltip = Instance;

        if (tooltip == null || item == null)
        {
            return;
        }

        tooltip.Show(item);
    }

    public static void HideTooltip()
    {
        TooltipUI tooltip = Instance;

        if (tooltip == null)
        {
            return;
        }

        tooltip.Hide();
    }

    public void Show(ItemSO item)
    {
        EnsureInitialized();

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        Clear();

        List<TooltipLine> lines = item.GetTooltip();

        foreach (var line in lines)
        {
            GameObject obj = Instantiate(linePrefab, container);

            var texts = obj.GetComponentsInChildren<TextMeshProUGUI>();

            texts[0].text = line.Left;
            texts[1].text = line.Right;

            texts[0].color = line.color;
            texts[1].color = line.color;
        }

        Transform targetParent = dragLayer != null ? dragLayer : originalParent;

        if (targetParent != null)
        {
            transform.SetParent(targetParent, true);
        }

        transform.SetAsLastSibling();
        UpdatePosition();
        UpdateSize();

        if (panel != null)
        {
            panel.SetActive(true);
        }
    }

    public void Hide()
    {
        EnsureInitialized();

        if (originalParent != null && transform.parent != originalParent)
        {
            Vector3 worldPos = transform.position;
            transform.SetParent(originalParent, true);
            transform.position = worldPos;
        }

        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    void EnsureInitialized()
    {
        if (isInitialized)
        {
            return;
        }

        originalParent = transform.parent;

        if (dragLayer == null)
        {
            GameObject dragLayerObject = GameObject.FindWithTag("DragLayer");

            if (dragLayerObject != null)
            {
                dragLayer = dragLayerObject.transform;
            }
        }

        isInitialized = true;
    }

    void UpdatePosition()
    {
        transform.position = Input.mousePosition + MouseOffset;
    }

    void Clear()
    {
        if (container == null)
        {
            return;
        }

        foreach (Transform child in container)
            Destroy(child.gameObject);
    }

    void UpdateSize()
    {
        if (panel == null || container == null)
        {
            return;
        }

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        RectTransform containerRect = (RectTransform)container;

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(containerRect);

        float preferredHeight = LayoutUtility.GetPreferredHeight(containerRect);
        panelRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredHeight);
    }
}