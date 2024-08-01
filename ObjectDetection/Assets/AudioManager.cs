using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AudioManager : MonoBehaviour
{
    // Serialized array of audio clips
    [SerializeField]
    private AudioClip[] audioClips = new AudioClip[12];

    // Reference to the AudioSource component
    private AudioSource audioSource;

    // Index to track the current audio clip
    private int currentClipIndex = 0;

    // Variable to control the loop condition
    public bool A;

    // TextMeshPro related variables
    public TextMeshProUGUI textComponent;
    private string textVariable;
    private string[] words;
    private int currentIndex = 0;
    [SerializeField]
    private int wordsPerChunk;
    [SerializeField]
    private float howFastTextAppear;
    [SerializeField]
    private float howFastVoiceReturn;
    private int numWords = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Get the AudioSource component attached to the same GameObject
        audioSource = GetComponent<AudioSource>();
        textComponent.text = "";
        textVariable = "Hi, my name is Bob, welcome to the study about bringing real-world objects into VR. We thank you for being a part of this research about bringing real-world objects into VR. This session is anonymized and just recorded from your point of view to understand how you interact with the real-world objects in Virtual Reality. I am going to give you some tasks that involve importing objects from the real world to the virtual world and ask you to perform some tasks with them. In order to import an object, you must perform a pinching gesture with your dominant hand in order to create a polygon of the object you want to import. For example, if one task is to bring your cellphone to VR, you must pinch the corners of your phone with your index finger with your dominant hand and then confirm the selection by pressing with your index finger in your hand. You can also delete or lock an object using the menu on your dominant hand. These two actions require you to have the object in your hand and then press the respective button on the outside of your hand. I am going to give you four tasks. When you complete a task, please inform a researcher before the study can continue. If at any point you feel nauseous, please do not hesitate to take off your display, or you can inform a researcher. First task: With your hand do the pinch gesture and make a polygon in the shape of a laptop. Then confirm the calibration and search a tutorial on Google of how to draw a dragon.";
        words = textVariable.Split(' ');
        numWords = words.Length;
        // StartCoroutine(SplitTextCoroutine());

        // Play the first audio clip
        if (audioClips.Length > 0)
        {
            // PlayClip(currentClipIndex);

        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the audio has finished playing and start the coroutine if necessary
        // if (!audioSource.isPlaying)
        // {
        //     StartCoroutine(WaitAndPlayNextClip());
        // }
    }

    public void RemoteStart()
    {
        StartCoroutine(SplitTextCoroutine());
        if (audioClips.Length > 0)
        {
            PlayClip(currentClipIndex);
        }
    }

    // Coroutine to wait for X seconds before playing the next clip
    public IEnumerator WaitAndPlayNextClip()
    {

        // Check if we are dealing with the last 4 clips
        if (currentClipIndex >= 8)
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
                if (currentClipIndex >= audioClips.Length)
                {
                    currentClipIndex = 8; // Loop back to the 9th clip
                }
                PlayClip(currentClipIndex);
            }
        }
        else
        {
            // Move to the next clip
            currentClipIndex++;
            if (currentClipIndex < 8)
            {
                PlayClip(currentClipIndex);
            }
        }

        yield return new WaitForSeconds(howFastVoiceReturn);
    }

    // Method to play a clip given its index
    private void PlayClip(int clipIndex)
    {
        audioSource.clip = audioClips[clipIndex];
        audioSource.loop = clipIndex >= 8 && !A; // Loop if it's one of the last 4 clips and A is false
        audioSource.Play();
    }

    IEnumerator SplitTextCoroutine()
    {
        while (currentIndex < numWords)
        {
            string chunk = GetNextChunk();
            textComponent.text = chunk;
            yield return new WaitForSeconds(howFastTextAppear);
        }
    }

    string GetNextChunk()
    {
        int endIndex = Mathf.Min(currentIndex + wordsPerChunk, numWords);
        string chunk = string.Join(" ", words, currentIndex, endIndex - currentIndex);
        currentIndex = endIndex;
        return chunk;
    }
}