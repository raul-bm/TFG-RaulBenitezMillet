using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SwordPartInventory : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public SwordPartScripteable partScriptable { get; private set; }

    private Transform originalParent;
    private CanvasGroup canvasGroup;
    public RectTransform rectTransform;

    public string textToTooltip = "";

    #region Stats
    public string partName { get; private set; }
    public string description { get; private set; }
    public TypeSwordPart partType { get; private set; }
    public SetOfSword setOfSword { get; private set; }
    public List<StatModifier> statModifiers { get; private set; }
    #endregion

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if(canvasGroup == null ) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void SetData(SwordPartScripteable part)
    {
        partName = part.partName;
        description = part.description;
        partType = part.partType;
        setOfSword = part.setOfSword;

        statModifiers = part.statModifiers;

        this.GetComponent<Image>().sprite = part.partImageIcon;

        partScriptable = part;

        ChangeText();
    }

    private void ChangeText()
    {
        textToTooltip += "<b>" + partName + "</b>\n\n";
        textToTooltip += description + "\n\n";

        textToTooltip += "<b><color=" + InventoryUI.Instance.GetColorHexForEachSet(setOfSword) + ">" + setOfSword.ToString() + " Sword</color> Set</b>\n\n";

        foreach( var modifier in statModifiers )
        {
            textToTooltip += "<b>";
            textToTooltip += "<color=" + InventoryUI.Instance.GetColorHexForEachStat(modifier.stat) + ">" + InventoryUI.Instance.GetFormattedStatName(modifier.stat) + ":</color> " + (modifier.value >= 0 ? "+" : "");
            textToTooltip += modifier.value;
            textToTooltip += "</b>\n";
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        GameObject dropTarget = eventData.pointerCurrentRaycast.gameObject;

        // Si el raycast da al objeto coge el slot (si acaso sale fuera del slot el raycast, igual vuelve a su posicion)
        if (dropTarget != null && !dropTarget.CompareTag("Slot")) dropTarget = dropTarget.transform.parent.gameObject;

        bool isInsideInventory = RectTransformUtility.RectangleContainsScreenPoint(InventoryUI.Instance.inventoryArea, Input.mousePosition);

        if (dropTarget != null && dropTarget.CompareTag("Slot"))
        {
            // Si se mueve a un slot de inventario se cambia a ese slot (y en el que estaba se pone el objeto que hay en el slot target si hay un objeto)
            if (InventoryUI.Instance.inventorySlots.Contains(dropTarget))
                InventoryUI.Instance.ChangeSwordPartSlotInventory(this.gameObject, originalParent.gameObject, dropTarget);
            // Si se mueve a un slot de parte equipada se intenta equipar
            else if(!InventoryUI.Instance.EquipPartSword(this.gameObject, originalParent.gameObject, dropTarget))
            {
                transform.SetParent(originalParent);
                rectTransform.localPosition = Vector3.zero;
            }
        }
        // Si no ha tocado un slot pero está dentro del inventario aún, se vuelve a la posición donde estaba
        else if (isInsideInventory)
        {
            transform.SetParent(originalParent);
            rectTransform.localPosition = Vector3.zero;
        }
        // Si mueve el objeto fuera del inventario se dropea en el suelo
        else
        {
            if (originalParent.gameObject.GetComponent<SlotPartEquipped>() == null)
            {
                InventoryUI.Instance.DropAtPlayerPosition(partScriptable, originalParent.gameObject);
                InventoryUI.Instance.numActualObjects--;
            }
            else InventoryUI.Instance.DropOtherParts(this.gameObject);

            Destroy(this.gameObject);
        }
    }
}
