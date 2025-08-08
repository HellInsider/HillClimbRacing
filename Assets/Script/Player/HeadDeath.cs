using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class HeadDeath : MonoBehaviour
{
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform headPoint;
    [SerializeField] private Transform pCar;
    [SerializeField] private Rigidbody2D playerRigidbody;
    [SerializeField] private UpgradeCar upg;
    [SerializeField] private GameOver gameOver;
    private bool isDead;
    private void RespawnWithShield()
    {
        upg.isShieldActive = false;
        Vector3 collisionPoint = headPoint.position;
        Vector2 head = headPoint.position;
        playerRigidbody.linearVelocity = Vector2.zero;
        playerRigidbody.angularVelocity = 0f;
        pCar.rotation = Quaternion.Euler(new Vector3()); ;
        pCar.position = collisionPoint + new Vector3(0, collisionPoint.y+4f, collisionPoint.z);
    }
    private void Awake()
    {
        if (playerRigidbody == null)
        {
            playerRigidbody = GetComponent<Rigidbody2D>();
            if (playerRigidbody == null)
            {
                Debug.LogError("Rigidbody2D �� ������ �� �������!");
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
            Debug.LogError("HeadPoint �� ��������!");
            return;
        }
        Collider2D[] collisions = Physics2D.OverlapCircleAll(
            headPoint.position,
            checkRadius,
            groundLayer
        );
        if (collisions.Length > 0 && playerRigidbody != null)
        {
            StartCoroutine(ReloadLevel());
        }
    }

    private System.Collections.IEnumerator ReloadLevel()
    {
        if (upg.isShieldActive)
        {
            RespawnWithShield();
        }
        else
        {
            isDead = true;
            Car _car = GetComponent<Car>();
            if (_car != null)
            {
                _car._check = false;
            }
            if (playerRigidbody != null)
            {
                playerRigidbody.linearVelocity = Vector2.zero;
            }
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            yield return new WaitForSeconds(0f);
            gameOver.GameOverPlayer();
        }
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