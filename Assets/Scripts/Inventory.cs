using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory {
    private List<Item> itemList;
    private Action<Item> useItemAction;

    public Inventory(Action<Item> useItemAction)
    {
        this.useItemAction = useItemAction;
        itemList = new List<Item>();
    }

    public Action OnItemListChanged { get; internal set; }

    public void AddItem(Item item){
        itemList.Add(item);
        OnItemListChanged?.Invoke();
    }

    public void RemoveItem(Item item)
    {
        itemList.Remove(item);
        OnItemListChanged?.Invoke();
    }

    public List<Item> GetItemList()
    {
        return itemList;
    }

    public void UseItem(Item item){
        useItemAction(item);
    }

    [System.Serializable]
    private class InventorySaveData
    {
        public List<Item> items;
    }

    public string ToJson()
    {
        InventorySaveData saveData = new InventorySaveData
        {
            items = this.itemList
        };
        return JsonUtility.ToJson(saveData);
    }

    public void LoadFromJson(string json)
    {
        if (string.IsNullOrEmpty(json)) return;

        InventorySaveData loaded = JsonUtility.FromJson<InventorySaveData>(json);
        if (loaded != null && loaded.items != null)
        {
            this.itemList = loaded.items;
        }
        else
        {
            this.itemList = new List<Item>();
        }

        OnItemListChanged?.Invoke();
    }

}

