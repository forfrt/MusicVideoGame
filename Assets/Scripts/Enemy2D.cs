using UnityEngine;
using System.Collections;

public class Enemy2D : MonoBehaviour
{
    //GameManager脚本的参量
    public GameManager gameManager;
    //敌人移动的初始和停止位置，用于控制敌人在一定范围内移动
    float startPos;
    float endPos;
    //控制敌人向右移动的一个增量
    public int unitsToMove = 5;
    //敌人移动的速度
    public int moveSpeed = 2;
    //左右移动的布尔值
    bool moveRight = true;

    //敌人的HP
    int enemyHealth = 1;
    //敌人的种类
    public bool basicEnemy;
    public bool advancedEnemy;

    void Awake()
    {
        startPos = transform.position.x;
        endPos = startPos + unitsToMove;

        if (basicEnemy)
        {
            enemyHealth = 3;
        }

        if (advancedEnemy)
        {
            enemyHealth = 6;
        }
    }
    //此处这个Update函数用于控制敌人的左右移动，当向右移动到一定距离后就会反向移动，同理，左移一定距离之后也是。
    void Update()
    {
        if (moveRight)
        {
            GetComponent<Rigidbody>().position += Vector3.right * moveSpeed * Time.deltaTime;
        }
        if (GetComponent<Rigidbody>().position.x >= endPos)
        {
            moveRight = false;
        }
        if (moveRight == false)
        {
            GetComponent<Rigidbody>().position -= Vector3.right * moveSpeed * Time.deltaTime;
        }
        if (GetComponent<Rigidbody>().position.x <= startPos)
        {
            moveRight = true;
        }
    }


    int damageValue = 1;
    //这里利用sendmessage函数使得角色与敌人自己碰撞时，发送一个扣血的message到gamemanager函数之中，然后就会在每次碰撞时减掉一滴血。
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            gameManager.SendMessage("PlayerDamaged", damageValue, SendMessageOptions.DontRequireReceiver);
            gameManager.controller2D.SendMessage("TakenDamage", SendMessageOptions.DontRequireReceiver);
        }
    }

    //下面这个函数用于判定敌人自己被攻击时的扣血。
    void EnemyDamaged(int damage)
    {
        if (enemyHealth > 0)
        {
            enemyHealth -= damage;
        }

        if (enemyHealth <= 0)
        {
            enemyHealth = 0;
            Destroy(gameObject);
            gameManager.curEXP += 10;
        }
    }
    /*
    void OnTriggerEnter(Collider other)
    {
        gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
    }
    */
}
