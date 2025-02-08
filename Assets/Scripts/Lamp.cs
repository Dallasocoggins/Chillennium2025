using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Animator))]
public class Lamp : MonoBehaviour
{
    public enum Animation
    {
        NeverTurnsOff = 0,
        On10OffShort = 1,
        On5Off1 = 2,
        On3Off2 = 3,
        On1Off1 = 4,
    }

    public new Animation animation;
    public float pointLightMultiplier;
    public float pointLightMinIntensity;
    public Light2D spotLight;
    public Light2D pointLight;

    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetInteger("animation", (int)animation);

    }

    // Update is called once per frame
    void Update()
    {
        pointLight.intensity = Mathf.Max(spotLight.intensity * pointLightMultiplier, pointLightMinIntensity);
    }
}
