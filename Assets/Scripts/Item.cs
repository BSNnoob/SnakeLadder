using UnityEngine;


[System.Serializable]
public class Item
{
    public enum ItemType
    {
        AvoidSnake,
        DoubleDice,
        LadderGrab
    }

    public ItemType itemType;

    public string GetTitle()
    {
        switch (itemType)
        {
            case ItemType.AvoidSnake: return "Avoid Snake";
            case ItemType.DoubleDice: return "Double Dice";
            case ItemType.LadderGrab: return "Ladder Grab";
            default: return "Unknown Item";
        }
    }

    public string GetDescription()
    {
        switch (itemType)
        {
            case ItemType.AvoidSnake: return "Prevents a snake from biting you once.";
            case ItemType.DoubleDice: return "Rolls two dice instead of one.";
            case ItemType.LadderGrab: return "Pulls you to the nearest pipe.";
            default: return "";
        }
    }

    public Sprite GetSprite()
    {
        return ItemAssets.Instance.GetSprite(itemType);
    }
}
