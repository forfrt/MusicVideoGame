using UnityEngine;

namespace Scripts {
    //Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
    public class Enemy : MovingObject {
        private const int Damage = 2; //The amount of damage to subtract from the player or wall when attacking.
        private static readonly int EnemyAttack = Animator.StringToHash("enemyAttack");
        private Animator _animator; //Variable of type Animator to store a reference to the enemy's Animator component.

        private int _hp = 1;
        private bool _skipMove; //Boolean to determine whether or not enemy should skip a turn or move this turn.
        private Transform _target; //Transform to attempt to move toward each turn.
        public AudioClip attackPlayer1; //First of two audio clips to play when attacking the player.
        public AudioClip attackPlayer2; //Second of two audio clips to play when attacking the player.
        public AudioClip attackWall1; //First of two audio clips to play when attacking the player.
        public AudioClip attackWall2; //Second of two audio clips to play when attacking the player.

        //DamageWall is called when the player attacks a wall.
        public void DamageEnemy(int loss) {
            //Subtract loss from hit point total.
            _hp -= loss;
            print($"{name} at {transform.position} hp = {_hp}");

            //If hit points are less than or equal to zero:
            if (_hp > 0) return;
            print($"{name} at {transform.position} de-active");
            //Disable the gameObject.
            gameObject.SetActive(false); // clear object image from visual port
            SoundManager.Instance.RandomizeSfx(attackWall1, attackWall2);
        }

        //Start overrides the virtual Start function of the base class.
        protected override void Start() {
            //Register this enemy with our instance of GameManager by adding it to a list of Enemy objects. 
            //This allows the GameManager to issue movement commands.
            GameManager.Instance.AddEnemyToList(this);

            //Get and store a reference to the attached Animator component.
            _animator = GetComponent<Animator>();

            //Find the Player GameObject using it's tag and store a reference to its transform component.
            _target = GameObject.FindGameObjectWithTag("Player").transform;

            //Call the start function of our base class MovingObject.
            base.Start();
        }


        //Override the AttemptMove function of MovingObject to include functionality needed for Enemy to skip turns.
        //See comments in MovingObject for more on how base AttemptMove function works.
        private new void AttemptMove(int xDir, int yDir) {
            //Check if skipMove is true, if so set it to false and skip this turn.
            if (_skipMove) {
                _skipMove = false;
                return;
            }

            //Call the AttemptMove function from MovingObject.
            base.AttemptMove(xDir, yDir);

            //Now that Enemy has moved, set skipMove to true to skip next move.
            _skipMove = true;
        }


        //MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.
        public void MoveEnemy() {
            //Declare variables for X and Y axis move directions, these range from -1 to 1.
            //These values allow us to choose between the cardinal directions: up, down, left and right.
            var xDir = 0;
            var yDir = 0;

            //If the difference in positions is approximately zero (Epsilon) do the following:
            if (Mathf.Abs(_target.position.x - transform.position.x) < float.Epsilon)

                //If the y coordinate of the target's (player) position is greater than the y coordinate of this enemy's position set y direction 1 (to move up). If not, set it to -1 (to move down).
                yDir = _target.position.y > transform.position.y ? 1 : -1;

            //If the difference in positions is not approximately zero (Epsilon) do the following:
            else
                //Check if target x position is greater than enemy's x position, if so set x direction to 1 (move right), if not set to -1 (move left).
                xDir = _target.position.x > transform.position.x ? 1 : -1;

            //Call the AttemptMove function and pass in the generic parameter Player, because Enemy is moving and expecting to potentially encounter a Player
            AttemptMove(xDir, yDir);
        }


        //OnCantMove is called if Enemy attempts to move into a space occupied by a Player, it overrides the OnCantMove function of MovingObject 
        //and takes a generic parameter T which we use to pass in the component we expect to encounter, in this case Player
        protected override void OnCantMove(MonoBehaviour component) {
            switch (component) {
                //Declare hitPlayer and set it to equal the encountered component.
                //Call the LoseFood function of hitPlayer passing it playerDamage, the amount of food points to be subtracted.
                case Player player:
                    player.LoseFood(Damage);

                    //Call the RandomizeSfx function of SoundManager passing in the two audio clips to choose randomly between.
                    SoundManager.Instance.RandomizeSfx(attackPlayer1, attackPlayer2);
                    break;
                case Wall wall:
                    wall.DamageWall(Damage);
                    break;
                case Enemy enemy:
                    enemy.DamageEnemy(Damage);
                    break;
            }

            //Set the attack trigger of animator to trigger Enemy attack animation.
            _animator.SetTrigger(EnemyAttack);
        }
    }
}