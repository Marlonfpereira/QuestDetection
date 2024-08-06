using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AudioManager : MonoBehaviour
{
    // Serialized array of audio clips
    [SerializeField]
    private AudioClip[] audioClips = new AudioClip[15];

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

    struct Subtitles
    {
        public string text;
        public string[] words;
        public int numWords;
    }

    private Subtitles[] subtitles = new Subtitles[15];

    // Start is called before the first frame update
    void Start()
    {
        // Get the AudioSource component attached to the same GameObject
        audioSource = GetComponent<AudioSource>();
        textComponent.text = "";
        words = textVariable.Split(' ');
        numWords = words.Length;
        // StartCoroutine(SplitTextCoroutine());

        subtitles[0].text = "Welcome to our study!";
        subtitles[1].text = "This study is made in order to know the user experience in importing and using real-world objects into virtual reality.";
        subtitles[2].text = "For doing this, you will have a brief explanation on how to do the gestures in order to perform them in VR.";
        subtitles[3].text = "Now that you have some experience, let's try performing some tasks that consists in importing real-world objects into Virtual Reality and manipulating it.";
        subtitles[4].text = "The first objective is to import the laptop into virtual reality. You should create the shape of a laptop and edit it to fit in the best way possible.";
        subtitles[5].text = "After you have imported the laptop, please lock its shape. Once you have locked it, please answer the email that is on the screen.";
        subtitles[6].text = "The second objective will be importing four tools from the table. To do this follow the instructions and advice the supervisor when you have completed each of the tasks.";
        subtitles[7].text = "Task 1: Place vertices around the hammer to resemble its shape, then grab and lift the hammer from the table.";
        subtitles[8].text = "Task 2: Place vertices around the pliers to resemble its shape,  grab it, and try to opening and closing it.";
        subtitles[9].text = "Step 1: Import the screwdriver creating vertices around it. Step 2: move the slider down to use it counterclockwise. Step 3: move the slider up to use it clockwise.";
        subtitles[10].text = "Step 1: Import the wrench creating vertices around it. Step 2: Pull the spinner down to make the wrench open. Step 3: Push the spinner up to make the wrench closer.";
        subtitles[12].text = "The third objective is to import the mug into virtual reality. Then, grab the mug and pretend to take a sip.";
        subtitles[13].text = "Objective four is to import the whiteboard in front of you. Then, we'll present three app interfaces for your feedback. Please write your preferred choice and reasons on the whiteboard.";
        subtitles[14].text = "";

        for (int i = 0; i < subtitles.Length; i++)
        {
            subtitles[i].words = subtitles[i].text.Split(' ');
            subtitles[i].numWords = subtitles[i].words.Length;
        }

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
    public void PlayClip(int clipIndex)
    {
        audioSource.clip = audioClips[clipIndex];
        audioSource.loop = false;//clipIndex >= 8 && !A; // Loop if it's one of the last 4 clips and A is false
        audioSource.Play();
        currentIndex = 0;
        StartCoroutine(SplitTextCoroutine(subtitles[clipIndex]));
    }

    IEnumerator SplitTextCoroutine(Subtitles sub)
    {
        while (currentIndex < sub.numWords)
        {
            string chunk = GetNextChunk(sub);
            textComponent.text = chunk;
            yield return new WaitForSeconds(howFastTextAppear);
        }
    }

    string GetNextChunk(Subtitles sub)
    {
        int endIndex = Mathf.Min(currentIndex + wordsPerChunk, sub.numWords);
        string chunk = string.Join(" ", sub.words, currentIndex, endIndex - currentIndex);
        currentIndex = endIndex;
        return chunk;
    }
}