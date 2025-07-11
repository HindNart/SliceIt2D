using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI targetRatioText;
    [SerializeField] private TextMeshProUGUI cutRatioText;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private GameObject perfect;
    [SerializeField] private GameObject good;

    [Header("Tutorial Panel")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private Button tutorialButton;

    [Header("Pause Panel")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private Sprite resumeSprite;
    [SerializeField] private Sprite pauseSprite;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private RectTransform pausePanelRect;
    [SerializeField] private Button sfxButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button homeButton;


    [Header("Game Over Panel")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private RectTransform gameOverPanelRect;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    [SerializeField] private TextMeshProUGUI gameOverBestScoreText;
    [SerializeField] private Button restartButton;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        tutorialPanel.SetActive(true);
        nextLevelButton.gameObject.SetActive(false);

        perfect.SetActive(false);
        good.SetActive(false);

        gameOverPanel.SetActive(false);
        gameOverPanelRect.localScale = Vector3.zero;

        pausePanel.SetActive(false);
        pausePanelRect.localScale = Vector3.zero;

        if (tutorialButton != null)
        {
            tutorialButton.onClick.AddListener(() => UIEffect.AnimateHide(tutorialPanel));
            UIEffect.SetupButtonAnimation(tutorialButton);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(() => GameManager.Instance.RestartGame());
            UIEffect.SetupButtonAnimation(restartButton);
        }

        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.AddListener(() => GameManager.Instance.NextLevel());
            UIEffect.SetupButtonAnimation(nextLevelButton);
        }

        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(() => GameManager.Instance.PauseGame());
            UIEffect.SetupButtonAnimation(pauseButton);
        }

        if (sfxButton != null)
        {
            sfxButton.onClick.AddListener(() => AudioManager.Instance.ToggleSFX());
            UIEffect.SetupButtonAnimation(sfxButton);
        }

        if (musicButton != null)
        {
            musicButton.onClick.AddListener(() => AudioManager.Instance.ToggleMusic());
            UIEffect.SetupButtonAnimation(musicButton);
        }

        if (homeButton != null)
        {
            homeButton.onClick.AddListener(() => GameManager.Instance.GoToHome());
            UIEffect.SetupButtonAnimation(homeButton);
        }
    }

    public void UpdateTargetRatioUI(float targetRatio)
    {
        targetRatioText.text = $"{targetRatio * 100:F0}%";
        UIEffect.AnimateShow(targetRatioText.gameObject);
    }

    public void AddScoreEffect(float currentScore, float targetScore, float countDuration)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("ScoreUpdate");
        }

        DOTween.To(() => currentScore, x => currentScore = x, targetScore, countDuration)
            .SetEase(Ease.InOutQuad)
            .OnUpdate(() => UpdateScoreUI(currentScore));
    }

    public void UpdateScoreUI(float score)
    {
        scoreText.text = $"{score:F0}";
        // UIEffect.AnimateText(scoreText);
    }

    public IEnumerator UpdateCutRatioUI(float cutRatio, float targetRatio)
    {
        yield return new WaitForSeconds(0.5f);
        cutRatio = Mathf.Clamp(cutRatio, 0f, 1f);
        cutRatioText.color = Mathf.Abs(cutRatio - GameManager.Instance.TargetRatio) <= 0.1f ? Color.green : Color.red;
        cutRatioText.text = $"{cutRatio * 100:F2}%";
        UIEffect.AnimateShow(cutRatioText.gameObject);

        yield return new WaitForSeconds(0.5f);
        if (Mathf.Abs(cutRatio - targetRatio) <= 0.01f)
        {
            if (!perfect.activeSelf)
            {
                // perfect.transform.localScale = Vector3.zero;
                UIEffect.AnimateShow(perfect);
                // perfect.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
            }
        }
        else if (Mathf.Abs(cutRatio - targetRatio) <= 0.03f)
        {
            if (!good.activeSelf)
            {
                // good.transform.localScale = Vector3.zero;
                UIEffect.AnimateShow(good);
                // good.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
            }
        }

        yield return new WaitForSeconds(2f);
        // cutRatioText.gameObject.SetActive(false);
        UIEffect.AnimateHide(cutRatioText.gameObject);
        UIEffect.AnimateHide(perfect);
        UIEffect.AnimateHide(good);
    }

    public void ShowNextLevelBtn()
    {
        nextLevelButton.gameObject.SetActive(true);
        nextLevelButton.transform.localScale = Vector3.zero;
        nextLevelButton.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);
    }

    public void HideNextLevelBtn()
    {
        nextLevelButton.transform.DOScale(0f, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            nextLevelButton.gameObject.SetActive(false);
        });
    }

    public void ShowGameOver(float finalScore)
    {
        gameOverPanel.SetActive(true);
        gameOverPanelRect.localScale = Vector3.zero;
        gameOverPanelRect.DOScale(1f, 0.5f).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            GameOverUIEffect(finalScore, 1.5f);
        });

        // AdManager.Instance.ShowInterstitialAd();
    }

    public void HideGameOver()
    {
        gameOverPanelRect.DOScale(0f, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            gameOverPanel.SetActive(false);
        });
    }

    private void GameOverUIEffect(float finalScore, float countDuration)
    {
        float currentScore = 0f, bestScore = 0f;
        DOTween.To(() => currentScore, x => currentScore = x, finalScore, countDuration)
            .SetEase(Ease.InOutQuad)
            .OnUpdate(() => UpdateGameOverUI(currentScore, PlayerPrefs.GetFloat("BestScore", 0f)));

        DOTween.To(() => bestScore, x => bestScore = x, PlayerPrefs.GetFloat("BestScore", 0f), countDuration)
            .SetEase(Ease.InOutQuad)
            .OnUpdate(() => UpdateGameOverUI(currentScore, bestScore));
    }

    public void UpdateGameOverUI(float finalScore, float bestScore)
    {
        gameOverScoreText.text = $"{finalScore:F0}";
        gameOverBestScoreText.text = $"{bestScore:F0}";
    }

    public void ShowPausePanel()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("PopupOpen");
        }

        pauseButton.image.sprite = pauseSprite;
        pausePanel.SetActive(true);
        pausePanelRect.localScale = Vector3.zero;
        pausePanelRect.DOScale(1f, 0f).SetEase(Ease.OutBounce);
    }

    public void HidePausePanel()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("PopupClose");
        }

        pauseButton.image.sprite = resumeSprite;
        pausePanelRect.DOScale(0f, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            pausePanel.SetActive(false);
        });
    }

    private void OnDestroy()
    {
        restartButton.onClick.RemoveAllListeners();
        nextLevelButton.onClick.RemoveAllListeners();
        pauseButton.onClick.RemoveAllListeners();
        sfxButton.onClick.RemoveAllListeners();
        musicButton.onClick.RemoveAllListeners();
        homeButton.onClick.RemoveAllListeners();
        Instance = null;
        DOTween.KillAll();
    }
}
