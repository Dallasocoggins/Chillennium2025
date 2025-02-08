using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class LightBurstPhysics : LightPhysics
{
    public float burstAliveTime;

    private new Light2D light;
    private new CircleCollider2D collider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        light = GetComponent<Light2D>();
        collider = GetComponent<CircleCollider2D>();

        StartCoroutine(DestroyAfterDelay());
    }

    // Update is called once per frame
    void Update()
    {
        collider.radius = light.pointLightOuterRadius;
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(burstAliveTime);
        Destroy(this.gameObject);
    }
}
