using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public float farDistanceToMonster;

    public AudioClip[] teleportClips;
    public AudioClip jumpscare;
    public AudioClip ambientBass;
    public float ambientBassCooldown;
    public AudioClip ambientGong;
    public float ambientGongCooldown;

    private static MusicManager instance;

    private GameObject templateAudioPlayer;
    private float timeSinceBass;
    private float timeSinceGong;

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
        timeSinceBass += Time.unscaledDeltaTime;
        timeSinceGong += Time.unscaledDeltaTime;

        if (timeSinceBass > ambientBassCooldown && Random.value < 0.02f)
        {
            PlayAudio(ambientBass, false, Vector3.zero);
            timeSinceBass = 0;
        }

        if (timeSinceGong > ambientGongCooldown && Random.value < 0.02f)
        {
            PlayAudio(ambientGong, false, Vector3.zero);
            timeSinceGong = 0;
        }
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
            var index = (int)(teleportClips.Length * Random.value);
            if (index == teleportClips.Length)
            {
                index--;
            }
            var clip = teleportClips[index];
            PlayAudio(clip, true, position);
        }
    }

    public void MonsterAppearsNextToPlayer() {
        PlayAudio(jumpscare, false, Vector3.zero);
    }
}
