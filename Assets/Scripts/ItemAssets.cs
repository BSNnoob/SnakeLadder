using UnityEngine;

public class ItemAssets : MonoBehaviour
{
    public static ItemAssets Instance;

    public Sprite avoidSnakeSprite;
    public Sprite doubleDiceSprite;
    public Sprite ladderGrabSprite;

    void Awake()
    {
        Instance = this;
    }

    public Sprite GetSprite(Item.ItemType type)
    {
        switch (type)
        {
            case Item.ItemType.AvoidSnake: return avoidSnakeSprite;
            case Item.ItemType.DoubleDice: return doubleDiceSprite;
            case Item.ItemType.LadderGrab: return ladderGrabSprite;
            default: return null;
        }
    }
}
