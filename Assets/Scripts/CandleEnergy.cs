using UnityEngine;
using UnityEngine.UI;

public class CandleEnergy : MonoBehaviour
{
    public float lengthWhenFull;
    public float lengthWhenEmpty;
    [Range(0f, 1f)]
    public float t;

    public Image stem;
    public Image end;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var position = end.rectTransform.localPosition;
        position.y = GetLength();
        end.rectTransform.localPosition = position;

        var offsetMax = stem.rectTransform.offsetMax;
        offsetMax.y = GetLength();
        stem.rectTransform.offsetMax = offsetMax;
    }

    private float GetLength()
    {
        return Mathf.Lerp(lengthWhenEmpty, lengthWhenFull, t);
    }
}
