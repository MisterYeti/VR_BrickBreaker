using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    private GameManager _gameManager;

    [SerializeField] private GameObject _canvases;

    [Header("Scoring")]
    [SerializeField] private TextMeshProUGUI _totalScoreText;
    [SerializeField] private float _scoringAnimationTime = 1.0f;
    [SerializeField] private GameObject _addedScorePrefab;
    [SerializeField] private RectTransform _rectTransformScores;

    [Header("Level")]
    [SerializeField] private GameObject _levelUICanvas;
    [SerializeField] private TextMeshProUGUI _levelName;
    [SerializeField] private float _animationTextDuration;

    [Header("Pausing")]
    [SerializeField] private Button _pauseButton;
    [SerializeField] private GameObject _pauseCanvas;
    [SerializeField] private Button _resumeButton;

    [Header("Losing")]
    [SerializeField] private GameObject _loseCanvas;

    [Header("Winning")]
    [SerializeField] private GameObject _winCanvas;
    [SerializeField] private Button _nextButton;

    [Header("Generics")]
    [SerializeField] private List<Button> _quitButtons, _retryButtons;

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _canvases.SetActive(false);
        _pauseButton.onClick.AddListener(SetGamePausing);
        _resumeButton.onClick.AddListener(SetGameResume);
        _nextButton.onClick.AddListener(SetNextLevel);
        foreach (Button retryBtn in _retryButtons)
        {
            retryBtn.onClick.AddListener(SetRetryLevel);
        }
        foreach (Button quitBtn in _quitButtons)
        {
            quitBtn.onClick.AddListener(QuitLevel);
        }
    }


    private void SetGamePausing()
    {
        _gameManager.ChangeState(GameState.Pausing);
        _pauseCanvas.SetActive(true);
    }
    
    private void SetGameResume()
    {
        _gameManager.ChangeState(GameState.Playing);
        _pauseCanvas.SetActive(false);
    }

    private void SetNextLevel()
    {
        LevelManager.Instance.SetNextLevel();
        _gameManager.ChangeState(GameState.LoadingLevel);
        _winCanvas.SetActive(false);
    }

    private void SetRetryLevel()
    {
        _gameManager.ChangeState(GameState.LoadingLevel);
        _loseCanvas.SetActive(false);
    }

    private void QuitLevel()
    {
        Helper.QuiApplication();
    }

    public void SetScoreText(string strTotalScore, string strAddedScore, Color color)
    {
        StopAllCoroutines();
        IntantiateAddedScore(strAddedScore, color);
        StartCoroutine(IncreaseScoreCoroutine(float.Parse(_totalScoreText.text), float.Parse(strTotalScore)));         
    }

    public IEnumerator ShowLevel(string levelName)
    {
        _levelName.text = "Level " + levelName;
        _levelUICanvas.SetActive(true);
        yield return AnimateTextLevel(_animationTextDuration);
        _levelUICanvas.SetActive(false);
    }

    private IEnumerator AnimateTextLevel(float duration)
    {
        RectTransform rect = _levelName.GetComponent<RectTransform>();
        float currentTime = 0.0f;
        float rot_y = 0.0f;
        yield return new WaitForSeconds(duration);
        int nbTime = 0;
        while (nbTime < 2)
        {
            currentTime = 0.0f;
            while (currentTime < duration/2)
            {
                currentTime += Time.deltaTime;
                rect.localEulerAngles = new Vector3(0.0f, Mathf.Lerp(rot_y, -180.0f, currentTime / (duration/2)));
                yield return null;
            }
            currentTime = 0.0f;
            while (currentTime < duration/2)
            {
                currentTime += Time.deltaTime;
                rect.localEulerAngles = new Vector3(0.0f, Mathf.Lerp(-180.0f, rot_y, currentTime / (duration / 2)));
                yield return null;
            }
            nbTime++;
        }
        yield return new WaitForSeconds(duration);

    }

    public void SetWinningUI()
    {
        _nextButton.gameObject.SetActive(true);
        _winCanvas.SetActive(true);
        if (LevelManager.Instance.HasNextLevel() == false)
        {
            _nextButton.gameObject.SetActive(false);
        }
    }

    public void SetLosingUI()
    {
        _loseCanvas.SetActive(true);
    }

    public void ResetUI()
    {
        _pauseCanvas.SetActive(false);
        _loseCanvas.SetActive(false);
        _winCanvas.SetActive(false);
        _levelUICanvas.SetActive(false);
        _totalScoreText.text = "0000";
        Helper.DestroyChildren(_rectTransformScores.transform);
    }

    private IEnumerator IncreaseScoreCoroutine(float strFrom, float strTo)
    {
        float currentTime = 0.0f;

        while (currentTime < _scoringAnimationTime)
        {
            currentTime += Time.deltaTime;
            _totalScoreText.text = ((int)Mathf.Lerp(strFrom, strTo, currentTime / _scoringAnimationTime)).ToString();
            yield return null;
        }
    }

    private void IntantiateAddedScore(string strAddedScore, Color color)
    {
        GameObject addedScore = Instantiate(_addedScorePrefab);
        addedScore.transform.parent = _rectTransformScores.transform;
        addedScore.transform.localScale = Vector3.one;
        addedScore.transform.localPosition = new Vector3(Random.Range(_rectTransformScores.rect.xMin, _rectTransformScores.rect.xMax),
                   Random.Range(_rectTransformScores.rect.yMin, _rectTransformScores.rect.yMax));
        addedScore.GetComponent<AddedScoreText>().PlayAnimation(strAddedScore,color);
    }

    public void SetActiveUI(bool on)
    {
        _canvases.SetActive(on);
    }

}
