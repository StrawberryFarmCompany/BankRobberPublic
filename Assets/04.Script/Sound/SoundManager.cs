using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] bgmClips;
    [SerializeField] private AudioClip[] sfxClips;

    [Header("Default Volumes")]
    [Range(0f, 1f)][SerializeField] private float defaultBGMVolume = 1f;
    [Range(0f, 1f)][SerializeField] private float defaultSFXVolume = 1f;

    private const string BGM_KEY = "BGMVolume";
    private const string SFX_KEY = "SFXVolume";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        // 씬이 로드될 때마다 콜백 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private void Start()
    {
        if (bgmSource == null || sfxSource == null)
        {
            Debug.LogWarning("[SoundManager] AudioSource가 설정되지 않았습니다.");
            return;
        }

        bgmSource.loop = true;

        // 저장된 볼륨 불러오기 (없으면 기본값)
        float bgmVol = PlayerPrefs.GetFloat(BGM_KEY, defaultBGMVolume);
        float sfxVol = PlayerPrefs.GetFloat(SFX_KEY, defaultSFXVolume);

        SetBGMVolume(bgmVol);
        SetSFXVolume(sfxVol);

        // 첫 번째 BGM 자동 실행
        if (bgmClips != null && bgmClips.Length > 0)
            PlayBGM(0);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 이름으로 분기
        switch (scene.name)
        {
            case "TitleScene": // 이후 씬 이름 바뀌면 수정
                PlayBGM(0);
                break;
            case "LobbyScene": // 이후 씬 이름 바뀌면 수정
                PlayBGM(1);
                break;
            case "TutorialScene": // 이후 씬 이름 바뀌면 수정
                PlayBGM(2);
                break;
            case "추후 기입": // 본 게임 씬 이름 넣기
                PlayBGM(3);
                break;
            default:
                StopBGM(); // 지정된 게 없으면 정지
                break;
        }
    }

    // BGM 재생
    public void PlayBGM(int index)
    {
        if (bgmSource == null || index < 0 || index >= bgmClips.Length)
            return;

        // 같은 곡 중복 재생 방지
        if (bgmSource.clip == bgmClips[index] && bgmSource.isPlaying)
            return;

        bgmSource.clip = bgmClips[index];
        bgmSource.Play();
    }

    // BGM 정지
    public void StopBGM()
    {
        if (bgmSource == null) return;
        bgmSource.Stop();
    }

    // 효과음 재생
    public void PlaySFX(int index)
    {
        if (sfxSource == null || index < 0 || index >= sfxClips.Length)
            return;

        sfxSource.PlayOneShot(sfxClips[index]);
    }

    // 볼륨 설정
    public void SetBGMVolume(float volume)
    {
        if (bgmSource == null) return;
        bgmSource.volume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(BGM_KEY, bgmSource.volume);
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource == null) return;
        sfxSource.volume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(SFX_KEY, sfxSource.volume);
    }

    // 현재 볼륨 조회
    public float GetBGMVolume() => bgmSource != null ? bgmSource.volume : 0f;
    public float GetSFXVolume() => sfxSource != null ? sfxSource.volume : 0f;

    public float DefaultBGMVolume => defaultBGMVolume;
    public float DefaultSFXVolume => defaultSFXVolume;
}