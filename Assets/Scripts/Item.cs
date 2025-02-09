using UnityEngine;

public class Item : MonoBehaviour
{
    public float bobHeight = 0.2f;
    public float bobPeriod = 2;
    public float collectTime = 0.3f;
    public Vector3 targetScale = Vector3.zero;

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

        timePassed = Random.value * 10;
        SetPosition();
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
            if (collectCountUp > collectTime)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            SetPosition();
            timePassed += Time.deltaTime;
        }
    }


    private void SetPosition()
    {
        var displacement = Mathf.Sin(timePassed / bobPeriod * 2 * Mathf.PI) * bobHeight;
        transform.position = start + Vector3.up * displacement;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !collected)
        {
            collected = true;
            player = collision.transform;
            start = transform.position;
            OnItemCollect(collision.gameObject.GetComponent<Player>());
        }
    }

    public virtual void OnItemCollect(Player player) { }
}
