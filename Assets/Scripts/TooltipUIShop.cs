using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipUIShop : MonoBehaviour
{
    private RectTransform background;
    [SerializeField] private TextMeshProUGUI toolTipText;
    [SerializeField] private Vector2 padding = new Vector2(8, 8);

    private RectTransform canvasRect;
    private RectTransform toolTipRect;
    private Camera uiCamera;

    private void Awake()
    {
        toolTipRect = GetComponent<RectTransform>();
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        uiCamera = GetComponentInParent<Canvas>().worldCamera;
        HideTooltip();
    }

    private void Update()
    {
        FollowMouse();
    }

    public void ShowTooltip()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(toolTipRect);

        FollowMouse();
        gameObject.SetActive(true);

    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    private void FollowMouse()
    {
        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, uiCamera, out anchoredPos);

        Vector2 tooltipSize = toolTipRect.sizeDelta;
        Vector2 pivot = new Vector2(0f, 1f);

        if (anchoredPos.y - tooltipSize.y < -canvasRect.rect.height / 2) pivot.y = 0f;
        else pivot.y = 1f;

        if (anchoredPos.x + tooltipSize.x > canvasRect.rect.width / 2) pivot.x = 1f;
        else pivot.x = 0f;

        toolTipRect.pivot = pivot;
        toolTipRect.anchoredPosition = anchoredPos + new Vector2(padding.x, -padding.y);
    }

    public void ChangeText(string text)
    {
        toolTipText.text = text;
        LayoutRebuilder.ForceRebuildLayoutImmediate(toolTipRect);
    }
}
