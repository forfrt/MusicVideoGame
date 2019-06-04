using UnityEngine;
using System.Collections;

public class Controller2D : MonoBehaviour {
	//引用CharacterController
	CharacterController characterController;
    AudioSource audioSource;
    //重力
    // public float gravity = 10;
    //水平移动的速度
    public float walkSpeed = 5;
	//弹跳高度
	// public float jumpHeight = 5;

	//显示角色当前正受到攻击
	float takenDamage = 0.2f;

	// 控制角色的移动方向
	Vector3 moveDirection = Vector3.zero;
	float horizontal = 0;
    float vertical = 0;
    //原PlayerAttack脚本里面的变量，把那个脚本和当前脚本合并，PlayerAttack脚本已经删除。
    public Rigidbody bulletPrefab;

	float attackRate = 0.5f;
	float coolDown;

    bool lookRight = true;

    // Use this for initialization
    

    float X_step = 20.0f;
    float Y_step = 11.25f;

    int MovedIndex = 0;

    //float[] timeline = new float[20] { 10, 20, 30, 45, 55, 60, 78, 90, 100, 110, 121, 132, 144, 156, 168, 180, 192, 211, 220};

    float[] timeline = new float[10] { 0.5f, 1.0f, 1.5f, 2.0f, 2.5f, 3.0f, 3.5f, 4.0f, 4.5f, 5.0f};



    void Start () {
		characterController = GetComponent<CharacterController>();
        audioSource= GetComponent<AudioSource>();
        audioSource.Play();
    }
	
	// Update is called once per frame
	void Update () {
        int index = -1;
        for (int i = 0; i<240; i++)
        {
            if (Time.time - (i+1)*0.5f < 0.25f & Time.time- (i + 1) * 0.5 > 0.0f)
            {
                if (MovedIndex == i)
                {
                    index = i;
                    MovedIndex++;
                    break;
                }
            }
        }

        if (index > 0) {

            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            if (horizontal == 0)
            {
                moveDirection.x = horizontal;
            }

            if (vertical == 0)
            {
                moveDirection.y = vertical;
            }

            //控制角色右移（按d键和右键时）  在这里不直接使用0而是用0.01f是因为使用0之后会持续移动，无法静止
            if (horizontal > 0.01f)
            {
                lookRight = true;

                if (!Move(X_step, 0)) { 
                    characterController.Move(new Vector3(X_step, 0, 0));
                }
            }
            //控制角色左移（按a键和左键时）
            if (horizontal < -0.01f)
            {
                lookRight = false;
                if (!Move(-X_step, 0))
                {
                    characterController.Move(new Vector3(-X_step, 0, 0));
                }
            }
            //控制角色右移（按d键和右键时）  在这里不直接使用0而是用0.01f是因为使用0之后会持续移动，无法静止
            if (vertical > 0.01f)
            {
                lookRight = true;
                if (!Move(0, Y_step))
                {
                    characterController.Move(new Vector3(0, Y_step, 0));
                }
            }
            //控制角色左移（按a键和左键时）
            if (vertical < -0.01f)
            {
                lookRight = false;
                if (!Move(0, -Y_step))
                {
                    characterController.Move(new Vector3(0, -Y_step, 0));
                }
            }

            //原PlayerAttack的函数
            if (Time.time >= coolDown)
            {
                if (Input.GetMouseButton(0)) { 
                    Ray ray= Camera.main.ScreenPointToRay(Input.mousePosition);
                    
                    /*
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit)) {
                        if (hit.collider.gameObject.name.Contains("Enemy"))
                        {
                            BulletAttack(hit.collider.gameObject.transform.position - characterController.transform.position);
                            print("hit enemy");
                        }
                        else {
                            BulletAttack(ray.GetPoint(1) - characterController.transform.position);
                            print("hit");
                        }
                        
                        
                    }
                    */    
                    
                    BulletAttack(ray.GetPoint(1) - characterController.transform.position);
                    print("hit");
                    

                }
            }
        }
	}

    //原PlayerAttack的函数，现在和当前脚本合并了。
    //按下攻击按键时创建子弹的prefab，也就是bulletPrefab。
    void BulletAttack(Vector3 vector){

        vector.z = 0;


        if (lookRight) {
			//下面的这句话非常经典，利用as Rigidbody将Instantiate的GameObject强制转换为Rigidbody类型。
			Rigidbody bPrefab = Instantiate (bulletPrefab, transform.position, Quaternion.identity)as Rigidbody;
			bPrefab.GetComponent<Rigidbody>().AddForce (vector * 50);
			coolDown = Time.time + attackRate;
		}
		else {
			//下面的这句话非常经典，利用as Rigidbody将Instantiate的GameObject强制转换为Rigidbody类型。
			Rigidbody bPrefab = Instantiate (bulletPrefab, transform.position, Quaternion.identity)as Rigidbody;
			bPrefab.GetComponent<Rigidbody>().AddForce (vector * 50);
			coolDown = Time.time + attackRate;
		}
	}
	
	public IEnumerator TakenDamage(){
		GetComponent<Renderer>().enabled = false;
		yield return new WaitForSeconds(takenDamage);
		GetComponent<Renderer>().enabled = true;
		yield return new WaitForSeconds(takenDamage);
		GetComponent<Renderer>().enabled = false;
		yield return new WaitForSeconds(takenDamage);
		GetComponent<Renderer>().enabled = true;
		yield return new WaitForSeconds(takenDamage);
		GetComponent<Renderer>().enabled = false;
		yield return new WaitForSeconds(takenDamage);
		GetComponent<Renderer>().enabled = true;
		yield return new WaitForSeconds(takenDamage);
    }
    
    void OnTriggerEnter(Collider other)
    {

    }

    protected bool Move(float xDir, float yDir)
    {
        //Store start position to move from, based on objects current transform position.
        Vector3 start = characterController.transform.position;

        print("characterController.transform.position");
        print(start);

        // Calculate end position based on the direction parameters passed in when calling Move.
        var end = start + new Vector3(xDir, yDir, 0);
        print(end);

        //Disable the boxCollider so that linecast doesn't hit this object's own collider.
        characterController.enabled = false;

        //Cast a line from start point to end point checking collision on blockingLayer.
        Debug.DrawLine(start, end, Color.red);
        bool result = Physics.Linecast(start, end);
        characterController.enabled = true;
        print(result);
        return result;

        //Re-enable boxCollider after linecast
        

        // return the object that hit, maybe null.
        //return hit.transform;
        
    }
}
