using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI Instance;

    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject inventoryObjectPrefab;
    [SerializeField] private GameObject swordPartWorld;
    [SerializeField] private List<TypeSwordPart> dictionarySlotsPartsEquippedKeys;
    [SerializeField] private List<GameObject> dictionarySlotsPartsEquippedValues;
    [SerializeField] private TextMeshProUGUI currentStatsText;
    [SerializeField] private TextMeshProUGUI bonusSetText;
    [SerializeField] private PlayerController playerController;
    public List<GameObject> inventorySlots;
    public Dictionary<TypeSwordPart, GameObject> inventorySlotsPartsEquipped;
    public GameObject tooltipUI;

    public RectTransform inventoryArea;

    public Image cooldownImageFiller;
    public Image healthImageFiller;

    // First GameObject = Slot ---- Second GameObject = the added sword part
    private Dictionary<GameObject, GameObject> inventorySlotsOccupied = new Dictionary<GameObject, GameObject>();
    private Dictionary<GameObject, GameObject> inventorySlotsPartsEquippedOccupied = new Dictionary<GameObject, GameObject>();
    public int numActualObjects = 0;

    private void Awake()
    {
        Instance = this;

        inventorySlotsPartsEquipped = new Dictionary<TypeSwordPart, GameObject>();

        for (int i = 0; i < Mathf.Min(dictionarySlotsPartsEquippedKeys.Count, dictionarySlotsPartsEquippedValues.Count); i++) {
            if (!inventorySlotsPartsEquipped.ContainsKey(dictionarySlotsPartsEquippedKeys[i])) inventorySlotsPartsEquipped.Add(dictionarySlotsPartsEquippedKeys[i], dictionarySlotsPartsEquippedValues[i]);
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab)) {
            if (inventoryUI.activeSelf)
            {
                Time.timeScale = 1f;
                inventoryUI.SetActive(false);
                tooltipUI.SetActive(false);
            }
            else
            {
                Time.timeScale = 0f;
                inventoryUI.SetActive(true);
            } 
        }
    }

    public bool AddObject(SwordPartScripteable part)
    {
        if(numActualObjects <= inventorySlots.Count)
        {
            GameObject slotToOccupy = null;

            foreach (var slot in inventorySlots)
            {
                if (!inventorySlotsOccupied.ContainsKey(slot))
                {
                    slotToOccupy = slot;
                    break;
                }
            }

            if(slotToOccupy != null)
            {
                GameObject newObject = Instantiate(inventoryObjectPrefab, slotToOccupy.transform);
                newObject.GetComponent<SwordPartInventory>().SetData(part);

                inventorySlotsOccupied.Add(slotToOccupy, newObject);

                numActualObjects++;

                return true;
            }
        }

        return false;
    }

    public void ChangeSwordPartSlotInventory(GameObject swordPart, GameObject oldSlot, GameObject newSlot) // SIEMPRE NEWSLOT ES UN SLOT DEL INVENTARIO
    {
        if(oldSlot.GetComponent<SlotPartEquipped>() == null) // El objeto viene de un slot del inventario
        {
            // Intercambiar objetos en los slots
            if (inventorySlotsOccupied.ContainsKey(newSlot))
            {
                GameObject otherSwordPart = inventorySlotsOccupied[newSlot];

                inventorySlotsOccupied[newSlot] = swordPart;
                inventorySlotsOccupied[oldSlot] = otherSwordPart;

                swordPart.transform.SetParent(newSlot.transform);
                otherSwordPart.transform.SetParent(oldSlot.transform);

                swordPart.transform.localPosition = Vector3.zero;
                otherSwordPart.transform.localPosition = Vector3.zero;
            }
            else // Colocar el objeto en el nuevo slot
            {
                inventorySlotsOccupied.Remove(oldSlot);
                inventorySlotsOccupied.Add(newSlot, swordPart);

                swordPart.transform.SetParent(newSlot.transform);
                swordPart.transform.localPosition = Vector3.zero;
            }
        }
        /* 
         * Si se mueve de dentro de los equipados un objeto a un hueco vacio del inventario:
         *      - Lo quito de los equipados
         *      - Quito los de las fases siguientes (es decir si quito el pomo quito todo, si quito el crossguard quito el blade, etc.
         * 
         * Si se mueve a un objeto del mismo tipo se intercambian
         * Si se mueve a un objeto de distinto tipo se vuelve a dónde estaba sin hacer ningún cambio
         */
        else
        {
            if(!inventorySlotsOccupied.ContainsKey(newSlot)) // ES UN HUECO VACIO
            {
                // Lo quito de los equipados
                inventorySlotsPartsEquippedOccupied.Remove(oldSlot);
                inventorySlotsOccupied.Add(newSlot, swordPart);
                swordPart.transform.SetParent(newSlot.transform);
                swordPart.transform.localPosition = Vector3.zero;

                numActualObjects++;

                //Quito los de las fases siguientes (es decir si quito el pomo quito todo, si quito el crossguard quito el blade, etc.
                TypeSwordPart swordPartPhase = swordPart.GetComponent<SwordPartInventory>().partType + 1;
                int contSlot = 0;

                while ((int)swordPartPhase < Enum.GetValues(typeof(TypeSwordPart)).Length && inventorySlotsPartsEquippedOccupied.ContainsKey(inventorySlotsPartsEquipped[swordPartPhase]) && contSlot < inventorySlots.Count)
                {
                    if (!inventorySlotsOccupied.ContainsKey(inventorySlots[contSlot]))
                    {
                        GameObject tempSwordPart = inventorySlotsPartsEquippedOccupied[inventorySlotsPartsEquipped[swordPartPhase]];

                        inventorySlotsPartsEquippedOccupied.Remove(inventorySlotsPartsEquipped[swordPartPhase]);
                        inventorySlotsOccupied.Add(inventorySlots[contSlot], tempSwordPart);
                        tempSwordPart.transform.SetParent(inventorySlots[contSlot].transform);
                        tempSwordPart.transform.localPosition = Vector3.zero;

                        numActualObjects++;

                        swordPartPhase++;
                    }

                    contSlot++;
                }

                DropOtherParts(swordPart, (int)swordPartPhase);
            }
            else // NO ES UN HUECO VACIO
            {
                GameObject otherSwordPart = inventorySlotsOccupied[newSlot];
                SwordPartInventory scriptOtherSwordPart = otherSwordPart.GetComponent<SwordPartInventory>();

                // Si se mueve a un objeto del mismo tipo se intercambian
                if (swordPart.GetComponent<SwordPartInventory>().partType == scriptOtherSwordPart.partType)
                {
                    inventorySlotsOccupied[newSlot] = swordPart;
                    inventorySlotsPartsEquippedOccupied[oldSlot] = otherSwordPart;

                    swordPart.transform.SetParent(newSlot.transform);
                    otherSwordPart.transform.SetParent(oldSlot.transform);

                    swordPart.transform.localPosition = Vector3.zero;
                    otherSwordPart.transform.localPosition = Vector3.zero;
                }
                // Si se mueve a un objeto de distinto tipo se vuelve a dónde estaba sin hacer ningún cambio
                else
                {
                    swordPart.transform.SetParent(oldSlot.transform);
                    swordPart.transform.localPosition = Vector3.zero;
                }
            }

            ApplySwordParts();
        }
        
    }

    public void DropOtherParts(GameObject swordPart, int swordPartPhaseInt = -1)
    {
        TypeSwordPart swordPartPhase = swordPartPhaseInt != -1 ? (TypeSwordPart)swordPartPhaseInt : swordPart.GetComponent<SwordPartInventory>().partType;

        while ((int)swordPartPhase < Enum.GetValues(typeof(TypeSwordPart)).Length && inventorySlotsPartsEquippedOccupied.ContainsKey(inventorySlotsPartsEquipped[swordPartPhase]))
        {
            GameObject tempSwordPart = inventorySlotsPartsEquippedOccupied[inventorySlotsPartsEquipped[swordPartPhase]];

            DropAtPlayerPosition(tempSwordPart.GetComponent<SwordPartInventory>().partScriptable, inventorySlotsPartsEquipped[swordPartPhase]);
            Destroy(tempSwordPart);

            swordPartPhase++;
        }
    }

    public void DropAtPlayerPosition(SwordPartScripteable partScriptable, GameObject oldSlot)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("Player not found. Why? xd");
            return;
        }

        Vector3 dropPosition = player.transform.position;
        dropPosition += Vector3.down * 0.5f;

        GameObject newObject = Instantiate(swordPartWorld, dropPosition, Quaternion.identity);
        newObject.GetComponent<SwordPartPickup>().SetSwordPart(partScriptable);

        if (oldSlot.GetComponent<SlotPartEquipped>() == null) inventorySlotsOccupied.Remove(oldSlot);
        else inventorySlotsPartsEquippedOccupied.Remove(oldSlot);

        ApplySwordParts();
    }

    public bool EquipPartSword(GameObject swordPart, GameObject oldSlot, GameObject newSlot) // SIEMRPE NEWSLOT ES UN SLOT DEL EQUIPAMIENTO
    {
        SwordPartInventory scriptSwordPart = swordPart.GetComponent<SwordPartInventory>();
        SlotPartEquipped scriptNewSlot = newSlot.GetComponent<SlotPartEquipped>();

        if(scriptSwordPart.partType == scriptNewSlot.typeSwordPartSlot && inventorySlotsPartsEquippedOccupied.ContainsKey(newSlot))
        {
            GameObject otherSwordPart = inventorySlotsPartsEquippedOccupied[newSlot];

            inventorySlotsOccupied[oldSlot] = otherSwordPart;
            inventorySlotsPartsEquippedOccupied[newSlot] = swordPart;

            swordPart.transform.SetParent(newSlot.transform);
            otherSwordPart.transform.SetParent(oldSlot.transform);

            swordPart.transform.localPosition = Vector3.zero;
            otherSwordPart.transform.localPosition = Vector3.zero;

            ApplySwordParts();

            return true;
        }
        else if(scriptSwordPart.partType == scriptNewSlot.typeSwordPartSlot &&
            !inventorySlotsPartsEquippedOccupied.ContainsKey(newSlot) &&
            (scriptSwordPart.partType == TypeSwordPart.Pommel && inventorySlotsPartsEquippedOccupied.Count == 0 ||
            scriptSwordPart.partType == TypeSwordPart.Grip && inventorySlotsPartsEquippedOccupied.Count == 1 ||
            scriptSwordPart.partType == TypeSwordPart.Crossguard && inventorySlotsPartsEquippedOccupied.Count == 2 ||
            scriptSwordPart.partType == TypeSwordPart.Blade && inventorySlotsPartsEquippedOccupied.Count == 3))
        {
            inventorySlotsOccupied.Remove(oldSlot);
            inventorySlotsPartsEquippedOccupied.Add(newSlot, swordPart);

            swordPart.transform.SetParent(newSlot.transform);
            swordPart.transform.localPosition = Vector3.zero;

            numActualObjects--;

            ApplySwordParts();

            return true;
        }

        return false;
    }

    private void ApplySwordParts()
    {
        List<SwordPartInventory> swordParts = new List<SwordPartInventory>();

        foreach(var swordPartDictionary in inventorySlotsPartsEquippedOccupied)
        {
            swordParts.Add(swordPartDictionary.Value.GetComponent<SwordPartInventory>());
        }

        playerController.ApplySwordParts(swordParts);
    }

    // Returns the equivalent hex color of the stat type for showing it on UI
    public string GetColorHexForEachStat(StatType type)
    {
        string hexColor = "";

        switch(type)
        {
            case (StatType.Damage):
                hexColor = "#e67e22";
                break;

            case (StatType.Defense):
                hexColor = "#2980b9";
                break;

            case (StatType.AttackRange):
                hexColor = "#9b59b6";
                break;

            case (StatType.AttackSpeed):
                hexColor = "#f1c40f";
                break;

            default:
                hexColor = "#FFFFFF";
                break;
        }

        return hexColor;
    }

    // Returns the equivalent hex color of the set of sword for showing it on UI
    public string GetColorHexForEachSet(SetOfSword set)
    {
        string hexColor = "";

        switch(set)
        {
            case (SetOfSword.Ice):
                hexColor = "#bddeec";
                break;

            case (SetOfSword.Blood):
                hexColor = "#8a0303";
                break;
        }

        return hexColor;
    }

    // Returns the string with the words with spaces
    public string GetFormattedStatName(StatType type)
    {
        return Regex.Replace(type.ToString(), "(\\B[A-Z])", " $1");
    }

    public void ChangeCurrentStatsText(string text)
    {
        currentStatsText.text = text;
    }

    public void ChangeBonusSetText(string text)
    {
        bonusSetText.text = text;
    }
}