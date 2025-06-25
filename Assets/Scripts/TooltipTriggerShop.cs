using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTriggerShop : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private SwordPartShop swordPartShop;
    [SerializeField] private TooltipUIShop tooltip;

    private Coroutine showCoroutine;

    public float delay = 0.3f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.ChangeText(swordPartShop.textToTooltip);
        showCoroutine = StartCoroutine(ShowTooltipDelayed());
    }

    private IEnumerator ShowTooltipDelayed()
    {
        yield return new WaitForSecondsRealtime(delay);
        tooltip.ShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (showCoroutine != null) StopCoroutine(showCoroutine);
        tooltip.HideTooltip();
    }
}
