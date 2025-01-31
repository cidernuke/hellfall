using System.Collections;
using UnityEngine;

public class guide_Script : MonoBehaviour
{
    // Player variables

    public GameObject player;                           // Player Game Object
    private Rigidbody2D playerRigidBody;                // Player Rigidbody
    public GameObject playerSpeech;                     // Player Speech Bubble Object
    private SpriteRenderer playerSpeechSpriteRenderer;  // Player speech Bubble
    private PlayerMovement playerMovementScript;

    // guide variables
    public GameObject guide;                           // guide Game Object
    private Rigidbody2D guideRigidBody;                // guide Rigidbody
    public GameObject guideSpeech;                     // guide Speech Bubble Object
    private SpriteRenderer guideSpeechSpriteRenderer;  // guide speech Bubble
    public Animator guideAnimator;

    // Camera variables
    public GameObject camera;

    // Title variables
    public GameObject title;
    public SpriteRenderer titleSpriteRenderer;
    public Animator titleAnimator;

    // Hint varaibles
    public GameObject hintBubble;
    private SpriteRenderer hintBubbleSpriteRenderer;

    // Check variables
    public int sequence;    // defines current sequence
    public bool skipped;    // true, if speech bubble skipped
    public bool isTitleAnimating; // true, if title is currently animating


    /// <summary>
    /// Start is called before the first frame update.
    /// Initializes various game objects and components.
    /// </summary>
    void Start()
    {
        
        player = GameObject.Find("Player");
        playerRigidBody = player.GetComponent<Rigidbody2D>();
        playerSpeech = GameObject.Find("Player_Speech_Bubble");
        playerSpeechSpriteRenderer = playerSpeech.GetComponent<SpriteRenderer>();
        playerMovementScript = player.GetComponent<PlayerMovement>();
        
        guide = GameObject.Find("Guide");
        guideRigidBody = guide.GetComponent<Rigidbody2D>();
        guideSpeech = GameObject.Find("Guide_Speech_Bubble");
        guideSpeechSpriteRenderer = guideSpeech.GetComponent<SpriteRenderer>();
        guideAnimator = guide.GetComponent<Animator>();

        
        camera = GameObject.Find("Main Camera");

        title = GameObject.Find("Title");
        titleSpriteRenderer = title.GetComponent<SpriteRenderer>();
        titleAnimator = title.GetComponent<Animator>();

        hintBubble = GameObject.Find("Hint_Bubble");
        hintBubbleSpriteRenderer = hintBubble.GetComponent<SpriteRenderer>();

        sequence = 0;
        isTitleAnimating = false;

        

    }

    /// <summary>
    /// Update is called once per frame.
    /// Handles the sequence of events based on the player's position and actions.
    /// </summary>
    void Update()
    {
        if (player != null)
        {
            // Get the players position
            Vector2 playerPosition = player.transform.position;

            if (isTitleAnimating)
            {
                title.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, -1);
            }

            // Start title animation
            if(sequence == 0)
            {
                StartCoroutine(moveSomeone(player, player.transform.position, new Vector2(player.transform.position.x, -2.32f), 0.05f));
                StartCoroutine(titleAnimation());
                StopPlayer();
                sequence = 1;
            }

            // Check if title is finished
            if (!isTitleAnimating && sequence == 1)
            {
                StartCoroutine(firstSequence());
                sequence = 2;
            }

            // Check if player has walked to in front of guide after testing |A| and |D| 
            if (playerPosition.x >= 23f && playerPosition.x <= 24f && sequence == 2)
            {
                StartCoroutine(secondSequence());
                sequence = 3;
            }

            // Check if player has jumped
            if(playerPosition.y >= -2 && sequence == 3) 
            {
                StartCoroutine(thirdSequence());
            }

            // Check if player has double-jumped
            if(playerPosition.y >= -1 && sequence == 4) 
            {
                StartCoroutine(fourthSequence());
            }

            // Check if player made it to just before the platforms
            if(playerPosition.x >= 76.5 && playerPosition.x <= 78 && sequence == 5) 
            {
                StartCoroutine(fifthSequence());
                sequence = 6;
            }

            // Check if player has made it passed the platforms
            if(playerPosition.x >= 97 && playerPosition.x <= 98 && playerPosition.y > 12.5 && sequence == 6) 
            {
                StartCoroutine(sixthSequence());
                sequence = 7;
            }

            // Check if player dashed
            if(playerPosition.x >= 118.5 && playerPosition.x <= 120 && sequence == 7) 
            {
                StartCoroutine(seventhSequence());
                sequence = 8;
            }

            if(playerPosition.x >= 141.5 && playerPosition.x <= 143 && sequence == 8) 
            {
                StartCoroutine(eighthSequence());
                sequence = 9;
            }

            if(playerPosition.x >= 158 && playerPosition.x <= 159 && sequence == 9)
            {
                StartCoroutine(ninthSequence());
                sequence = 10;
            }
        }
        
    }

    /// <summary>
    /// Displays a speech bubble for a character.
    /// </summary>
    /// <param name="someonesSpriteRenderer">The SpriteRenderer of the character's speech bubble.</param>
    /// <param name="spriteURL">The URL of the speech bubble sprite.</param>
    /// <param name="skippable">Whether the speech bubble can be skipped.</param>
    /// <returns>IEnumerator for coroutine.</returns>
    IEnumerator someoneSpeaks(SpriteRenderer someonesSpriteRenderer, string spriteURL, bool skippable)
    {
        Sprite speechBubble = Resources.Load<Sprite>(spriteURL);
        someonesSpriteRenderer.sprite = speechBubble;

        skipped = false;
        
        if(skippable) {
            // Wait for either the full time or until "Q" is pressed to skip
            while (!skipped) 
            {
                if (Input.GetKeyDown(KeyCode.Q)) 
                {
                    // Hide the speech bubble and exit early if "Q" is pressed
                    someonesSpriteRenderer.sprite = null;
                    skipped = true;
                    yield break;
                }

                yield return null; // Wait for the next frame
            }
            
            someonesSpriteRenderer.sprite = null;

        }

    }

    /// <summary>
    /// Checks if the speech bubble has been skipped.
    /// </summary>
    /// <returns>IEnumerator for coroutine.</returns>
    IEnumerator skipCheck()
    {
        while(!skipped)
        {
            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// Moves a game object from a start position to an end position over a duration.
    /// </summary>
    /// <param name="someone">The game object to move.</param>
    /// <param name="start">The start position.</param>
    /// <param name="end">The end position.</param>
    /// <param name="duration">The duration of the movement.</param>
    /// <returns>IEnumerator for coroutine.</returns>
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

    /// <summary>
    /// Stops the player's movement and animations.
    /// </summary>
    void StopPlayer() 
    {
        playerMovementScript.blockInput = true;

        playerMovementScript.body.velocity = new Vector2(0, playerMovementScript.body.velocity.y);

        playerMovementScript.animator.SetBool("grounded", true);
        playerMovementScript.animator.SetBool("is_falling", false);
        playerMovementScript.animator.SetBool("is_idle", true);

        playerRigidBody.constraints = RigidbodyConstraints2D.FreezePosition;

        hintBubble.transform.position = new Vector3(playerSpeech.transform.position.x, playerSpeech.transform.position.y - 3, -1);

        guideAnimator.SetBool("hasDisappeared", false);
        guideAnimator.SetBool("playerArrived", true);
        guideAnimator.SetBool("isIdle", true);

        
    }

    /// <summary>
    /// Starts the player's movement and animations.
    /// </summary>
    /// <param name="jumpStop">Whether to stop the player's jump.</param>
    void StartPlayer(bool jumpStop)
    {
        playerMovementScript.enabled = true;
        playerMovementScript.animator.SetBool("is_idle", false);

        playerMovementScript.blockInput = false;

        playerRigidBody.constraints = RigidbodyConstraints2D.None;

        if(jumpStop) {
            playerRigidBody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        } else {
            playerRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    /// <summary>
    /// Handles the title animation.
    /// </summary>
    /// <returns>IEnumerator for coroutine.</returns>
    IEnumerator titleAnimation()
    {

        // Start following the camera
        isTitleAnimating = true;

        // Wait for 2 seconds while the animation plays
        yield return new WaitForSeconds(2f);

        // Stop following the camera
        titleSpriteRenderer.sprite = null;

        isTitleAnimating = false;
    }

    /// <summary>
    /// Resets the guide's animation states.
    /// </summary>
    void ResetAnimation()
    {
        guideAnimator.SetBool("isIdle", false);
        guideAnimator.SetBool("playerArrived", false);
        guideAnimator.SetBool("hasDisappeared", true);
    }

    /// <summary>
    /// Handles the first sequence of events.
    /// </summary>
    /// <returns>IEnumerator for coroutine.</returns>
    IEnumerator firstSequence() {
        skipped = false;
        StopPlayer();

        // Hint: Use | Q | to skip dialogue. (7 words)
        StartCoroutine(someoneSpeaks(hintBubbleSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/hint_bubble_1", false));
        
        // S: Death may be the greatest of all human blessings (9 words)
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/guide_speech_2", true));
        yield return skipCheck();

        // P: What? (1 word)
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/player_speech_2", true));
        yield return skipCheck();

        // P: Where am I?
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/player_speech_2a", true));
        yield return skipCheck();

        // S: Welcome to the first circle of hell! (9 words)
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/guide_speech_5", true));
        yield return skipCheck();

        // P: What? (1 Word)
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/player_speech_2", true));
        yield return skipCheck();

        // P: Hell?
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/player_speech_6", true));
        yield return skipCheck();

        // P: Why?
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/player_speech_5", true));
        yield return skipCheck();

        // S: Well like I always say: "It's ... (9 words)
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/guide_speech_6", true));
        yield return skipCheck();

        // S: Aaaaand you apparently did not my friend. (7 words)
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/guide_speech_7", true));
        yield return skipCheck();

        // P: Who are you to judge my life? (8 words)
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/player_speech_7", true));
        yield return skipCheck();

        // S: I did not judge your life...
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/guide_speech_9", true));
        yield return skipCheck();

        // S: God did! (2 words)
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/guide_speech_10", true));
        yield return skipCheck();

        // P: Okay... Okay... (2 words)
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/player_speech_8", true));
        yield return skipCheck();

        // P: How the hell do I get out of here? (9 words)
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/player_speech_9", true));
        yield return skipCheck();

        // // S: Well, that's a more difficult question. (6 words)
        // StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/guide_speech_11", 3.0f));
        // yield return skipCheck(3.1f);

        // S: At the moment you're in the first of eight circles. (10 words)
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/guide_speech_12", true));
        yield return skipCheck();

        // G: I suppose you can try and fight your way out of hell. (11 words)
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/guide_speech_12a", true));
        yield return skipCheck();

        // S: But if you decide to leave I'll have to teach you a few things first. (14 words)
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/guide_speech_16", true));
        yield return skipCheck();

        // S: Let's get started! (3 words)
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/guide_speech_18", true));
        yield return skipCheck();

        // S: Try moving forwards and backwards with | A | and | D |. (10 words)
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/guide_speech_19", true));
        yield return skipCheck();


        ResetAnimation();
        yield return new WaitForSeconds(1f);

        guide.transform.position = new Vector2(30f, guide.transform.position.y);

        StartPlayer(false);
    }

    /// <summary>
    /// Handles the second sequence of events.
    /// </summary>
    /// <returns>IEnumerator for coroutine.</returns>
    IEnumerator secondSequence()
    {
        playerSpeech.transform.position = new Vector2(24.5f, -1.75f);
        guideSpeech.transform.position = new Vector2(28.5f, -1.75f);

        StopPlayer();
        playerMovementScript.enabled = false;

        StartCoroutine(moveSomeone(player, player.transform.position, new Vector2(23f, -2.34f), 0.05f));

        // S: you can jump with | space |...
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/guide_speech_20", true));
        yield return skipCheck();

        StartPlayer(true);
    }

    /// <summary>
    /// Handles the third sequence of events.
    /// </summary>
    /// <returns>IEnumerator for coroutine.</returns>
    IEnumerator thirdSequence()
    {
        yield return new WaitForSeconds(2f);
        if(player.transform.position.x < -2) {
            StartCoroutine(moveSomeone(player, player.transform.position, new Vector2(23f, -2.34f), 0.05f));
        } else {
            yield return new WaitForSeconds(0.2f);
        }

        StopPlayer();
        playerMovementScript.enabled = false;

        // S: ...you can also double-jump by hitting | space | twice.
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/guide_speech_21", true));
        yield return skipCheck();

        StartPlayer(true);

        sequence = 4;
    }

    /// <summary>
    /// Handles the fourth sequence of events.
    /// </summary>
    /// <returns>IEnumerator for coroutine.</returns>
    IEnumerator fourthSequence()
    {
        yield return new WaitForSeconds(2f);

        StopPlayer();
        playerMovementScript.enabled = false;

        StartCoroutine(moveSomeone(player, player.transform.position, new Vector2(23f, -2.34f), 0.2f));

        // S: Follow me for your next lesson.
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/guide_speech_22", true));
        yield return skipCheck();

        ResetAnimation();
        yield return new WaitForSeconds(1f);
        guide.transform.position = new Vector2(80.8f, 1.4f);

        StartPlayer(false);

        sequence = 5;
    }

    /// <summary>
    /// Handles the fifth sequence of events.
    /// </summary>
    /// <returns>IEnumerator for coroutine.</returns>
    IEnumerator fifthSequence()
    {
        StartCoroutine(moveSomeone(player, player.transform.position, new Vector2(77f, 1.65f), 0.2f));
        yield return new WaitForSeconds(0.2f);
        StopPlayer();
        yield return new WaitForSeconds(1.1f);

        playerMovementScript.enabled = false;
        

        playerSpeech.transform.position = new Vector2(77.75f, 2.7f);
        guideSpeech.transform.position = new Vector2(77.75f, 2f);

        yield return new WaitForSeconds(1f);

        // S: You can move through certain platforms with | SPACE | and | S | (9 words)
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/guide_speech_23", true));
        yield return skipCheck();

        // S: You can Wall-Jump by jumping against a wall (8 words)
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/guide_speech_24", true));
        yield return skipCheck();

        // S: Let's see if you can make it past this next part! (11 words)
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/guide_speech_25", true));
        yield return skipCheck();

        ResetAnimation();
        yield return new WaitForSeconds(1f);

        guide.transform.position = new Vector2(101.3f, 12.89f);

        StartPlayer(false);
    }
    
    /// <summary>
    /// Handles the sixth sequence of events.
    /// </summary>
    /// <returns>IEnumerator for coroutine.</returns>
    IEnumerator sixthSequence()
    {
        StartCoroutine(moveSomeone(player, player.transform.position, new Vector2(98f, 12.67f), 0.3f));
        yield return new WaitForSeconds(0.3f);
        StopPlayer();

        yield return new WaitForSeconds(1f);

        guideSpeech.transform.position = new Vector2(100.2f, 13f);

        // S: Try crouching using | S |
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/guide_speech_26", true));
        yield return skipCheck();


        ResetAnimation();
        yield return new WaitForSeconds(2f);

        guide.transform.position = new Vector2(124f, 7.32f);

        StartPlayer(false);

    }

    /// <summary>
    /// Handles the seventh sequence of events.
    /// </summary>
    /// <returns>IEnumerator for coroutine.</returns>
    IEnumerator seventhSequence()
    {
        StartCoroutine(moveSomeone(player, player.transform.position, new Vector2(119f, 7.65f), 0.4f));
        guideSpeech.transform.position = new Vector3(122.5f, 8f, -1f);
        yield return new WaitForSeconds(0.4f);


        StopPlayer();
        yield return new WaitForSeconds(1f);



        // S: You can also Dash using | Shift | 
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/guide_speech_27", true));
        yield return skipCheck();
        // S: You can also Dash using | Shift | 
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/guide_speech_27a", true));
        yield return skipCheck();

        ResetAnimation();
        yield return new WaitForSeconds(1f);
        guide.transform.position = new Vector2(146f, -1.75f);

        StartPlayer(false);
        playerRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    /// <summary>
    /// Handles the eighth sequence of events.
    /// </summary>
    /// <returns>IEnumerator for coroutine.</returns>
    IEnumerator eighthSequence()
    {
        hintBubble.transform.position = new Vector3(145f, -4.6f, -1f);

        StopPlayer();
        StartCoroutine(moveSomeone(player, player.transform.position, new Vector2(141f, -2.3f), 0.4f));
        guideSpeech.transform.position = new Vector2(144.5f, -1.5f);


        yield return new WaitForSeconds(1f);

        // S: Didn't think you'd manage all that.
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/guide_speech_28", true));
        yield return skipCheck();
        // S: Maybe you do have a chance to make it out.
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/guide_speech_29", true));
        yield return skipCheck();
        // S: Finally, before you go, you'll have to do some fighting in the next circles
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/guide_speech_30", true));
        yield return skipCheck();
        // S: You can hit enemies with Left-Click
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/guide_speech_31", true));
        yield return skipCheck();
        // S: To hit enemies with spells, | Right-Click |
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/guide_speech_34", true));
        yield return skipCheck();
        // S: Now you are ready head down into the depths of hell.
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/guide_speech_32", true));
        yield return skipCheck();
        // S: Try not to die again!
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/guide_speech_33", true));
        yield return skipCheck();

        ResetAnimation();
        yield return new WaitForSeconds(1f);

        guide.transform.position = new Vector2(3f, 7.32f);

        StartPlayer(false);
        playerRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;

    }

    /// <summary>
    /// Handles the ninth sequence of events.
    /// </summary>
    /// <returns>IEnumerator for coroutine.</returns>
    IEnumerator ninthSequence()
    {
        hintBubble.transform.position = new Vector3(159f, -4.6f, -1f);
         // H: Use E to pick up Item.
        StartCoroutine(someoneSpeaks(hintBubbleSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/hint_bubble_2", true));
        yield return skipCheck();
         // H: press | 1 |, | 2 |, | 3 | to use items
        StartCoroutine(someoneSpeaks(hintBubbleSpriteRenderer, "Sprites/Level_Zero/Speech_Bubbles/hint_bubble_3", true));
        yield return skipCheck();
    }
}
