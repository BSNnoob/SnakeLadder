using UnityEngine;

public class Item 
{
    public enum ItemType {
        DoubleDice,
        AvoidSnake,
        LadderGrab,
    }

    public ItemType itemType;

    public Sprite GetSprite(){
        switch(itemType){
        default:
        case ItemType.AvoidSnake: return ItemAssets.Instance.avoidSnakeSprite;
        case ItemType.DoubleDice: return ItemAssets.Instance.doubleDiceSprite;
        case ItemType.LadderGrab: return ItemAssets.Instance.ladderGrabSprite;
        }
    }

}
