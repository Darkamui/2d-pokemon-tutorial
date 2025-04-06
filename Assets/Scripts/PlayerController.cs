using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Public variables can be used inside Unity UI
    public float moveSpeed;
    public LayerMask solidObjectsLayer;

    private bool isMoving;
    private Vector2 input;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        if (!isMoving)
        {
            /* GetAxisRaw returns 1 or -1 */
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            // Prevent diagonal movement
            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                // Pass input to animator for walking animation
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);

                // Set target position based on current position w/ user input
                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                // If no collisions
                if (IsWalkable(targetPos))
                    StartCoroutine(Move(targetPos));
            }
        }
        // Send data to animator
        animator.SetBool("isMoving", isMoving);
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        /* Smooth movement */
        // While the difference between the targetPosition and the currentPosition is higher than a very small amount continue moving towards it
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        // Stop at targeted position
        transform.position = targetPos;
        isMoving = false;
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        // Creates a small 0.2radius circle around the player and check if the circle overlaps with selected layer a.k.a collision
        if (Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer) != null)
        {
            return false;
        }

        return true;
    }
}
