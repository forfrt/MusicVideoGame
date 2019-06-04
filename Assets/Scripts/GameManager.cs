using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	//Controller2D脚本的参量
	public Controller2D controller2D;

    public Rigidbody blockPrefab;
    //角色生命值
    public Texture playersHealthTexture;
	//控制上面那个Teture的屏幕所在位置
	public float screenPositionX;
	public float screenPositionY;
	//控制桌面图标的大小
	public int iconSizeX = 25;
	public int iconSizeY = 25;
	//初始生命值
	public int playersHealth = 3;
	GameObject player;

	public int curEXP = 0;
	int maxEXP = 50;
	int level = 1;
	//这个布尔值用于判定是否显示角色的状态
	bool playerStats;
	//下面这个用于显示角色的状态
	public GUIText statsDisplay;

	//这个地方定义了私有变量player作为一个GameObject，然后用下面的FindGameObjectWithTag获取它，这样的话，在下面的伤害判断时，就可以用player.renderer了。
	void Start(){
        player = GameObject.FindGameObjectWithTag("Player");

        //System.Random random;
        int[] blockX = new int[5];
        int[] blockY = new int[5];

        int x;
        int y;

        for (int i = 0; i < 5; i++){
            
            x = Random.Range(0, 19);
            y = Random.Range(0, 19);

            
            while (NotInPreset(x, y)) {
                x = Random.Range(0, 19);
                y = Random.Range(0, 19);
            }
            
            
            blockX[i] = x;
            blockY[i] = y;
            
        }



        
        for (int i = 0; i < 5; i++)
        {
            Rigidbody bPrefab = Instantiate(blockPrefab, transform.position, Quaternion.identity) as Rigidbody;
            print("Generated X & Y");
            print(blockX[i]);
            print(blockY[i]);

            bPrefab.transform.position = new Vector3(-8.0f * blockX[i] + 76, -4.5f * blockY[i] + 42.75f, -4f);
            /*
            bPrefab.transform.position.x =-8.0f*blockX[i]+76;
            bPrefab.transform.position.y = -4.5f*blockY[i]+42.75f;
            */
        }
        


    }

    
    bool NotInPreset(int x, int y) {
        if (x == 0 & y == 0)
        {
            return true;
        }
        if (x == 19 & y == 19)
        {
            return true;
        }

        return false;
    }
    


    void Update(){
		if (curEXP >= maxEXP) {
			LevelUp();		
		}

		if (Input.GetKeyDown (KeyCode.C)) {
			playerStats = !playerStats;		
		}

		if (Input.GetKeyDown (KeyCode.E)) {
			curEXP += 10;		
		}

		if (playerStats) {
			statsDisplay.text = "等级:" + level + "\n经验:" + curEXP + "/" + maxEXP;
		} 
		else {
			statsDisplay.text = "";	
		}
	}
	//这个函数用于角色升级
	void LevelUp(){
		curEXP = 0;
		maxEXP = maxEXP + 50;
		level++;

		//升级的功效
		playersHealth++;
	}
	

	//OnGUI函数最好不要出现多次，容易造成混乱，所以我把要展示的东西都整合在这个里面
	void OnGUI(){

		//控制角色生命值的心的显示
		for (int h =0; h < playersHealth; h++) {
			GUI.DrawTexture(new Rect(screenPositionX + (h*iconSizeX),screenPositionY,iconSizeX,iconSizeY),playersHealthTexture,ScaleMode.ScaleToFit,true,0);
		}
	}

	void PlayerDamaged(int damage){   //此处使用player.renderer.enabled来进行判断，如果角色没有在闪烁，也就是存在的状态为真，那么才会受到伤害，这样可以避免角色连续受伤，还有另外一种方法是采用计时，这里没有采用那种方法。
		if (player.GetComponent<Renderer>().enabled) {
						if (playersHealth > 0) {
								playersHealth -= damage;	
						}

						if (playersHealth <= 0) {
								RestartScene ();	
						}
				}
	}

	void RestartScene(){
		Application.LoadLevel (Application.loadedLevel);
	}
}
