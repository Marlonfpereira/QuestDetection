using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Serialized array of audio clips
    [SerializeField]
    private AudioClip[] audioClips = new AudioClip[11];

    // Reference to the AudioSource component
    private AudioSource audioSource;

    // Index to track the current audio clip
    private int currentClipIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Get the AudioSource component attached to the same GameObject
        audioSource = GetComponent<AudioSource>();

        // Play the first audio clip
        if (audioClips.Length > 0)
        {
            PlayClip(currentClipIndex);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the audio has finished playing
        if (!audioSource.isPlaying)
        {
            StartCoroutine(WaitAndPlayNextClip());
        }
    }

    // Coroutine to wait for 0.5 seconds before playing the next clip
    private IEnumerator WaitAndPlayNextClip()
    {
        yield return new WaitForSeconds(0.5f);

        // Move to the next clip, looping back to the start if necessary
        currentClipIndex = (currentClipIndex + 1) % audioClips.Length;

        // Play the next clip
        PlayClip(currentClipIndex);
    }

    // Method to play a clip given its index
    private void PlayClip(int clipIndex)
    {
        audioSource.clip = audioClips[clipIndex];
        audioSource.loop = clipIndex >= audioClips.Length - 4; // Loop if it's one of the last 4 clips
        audioSource.Play();
    }
}
