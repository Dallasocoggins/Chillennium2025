using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FadeScreen : MonoBehaviour
{
    public float fadeTime;

    private Image image;
    private bool fading = false;
    private float fadeProgress = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = GetComponent<Image>();
        image.color = new Color(0, 0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (fading)
        {
            fadeProgress = Mathf.Min(fadeProgress + Time.unscaledDeltaTime, fadeTime);
            var t = Mathf.SmoothStep(0, 1, fadeProgress / fadeTime);
            image.color = new Color(0, 0, 0, t);
        }

        if (fadeProgress >= fadeTime)
        {
            Debug.Log("Do something Dallas");
            Application.Quit();
        }
    }

    public void FadeToBlack()
    {
        fading = true;
    }
}
