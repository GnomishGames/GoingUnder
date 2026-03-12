using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance;

    public GameObject panel;
    public Transform container;
    public GameObject linePrefab;
    Transform originalParent;

    //drag layer reference
    Transform dragLayer;

    void Awake()
    {
        originalParent = transform.parent;
        Instance = this;
        Hide();

        //draglayer keeps icons on top when dragging
        dragLayer = GameObject.FindWithTag("DragLayer").transform;
    }

    void Update()
    {
        if (panel.activeSelf)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void Show(ItemSO item)
    {
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

        transform.SetParent(dragLayer, true);
        transform.SetAsLastSibling();

        panel.SetActive(true);
    }

    public void Hide()
    {
        if (transform.parent != originalParent)
        {
            // If the tooltip is currently a child of the drag layer, move it back to its original parent
            transform.SetParent(originalParent, true);
            Vector3 worldPos = transform.position;
            transform.SetParent(originalParent, true);
            transform.position = worldPos;
        }
        panel.SetActive(false);
    }

    void Clear()
    {
        foreach (Transform child in container)
            Destroy(child.gameObject);
    }
}