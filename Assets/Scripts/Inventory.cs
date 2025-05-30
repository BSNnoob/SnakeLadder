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

    public List<Item> GetItemList(){
        return itemList;
    }

    public void UseItem(Item item){
        useItemAction(item);
    }
}

