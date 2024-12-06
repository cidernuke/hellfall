using System.Collections;
using System.Collections.Generic;
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


    // Start is called before the first frame update
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

            // Start title animation
            if(sequence == 0)
            {
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
            if(playerPosition.x >= 96.5 && playerPosition.x <= 98 && playerPosition.y > 12.5&& sequence == 6) 
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
        // playerMovementScript.blockWalk = true;
        playerMovementScript.blockJump = true;
        playerMovementScript.blockCrouch = true;
        playerMovementScript.animator.SetBool("grounded", true);

        playerRigidBody.constraints = RigidbodyConstraints2D.FreezePosition;

        guideAnimator.SetBool("hasDisappeared", false);
        guideAnimator.SetBool("playerArrived", true);
        guideAnimator.SetBool("isIdle", true);

        // while(playerMovementScript.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
        //     Debug.Log(playerMovementScript.animator.GetCurrentAnimatorStateInfo(0));
        //     playerMovementScript.animator.Play("Idle", 0, 0f);
        // }

        playerMovementScript.enabled = false;
    }

    void StartPlayer(int which)
    {
        // playerMovementScript.enabled = true;
        if(which == 2) {
            playerRigidBody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        } else {
            playerRigidBody.constraints = RigidbodyConstraints2D.None;
            playerRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
       

        switch(which)
        {
            case 1:
                playerMovementScript.blockWalk = false;
                break;
            case 2:
                playerMovementScript.blockJump = false;
                break;
            case 3:
                playerMovementScript.blockWalk = false;
                playerMovementScript.blockJump = false;
                break;
            case 4:
                playerMovementScript.blockWalk = false;
                playerMovementScript.blockJump = false;
                playerMovementScript.blockCrouch = false;
                break;
            case 5:
                playerMovementScript.blockWalk = false;
                playerMovementScript.blockJump = false;
                playerMovementScript.blockCrouch = false;
                playerMovementScript.blockDash = false;
                break;
            default:
                playerMovementScript.blockWalk = false;
                playerMovementScript.blockJump = false;
                playerMovementScript.blockCrouch = false;
                playerMovementScript.blockDash = false;
                break;
        }

    }

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

    void ResetAnimation()
    {
        guideAnimator.SetBool("isIdle", false);
        guideAnimator.SetBool("playerArrived", false);
        guideAnimator.SetBool("hasDisappeared", true);
    }



    IEnumerator firstSequence() {
        skipped = false;
        StopPlayer();
        yield return new WaitForSeconds(1f);

        // Hint: Use | Q | to skip dialogue. (7 words)
        StartCoroutine(someoneSpeaks(hintBubbleSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/hint_bubble_1", 3.5f));
        yield return skipCheck(1f);
        
        // S: Death may be the greatest of all human blessings (9 words)
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/guide_speech_2", 4.0f));
        yield return skipCheck(4.1f);

        // P: What? (1 word)
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_2", 0.7f));
        yield return skipCheck(0.8f);

        // P: Where am I?
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_2a", 1.6f));
        yield return skipCheck(1.7f);

        // // P: That's dark (2 words)
        // StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_3", 1.5f));
        // yield return skipCheck(1.6f);

        // // P: Why are you talking about death? (6 words)
        // StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_4", 3.0f));
        // yield return skipCheck(3.1f);

        // // S: Why? (1 word)
        // StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/guide_speech_3", 1.2f));
        // yield return skipCheck(1.3f);

        // // S: Cause we're dead of course (5 words)
        // StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/guide_speech_4", 2.5f));
        // yield return skipCheck(2.6f);

        // // P: Dead? (1 word)
        // StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_5", 1.2f));
        // yield return skipCheck(1.3f);

        // S: Yes, you're in the first circle of hell. (9 words)
        // -> Welcome to the first circle of hell!
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/guide_speech_5", 4.0f));
        yield return skipCheck(4.1f);

        // P: What? (1 Word)
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_2", 1.2f));
        yield return skipCheck(1.3f);

        // P: Hell?
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_6", 1.2f));
        yield return skipCheck(1.3f);

        // P: Why?
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_5", 1.2f));
        yield return skipCheck(1.3f);

        // S: Well like I always say: "It's ... (9 words)
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/guide_speech_6", 4.0f));
        yield return skipCheck(4.1f);

        // S: Aaaaand you apparently did not my friend. (7 words)
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/guide_speech_7", 3.5f));
        yield return skipCheck(3.6f);

        // P: Who are you to judge my life? (8 words)
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_7", 3.5f));
        yield return skipCheck(3.6f);

        // S: I did not judge your life...
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/guide_speech_9", 3.5f));
        yield return skipCheck(3.6f);

        // S: God did! (2 words)
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/guide_speech_10", 1.5f));
        yield return skipCheck(1.6f);

        // P: Okay... Okay... (2 words)
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_8", 1.5f));
        yield return skipCheck(1.6f);

        // P: How the hell do I get out of here? (9 words)
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_9", 4.0f));
        yield return skipCheck(4.1f);

        // // S: Well, that's a more difficult question. (6 words)
        // StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/guide_speech_11", 3.0f));
        // yield return skipCheck(3.1f);

        // S: At the moment you're in the first of eight circles. (10 words)
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/guide_speech_12", 4.5f));
        yield return skipCheck(4.6f);

        // G: I suppose you can try and fight your way out of hell. (11 words)
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/guide_speech_12a", 5.0f));
        yield return skipCheck(5.1f);

        // S: But if you decide to leave I'll have to teach you a few things first. (14 words)
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/guide_speech_16", 5.5f));
        yield return skipCheck(5.6f);

        // S: Let's get started! (3 words)
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/guide_speech_18", 2.0f));
        yield return skipCheck(2.1f);

        // S: Try moving forwards and backwards with | A | and | D |. (10 words)
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/guide_speech_19", 3.5f));
        yield return skipCheck(3.6f);

        ResetAnimation();
        yield return new WaitForSeconds(1f);

        guide.transform.position = new Vector2(30f, guide.transform.position.y);

        StartPlayer(1);
    }

    
    IEnumerator secondSequence()
    {
        playerSpeech.transform.position = new Vector2(24.5f, -1.75f);
        guideSpeech.transform.position = new Vector2(28.5f, -1.75f);

        StopPlayer();
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(moveSomeone(player, player.transform.position, new Vector2(23f, -2.5f), 0.05f));

        // S: you can jump with | space |...
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/guide_speech_20", 1.5f));
        yield return skipCheck(1.5f);

        StartPlayer(2);
    }

    IEnumerator thirdSequence()
    {

        yield return new WaitForSeconds(1.1f);
        StartCoroutine(moveSomeone(player, player.transform.position, new Vector2(23f, -2.5f), 0.05f));

        StopPlayer();
        yield return new WaitForSeconds(0.5f);


        // S: ...you can also double-jump by hitting | space | twice.
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/guide_speech_21", 1.5f));
        yield return skipCheck(1.5f);

        StartPlayer(2);

        sequence = 4;
    }

    IEnumerator fourthSequence()
    {
        yield return new WaitForSeconds(1f);
        StopPlayer();
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(moveSomeone(player, player.transform.position, new Vector2(23f, -2.5f), 0.2f));

        // S: Follow me for your next lesson.
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/guide_speech_22", 1f));
        yield return skipCheck(1f);

        ResetAnimation();
        yield return new WaitForSeconds(1f);
        guide.transform.position = new Vector2(80.8f, 1.4f);

        StartPlayer(3);

        sequence = 5;
    }

    IEnumerator fifthSequence()
    {
        StartCoroutine(moveSomeone(player, player.transform.position, new Vector2(77f, 1.48f), 0.2f));
        StopPlayer();
        

        playerSpeech.transform.position = new Vector2(77.75f, 2.7f);
        guideSpeech.transform.position = new Vector2(77.75f, 2f);

        yield return new WaitForSeconds(1f);

        // S: You can move through certain platforms with | SPACE | and | S | (9 words)
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/guide_speech_23", 4.0f));
        yield return skipCheck(4.1f);

        // S: You can Wall-Jump by jumping against a wall (8 words)
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/guide_speech_24", 3.5f));
        yield return skipCheck(3.6f);

        // S: Let's see if you can make it past this next part! (11 words)
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/guide_speech_25", 4.5f));
        yield return skipCheck(4.6f);

        ResetAnimation();
        yield return new WaitForSeconds(1f);

        guide.transform.position = new Vector2(101.3f, 12.89f);

        StartPlayer(3);
    }
    
    IEnumerator sixthSequence()
    {
        StartCoroutine(moveSomeone(player, player.transform.position, new Vector2(98f, 12.502f), 0.4f));
        StopPlayer();

        yield return new WaitForSeconds(1f);

        guideSpeech.transform.position = new Vector2(100.2f, 13f);

        // S: Try crouching using | S |
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/guide_speech_26", 2f));
        yield return skipCheck(2f);


        ResetAnimation();
        yield return new WaitForSeconds(2f);

        guide.transform.position = new Vector2(123f, 7.32f);

        StartPlayer(4);

    }

    IEnumerator seventhSequence()
    {
        StartCoroutine(moveSomeone(player, player.transform.position, new Vector2(119f, 7.5f), 0.4f));
        guideSpeech.transform.position = new Vector2(121.5f, 8f);

        StopPlayer();
        yield return new WaitForSeconds(1f);



        // S: You can also Dash using | Shift | 
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/guide_speech_27", 2f));
        yield return skipCheck(2f);

        ResetAnimation();
        yield return new WaitForSeconds(1f);

        StartPlayer(5);
    }

    IEnumerator eighthSequence()
    {
        // StartCoroutine(moveSomeone(player, player.transform.position, new Vector2(141f, -2.49f), 0.2f));
        playerRigidBody.constraints = RigidbodyConstraints2D.FreezePosition;
        Debug.Log("Guide Speech: " + guideSpeech.transform.position);
        guideSpeech.transform.position = new Vector2(144.5f, -1.5f);
        Debug.Log("Guide Speech: " + guideSpeech.transform.position);


        StopPlayer();
        yield return new WaitForSeconds(1f);

        // S: Didn't think you'd manage all that.
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/guide_speech_28", 3f));
        yield return skipCheck(3f);
        // S: Maybe you do have a chance to make it out.
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/guide_speech_29", 3f));
        yield return skipCheck(3f);
        // S: Finally, before you go, you'll have to do some fighting in the next circles
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/guide_speech_30", 5f));
        yield return skipCheck(5f);
        // S: You can hit enemies with Left-Click
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/guide_speech_31", 3f));
        yield return skipCheck(3f);
        // S: Now you are ready head down into the depths of hell.
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/guide_speech_32", 4f));
        yield return skipCheck(4f);
        // S: Try not to die again!
        StartCoroutine(someoneSpeaks(guideSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/guide_speech_33", 2.5f));
        yield return skipCheck(2.5f);

        ResetAnimation();
        yield return new WaitForSeconds(1f);

        guide.transform.position = new Vector2(135f, 7.32f);

        StartPlayer(5);
    }

}
