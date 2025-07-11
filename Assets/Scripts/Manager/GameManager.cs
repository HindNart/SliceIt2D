using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int CutAttempts => cutAttempts;
    public float TargetRatio => targetRatio;
    private float currentScore = 0;
    private float targetScore = 0;
    private int currentLevel;
    private float targetRatio;
    private int cutAttempts = 1;
    private bool isPaused = false;

    public UnityEvent OnNextLevelEvent;

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
        }

        CutEventManager.OnCutPerformed += OnCutPerformed;
    }

    private void OnDestroy()
    {
        CutEventManager.OnCutPerformed -= OnCutPerformed;
        OnNextLevelEvent?.RemoveAllListeners();
    }

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM();
        }

        InitializeLevel(1);
        UIManager.Instance.UpdateScoreUI(currentScore);
    }

    public void InitializeLevel(int level)
    {
        Time.timeScale = 1f;
        currentLevel = level;
        cutAttempts = 1;
        targetRatio = CalculateTargetRatio(level);
        ShapeFactory.Instance.DestroyShape();
        ShapeFactory.Instance.CreateShape();
        UIManager.Instance.UpdateTargetRatioUI(targetRatio);
        OnNextLevelEvent?.Invoke();
    }

    private float CalculateTargetRatio(int level)
    {
        return level < 3 ? 0.5f : Random.Range(0.2f, 0.9f);
    }

    public void OnCutPerformed(float actualRatio)
    {
        StartCoroutine(HandleCutPerformed(actualRatio));
    }

    private IEnumerator HandleCutPerformed(float actualRatio)
    {
        yield return UIManager.Instance.UpdateCutRatioUI(actualRatio, targetRatio);
        cutAttempts--;
        bool isSuccess = Mathf.Abs(actualRatio - targetRatio) <= 0.1f;
        if (isSuccess)
        {
            targetScore += 1000 * (0.1f - Mathf.Abs(actualRatio - targetRatio));
            UIManager.Instance.AddScoreEffect(currentScore, targetScore, 1f);
            currentScore = targetScore;
            UIManager.Instance.ShowNextLevelBtn();
        }
        else
        {
            GameOver();
            UIManager.Instance.ShowGameOver(currentScore);
        }
    }

    private void GameOver()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("GameOver");
        }

        if (currentScore > PlayerPrefs.GetFloat("BestScore", 0f))
        {
            PlayerPrefs.SetFloat("BestScore", currentScore);
            PlayerPrefs.Save();
        }
    }

    public void NextLevel()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("ButtonClick");
        }

        UIManager.Instance.HideNextLevelBtn();
        InitializeLevel(currentLevel + 1);
    }

    public void RestartGame()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("ButtonClick");
        }

        UIManager.Instance.HideGameOver();
        // Reset the game state
        currentScore = 0;
        targetScore = 0;
        UIManager.Instance.UpdateScoreUI(currentScore);
        InitializeLevel(1);
    }

    public void PauseGame()
    {
        if (isPaused)
        {
            Time.timeScale = 1f;
            isPaused = false;
            UIManager.Instance.HidePausePanel();
        }
        else
        {
            Time.timeScale = 0f;
            isPaused = true;
            UIManager.Instance.ShowPausePanel();
        }
    }

    public void GoToHome()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("ButtonClick");
        }

        if (Instance != null) Destroy(Instance.gameObject);
        if (AudioManager.Instance != null) Destroy(AudioManager.Instance.gameObject);
        SceneManager.LoadScene("MenuHome");
    }
}