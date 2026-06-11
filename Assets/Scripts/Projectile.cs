
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 1;
    public float speed = 2.5f;
    public int pierce = 3;
    public float time = 1f;
    public float rotationOffset = 0f;
    public bool isPlayerProjectile = false;
    float timer = 0f;
    public GameObject nextProjectile;
    public Rigidbody2D rb;
    public virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        transform.Rotate(0,0,rotationOffset);
    }
    public virtual void FixedUpdate()
    {
        float angle = Mathf.Deg2Rad * (transform.eulerAngles.z - rotationOffset);
        rb.MovePosition((Vector2)rb.position + speed * Time.fixedDeltaTime * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)));
    }
    public virtual void Update()
    {
        if (timer + Time.deltaTime >= time)
        {
            Die();
        }
        timer += Time.deltaTime;
    }
    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && isPlayerProjectile)
        {
            pierce--;
            IEnemy enemy = other.GetComponent<IEnemy>();
            enemy.dealDamage(damage);

        }
        else if (other.CompareTag("Player") && !isPlayerProjectile)
        {
            pierce--;
            PlayerController player = other.GetComponent<PlayerController>();
            player.dealDamage(damage);
        }
        else if (other.CompareTag("Walls"))
        {
            Die();
        }
        if (pierce == 0) Die();
    }
    public virtual void Die()
    {
        transform.Rotate(new Vector3(0,0,-rotationOffset));
        if (nextProjectile != null) Instantiate(nextProjectile, transform.position, transform.rotation);
        transform.Rotate(new Vector3(0,0,rotationOffset));
        Destroy(gameObject);
    }
}