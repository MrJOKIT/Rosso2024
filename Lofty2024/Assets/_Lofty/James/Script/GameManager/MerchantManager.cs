using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemList
{
    HealthPotion,
    ShieldArmor,
}
[Serializable]
public class ItemStock
{
    public ItemList itemName;
    public int itemCost;
}
public class MerchantManager : Singeleton<MerchantManager>
{
    [Header("GUI")]
    public GameObject merchantCanvas;
    public bool shopActive;
    [Space(10)] 
    [Header("Merchant")] 
    public List<ItemStock> itemStocks;
    
    public void OpenShop()
    {
        merchantCanvas.SetActive(true);
        shopActive = true;
    }

    public void CloseShop()
    {
        merchantCanvas.SetActive(false);
        shopActive = false;
    }

    public void BuyItem(int index)
    {
        if ( GetComponent<GameCurrency>().EricCoin < itemStocks[index].itemCost)
        {
            Debug.Log("Not enough money");
            return;
        }
        ItemActive(itemStocks[index].itemName);
        GetComponent<GameCurrency>().DecreaseEricCoin(itemStocks[index].itemCost);
    }

    private void ItemActive(ItemList itemName)
    {
        switch (itemName)
        {
            case ItemList.HealthPotion:
                GetComponent<GameManager>().currentRoomPos.GetComponent<RoomManager>().playerTrans.GetComponent<Player>().TakeHealth(1);
                break;
            case ItemList.ShieldArmor:
                GetComponent<GameManager>().currentRoomPos.GetComponent<RoomManager>().playerTrans.GetComponent<Player>().ActiveShield();
                break;
        }
    }
}
