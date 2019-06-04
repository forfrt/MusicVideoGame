using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Scripts
{
    public class GameManager : MonoBehaviour
    {
        private const float TurnDelay = 0.1f; //Delay between each Player turn.

        public static GameManager
            Instance; //Static instance of GameManager which allows it to be accessed by any other script.


        private bool
            _doingSetup = true; //Boolean to check if we're setting up board, prevent Player from moving during setup.

        private List<Enemy> _enemies; //List of all Enemy units, used to issue them move commands.
        private bool _enemiesMoving; //Boolean to check if enemies are moving.
        private int _level = 1; //Current level number, expressed in game as "Day 1".
        private GameObject _levelImage; //Image to block out level as levels are being set up, background for levelText.


        private Text _levelText; //Text to display current level number.
        public float levelStartDelay = 2f; //Time to wait before starting level, in seconds.
        public int playerFoodPoints = 100; //Starting value for Player food points.

        [HideInInspector]
        public bool playersTurn = true; //Boolean to check if it's players turn, hidden in inspector but public.

        //Awake is always called before any Start functions
        private void Awake()
        {
            //Check if instance already exists
            if (Instance == null)

                //if not, set instance to this
                Instance = this;

            //If instance already exists and it's not this:
            else if (Instance != this)

                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);

            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);

            //Assign enemies to a new List of Enemy objects.
            _enemies = new List<Enemy>();


            //Call the InitGame function to initialize the first level 
            InitGame();
        }

        //this is called only once, and the parameter tell it to be called only after the scene was loaded
        //(otherwise, our Scene Load callback would be called the very first load, and we don't want that)
        [RuntimeInitializeOnLoadMethod]
        public static void CallbackInitialization()
        {
            //register the callback to be called every time the scene is loaded
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        //This is called each time a scene is loaded.
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Instance._level++;
            Instance.InitGame();
        }


        //Initializes the game for each level.
        private void InitGame()
        {
            //While doingSetup is true the player can't move, prevent player from moving while title card is up.
            _doingSetup = true;

            //Get a reference to our image LevelImage by finding it by name.
            _levelImage = GameObject.Find("LevelImage");

            //Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
            _levelText = GameObject.Find("LevelText").GetComponent<Text>();

            //Set the text of levelText to the string "Day" and append the current level number.
            _levelText.text = "Day " + _level;

            //Set levelImage to active blocking player's view of the game board during setup.
            _levelImage.SetActive(true);

            //Call the HideLevelImage function with a delay in seconds of levelStartDelay.
            Invoke(nameof(HideLevelImage), levelStartDelay);

            //Clear any Enemy objects in our List to prepare for next level.
            _enemies.Clear();

            //Get a component reference to the attached BoardManager script
            var boardManager = BoardManager.Instance;
            //Call the SetupScene function of the BoardManager script, pass it current level number.
            boardManager.SetupScene(_level);
            boardManager.ClearFogAround(GameObject.Find("Player").transform.position);
        }

        //Hides black image used between levels
        private void HideLevelImage()
        {
            //Disable the levelImage gameObject.
            _levelImage.SetActive(false);

            //Set doingSetup to false allowing player to move again.
            _doingSetup = false;
        }

        //Update is called every frame.
        private void Update()
        {
            //Check that playersTurn or enemiesMoving or doingSetup are not currently true.
            if (playersTurn || _enemiesMoving || _doingSetup)

                //If any of these are true, return and do not start MoveEnemies.
                return;

            //Start moving enemies.
            StartCoroutine(MoveEnemies());
        }

        //Call this to add the passed in Enemy to the List of Enemy objects.
        public void AddEnemyToList(Enemy script)
        {
            //Add Enemy to List enemies.
            _enemies.Add(script);
        }


        //GameOver is called when the player reaches 0 food points
        public void GameOver()
        {
            //Set levelText to display number of levels passed and game over message
            _levelText.text = "After " + _level + " days, you starved.";

            //Enable black background image gameObject.
            _levelImage.SetActive(true);

            //Disable this GameManager.
            enabled = false;
        }

        //Coroutine to move enemies in sequence.
        private IEnumerator MoveEnemies()
        {
            //While enemiesMoving is true player is unable to move.
            _enemiesMoving = true;

            //Wait for turnDelay seconds, defaults to .1 (100 ms).
            yield return new WaitForSeconds(TurnDelay);

            //If there are no enemies spawned (IE in first level):
            if (_enemies.Count == 0) yield return new WaitForSeconds(TurnDelay);

            //Loop through List of Enemy objects.
            foreach (var enemy in _enemies)
            {
                if (enemy.gameObject.activeSelf) enemy.MoveEnemy();

                //Wait for Enemy's moveTime before moving next Enemy, 
                yield return new WaitForSeconds(enemy.moveTime);
            }

            //Once Enemies are done moving, set playersTurn to true so player can move.
            playersTurn = true;

            //Enemies are done moving, set enemiesMoving to false.
            _enemiesMoving = false;
        }
    }
}