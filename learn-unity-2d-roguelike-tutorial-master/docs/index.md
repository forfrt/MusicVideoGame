# Learn 2D Roguelike tutorial 

PV|Github
--|------
![View](http://hits.dwyl.io/XieEDeHeiShou/learn-unity-2d-roguelike-tutorial.svg)|![View](http://hits.dwyl.io/XieEDeHeiShou/learn-unity-2d-roguelike-tutorial-github.svg)

这篇文章是我通过 [2D Roguelike tutorial](https://unity3d.com/learn/tutorials/s/2d-roguelike-tutorial) 
项目在学习 Unity 的使用时留下的记录. 

官方教程中提供的视频来自 YouTube, 国内有搬运工已经把整套视频搬到 
[Bilibili](https://www.bilibili.com/video/av5480978) 了.

学习成果:

<video preload="auto" style="width:100%" controls="controls" autoplay="autoplay">
    <source src="video/2019-03-19-15-49-54.mp4" type="video/mp4">
    <a href="video/2019-03-19-15-49-54.mp4">学习成果.mp4</a>
</video>

## Rules

通过试玩, 我发现了这个小游戏的一些基础的游戏规则:

0. 回合制游戏, 由玩家移动触发回合.
0. 玩家可以通过 `WASD` 和 `↑↓←→` 进行移动.
0. 地图中的可移动区域为8x8大小, 四周由外墙 (outerWall) 包围.
0. 可移动区域内会生成位置随机, 数量随机的障碍物 (wall), 水果 (food) 和饮料 (soda).
0. 可移动区域内会生成位置随机, 数量逐渐增多的敌人 (enemy).
0. 玩家 (player) 初始生命值 (foodPoint) 为 100, 碰到水果提升 10 点, 碰到饮料
提升 20 点, 被敌人攻击时, 根据敌人类型丢失 10 点或 20 点.
0. 障碍物有 3 点生命值, 被玩家攻击第一次以后会更换材质, 被摧毁后允许玩家和敌人通过.
0. 出口固定在右上角位置.
0. 镜头固定在(x=3.5,y=3.5,z=-10)的位置.
0. 地图全开, 没有迷雾.

## File tree

```
根目录
 ┣ Assets (资产目录, 存放源代码, 素材的地方, 应该被 VCS 管理)
 ┃ ┣ _Complete-Game (完整示例)
 ┃ ┃ ┣ Animation (动画示例)
 ┃ ┃ ┣ Prefabs (预设物示例)
 ┃ ┃ ┣ Scenes (场景示例)
 ┃ ┃ ┣ Scripts (脚本示例)
 ┃ ┃ ┣ _Complete-Game.unity (完整示例的主场景)
 ┃ ┃ ┗ (**.meta (各个文件/文件夹的元数据文件, 由 Unity3D 编译产生, 用来记录 Inspector 中的各种数据))
 ┃ ┣ Audio (音频素材)
 ┃ ┣ Fonts (字体素材)
 ┃ ┣ (Plugins (插件目录))
 ┃ ┣ Sprites (贴图目录)
 ┃ ┣ TutorialInfo (教程目录)
 ┃ ┗ Readme.asset (使用说明对应的资产)
 ┣ Library  (库目录, 存放 Unity3D 编译期产物的地方, 应该被 VCS 忽略)
 ┣ Logs (编译期日志文件夹, )
 ┣ obj (C# 脚本编译产物的地方)
 ┣ Packages (依赖目录? 应该被 VCS 管理)
 ┃ ┗ manifest.json (清单文件, 记录了这个项目的第三方依赖等)
 ┣ ProjectSettings (工程配置, 应该被 VCS 管理)
 ┃ ┣ *.asset (各种资源管理器对应的元数据)
 ┃ ┗ ProjectVersion.txt (目前只记录了 Unity3D 编辑器的版本号)
 ┣ Temp (用途不明的临时文件夹, 应该被 VCS 忽略)
 ┣ *.csproj (Visual studio 编译项目时生产的文件, 应该被 VCS 忽略)
 ┗ *.sln (Visual studio 的项目管理文件, 应该被 VCS 管理)
```

## Version control

第一次接触 Unity 工程, 在做版本控制的时候搜了一些文档, 主要提到了要将 `Edit->Project
Settings->Editor` 中的 `Version control mode` 改为 `Visible Meta Files` 以及将
`Asset serialization mode` 改为 `Force Text`. 

仔细调试了一下后, 发现 `Visible Meta Files` 能够使 Unity 在导入工程的时候生成各种
`.meta` 后缀的文件. 而 `Force Text` 则控制着 `*.meta` 文件里的内容格式,
将各种元数据序列化为可读性较好的 `Unity YAML` 格式.

后来测试发现这些 `*.meta` 文件记录着 `Inspector` 面板中的数据, 是 Unity
的各种组件之间的依赖关系的持久化文件.

## Learn by reading

_Complete-Game 目录下包含整个教程完整的示例, 是我学习这个项目的主战场.

### Readme.asset

该文件中记录了 [TutorialInfo/Scripts/Readme.cs](https://github.com/XieEDeHeiShou/learn-unity-2d-roguelike-tutorial/blob/master/Assets/TutorialInfo/Scripts/Readme.cs)
类对应的元数据. 进一步探索发现这个资产的 `Inspector` 界面以及顶部工具栏里的 `Tutorial` 菜单都是通过
[TutorialInfo/Scripts/Editor/ReadmeEditor.cs](https://github.com/XieEDeHeiShou/learn-unity-2d-roguelike-tutorial/blob/master/Assets/TutorialInfo/Scripts/Editor/ReadmeEditor.cs)
控制的.

到处改一改, 发现 Unity 插件开发相当的敏捷, 不像 IDEA 的插件, 
每次测试插件效果都要重启一个 IDEA 实例. 这个应该也跟编程语言有关系吧.

### Main.unity

主场景. 仔细观察了一下发现在 Camera 上挂载着一个
[Loader](https://github.com/XieEDeHeiShou/learn-unity-2d-roguelike-tutorial/blob/master/Assets/_Complete-Game/Scripts/Loader.cs)
脚本. 

脚本内容很简单, 通过 `Inspector` 中设置的属性实例化 
[GameManager](https://github.com/XieEDeHeiShou/learn-unity-2d-roguelike-tutorial/blob/master/Assets/_Complete-Game/Scripts/GameManager.cs)
和 
[SoundManager](https://github.com/XieEDeHeiShou/learn-unity-2d-roguelike-tutorial/blob/master/Assets/_Complete-Game/Scripts/SoundManager.cs)
对象.

### SoundManager.cs

音效管理器. 由于对象在 Unity 环境中托管, 因此在生命周期函数 `Awake` 
中通过判断静态引用的方式实现了单利设计. 利用 `AudioSource` 类播放本地音频文件, 
为了避免听觉疲劳, 使用了随机数调整音效的高低音.

```c#
private void Awake() {
    //Check if there is already an instance of SoundManager
    if (Instance == null)
        //if not, set it to this.
        Instance = this;
    //If instance already exists:
    else if (Instance != this)
        //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
        Destroy(gameObject);

    //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
    DontDestroyOnLoad(gameObject);
    ...
}
```

### GameManger.cs

游戏管理器. 在 `Awake` 中定义了 [Enemy](https://github.com/XieEDeHeiShou/learn-unity-2d-roguelike-tutorial/blob/master/Assets/_Complete-Game/Scripts/Enemy.cs) 
集合, 并通过 `GetComponent<>()` 获取到了被托管的 
[BoardManager](https://github.com/XieEDeHeiShou/learn-unity-2d-roguelike-tutorial/blob/master/Assets/_Complete-Game/Scripts/BoardManager.cs)
对象完成成员变量的初始化. 

```c#
//Get a component reference to the attached BoardManager script
_boardScript = GetComponent<BoardManager>();
```

加下来调用 `InitGame` 方法控制 UI 层的遮罩以及关卡提示的显示与隐藏, 并调用了 
`BoardManager` 的 `SetupScene(int)` 初始化了每一级关卡的动态场景. 

在生命周期 `Update` 中, 如果不需要等待玩家移动, 其他敌人移动或者过场的话, 
则尝试开启协程移动每一个 `Enemy` 对象. 全部 `Enemy` 移动完成后, 允许 
[Player](https://github.com/XieEDeHeiShou/learn-unity-2d-roguelike-tutorial/blob/master/Assets/_Complete-Game/Scripts/Player.cs)
移动.

### BoardManager.cs

关卡管理器. 没有采用单例设计, 也没有实现生命周期方法. 

对外仅暴露部分成员属性用于依赖注入, 以及 `SetupScene(int)` 方法用于实例化每一级关卡的外墙,
地板, 内墙 ([Wall](https://github.com/XieEDeHeiShou/learn-unity-2d-roguelike-tutorial/blob/master/Assets/_Complete-Game/Scripts/Player.cs)),
食物, 苏打, 敌人以及出口.

具体的实例化方式则是调用 `Instantiate(...)` 方法, 将指定的预设物实例化在场景中.

```c#
//Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3
//corresponding to current grid position in loop, cast it to GameObject. Set the parent of our newly
//instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity, _boardHolder);
```

### Wall.cs

内墙. 对外暴露 `DamageWall(int)` 方法, 从实现的功能上来看应该是属于回调类型的方法
(更贴切一点的命名应该是 `OnDamage(int)` 吧 23333).

内墙被攻击时通过调用 `SpriteRenderer` 实现了运行时将换贴图替换为受损状态的贴图, 
hp 低于 0 时将 gameObject 设为失活从而将其从场景中隐藏.

```c#
//Set spriteRenderer to the damaged wall sprite.
_spriteRenderer.sprite = dmgSprite;
...
if (_hp > 0) return;
//If hit points are less than or equal to zero, disable the gameObject.
gameObject.SetActive(false);
```

### Enemy.cs

敌人. 继承自 MovingObject 类.

在 `Start` 生命周期中将自身注册到 `GameManager`(我觉得这个注册的时机应该由 `BoardManager`
管理), 并引用玩家的 `Transform` 对象, 以便在 `Update` 时向玩家移动.

寻路的算法比较简陋, 简单的判断了一下自身与玩家的水平方向的差距和竖直方向的差距,
然后尝试移动, 没有考虑到撞墙, 绕路等情况.

```c#
//MoveEnemy is called by the GameManger each turn to tell each Enemy to 
//try to move towards the player.
public void MoveEnemy() {
    //Declare variables for X and Y axis move directions, these range 
    //from -1 to 1. These values allow us to choose between the cardinal
    //directions: up, down, left and right.
    var xDir = 0;
    var yDir = 0;

    //If the difference in positions is approximately zero (Epsilon) do the following:
    if (Mathf.Abs(_target.position.x - transform.position.x) < float.Epsilon)
        //If the y coordinate of the target's (player) position is greater 
        //than the y coordinate of this enemy's position set y direction 1
        //(to move up). If not, set it to -1 (to move down).
        yDir = _target.position.y > transform.position.y ? 1 : -1;

    //If the difference in positions is not approximately zero (Epsilon) do the following:
    else
        //Check if target x position is greater than enemy's x position,
        //if so set x direction to 1 (move right), if not set to -1 (move left).
        xDir = _target.position.x > transform.position.x ? 1 : -1;

    //Call the AttemptMove function and pass in the generic parameter Player,
    //because Enemy is moving and expecting to potentially encounter a Player
    AttemptMove(xDir, yDir);
}
```

当遇到玩家导致不能移动时, 则攻击玩家, 触发攻击动画同时播放攻击音效.

```c#
//OnCantMove is called if Enemy attempts to move into a space occupied 
//by a Player, it overrides the OnCantMove function of MovingObject 
//and takes a generic parameter T which we use to pass in the component 
//we expect to encounter, in this case Player
protected override void OnCantMove <T> (T component)
{
	//Declare hitPlayer and set it to equal the encountered component.
	Player hitPlayer = component as Player;
	//Call the LoseFood function of hitPlayer passing it playerDamage, 
	//the amount of foodpoints to be subtracted.
	hitPlayer.LoseFood (playerDamage);
	//Set the attack trigger of animator to trigger Enemy attack animation.
	animator.SetTrigger ("enemyAttack");
	//Call the RandomizeSfx function of SoundManager passing in 
	//the two audio clips to choose randomly between.
	SoundManager.instance.RandomizeSfx (attackSound1, attackSound2);
}
```

### MovingObject.cs

可移动的对象. 作为敌人和玩家的父类, 为子类提供了移动方向的碰撞检测和平滑移动能力.

在尝试移动时, 调用 `Move(...)` 方法进行碰撞检测, 遇到障碍时调用回调方法 `OnCannotMove<>()`, 
否则开启协程进行平滑移动.

```c#
//Move returns true if it is able to move and false if not. 
//Move takes parameters for x direction, y direction and a RaycastHit2D to check collision.
protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
{
	...
	//Cast a line from start point to end point checking collision on blockingLayer.
	hit = Physics2D.Linecast (start, end, blockingLayer);
	...
	if(hit.transform == null)
	{
		//If nothing was hit, start SmoothMovement co-routine passing in 
		//the Vector2 end as destination
		StartCoroutine(SmoothMovement(end));
		//Return true to say that Move was successful
		return true;
	}
	//If something was hit, return false, Move was unsuccesful.
	return false;
}
```

`Move(...)` 方法同时输出了 2 个结果, 一个 `bool` 类型的返回值, 标志着能否朝指定方向移动,
以及一个 `RaycastHit2D` 类型的碰撞结果. 我个人感觉布尔类型的返回值有点多余, 
通过判断碰撞结果是否为 null 即可.

```c#
//The virtual keyword means AttemptMove can be overridden by inheriting classes 
//using the override keyword. AttemptMove takes a generic parameter T 
//to specify the type of component we expect our unit to interact with if blocked 
//(Player for Enemies, Wall for Player).
protected virtual void AttemptMove <T> (int xDir, int yDir)
	where T : Component
{
	//Hit will store whatever our linecast hits when Move is called.
	RaycastHit2D hit;
	//Set canMove to true if Move was successful, false if failed.
	bool canMove = Move (xDir, yDir, out hit);
	//Check if nothing was hit by linecast
	if(hit.transform == null)
		//If nothing was hit, return and don't execute further code.
		return;
	
	//Get a component reference to the component of type T attached to the object that was hit
	T hitComponent = hit.transform.GetComponent <T> ();
	//If canMove is false and hitComponent is not equal to null, meaning 
	//MovingObject is blocked and has hit something it can interact with.
	if(!canMove && hitComponent != null)
		//Call the OnCantMove function and pass it hitComponent as a parameter.
		OnCantMove (hitComponent);
}
```

`AttemptMove(int,int)` 判断到能移动时没有执行动作, 这里其实可以把 `Move()` 
方法中开协程进行平滑移动的代码移动到这里来的.

### Player.cs

玩家.

玩家类使用宏对运行平台进行了隔离, 移动端判断滑动方向, 桌面端则是判断 `WASD` 和 `↑↓←→`
进行移动. 不能移动时可以攻击内墙. 
```c#
        private static void HandleInput(out int horizontal, out int vertical) {
            //Check if we are running either in the Unity editor or in a standalone build.
#if UNITY_STANDALONE || UNITY_WEBPLAYER
            //Get input from the input manager, round it to an integer 
            //and store in horizontal to set x axis move direction
            horizontal = (int) Input.GetAxisRaw("Horizontal");
            //Get input from the input manager, round it to an integer 
            //and store in vertical to set y axis move direction
            vertical = (int) Input.GetAxisRaw("Vertical");
            //Check if moving horizontally, if so set vertical to zero.
            if (horizontal != 0) vertical = 0;
            //Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
			...
#endif //End of mobile platform dependent compilation section started above with #elif
        }

```


玩家实现了 `OnTriggerEnter2D(Collider2D)` 方法, 从而监听到了 2d 碰撞箱的碰撞事件,
对遇到食物和苏打分别进行了不同程度的生命恢复, 遇到出口时则触发场景的重载, 进入下一关卡.

```c#
//OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
private void OnTriggerEnter2D (Collider2D other)
{
	//Check if the tag of the trigger collided with is Exit.
	if(other.tag == "Exit")
	{
		//Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
		Invoke ("Restart", restartLevelDelay);
		//Disable the player object since level is over.
		enabled = false;
	}
	//Check if the tag of the trigger collided with is Food.
	else if(other.tag == "Food")
	{
		//Add pointsPerFood to the players current food total.
		food += pointsPerFood;
		//Update foodText to represent current total and notify player that they gained points
		foodText.text = "+" + pointsPerFood + " Food: " + food;
		//Call the RandomizeSfx function of SoundManager and pass in 
		//two eating sounds to choose between to play the eating sound effect.
		SoundManager.instance.RandomizeSfx (eatSound1, eatSound2);
		//Disable the food object the player collided with.
		other.gameObject.SetActive (false);
	}
	//Check if the tag of the trigger collided with is Soda.
	else if(other.tag == "Soda")
	{
		...
	}
}
```

当玩家被敌人攻击时, 回调 `OnLossFood(int)` 方法, 触发被攻击动画, 移除生命值, 
并判断游戏是否结束.

## Learn by modifying

至此, Unity 官方教程 2d-roguelike-tutorial 分析完毕, 接下来就是到处改东西, 
看看会不会崩了 :).

### Resize movable area from 8x8 to random size

想要调整地图尺寸很简单, 在 `BoardManager` 中修改 `_columns` 和 `_rows` 成员变量就行了.

```c#
private void BoardSetup(){
    _columns = Random.Range(6, 10);
    _rows = Random.Range(6, 10);
    ...
}
```

### Bad apple 

地牢里的水果和饮料总是那么的新鲜, 看起来很诡异, 我决定加入一些烂苹果和腐败的饮料混在其中.

```c#
private void OnTriggerEnter2D(Collider2D other) {
    switch (other.tag) {
        case "Food":
            // simulate bad apple
            var randomFoodPoint = Random.Range(-pointsPerFood, pointsPerFood);
            _food += randomFoodPoint;
            ...
    }
}                
```

### Superior enemy

精英怪都是会拆墙的.

为了让玩家和敌人能够攻击不同类型的对象, 需要调用 `AttemptMove<>(int,int)` 方法, 
并传入不同的泛型:

```c#
...
base.AttemptMove<Wall>(xDir, yDir);
base.AttemptMove<Enemy>(xDir, yDir);
...
```

但这样写看起来很不简洁, 而且对象会移动多次. 因此需要把泛型从 `AttemptMove<>(int,int)` 
和 `OnCannotMove<T>(T) where T : MonoBehavior` 中移除.

```c#
protected override void OnCantMove(MonoBehavior component) {
    switch (component) {
        case Wall wall:
            wall.DamageWall(Damage);
            break;
    }
    ...
}
```

### Main role

镜头默认位于 (x=3.5,y=3.5,z=-10) 位置, 在地图尺寸随机的情况下, 有时候会看不全.
要使镜头跟随玩家的话, 需要在 `Hierarchy` 面板中将 `MainCamera` 移动到 `Player`
对象内部, 并将 `MainCamera` 的 position 调整为 (x=0,y=0,z=-10), 确保玩家位于镜头中央.

### Fog of war

战争迷雾的缺失, 使得游戏的探索性降低为 0, 玩家只需要在开始行动前规划好路径即可. 

要将战争迷雾这一特性加入游戏, 需要改造 `BoardManager` 和 `GameManager`:

0. 添加 `Fog` 预设物以及贴图, 并调整 `Sorting layer` 为 `Units`, 确保战争迷雾与玩家同层.
0. 添加 `List<GameObject>` 到 `BoardManager`, 用于存放战争迷雾实例.
0. 为 `BoardManager` 添加方法 `public void ClearFogAround(Vector3)`, 
通过控制 `GameObject` 的 `active` 属性实现迷雾的显示隐藏, 并延伸到移除指定位置近处的迷雾效果, 
恢复远处的迷雾效果.
0. 使 `BoardManager` 单例化, 以便能够跨组件调用.
0. `MovingObject` 添加移动结束回调, 以便玩家移动成功时能触发战争迷雾的动态效果.

[回到顶部](#learn-2d-roguelike-tutorial)
