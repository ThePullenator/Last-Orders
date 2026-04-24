using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResultBannerUI : MonoBehaviour
{
    public static ResultBannerUI Instance;
    [SerializeField] private RectTransform banner;
    [SerializeField] private Image bannerImage;
    [SerializeField] private Sprite successSprite;
    [SerializeField] private Sprite failSprite;
    [SerializeField] private float hiddenY = 500f;
    [SerializeField] private float shownY = -120f;
    [SerializeField] private float slideTime = 0.35f;
    [SerializeField] private float stayTime = 2f;
    bool playing;

    void Awake()
    {
        Instance = this;
        banner.anchoredPosition = new Vector2(banner.anchoredPosition.x, hiddenY);
       bannerImage.enabled = false;
    }

    public void ShowSuccess()
    {
        Show(successSprite);
    }

    public void ShowFail()
    {
        Show(failSprite);
    }
    Coroutine currentRoutine;

    void Show(Sprite sprite)
    {


        // Stop old banner if it is still running
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        playing = false;
        bannerImage.sprite = sprite;
        bannerImage.enabled = true;
        currentRoutine = StartCoroutine(SlideRoutine());

        IEnumerator SlideRoutine()
        {
            playing = true;
            FreezePlayer();
            banner.anchoredPosition = new Vector2(banner.anchoredPosition.x, hiddenY);
            yield return SlideTo(shownY);
            yield return new WaitForSeconds(stayTime);
            yield return SlideTo(hiddenY);
            bannerImage.enabled = false;
            UnfreezePlayer();
            playing = false;
            currentRoutine = null;

        }

        IEnumerator SlideTo(float targetY)
        {
            float startY = banner.anchoredPosition.y;
            float timer = 0f;

            while (timer < slideTime)
            {
                timer += Time.deltaTime;
                float t = timer / slideTime;
                float y = Mathf.Lerp(startY, targetY, t);
                banner.anchoredPosition = new Vector2(banner.anchoredPosition.x, y);
                yield return null;
            }

            banner.anchoredPosition = new Vector2(banner.anchoredPosition.x, targetY);
        }
        void FreezePlayer()
        {
            PlayerManager.FirstPersonController.enabled = false;
            PlayerManager.PlayerLook.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        void UnfreezePlayer()
        {
            PlayerManager.FirstPersonController.enabled = true;
            PlayerManager.PlayerLook.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}