using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AudioManager : MonoBehaviour
{
    // Serialized array of audio clips
    [SerializeField]
    private AudioClip[] audioClips = new AudioClip[22];

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

    private Subtitles[] subtitles = new Subtitles[22];

    // Start is called before the first frame update
    void Start()
    {
        // Get the AudioSource component attached to the same GameObject
        audioSource = GetComponent<AudioSource>();
        // textComponent.text = "";
        // words = textVariable.Split(' ');
        // numWords = words.Length;
        // StartCoroutine(SplitTextCoroutine());

        subtitles[0].text = "Importing and interacting with real-world objects in virtual reality";
        subtitles[1].text = "In this study, we will be testing a virtual reality (VR) system that allows you to bring real-world items into VR and use them. We would like you to do this through a few scenarios and provide feedback about your experience.";
        subtitles[2].text = "For doing this, you will first be introduced to the system, and you can practice before we move on to the actual tasks.";
        subtitles[3].text = "Now that you have some experience with the system, we have implemented four (4) scenarios where we would like you to import one or more items into VR and use them based on our instructions.";
        subtitles[4].text = "The first objective is to import the laptop into VR. You should create the shape of a laptop and edit it to fit in the best way possible. Once imported, lock its shape.";
        subtitles[5].text = "Your task is to use the laptop to answer an email. You will find an email written into a document. Write your response to the email in the same document.";
        subtitles[6].text = "In the second scenario, we are simulating a virtual lecture, where you will be learning about various tools. You will be asked to import them one by one, and then inspect and interact with them according to our instructions.";
        subtitles[7].text = "Place vertices around the hammer to resemble its shape.";
        subtitles[8].text = "The hammer is a tool commonly used to break things and drive nails in or out. It consists in two parts: a handle and a heavy metal head. Grab and lift the hammer from the table.";
        subtitles[9].text = "Place vertices around the pliers to resemble its shape.";
        subtitles[10].text = "Pliers can have a variety of purposes: bend wires, cut cables, firmly grip objects or manipulate small parts. Its shape can have variations, but usually they look like a pair of scissors. Grab the plier, and try to open and close it.";
        subtitles[11].text = "Place vertices around the screwdriver to resemble its shape.";
        subtitles[12].text = "The screwdriver is used for the insertion and removal of screws. It can have an interchangeable tip in order to be used with different types of screws. To facilitate its use, it is possible to change the direction it can be rotated. ";
        subtitles[13].text = "Grab the screwdriver, and do the following interactions: Step 1: move the slider down to use it counterclockwise. Step 2: move the slider up to use it clockwise.";
        subtitles[14].text = "Place vertices around the wrench to resemble its shape.";
        subtitles[15].text = "The wrench is a tool used to provide grip and apply torque to turn objects - usually parts like nuts and bolts. It can also be adjustable, including a spinner that changes its width. Grab the wrench, and do the following interactions:";
        subtitles[16].text = "Step 1: Pull the spinner down to make the wrench open. Step 2: Push the spinner up to make the wrench closer.";
        subtitles[17].text = "Import the mug into virtual reality. You will find it at the virtual coffee table.";
        subtitles[18].text = "Once imported, your task is to grab the mug from the coffee table, pretend to take a sip from it, then place the mug on your desk.";
        subtitles[19].text = "Import the whiteboard into VR. Your task is to use the whiteboard to list your opinions of three designs (presented next) and choose your favorite. You can use the markers or your fingers to write.";
        subtitles[20].text = "Using the whiteboard, provide your feedback for these three designs";
        subtitles[21].text = "Thank you for participating";


        
        
        
        
        
        
        for (int i = 0; i < subtitles.Length; i++)
        {
            subtitles[i].words = subtitles[i].text.Split(' ');
            subtitles[i].numWords = subtitles[i].words.Length;
        }

        // Play the first audio clip
        // if (audioClips.Length > 0)
        // {
            // PlayClip(currentClipIndex);

        // }
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