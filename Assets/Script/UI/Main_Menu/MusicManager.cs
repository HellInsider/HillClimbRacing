using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    [Header("UI Elements")]
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Slider volumeSlider;

    private float volume;

    void Start()
    {
        // Загружаем сохраненные настройки
        Load();

        // Применяем громкость сразу к аудио
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }

    void OnEnable()
    {
        // Этот метод вызывается автоматически при активации объекта
        InitializeMusicUI();
    }

    private void InitializeMusicUI()
    {
        // Настраиваем Toggle
        if (musicToggle != null)
        {
            musicToggle.isOn = volume > 0;
            musicToggle.onValueChanged.RemoveAllListeners();
            musicToggle.onValueChanged.AddListener(ToggleMusic);
        }

        // Настраиваем Slider
        if (volumeSlider != null)
        {
            volumeSlider.value = volume;
            volumeSlider.onValueChanged.RemoveAllListeners();
            volumeSlider.onValueChanged.AddListener(OnSliderChanged);
        }
    }

    private void OnSliderChanged(float value)
    {
        volume = value;
        Save();
        UpdateAudioVolume();

        // Обновляем состояние Toggle
        if (musicToggle != null)
        {
            musicToggle.isOn = volume > 0;
        }
    }

    private void ToggleMusic(bool isOn)
    {
        volume = isOn ? (volumeSlider != null ? volumeSlider.value : 1f) : 0f;
        Save();
        UpdateAudioVolume();
    }

    private void UpdateAudioVolume()
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("volume", volume);
        PlayerPrefs.Save();
    }

    private void Load()
    {
        volume = PlayerPrefs.GetFloat("volume", 1f); // Значение по умолчанию 1
    }
}