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
    public class SerializableInventory
    {
        public List<Item> items = new List<Item>();
    }


    public string ToJson()
    {
        SerializableInventory s = new SerializableInventory();
        s.items = GetItemList();
        return JsonUtility.ToJson(s);
    }


    public void LoadFromJson(string json)
    {
        if (string.IsNullOrEmpty(json)) return;

        SerializableInventory s = JsonUtility.FromJson<SerializableInventory>(json);
        itemList = s.items ?? new List<Item>();

        OnItemListChanged?.Invoke();
    }


}

