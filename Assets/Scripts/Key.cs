using UnityEngine;

public class Key : MonoBehaviour
{
    public float bobHeight;
    public float bobPeriod;
    public float collectTime;
    public Vector3 targetScale;

    private Vector3 start;
    private Vector3 startScale;
    private float timePassed;

    private bool collected = false;
    private float collectCountUp = 0;
    private Transform player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        start = transform.position;
        startScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (collected)
        {
            var t = Mathf.SmoothStep(0, 1, collectCountUp / collectTime);
            transform.position = Vector3.Lerp(start, player.position, t);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            collectCountUp += Time.deltaTime;
            if (collectCountUp > collectTime )
            {
                Destroy(gameObject);
            }
        }
        else
        {
            var displacement = Mathf.Sin(timePassed / bobPeriod * 2 * Mathf.PI) * bobHeight;
            transform.position = start + Vector3.up * displacement;
            timePassed += Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !collected)
        {
            collected = true;
            player = collision.transform;
            start = transform.position;
            collision.gameObject.GetComponent<Player>().CollectKey();
        }
    }
}
