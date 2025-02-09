using UnityEngine;

public class Door : MonoBehaviour
{
    public int NumberOfKeysNeeded = 1;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().UnlockDoor(this);
        }
    }
}
