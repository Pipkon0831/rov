using UnityEngine;

public class SceneExit : MonoBehaviour
{

    [Tooltip("切换音乐索引")] public int _perm;
    [Tooltip("需要过渡新场景的名称")] public string newSceneName;
    
    private AudioManager musicManager;

    private void Awake()
    {
        musicManager = FindObjectOfType<AudioManager>();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {   
            musicManager.PlayMusic(_perm);
            TransitionInternal();
        }
    }

    public void TransitionInternal()
    {
        SceneLoader.Instance.TransitionToScene(newSceneName);
    }
}
