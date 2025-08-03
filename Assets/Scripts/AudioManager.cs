using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    public AudioClip[] backgroundMusics;
    public AudioSource audioSource;
    
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(int index)
    {
        if(index >= 0 && index < backgroundMusics.Length)
        {
            if(audioSource.clip != backgroundMusics[index])
            {
                audioSource.clip = backgroundMusics[index];
                audioSource.Play();
            }
        }
        else
        {
            Debug.LogError($"Invalid music index: {index}");
        }
    }
}