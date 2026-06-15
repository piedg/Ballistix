using UnityEngine;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{
    [SerializeField] private bool isMusicManager;
    
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    private void Start()
    {
        _slider.value = isMusicManager ? MusicManager.Instance.GetVolume() : SoundManager.Instance.GetVolume();
    }
}
