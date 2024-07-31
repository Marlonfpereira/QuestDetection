using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AudioManager : MonoBehaviour
{
    // Serialized array of audio clips
    [SerializeField]
    private AudioClip[] audioClips = new AudioClip[11];

    // Reference to the AudioSource component
    private AudioSource audioSource;

    // Index to track the current audio clip
    private int currentClipIndex = 0;

    // Variable to control the loop condition
    public bool A;

    // TextMeshPro related variables
    public TextMeshProUGUI textComponent;
    public string textToType;
    public float revealSpeed = 0.2f;
    private int currentCharacter = 0;
    private float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        // Get the AudioSource component attached to the same GameObject
        audioSource = GetComponent<AudioSource>();
        textComponent.text = "";

        // Play the first audio clip
        if (audioClips.Length > 0)
        {
            PlayClip(currentClipIndex);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the audio has finished playing and start the coroutine if necessary
        if (!audioSource.isPlaying)
        {
            StartCoroutine(WaitAndPlayNextClip());
        }

        // Handle TextMeshPro text reveal
        if (currentCharacter < textToType.Length)
        {
            timer += Time.deltaTime;
            if (timer > revealSpeed)
            {
                textComponent.text += textToType[currentCharacter];
                currentCharacter++;
                timer = 0f;
            }
        }
    }

    // Coroutine to wait for 0.5 seconds before playing the next clip
    private IEnumerator WaitAndPlayNextClip()
    {
        yield return new WaitForSeconds(0.5f);

        // Check if we are dealing with clips 8, 9, 10, or 11
        if (currentClipIndex >= 7 && currentClipIndex < 11)
        {
            // Loop until condition A is true
            if (!A)
            {
                PlayClip(currentClipIndex);
            }
            else
            {
                // Move to the next clip
                currentClipIndex++;
                // Loop back to the start if necessary
                currentClipIndex = currentClipIndex % audioClips.Length;
                PlayClip(currentClipIndex);
            }
        }
        else
        {
            // Move to the next clip, looping back to the start if necessary
            currentClipIndex = (currentClipIndex + 1) % audioClips.Length;

            // Play the next clip
            PlayClip(currentClipIndex);
        }
    }

    // Method to play a clip given its index
    private void PlayClip(int clipIndex)
    {
        audioSource.clip = audioClips[clipIndex];
        audioSource.loop = clipIndex >= 7 && clipIndex < 11 && !A; // Loop if it's clip 8, 9, 10, or 11 and A is false
        audioSource.Play();
    }
}