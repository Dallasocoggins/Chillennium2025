using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public float farDistanceToMonster;


    private static MusicManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MonsterTeleport(float distanceFromPlayer)
    {

    }

    public void MonsterAppearsNextToPlayer() {
        
    }
}
