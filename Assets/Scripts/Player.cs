using UnityEngine;
using System.Collections;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;

public class Player : MovingObject {

    // characteristics
    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    
    // action sounds
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;
    public AudioClip openingDoor;


    public float restartLevelDelay = 0.5f;
    private Text foodText;                  
    private Animator animator;
    private int foodPoints;
    private Vector2 touchOrigin = -Vector2.one;
    private GameObject arms;

    protected override void OnEnable(){
        animator = GetComponent<Animator>();
        //animator.SetInteger("PlayerHasWeaponNum", 0);
        foodPoints = GameManager.instance.playerFoodPoints;
        animator.SetInteger("PlayerHasWeaponNum", GameManager.instance.playerWeapon);
        foodText = GameObject.Find("FoodText").GetComponent<Text>();
        SettingFoodText();
        base.OnEnable();
    }

    private void FixedUpdate() {
        if(!GameManager.instance.isPlayerTurn) return;
        int horizontal = 0;
        int vertical = 0;
        if (arms == null){
            animator.SetInteger("PlayerHasWeaponNum", 0);
        }
        
    #if UNITY_STANDALONE

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if(horizontal != 0)
            vertical = 0;
        
    #else

        if(Input.touchCount>0){
            Touch myTouch = Input.touches[0];
            if(myTouch.phase == TouchPhase.Began){
                touchOrigin = myTouch.position;
            }
            else if(myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0){
                Vector2 touchEnd = myTouch.position; 
                float x = touchEnd.x - touchOrigin.x;
                float y = touchEnd.y - touchOrigin.y;
                touchOrigin.x = -1;
                if(Mathf.Abs(x) > Mathf.Abs(y)){
                    horizontal = x>0 ? 1: -1;
                }
                else{
                    vertical = y >0 ? 1: -1;
                }
            }
        }
        
    #endif

        if(horizontal != 0 || vertical != 0){
            AttemptMove<Wall> (horizontal, vertical);
        }
    }

    private void Restart(){
        SceneManager.LoadScene("Main");
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Exit"){
            GameManager.instance.SaveRoomBeforeExit();
            SoundManager.instance.PlaySingle(openingDoor);
            Vector3 exitPosition = other.transform.position;
            MapManager mapManager = MapManager.instance;
            if(exitPosition.x>0 && exitPosition.y>0 && exitPosition.x>exitPosition.y){  // right door        
                GameManager.instance.spawnPosition = new Vector3(0, exitPosition.y, 0);
                mapManager.ChangePlayerCoordinates(0,1); //shift right y+=1
            }
            if(exitPosition.x>0 && exitPosition.y>0 && exitPosition.x<=exitPosition.y){ //Top door
                GameManager.instance.spawnPosition = new Vector3(exitPosition.x, 0, 0);
                mapManager.ChangePlayerCoordinates(-1,0); //shift top x += -1
            }
            if(exitPosition.x<0 && exitPosition.y>0){  // left door
                GameManager.instance.spawnPosition = new Vector3(15, exitPosition.y, 0);     // Тут мы записываем в спавнПлеерПозишн
                mapManager.ChangePlayerCoordinates(0,-1); //shift left y += -1
            }                                                                                // Мы берем позицию двери в которую вошли и записываем 
            if(exitPosition.x>0 && exitPosition.y<0){  // bot door                           // противоположную точку на карте где прибавляем +1 к координате чтоб заспавниться не в двери
                GameManager.instance.spawnPosition = new Vector3(exitPosition.x, 7, 0);      // НАДА ПЕРЕПИСАТЬ сделать переменные позиций дверей и перемещать игрока на противоположная дверь +1 к координате
                mapManager.ChangePlayerCoordinates(1,0); //shift bottom x += 1
            }
            
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        if (other.tag == "Food"){
            foodPoints += pointsPerFood;
            SettingFoodText();
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            other.gameObject.SetActive(false);
        }
        if (other.tag == "Soda"){
            foodPoints += pointsPerSoda;
            SettingFoodText();
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            other.gameObject.SetActive(false);
        }
        if(other.tag == "Weapon"){
            GameObject grabItemTxt = GameManager.instance.grabItemTxt;
            
            if(!grabItemTxt.activeSelf){
                grabItemTxt.SetActive(true);
                StartCoroutine("takeWeapon", other); // it's better to use this instead StartCoroutine(takeWeapon(other)) 
            }                                        //because when you try to StopCoroutine(takeWeapon(other)) it will think that it's another coroutine.
            else{
                grabItemTxt.SetActive(false);
                StopCoroutine("takeWeapon");  
            }
        }

    }

    private IEnumerator takeWeapon(Collider2D collider2D){
        while(!Input.GetKey(KeyCode.E)){ // wait until button will be pressed(unlimited time)
            yield return null;
        }
        if(Input.GetKey(KeyCode.E)){ // check if button "E" pressed
                int weaponNum = 0; 
                if(arms != null){  
                    arms.GetComponentInChildren<Weapon>().Drop();
                    animator.SetInteger("PlayerHasWeaponNum", weaponNum);
                }
                arms = collider2D.GetComponentInChildren<Weapon>().PickUp(transform); // Пока что просто приклееваем оружие к игроку     
               
                if (arms.GetComponent<Sword>() != null)
                {
                    weaponNum = 1;
                }
                else if(arms.GetComponent<Club>() != null)
                {
                    weaponNum = 2;
                }
                else if(arms.GetComponent<Axe>() != null)
                {
                    weaponNum = 3;
                }
                else
                {
                    Debug.Log("Unknown weapon, there is no animation for it");
                }
                animator.SetInteger("PlayerHasWeaponNum", weaponNum);   
                GameManager.instance.grabItemTxt.SetActive(false);
            }
     }

    public void LoseFood(int loss){
        animator.SetTrigger("isPlayerHit");
        foodPoints -= loss;
        SettingFoodText();
        isGameOver();
    }

    private void OnDisable() {
        GameManager.instance.playerFoodPoints = foodPoints;
        if (arms != null)
        {
            GameManager.instance.playerWeapon = arms.GetComponentInChildren<Weapon>().WeaponNum;    
        }
        
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        foodPoints --;
        SettingFoodText();
        base.AttemptMove<T>(xDir, yDir);
        
        RaycastHit2D hit;
        if (Move(xDir, yDir, out hit)){
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }
        isGameOver();
        GameManager.instance.isPlayerTurn = false;
    }

    private void isGameOver(){
        if(foodPoints <= 0){
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();
        }
    }

    protected override void onCantMove<T>(T component)
    {
        if(component.GetComponent<Wall>() != null){
            Wall hitWall = component as Wall;
            hitWall.DamageWall(wallDamage);
        }
        else if(component.GetComponent<Enemy>() != null){
            Enemy hitEnemy = component as Enemy;
            if (arms == null)
            {
                return;
            }
            arms.GetComponentInChildren<Weapon>().Attack(hitEnemy);
        }
        animator.SetTrigger("isPlayerChop");
    }

    private void SettingFoodText(){
        foodText.text = "Food: " + foodPoints;
    }

}
