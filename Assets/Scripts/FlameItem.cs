using UnityEngine;

public class FlameItem : Item
{
    public override void OnItemCollect(Player player)
    {
        player.CollectFlame();
    }
}
