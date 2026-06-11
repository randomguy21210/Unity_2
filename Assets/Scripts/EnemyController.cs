using System;
using UnityEngine;
using UnityEngine.Analytics;

public class EnemyController : MonoBehaviour, IEnemy
{
    public float moveSpeed = 3.8f;
    public LayerMask obstacleLayers = 8;
    private PlayerController player;
    public int health;
    public IWeapon activeWeapon;
    private bool isRanged = false;
    public GameObject weaponPrefab;
    public float getMoveSpeed(){return moveSpeed;}
    public void setMoveSpeed(float set){moveSpeed = set;}
    public int droppedCoins;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = Navigation.player;
        GameObject weaponGameObject = Instantiate(weaponPrefab,gameObject.GetComponent<Transform>());
        activeWeapon = weaponGameObject.GetComponent<IWeapon>();
        if (weaponGameObject.GetComponent<RangedWeapon>() != null) isRanged = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) player = Navigation.player;
        if (player != null && !Physics2D.Linecast(transform.position, player.transform.position, obstacleLayers))
        {
            if (!isRanged || (isRanged && Vector2.Distance(transform.position, player.transform.position) > Navigation.enemyRange)) transform.Translate(Vector2.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime)- new Vector2(transform.position.x,transform.position.y));
            if (activeWeapon.canSwing() && Vector2.Distance(transform.position, player.transform.position)<=activeWeapon.getAttackRange())activeWeapon.Attack();
        }
        else if (Navigation.directions[(int)Math.Floor(transform.position.x/Navigation.tilesize),(int)Math.Floor(transform.position.y/Navigation.tilesize)] != 0)
        {
            transform.Translate(Navigation.oppdir[Navigation.directions[(int)Math.Floor(transform.position.x/Navigation.tilesize),(int)Math.Floor(transform.position.y/Navigation.tilesize)]-1] * moveSpeed * Time.deltaTime);
        }
    }
    public void dealDamage(int damage)
    {
       health -= damage;
       if (health <= 0) {AudioManager.instance.Play("Coin");Destroy(gameObject);player.coins+=droppedCoins;}
    }
}
