using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PetProjectile : Projectile
{
    float timer2 = 0f;
    float turnTime = 0f;
    public float minTurnTime;
    public float maxTurnTime;
    public float slowAmount;
    public override void Start()
    {
        base.Start();
        turnTime = UnityEngine.Random.Range(minTurnTime, maxTurnTime);
    }
    public override void FixedUpdate()
    {
        float angle = Mathf.Deg2Rad * (transform.eulerAngles.z - rotationOffset);
        rb.MovePosition((Vector2)rb.position + speed * Time.fixedDeltaTime * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)));
    }
    public override void Update()
    {
        base.Update();
        timer2 += Time.deltaTime;
        if (timer2 >= turnTime)
        {
            timer2 -= turnTime;
            turnTime = UnityEngine.Random.Range(minTurnTime, maxTurnTime);
            transform.rotation = Quaternion.Euler(0,0,UnityEngine.Random.Range(0f, 360f));
        }
    }
    public override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && isPlayerProjectile)
        {
            pierce--;
            IEnemy enemy = other.GetComponent<IEnemy>();
            enemy.dealDamage(damage);
            enemy.setMoveSpeed(enemy.getMoveSpeed()*slowAmount);

        }
        else if (other.CompareTag("Player") && !isPlayerProjectile)
        {
            pierce--;
            PlayerController player = other.GetComponent<PlayerController>();
            player.dealDamage(damage);
            player.moveSpeed *= slowAmount;
        }
        else if (other.CompareTag("Walls"))
        {
            Die();
        }
        if (pierce == 0) Die();
    }
    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && isPlayerProjectile)
        {
            IEnemy enemy = other.GetComponent<IEnemy>();
            enemy.setMoveSpeed(enemy.getMoveSpeed()/slowAmount);

        }
        else if (other.CompareTag("Player") && !isPlayerProjectile)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            player.moveSpeed /= slowAmount;
        }
    }
}