using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DestroyAfterAudio : MonoBehaviour
{
    private AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying && isActiveAndEnabled)
        {
            Destroy(gameObject);
        }
    }
}
