using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public float farDistanceToMonster;

    public AudioClip woodBlock;
    public AudioClip jumpscare;

    private static MusicManager instance;

    private GameObject templateAudioPlayer;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        templateAudioPlayer = new GameObject("AudioPlayer", typeof(AudioSource), typeof(DestroyAfterAudio));
        templateAudioPlayer.SetActive(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PlayAudio(AudioClip clip, bool useSpatialAudio, Vector3 position)
    {
        var audioPlayer = Instantiate(templateAudioPlayer, position, Quaternion.identity);

        var audioSource = audioPlayer.GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.spatialBlend = useSpatialAudio ? 1 : 0;

        audioPlayer.SetActive(true);
    }

    public void MonsterTeleport(float distanceFromPlayer, Vector3 position)
    {
        if (distanceFromPlayer > farDistanceToMonster)
        {

        }
    }

    public void MonsterAppearsNextToPlayer() {
        PlayAudio(jumpscare, false, Vector3.zero);
    }
}
