using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

//Allows us to use UI.

namespace Scripts
{
    //Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
    public class Player : MovingObject
    {
        public float restartLevelDelay = 1f; //Delay time in seconds to restart level.
        public int pointsPerFood = 10; //Number of points to add to player food points when picking up a food object.
        public int pointsPerSoda = 20; //Number of points to add to player food points when picking up a soda object.

        [FormerlySerializedAs("wallDamage")]
        public int damage = 1; //How much damage a player does to a wall when chopping it.

        public Text foodText; //UI Text to display current player food total.
        public AudioClip moveSound1; //1 of 2 Audio clips to play when player moves.
        public AudioClip moveSound2; //2 of 2 Audio clips to play when player moves.
        public AudioClip eatSound1; //1 of 2 Audio clips to play when player collects a food object.
        public AudioClip eatSound2; //2 of 2 Audio clips to play when player collects a food object.
        public AudioClip drinkSound1; //1 of 2 Audio clips to play when player collects a soda object.
        public AudioClip drinkSound2; //2 of 2 Audio clips to play when player collects a soda object.
        public AudioClip gameOverSound; //Audio clip to play when player dies.

        private Animator _animator; //Used to store a reference to the Player's animator component.
        private int _food; //Used to store player food points total during level.
        private static readonly int PlayerChop = Animator.StringToHash("playerChop");
        private static readonly int PlayerHit = Animator.StringToHash("playerHit");
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        private Vector2 touchOrigin = -Vector2.one;	//Used to store location of screen touch origin for mobile controls.
#endif


        //Start overrides the Start function of MovingObject
        protected override void Start()
        {
            //Get a component reference to the Player's animator component
            _animator = GetComponent<Animator>();

            //Get the current food point total stored in GameManager.instance between levels.
            _food = GameManager.Instance.playerFoodPoints;

            //Set the foodText to reflect the current player food total.
            foodText.text = "Food: " + _food;

            //Call the Start function of the MovingObject base class.
            base.Start();
        }


        //This function is called when the behaviour becomes disabled or inactive.
        private void OnDisable()
        {
            //When Player object is disabled, store the current local food total in the GameManager so it can be re-loaded in next level.
            GameManager.Instance.playerFoodPoints = _food;
        }


        private void Update()
        {
            //If it's not the player's turn, exit the function.
            if (!GameManager.Instance.playersTurn) return;

            HandleInput(out var horizontal, out var vertical);

            //Check if we have a non-zero value for horizontal or vertical
            if (horizontal != 0 || vertical != 0) AttemptMove(horizontal, vertical);
        }

        //AttemptMove overrides the AttemptMove function in the base class MovingObject
        //AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
        private new void AttemptMove(int xDir, int yDir)
        {
            //Every time player moves, subtract from food points total.
            _food--;

            //Update food text display to reflect current score.
            foodText.text = "Food: " + _food;

            //Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
            base.AttemptMove(xDir, yDir);

            //Hit allows us to reference the result of the Linecast done in Move.
            //If Move returns true, meaning Player was able to move into an empty space.
            if (Move(xDir, yDir)) SoundManager.Instance.RandomizeSfx(moveSound1, moveSound2);

            //Since the player has moved and lost food points, check if the game has ended.
            CheckIfGameOver();

            //Set the playersTurn boolean of GameManager to false now that players turn is over.
            GameManager.Instance.playersTurn = false;
        }

        protected override void OnSmoothMoveFinish()
        {
            BoardManager.Instance.ClearFogAround(transform.position);
        }

        private static void HandleInput(out int horizontal, out int vertical)
        {
            //Check if we are running either in the Unity editor or in a standalone build.
#if UNITY_STANDALONE || UNITY_WEBPLAYER
            //Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
            horizontal = (int) Input.GetAxisRaw("Horizontal");

            //Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
            vertical = (int) Input.GetAxisRaw("Vertical");

            //Check if moving horizontally, if so set vertical to zero.
            if (horizontal != 0) vertical = 0;
            //Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
			//Check if Input has registered more than zero touches
			if (Input.touchCount > 0)
			{
				//Store the first touch detected.
				Touch myTouch = Input.touches[0];
				
				//Check if the phase of that touch equals Began
				if (myTouch.phase == TouchPhase.Began)
				{
					//If so, set touchOrigin to the position of that touch
					touchOrigin = myTouch.position;
				}
				
				//If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
				else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
				{
					//Set touchEnd to equal the position of this touch
					Vector2 touchEnd = myTouch.position;
					
					//Calculate the difference between the beginning and end of the touch on the x axis.
					float x = touchEnd.x - touchOrigin.x;
					
					//Calculate the difference between the beginning and end of the touch on the y axis.
					float y = touchEnd.y - touchOrigin.y;
					
					//Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
					touchOrigin.x = -1;
					
					//Check if the difference along the x axis is greater than the difference along the y axis.
					if (Mathf.Abs(x) > Mathf.Abs(y))
						//If x is greater than zero, set horizontal to 1, otherwise set it to -1
						horizontal = x > 0 ? 1 : -1;
					else
						//If y is greater than zero, set horizontal to 1, otherwise set it to -1
						vertical = y > 0 ? 1 : -1;
				}
			}
#endif //End of mobile platform dependent compilation section started above with #elif
        }

        //OnCantMove overrides the abstract function OnCantMove in MovingObject.
        //It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
        protected override void OnCantMove(MonoBehaviour component)
        {
            switch (component)
            {
                case Wall wall:
                    print("Clear my path.");
                    //Set hitWall to equal the component passed in as a parameter.
                    //Call the DamageWall function of the Wall we are hitting.
                    wall.DamageWall(damage);
                    break;
                case Enemy enemy:
                    print("Kill!");
                    enemy.DamageEnemy(damage);
                    break;
                default:
                    print($"hit {component}, it's a kind of {component.GetType()}");
                    break;
            }

            //Set the attack trigger of the player's animation controller in order to play the player's attack animation.
            _animator.SetTrigger(PlayerChop);
        }


        //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
        private void OnTriggerEnter2D(Collider2D other)
        {
            switch (other.tag)
            {
                //Check if the tag of the trigger collided with is Exit.
                case "Exit":
                    if (_food > 0)
                    {
                        //Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
                        Invoke(nameof(Restart), restartLevelDelay);
                        //Disable the player object since level is over.
                        enabled = false;
                    }

                    break;
                //Check if the tag of the trigger collided with is Food.
                case "Food":
                    //Add pointsPerFood to the players current food total.
                    var randomFoodPoint = Random.Range(-pointsPerFood, pointsPerFood) + pointsPerFood / 2;
                    _food += randomFoodPoint;

                    //Update foodText to represent current total and notify player that they gained points
                    foodText.text = "Food: " + randomFoodPoint + " Food: " + _food;

                    //Call the RandomizeSfx function of SoundManager and pass in two eating sounds to choose between to play the eating sound effect.
                    SoundManager.Instance.RandomizeSfx(eatSound1, eatSound2);

                    //Disable the food object the player collided with.
                    other.gameObject.SetActive(false);
                    if (randomFoodPoint < 0)
                        CheckIfGameOver();
                    break;
                //Check if the tag of the trigger collided with is Soda.
                case "Soda":
                    //Add pointsPerSoda to players food points total
                    var randomSodaPoint = Random.Range(-pointsPerSoda, pointsPerSoda) + pointsPerSoda / 2;
                    _food += randomSodaPoint;

                    //Update foodText to represent current total and notify player that they gained points
                    foodText.text = "Soda: " + randomSodaPoint + " Food: " + _food;

                    //Call the RandomizeSfx function of SoundManager and pass in two drinking sounds to choose between to play the drinking sound effect.
                    SoundManager.Instance.RandomizeSfx(drinkSound1, drinkSound2);

                    //Disable the soda object the player collided with.
                    other.gameObject.SetActive(false);
                    if (randomSodaPoint < 0)
                        CheckIfGameOver();
                    break;
                default:
                    print(other);
                    break;
            }
        }


        //Restart reloads the scene when called.
        private void Restart()
        {
            //Load the last scene loaded, in this case Main, the only scene in the game. And we load it in "Single" mode so it replace the existing one
            //and not load all the scene object in the current scene.
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }


        //LoseFood is called when an enemy attacks the player.
        //It takes a parameter loss which specifies how many points to lose.
        public void LoseFood(int loss)
        {
            //Set the trigger for the player animator to transition to the playerHit animation.
            _animator.SetTrigger(PlayerHit);

            //Subtract lost food points from the players total.
            _food -= loss;

            //Update the food display with the new total.
            foodText.text = "-" + loss + " Food: " + _food;

            //Check to see if game has ended.
            CheckIfGameOver();
        }


        //CheckIfGameOver checks if the player is out of food points and if so, ends the game.
        private void CheckIfGameOver()
        {
            //Check if food point total is less than or equal to zero.
            if (_food > 0) return;
            //Call the PlaySingle function of SoundManager and pass it the gameOverSound as the audio clip to play.
            SoundManager.Instance.PlaySingle(gameOverSound);

            //Stop the background music.
            SoundManager.Instance.StopBackgroundMusic();

            //Call the GameOver function of GameManager.
            GameManager.Instance.GameOver();
        }
    }
}