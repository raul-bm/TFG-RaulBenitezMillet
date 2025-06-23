using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ShopMenuItem
{
    public int setNumber;
    public int partNumber;
    public GameObject gameObject;
    public int cost;
}

public class ShopMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] ShopMenuItem[] shopItems;

    private Dictionary<(int, int), ShopMenuItem> dictionaryShopItems = new Dictionary<(int, int), ShopMenuItem>();

    private int money;

    public void InitializeShopMenu()
    {
        dictionaryShopItems = new Dictionary<(int, int), ShopMenuItem> ();
        
        foreach (var shopItem in shopItems)
        {
            dictionaryShopItems.Add((shopItem.setNumber, shopItem.partNumber), shopItem);
        }

        foreach (var shopItem in dictionaryShopItems.Values)
        {
            ShowItemAsNotBought(shopItem);
        }

        money = PlayerPrefs.GetInt("moneyPlayer");

        moneyText.text = money.ToString();

        string[] unlockedItems = PlayerPrefs.GetString("unlockedItems").Split('-');

        foreach(string unlockedItem in unlockedItems)
        {
            if(unlockedItem != "")
            {
                string[] infoUnlockedItem = unlockedItem.Split(' ');
                int setNumber = int.Parse(infoUnlockedItem[0]);
                int partNumber = int.Parse(infoUnlockedItem[1]);

                ShowItemAsBought(dictionaryShopItems[(setNumber, partNumber)]);
            }
        }
    }

    public void BuyItem(string info)
    {
        string[] infoData = info.Split(' ');

        int setNumber = int.Parse(infoData[0]);
        int partNumber = int.Parse(infoData[1]);
        int costMoney = int.Parse(infoData[2]);

        if(money >= costMoney)
        {
            // Change money
            money -= costMoney;
            moneyText.text = money.ToString();
            PlayerPrefs.SetInt("moneyPlayer", money);

            string unlockedItems = PlayerPrefs.GetString("unlockedItems");

            unlockedItems += setNumber.ToString() + " " + partNumber.ToString() + "-";

            PlayerPrefs.SetString("unlockedItems", unlockedItems);

            ShowItemAsBought(dictionaryShopItems[(setNumber, partNumber)]);
        }
    }

    private void ShowItemAsBought(ShopMenuItem unlockedGameObject)
    {
        unlockedGameObject.gameObject.GetComponent<Button>().interactable = false;
        unlockedGameObject.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Bought!";
    }

    private void ShowItemAsNotBought(ShopMenuItem unlockedGameObject)
    {
        unlockedGameObject.gameObject.GetComponent<Button>().interactable = true;
        unlockedGameObject.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = unlockedGameObject.cost.ToString();
    }
}
