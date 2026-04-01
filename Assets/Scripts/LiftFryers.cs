using UnityEngine;

public class LiftFryers : MonoBehaviour
{
    public float downY = -0.05f;
    public float speed = 2f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool isDown = false;

    void Start()
    {
        startPos = transform.localPosition;
        targetPos = startPos + new Vector3(0, downY, 0);
    }

    void OnMouseDown()
    {
        isDown = !isDown;
    }

    void Update()
    {
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            isDown ? targetPos : startPos,
            Time.deltaTime * speed
        );
    }
}