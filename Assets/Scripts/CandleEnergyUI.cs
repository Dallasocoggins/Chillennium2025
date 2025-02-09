using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class CandleEnergyUI : MonoBehaviour
{
    public float lengthWhenFull;
    public float lengthWhenEmpty;
    [Range(0f, 1f)]
    public float proportionLeft = 1;
    public float proportionFollowMultiplier;

    public Image stem;
    public Image end;

    private Animator animator;
    private float currentProportion = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        var difference = proportionLeft - currentProportion;
        currentProportion += difference * proportionFollowMultiplier * Time.deltaTime;

        var position = end.rectTransform.localPosition;
        position.y = GetLength();
        end.rectTransform.localPosition = position;

        var offsetMax = stem.rectTransform.offsetMax;
        offsetMax.y = GetLength();
        stem.rectTransform.offsetMax = offsetMax;
    }

    private float GetLength()
    {
        return Mathf.Lerp(lengthWhenEmpty, lengthWhenFull, currentProportion);
    }

    public void SetCandleOn(bool value)
    {
        animator.SetBool("candleOn", value);
    }

    public void LightBurst()
    {
        animator.SetTrigger("lightBurst");
    }
}
