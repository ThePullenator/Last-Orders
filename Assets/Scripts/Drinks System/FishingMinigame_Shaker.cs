using UnityEngine;
using UnityEngine.UI;

public class FishingMinigame_Shaker : MonoBehaviour
{
    public static FishingMinigame_Shaker Instance;

    [Header("UI")]
    public GameObject uiRoot;          // ShakingGameUI
    public RectTransform playArea;
    public RectTransform playerShaker; // Bobber (the shaker icon you control)
    public RectTransform greenZone;    // Green target area
    public Image progressFill;
    public Image meterFill;

    [Header("Space Bar Control")]
    public float upForce = 1100f;      // how strong space pushes up
    public float gravity = 1700f;      // how fast it falls
    public float maxVelocity = 900f;

    [Header("Green Zone Movement")]
    public float greenSpeed = 260f;
    public float greenWiggle = 1.2f;

    [Header("Progress")]
    public float startProgress = 0.45f;
    public float fillRate = 0.75f;
    public float drainRate = 0.95f;

    float _vel;
    float _progress;
    bool _playing;

    float _top;
    float _bottom;

    void Awake()
    {
        Instance = this;
        if (uiRoot != null) uiRoot.SetActive(false);
    }

    public void StartMinigame()
    {
        if (_playing) return;

        if (uiRoot == null || playArea == null || playerShaker == null || greenZone == null)
        {
            Debug.LogError("[Minigame] Missing inspector references!");
            return;
        }

        _playing = true;
        _vel = 0f;
        _progress = startProgress;

        uiRoot.SetActive(true);
        UIManager.Instance?.EnterMinigameMode();

        CacheBounds();
        ResetPositions();
        UpdateProgressUI();
    }

    void CacheBounds()
    {
        float halfTrack = playArea.rect.height * 0.5f;
        _top = halfTrack;
        _bottom = -halfTrack;
    }

    void ResetPositions()
    {
        float pHalf = playerShaker.rect.height * 0.5f;
        float gHalf = greenZone.rect.height * 0.5f;

        playerShaker.anchoredPosition = new Vector2(playerShaker.anchoredPosition.x,
            Mathf.Clamp(0f, _bottom + pHalf, _top - pHalf));

        greenZone.anchoredPosition = new Vector2(greenZone.anchoredPosition.x,
            Mathf.Clamp(0f, _bottom + gHalf, _top - gHalf));
    }

    void Update()
    {
        if (!_playing) return;

        MoveGreenZone();
        MovePlayerWithSpace();
        UpdateProgress();
        CheckEnd();

        if (Input.GetKey(KeyCode.Space))
            Debug.Log("SPACE HELD");
    }

    void MovePlayerWithSpace()
    {
        //Space bar hold to go up
        if (Input.GetMouseButton(0))
            _vel += upForce * Time.deltaTime;

        // gravity pulls down
        _vel -= gravity * Time.deltaTime;
        _vel = Mathf.Clamp(_vel, -maxVelocity, maxVelocity);

        float newY = playerShaker.anchoredPosition.y + _vel * Time.deltaTime;

        float pHalf = playerShaker.rect.height * 0.5f;
        newY = Mathf.Clamp(newY, _bottom + pHalf, _top - pHalf);

        // stop jitter on bounds
        if (newY <= _bottom + pHalf || newY >= _top - pHalf)
            _vel = 0f;

        playerShaker.anchoredPosition = new Vector2(playerShaker.anchoredPosition.x, newY);
    }

    void MoveGreenZone()
    {
        float t = Time.time * greenWiggle;
        float noise = Mathf.PerlinNoise(t, 0.133f);
        float targetY = Mathf.Lerp(_bottom, _top, noise);

        float gHalf = greenZone.rect.height * 0.5f;
        targetY = Mathf.Clamp(targetY, _bottom + gHalf, _top - gHalf);

        float newY = Mathf.MoveTowards(greenZone.anchoredPosition.y, targetY, greenSpeed * Time.deltaTime);
        greenZone.anchoredPosition = new Vector2(greenZone.anchoredPosition.x, newY);
    }

    void UpdateProgress()
    {
        float pY = playerShaker.anchoredPosition.y;
        float gY = greenZone.anchoredPosition.y;
        float gHalf = greenZone.rect.height * 0.5f;

        bool inside = (pY >= gY - gHalf) && (pY <= gY + gHalf);

        if (inside) _progress += fillRate * Time.deltaTime;
        else _progress -= drainRate * Time.deltaTime;

        _progress = Mathf.Clamp01(_progress);
        UpdateProgressUI();
    }

    void UpdateProgressUI()
    {
        if (meterFill != null)
            meterFill.fillAmount = _progress;
    }

    void CheckEnd()
    {
        if (_progress >= 1f) Win();
        else if (_progress <= 0f) Fail();
    }


    void Win()
    {
        _playing = false;
        uiRoot.SetActive(false);
        UIManager.Instance?.ExitMinigameMode();
        ResultBannerUI.Instance?.ShowSuccess();
        Shaker.Current?.OnMinigameWin();
    }


    void Fail()
    {
        _playing = false;
        uiRoot.SetActive(false);
        UIManager.Instance?.ExitMinigameMode();
        ResultBannerUI.Instance?.ShowFail();
        Shaker.Current?.OnMinigameFail();
    }
}