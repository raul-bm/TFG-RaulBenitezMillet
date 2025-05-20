using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private SwordPartInventory swordPartInventory;

    private Coroutine showCoroutine;

    public float delay = 0.3f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        UI.Instance.tooltipUI.GetComponent<TooltipUI>().ChangeText(swordPartInventory.textToTooltip);
        showCoroutine = StartCoroutine(ShowTooltipDelayed());
    }

    private IEnumerator ShowTooltipDelayed()
    {
        yield return new WaitForSecondsRealtime(delay);
        TooltipUI tooltip = UI.Instance.tooltipUI.GetComponent<TooltipUI>();
        tooltip.ShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(showCoroutine != null) StopCoroutine(showCoroutine);

        TooltipUI tooltip = UI.Instance.tooltipUI.GetComponent<TooltipUI>();
        tooltip.HideTooltip();
    }
}
