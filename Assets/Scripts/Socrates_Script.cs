using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Socrates_Script : MonoBehaviour
{
    // Player variables

    public GameObject player;                           // Player Game Object
    private Rigidbody2D playerRigidBody;                // Player Rigidbody
    public GameObject playerSpeech;                     // Player Speech Bubble Object
    private SpriteRenderer playerSpeechSpriteRenderer;  // Player speech Bubble

    private PlayerMovement playerMovementScript;

    // Socrates variables
    public GameObject socrates;                           // Socrates Game Object
    private Rigidbody2D socratesRigidBody;                // Socrates Rigidbody
    public GameObject socratesSpeech;                     // Socrates Speech Bubble Object
    private SpriteRenderer socratesSpeechSpriteRenderer;  // Socrates speech Bubble

    // Camera variables
    public GameObject camera;

    // Title variables
    public GameObject title;
    public SpriteRenderer titleSpriteRenderer;
    public Animator titleAnimator;

    // Hint varaibles
    public GameObject hintBubble;
    private SpriteRenderer hintBubbleSpriteRenderer;

    public int sequence;    // defines current sequence
    public bool skipped;    // true, if speech bubble skipped
    public bool isTitleAnimating;


    // Start is called before the first frame update
    void Start()
    {
        
        player = GameObject.Find("Player");
        playerRigidBody = player.GetComponent<Rigidbody2D>();
        playerSpeech = GameObject.Find("Player_Speech_Bubble");
        playerSpeechSpriteRenderer = playerSpeech.GetComponent<SpriteRenderer>();
        playerMovementScript = player.GetComponent<PlayerMovement>();
        
        socrates = GameObject.Find("Socrates");
        socratesRigidBody = socrates.GetComponent<Rigidbody2D>();
        socratesSpeech = GameObject.Find("Socrates_Speech_Bubble");
        socratesSpeechSpriteRenderer = socratesSpeech.GetComponent<SpriteRenderer>();
        
        camera = GameObject.Find("Main Camera");

        title = GameObject.Find("Title");
        titleSpriteRenderer = title.GetComponent<SpriteRenderer>();
        titleAnimator = title.GetComponent<Animator>();

        hintBubble = GameObject.Find("Hint_Bubble");
        hintBubbleSpriteRenderer = hintBubble.GetComponent<SpriteRenderer>();

        playerMovementScript.blockCrouch = true;
        playerMovementScript.blockJump = true;
        playerMovementScript.blockDash = true;

        sequence = 0;
        isTitleAnimating = false;

    }

    // Update is called once per frame
    void Update()
    {
        
        if (player != null)
        {
            // Get the players position
            Vector2 playerPosition = player.transform.position;

            if (isTitleAnimating)
            {
                title.transform.position = new Vector2(camera.transform.position.x, camera.transform.position.y);
            }

            if(sequence == 0)
            {
                StartCoroutine(titleAnimation());
                sequence = 1;
            }

            // Check if player has arrived in front of Socrates after spawn
            if (playerPosition.x >= 8f && playerPosition.x <= 9f && sequence == 1)
            {
                StartCoroutine(firstSequence());
                sequence = 2;
            }

            // Check if player has walked to in front of socrates after testing |A| and |D| 
            if (playerPosition.x >= 23f && playerPosition.x <= 24f && sequence == 2)
            {
                StartCoroutine(secondSequence());
                sequence = 3;
            }

            // Check if player has jumped
            if(playerPosition.y >= -2 && sequence == 3) 
            {
                StartCoroutine(thirdSequence());
                sequence = 4;
            }

            // Check if player has double-jumped
            if(playerPosition.y >= -0.5 && sequence == 4) 
            {
                StartCoroutine(fourthSequence());
                sequence = 5;
            }

            // Check if player made it to just before the platforms
            if(playerPosition.x >= 76.5 && playerPosition.x <= 78 && sequence == 5) 
            {
                StartCoroutine(fifthSequence());
                sequence = 6;
            }

            // Check if player has made it passed the platforms
            if(playerPosition.x >= 96.5 && playerPosition.x <= 98 && playerPosition.y > 12.5&& sequence == 6) 
            {
                StartCoroutine(sixthSequence());
                sequence = 7;
            }

            // Check if player dashed
            if(playerPosition.x >= 118.5 && playerPosition.x <= 120 && playerPosition.y <= 8 && sequence == 7) 
            {
                StartCoroutine(seventhSequence());
                sequence = 8;
            }

            if(playerPosition.x >= 142.5 && playerPosition.x <= 144 && sequence == 8) 
            {
                StartCoroutine(eighthSequence());
                sequence = 9;
            }



        }
        
    }

    IEnumerator someoneSpeaks(SpriteRenderer someonesSpriteRenderer, string spriteURL, float time)
    {
        Sprite speechBubble = Resources.Load<Sprite>(spriteURL);
        someonesSpriteRenderer.sprite = speechBubble;
        
        float elapsedTime = 0f;

        // Wait for either the full time or until "Q" is pressed to skip
        while (elapsedTime < time) 
        {
            if (Input.GetKeyDown(KeyCode.Q)) 
            {
                // Hide the speech bubble and exit early if "Q" is pressed
                someonesSpriteRenderer.sprite = null;
                skipped = true;
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        someonesSpriteRenderer.sprite = null;
    }

    IEnumerator moveSomeone(GameObject someone, Vector2 start, Vector2 end, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            // Lerp position between start and end based on elapsed time
            someone.transform.position = Vector2.Lerp(start, end, elapsedTime / duration);

            // Increment elapsed time by the time passed since last frame
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure final position is set to end
        someone.transform.position = end;
    }

    IEnumerator skipCheck(float time)
    {
        for(float i = 0f; i < time; i += 0.2f)
        {
            if(skipped) {
                    skipped = false;
                    break;
                }
            yield return new WaitForSeconds(0.2f);    
        }
    }

    void StopPlayer() 
    {
        playerMovementScript.blockWalk = true;
        playerMovementScript.blockJump = true;
        playerMovementScript.blockCrouch = true;
        playerMovementScript.animator.SetBool("grounded", true);

        playerRigidBody.constraints = RigidbodyConstraints2D.FreezePosition;
    }

    IEnumerator titleAnimation()
    {

        // Start following the camera
        isTitleAnimating = true;

        // Wait for 2 seconds while the animation plays
        yield return new WaitForSeconds(2f);

        // Stop following the camera
        isTitleAnimating = false;

        titleSpriteRenderer.sprite = null;
    }



    IEnumerator firstSequence() {
        skipped = false;
        StopPlayer();

        playerSpeech.transform.position = new Vector2(9.65f, -1.75f);


        StartCoroutine(moveSomeone(player, player.transform.position, new Vector2(8f, -2.5f), 0.2f));
        yield return new WaitForSeconds(1f);

        // P: What.. what happened? (3 words)
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_1", 2.0f));
        yield return skipCheck(2.1f);

        // Hint: Use | Q | to skip dialogue. (7 words)
        StartCoroutine(someoneSpeaks(hintBubbleSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/hint_bubble_1", 3.5f));
        yield return skipCheck(3.6f);

        // S: Long Socrates Quote (Assume ~10 words)
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_1", 5.0f));
        yield return skipCheck(5.1f);

        // P: What? (1 word)
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_2", 1.2f));
        yield return skipCheck(1.3f);

        // S: Death may be the greatest of all human blessings (9 words)
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_2", 4.0f));
        yield return skipCheck(4.1f);

        // P: That's dark (2 words)
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_3", 1.5f));
        yield return skipCheck(1.6f);

        // P: Why are you talking about death? (6 words)
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_4", 3.0f));
        yield return skipCheck(3.1f);

        // S: Why? (1 word)
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_3", 1.2f));
        yield return skipCheck(1.3f);

        // S: Cause we're dead of course (5 words)
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_4", 2.5f));
        yield return skipCheck(2.6f);

        // P: Dead? (1 word)
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_5", 1.2f));
        yield return skipCheck(1.3f);

        // S: Yes, you're in the first circle of hell. (9 words)
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_5", 4.0f));
        yield return skipCheck(4.1f);

        // P: What? Hell? Why? (3 words)
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_6", 2.0f));
        yield return skipCheck(2.1f);

        // S: Well like I always say: "It's ... (9 words)
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_6", 4.0f));
        yield return skipCheck(4.1f);

        // S: Aaaaand you apparently did not my friend. (7 words)
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_7", 3.5f));
        yield return skipCheck(3.6f);

        // P: Who are you to judge my life? (8 words)
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_7", 3.5f));
        yield return skipCheck(3.6f);

        // S: I am Socrates! (3 words)
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_8", 2.0f));
        yield return skipCheck(2.1f);

        // S: But I did not judge your life... (7 words)
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_9", 3.5f));
        yield return skipCheck(3.6f);

        // S: God did! (2 words)
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_10", 1.5f));
        yield return skipCheck(1.6f);

        // P: Okay... Okay... (2 words)
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_8", 1.5f));
        yield return skipCheck(1.6f);

        // P: How the hell do I get out of here? (9 words)
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_9", 4.0f));
        yield return skipCheck(4.1f);

        // S: Well, that's a more difficult question (6 words)
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_11", 3.0f));
        yield return skipCheck(3.1f);

        // S: At the moment you're in the first of eight circles of hell. (13 words)
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_12", 5.0f));
        yield return skipCheck(5.1f);

        // S: But if you decide to leave I'll have to teach you a few things first (14 words)
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_16", 5.5f));
        yield return skipCheck(5.6f);

        // S: Let's get started! (3 words)
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_18", 2.0f));
        yield return skipCheck(2.1f);

        // S: Try moving forwards and backwards with | A | and | D |. (10 words)
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_19", 4.5f));
        yield return skipCheck(4.6f);

        StartCoroutine(moveSomeone(socrates, socrates.transform.position, new Vector2(30f, socrates.transform.position.y), 2f));
        yield return new WaitForSeconds(1f);

        playerRigidBody.constraints = RigidbodyConstraints2D.None;
        playerRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        playerMovementScript.blockWalk = false;
    }

    
    IEnumerator secondSequence()
    {
        playerSpeech.transform.position = new Vector2(24.5f, -1.75f);
        socratesSpeech.transform.position = new Vector2(28.5f, -1.75f);

        StopPlayer();
        StartCoroutine(moveSomeone(player, player.transform.position, new Vector2(23f, -2.5f), 0.05f));

        // S: you can jump with | space |...
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_20", 1.5f));
        yield return skipCheck(1.5f);

        playerRigidBody.constraints = RigidbodyConstraints2D.None;
        playerRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        playerRigidBody.constraints = RigidbodyConstraints2D.FreezePositionX;

        playerMovementScript.blockJump = false;

    }

    IEnumerator thirdSequence()
    {
        StopPlayer();

        yield return new WaitForSeconds(1f);
        StartCoroutine(moveSomeone(player, player.transform.position, new Vector2(23f, -2.5f), 0.05f));
        StopPlayer();

        // S: ...you can also double-jump by hitting | space | twice.
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_21", 1.5f));
        yield return skipCheck(1.5f);


        playerRigidBody.constraints = RigidbodyConstraints2D.None;
        playerRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        playerRigidBody.constraints = RigidbodyConstraints2D.FreezePositionX;
        
        playerMovementScript.blockJump = false;
        
    }

    IEnumerator fourthSequence()
    {
        yield return new WaitForSeconds(1f);
        StopPlayer();
        StartCoroutine(moveSomeone(player, player.transform.position, new Vector2(23f, -2.5f), 0.2f));

        // S: Follow me for your next lesson.
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_22", 1f));
        yield return skipCheck(1f);

        socrates.transform.position = new Vector2(80.8f, 1.4f);

        playerRigidBody.constraints = RigidbodyConstraints2D.None;
        playerRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        playerMovementScript.blockWalk = false;
        playerMovementScript.blockJump = false;

    }

    IEnumerator fifthSequence()
    {
        StopPlayer();
        
        StartCoroutine(moveSomeone(player, player.transform.position, new Vector2(77f, 1.48f), 0.2f));

        playerSpeech.transform.position = new Vector2(77.75f, 2.7f);
        socratesSpeech.transform.position = new Vector2(78.75f, 2.3f);

        // S: You can move through certain platforms with | SPACE | and | S | (9 words)
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_23", 4.0f));
        yield return skipCheck(4.1f);

        // S: You can Wall-Jump by jumping against a wall (8 words)
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_24", 3.5f));
        yield return skipCheck(3.6f);

        // S: Let's see if you can make it past this next part! (11 words)
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_25", 4.5f));
        yield return skipCheck(4.6f);

        socrates.transform.position = new Vector2(101.3f, 12.89f);

        playerMovementScript.blockWalk = false;
        playerMovementScript.blockJump = false;

        playerRigidBody.constraints = RigidbodyConstraints2D.None;
        playerRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    
    IEnumerator sixthSequence()
    {
        StopPlayer();

        StartCoroutine(moveSomeone(player, player.transform.position, new Vector2(98f, 13.5f), 0.2f));
        socratesSpeech.transform.position = new Vector2(100.2f, 13.8f);

        // S: Try crouching using | S |
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_26", 2f));
        yield return skipCheck(2f);

        socrates.transform.position = new Vector2(123f, 7.32f);

        playerMovementScript.blockWalk = false;
        playerMovementScript.blockJump = false;
        playerMovementScript.blockCrouch = false;


        playerRigidBody.constraints = RigidbodyConstraints2D.None;
        playerRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;

    }

    IEnumerator seventhSequence()
    {
        StopPlayer();

        StartCoroutine(moveSomeone(player, player.transform.position, new Vector2(119f, 7.5f), 0.2f));

        socratesSpeech.transform.position = new Vector2(122.5f, 8f);

        // S: You can also Dash using | Shift | 
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_27", 2f));
        yield return skipCheck(2f);

        playerMovementScript.blockWalk = false;
        playerMovementScript.blockJump = false;
        playerMovementScript.blockCrouch = false;
        playerMovementScript.blockDash = false;

        socrates.transform.position = new Vector2(147f, -2.35f);
        playerRigidBody.constraints = RigidbodyConstraints2D.None;
        playerRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    IEnumerator eighthSequence()
    {
        StopPlayer();

        StartCoroutine(moveSomeone(player, player.transform.position, new Vector2(144f, -2.35f), 0.2f));

        socratesSpeech.transform.position = new Vector2(146.5f, -1.75f);
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_28", 1f));
        yield return skipCheck(1f);
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_29", 1f));
        yield return skipCheck(1f);
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_30", 1f));
        yield return skipCheck(1f);
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_31", 1f));
        yield return skipCheck(2f);
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_32", 2f));
        yield return skipCheck(2f);
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_33", 2f));
        yield return skipCheck(2f);

        playerMovementScript.blockWalk = false;
        playerMovementScript.blockJump = false;
        playerMovementScript.blockCrouch = false;
        playerMovementScript.blockDash = false;

        playerRigidBody.constraints = RigidbodyConstraints2D.None;
        playerRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;

    }

}
