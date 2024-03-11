using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public GameObject player;
    public float speed;
    public Text distanceText; // UI text to display distance

    private void Update()
    {
        float distance = Vector2.Distance(transform.position, player.transform.position);
        Vector2 direction = player.transform.position - transform.position;

        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

        // Display distance in the UI text
        if (distanceText != null)
        {
            distanceText.text = "Enemy Distance : " + distance.ToString("F2");
        }
    }
}
