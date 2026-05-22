using UnityEngine;

public class LiftFryers : MonoBehaviour
{
    public float downY = -0.05f;
    public float speed = 0.5f; 

    Vector3 startPos;
    Vector3 targetPos;

    bool isDown = false;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true; 
        }

        startPos = transform.localPosition;
        targetPos = startPos + new Vector3(0, downY, 0);
    }

    void OnMouseDown()
    {
        ToggleLift();
    }

    public void ToggleLift()
    {
        isDown = !isDown;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void Update()
    {

        Vector3 target = isDown ? targetPos : startPos;

        Vector3 nextPos = Vector3.MoveTowards(
            transform.localPosition,
            target,
            speed * Time.deltaTime 
        );

        if (rb != null)
        {
            Vector3 worldPos = transform.parent != null ? transform.parent.TransformPoint(nextPos) : nextPos;
            rb.MovePosition(worldPos);
        }
        else
        {
            transform.localPosition = nextPos;
        }
    }
}