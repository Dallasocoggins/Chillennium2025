using UnityEngine;

public class Key : Item
{
    public override void OnItemCollect(Player player)
    {
        player.CollectKey();
    }
}
