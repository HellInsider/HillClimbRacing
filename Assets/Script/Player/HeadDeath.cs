using UnityEngine;
using UnityEngine.SceneManagement;

public class HeadDeath : MonoBehaviour
{
   
    [SerializeField] private float minDeathSpeed = 8f;
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

   
    [SerializeField] private Transform headPoint;
    [SerializeField] private Rigidbody2D playerRigidbody;

    private bool isDead;

    private void Awake()
    {
        if (playerRigidbody == null)
        {
            playerRigidbody = GetComponent<Rigidbody2D>();
            if (playerRigidbody == null)
            {
                Debug.LogError("Rigidbody2D не найден на объекте!");
            }
        }
    }
    private void Update()
    {
        if (isDead) return;
        CheckForHeadCollision();
    }
    private void CheckForHeadCollision()
    {
        if (headPoint == null)
        {
            Debug.LogError("HeadPoint не назначен!");
            return;
        }
        Collider2D[] collisions = Physics2D.OverlapCircleAll(
            headPoint.position,
            checkRadius,
            groundLayer
        );
        if (collisions.Length > 0 && playerRigidbody != null &&
            playerRigidbody.velocity.magnitude > minDeathSpeed)
        {
            StartCoroutine(ReloadLevel());
        }
    }

    private System.Collections.IEnumerator ReloadLevel()
    {
        isDead = true;

       
        Car _car = GetComponent<Car>();
        if (_car != null)
        {
            _car._check = false;
        }

        if (playerRigidbody != null)
        {
            playerRigidbody.velocity = Vector2.zero;
        }

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDrawGizmosSelected()
    {
        if (headPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(headPoint.position, checkRadius);
        }
    }
}