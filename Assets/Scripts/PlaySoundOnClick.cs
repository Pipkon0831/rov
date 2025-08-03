using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlaySoundOnClick : MonoBehaviour
{
    [SerializeField] private AudioClip clickSound; // 点击音效
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        
        // 安全检查
        if (clickSound == null)
        {
            Debug.LogError("请分配点击音效文件！", this);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 0 表示鼠标左键
        {
            PlayClickSound();
        }
    }

    private void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            // 使用PlayOneShot可以同时播放多个音效不被打断
            audioSource.PlayOneShot(clickSound);
        }
    }
}