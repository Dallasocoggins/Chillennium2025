using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
[RequireComponent(typeof(PolygonCollider2D))]
public class SpotLightPhysics : LightPhysics
{
    public int points = 5;

    private new Light2D light;
    private new PolygonCollider2D collider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        light = GetComponent<Light2D>();
        collider = GetComponent<PolygonCollider2D>();

        collider.isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        var colliderPoints = new Vector2[points + 1];
        var lightRadius = light.pointLightOuterRadius;
        var startAngle = -light.pointLightOuterAngle / 2;
        var endAngle = light.pointLightOuterAngle / 2;
        for (int i = 0; i < points; i++)
        {
            var angle = Mathf.Lerp(startAngle, endAngle, (float)i / (points - 1));
            var direction = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad));
            var worldDirection = transform.localToWorldMatrix * direction;
            var hits = Physics2D.RaycastAll(transform.position, worldDirection, lightRadius, 1 << 3);
            var hit = hits.FirstOrDefault((hit) => hit.collider.tag != "OneWayPlatform");
            var actualRadius = hit ? hit.distance : lightRadius;
            colliderPoints[i] = direction * actualRadius;
        }
        colliderPoints[points] = Vector2.zero;
        collider.points = colliderPoints;
    }
}
