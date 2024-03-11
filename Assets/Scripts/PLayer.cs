using UnityEngine;
using UnityEngine.UI;

public class ClickToMove : MonoBehaviour
{
    public float maxSpeed = 5f;
    public float rotationSpeed = 10f;
    public float slowingDistance = 2f; // Distance at which player starts slowing down

    public Text speedText; // UI text to display player speed

    private Vector3 targetPosition;
    private Vector3 previousPosition;
    private float previousTime;

    private void Start()
    {
        previousPosition = transform.position;
        previousTime = Time.time;
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = transform.position.z; // Ensure the z position is the same as the player
        }

        // Check if the player has reached the target position
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        if (distanceToTarget > 0.1f)
        {
            // Calculate direction to the target
            Vector2 direction = (targetPosition - transform.position).normalized;

            // Calculate current speed based on distance to target
            float currentSpeed = maxSpeed;

            // Adjust speed based on distance to target
            if (distanceToTarget < slowingDistance)
            {
                currentSpeed *= distanceToTarget / slowingDistance;
            }

            // Move the player towards the target with the current speed using kinematics
            transform.position += (Vector3)direction * currentSpeed * Time.fixedDeltaTime;

            // Rotate the player to face the target
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

            // Calculate player speed in m/s
            Vector3 currentPosition = transform.position;
            float distanceMoved = Vector3.Distance(currentPosition, previousPosition);
            float deltaTime = Time.time - previousTime;
            float speedInMetersPerSecond = 0f; // Default speed to zero

            if (distanceMoved > 0f && deltaTime > 0f)
            {
                speedInMetersPerSecond = distanceMoved / deltaTime;
            }

            // Display the speed in the UI text
            speedText.text = "Current Speed : " + speedInMetersPerSecond.ToString("F2") + " m/s";

            // Update previous position and time for next calculation
            previousPosition = currentPosition;
            previousTime = Time.time;
        }
        else
        {
            // Display zero speed when the player stops moving
            speedText.text = "Current Speed : 0.00 m/s";
        }
    }
}
