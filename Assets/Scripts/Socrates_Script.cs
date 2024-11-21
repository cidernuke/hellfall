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

    // Socrates variables
    public GameObject socrates;                           // Socrates Game Object
    private Rigidbody2D socratesRigidBody;                // Socrates Rigidbody
    public GameObject socratesSpeech;                     // Socrates Speech Bubble Object
    private SpriteRenderer socratesSpeechSpriteRenderer;  // Socrates speech Bubble

    public int sequence;    // defines current sequence
    public bool skipped;    // true, if speech bubble skipped


    // Start is called before the first frame update
    void Start()
    {
        
        player = GameObject.Find("Player");
        playerRigidBody = player.GetComponent<Rigidbody2D>();
        playerSpeech = GameObject.Find("Player_Speech_Bubble");
        playerSpeechSpriteRenderer = playerSpeech.GetComponent<SpriteRenderer>();
        
        socrates = GameObject.Find("Socrates");
        socratesRigidBody = socrates.GetComponent<Rigidbody2D>();
        socratesSpeech = GameObject.Find("Socrates_Speech_Bubble");
        socratesSpeechSpriteRenderer = socratesSpeech.GetComponent<SpriteRenderer>();
        
        sequence = 1;

    }

    // Update is called once per frame
    void Update()
    {
        
        if (player != null)
        {
            // Get the players position
            Vector2 playerPosition = player.transform.position;
            //Debug.Log("Player Position from Enemy: " + playerPosition.x);

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
            if(playerPosition.y >= -1 && sequence == 4) 
            {
                StartCoroutine(fourthSequence());
                sequence = 5;
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
        for(float i = 0f; i < time; i += 0.5f)
        {
            yield return new WaitForSeconds(i);
                if(skipped) {
                    skipped = false;
                    break;
                }
        }
    }


    IEnumerator firstSequence() {
        playerRigidBody.constraints = RigidbodyConstraints2D.FreezePosition;
        StartCoroutine(moveSomeone(player, player.transform.position, new Vector2(8f, -2.5f), 0.2f));
        yield return new WaitForSeconds(1f);

        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_1", 2f));
        yield return skipCheck(2f);
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_1", 5f));
        yield return skipCheck(3f);
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_2", 1f));
        yield return skipCheck(1f);
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_2", 3f));
        yield return skipCheck(3f);
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_3", 2f));
        yield return skipCheck(2f);
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_4", 3f));
        yield return skipCheck(2f);
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_3", 1.5f));
        yield return skipCheck(2f);
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_4", 3f));
        yield return skipCheck(2f);
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_5", 1.5f));
        yield return skipCheck(1f);
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_5", 4f));
        yield return skipCheck(3f);
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_6", 3f));
        yield return skipCheck(2f);
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_6", 4f));
        yield return skipCheck(2f);
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_7", 4f));
        yield return skipCheck(2f);
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_7", 3f));
        yield return skipCheck(2f);
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_8", 2.5f));
        yield return skipCheck(2f);
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_9", 3.7f));
        yield return skipCheck(2f);
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_10", 2f));
        yield return skipCheck(2f);
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_8", 2f));
        yield return skipCheck(2f);
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_9", 3f));
        yield return skipCheck(2f);
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_11", 3f));
        yield return skipCheck(2f);
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_12", 3f));
        yield return skipCheck(2f);
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_13", 3f));
        yield return skipCheck(2f);
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_10", 3f));
        yield return skipCheck(2f);
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_11", 3f));
        yield return skipCheck(2f);
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_14", 3f));
        yield return skipCheck(2f);
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_15", 5f));
        yield return skipCheck(2f);
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_16", 5f));
        yield return skipCheck(2f);
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_17", 5f));
        yield return skipCheck(2f);
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_12", 3f));
        yield return skipCheck(2f);
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_18", 3f));
        yield return skipCheck(2f);

        // S: You can move forwards and backwards
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_19", 5f));
        yield return skipCheck(2f);

        // P: use |D| to move forwards and |A| to move backwards
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_13", 1.5f));
        StartCoroutine(moveSomeone(socrates, socrates.transform.position, new Vector2(30f, socrates.transform.position.y), 5f));
        yield return new WaitForSeconds(2f);

        playerRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;

    }

    
    IEnumerator secondSequence()
    {
        
        playerSpeech.transform.position = new Vector2(24.5f, -1.75f);
        socratesSpeech.transform.position = new Vector2(28.5f, -1.75f);

        StartCoroutine(moveSomeone(player, player.transform.position, new Vector2(23f, -2.5f), 0.05f));
        playerRigidBody.constraints = RigidbodyConstraints2D.FreezePosition;


        // S: you can jump
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_20", 3f));
        yield return skipCheck(2f);
        
        // P: Use |Space| to jump
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_14", 1.5f));
        yield return skipCheck(2f);

        playerRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        playerRigidBody.constraints = RigidbodyConstraints2D.FreezePositionX;  

    }

    IEnumerator thirdSequence()
    {

        // S: you can double-jump
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_21", 3f));
        yield return skipCheck(2f);

        // P: Use |Space| twice to double-jump
        StartCoroutine(someoneSpeaks(playerSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/player_speech_15", 2f));
        yield return skipCheck(2f);

        playerRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;


    }

    IEnumerator fourthSequence()
    {
        yield return new WaitForSeconds(1f);
        playerRigidBody.constraints = RigidbodyConstraints2D.FreezePosition;
        StartCoroutine(moveSomeone(player, player.transform.position, new Vector2(23f, -2.5f), 0.2f));
        StartCoroutine(someoneSpeaks(socratesSpeechSpriteRenderer, "Sprites/Level_One/Speech_Bubbles/socrates_speech_22", 2f));
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(moveSomeone(socrates, socrates.transform.position, new Vector2(38f, socrates.transform.position.y), 0.2f));
        yield return new WaitForSeconds(0.2f);
        socrates.transform.position = new Vector2(80.8f, 4f);
        playerRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;

    }
}
