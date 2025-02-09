using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public float farDistanceToMonster;

    public AudioClip[] teleportClips;
    public float teleportCooldown;
    public AudioClip jumpscare;
    public float jumpscareCooldown;
    public AudioClip ambientBass;
    public float ambientBassCooldown;
    public AudioClip ambientGong;
    public float ambientGongCooldown;
    public AudioClip die;
    public AudioClip eaten;
    public float timeToMuteMusic;

    public int[] menuSceneIds;

    private static MusicManager instance;

    private AudioSource mainMenuAudioSource;

    private GameObject templateAudioPlayer;
    private float timeSinceTeleport;
    private float timeSinceJumpscare;
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
        mainMenuAudioSource = GetComponentInChildren<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceTeleport += Time.unscaledDeltaTime;
        timeSinceJumpscare += Time.unscaledDeltaTime;
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

        mainMenuAudioSource.volume = Mathf.Clamp01(
            mainMenuAudioSource.volume + Time.deltaTime / timeToMuteMusic *
            (InMenu() ? 1 : -1));
    }

    private void PlayAudio(AudioClip clip, bool useSpatialAudio, Vector3 position, bool playInMenu = false)
    {
        if (playInMenu || !InMenu())
        {
            var audioPlayer = Instantiate(templateAudioPlayer, position, Quaternion.identity);

            var audioSource = audioPlayer.GetComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.spatialBlend = useSpatialAudio ? 1 : 0;

            audioPlayer.SetActive(true);
        }
    }

    private bool InMenu()
    {
        return menuSceneIds.Contains(SceneManager.GetActiveScene().buildIndex);
    }

    public void MonsterTeleport(Vector3 position)
    {
        if (timeSinceTeleport > teleportCooldown)
        {
            var index = (int)(teleportClips.Length * Random.value);
            if (index == teleportClips.Length)
            {
                index--;
            }
            var clip = teleportClips[index];
            PlayAudio(clip, true, position);
            timeSinceTeleport = 0;
        }
    }

    public void MonsterAppearsNextToPlayer() {
        if (timeSinceJumpscare > jumpscareCooldown)
        {
            PlayAudio(jumpscare, false, Vector3.zero);
            timeSinceJumpscare = 0;
        }
    }

    public void Die()
    {
        PlayAudio(die, false, Vector3.zero);
    }

    public void Eaten()
    {
        PlayAudio(eaten, false, Vector3.zero);
    }
}
