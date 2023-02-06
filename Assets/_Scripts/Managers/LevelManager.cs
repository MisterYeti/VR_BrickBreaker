using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] List<ScriptableLevel> _levels = null;
    private ScriptableLevel _currentLevel = null;

    [SerializeField] private GameObject _ballPrefab;

    [Header("Level instantiation Data")]
    [SerializeField] Transform _bricksParent;
    [SerializeField] Transform _ballParent;
    [SerializeField] Transform _initialBrickPosition;
    [SerializeField] float x_spacing = +0.22f, y_spacing = -0.125f;

    [Header("Level reset")]
    [SerializeField] private float destroyBrickDuration = 0.1f;

    private List<Brick> _brickList = new List<Brick>();
    private GameObject _ball;
    private int _bricksToDestroy = 0;
    private int _bricksDestroyed = 0;

    public IEnumerator InstantiateLevel()
    {
        yield return ResetLevel();

        if (_currentLevel == null)
        {
            _currentLevel = _levels[0];
        }

        Vector3 brickPos = _initialBrickPosition.transform.localPosition;
        float initX = brickPos.x;       

        foreach (var row in _currentLevel.Bricks.rows)
        {
            foreach (var brick in row.row)
            {
                if (brick != null && brick.PrefabObject != null)
                {
                    GameObject brickObject = Instantiate(brick.PrefabObject, _bricksParent);
                    brickObject.transform.localPosition = brickPos;
                    brickObject.name = brick.name;
                    Brick newBrick = brickObject.GetComponent<Brick>();
                    _brickList.Add(newBrick);
                    if (brick.BrickEffect != BrickEffect.Invincible)
                    {
                        _bricksToDestroy++;
                    }
                    //Popup the brick
                    yield return newBrick.Setup(brick);
                }
                brickPos = new Vector3(brickPos.x + x_spacing, brickPos.y);
            }

            brickPos = new Vector3(initX, brickPos.y + y_spacing);
        }
        GameObject ballObject = Instantiate(_ballPrefab, _ballParent).gameObject;
        ballObject.name = "Ball";
        Ball ball = ballObject.GetComponent<Ball>();
        ball.SetSpeed(_currentLevel.LevelDifficulty.speed);
        _ball = ballObject;
        PaddleManager.Instance.Ball = ball;
    }

    public IEnumerator ShowCurrentLevel()
    {
        yield return UIManager.Instance.ShowLevel(_currentLevel.LevelDifficulty.level.ToString());
    }

    public void SetNextLevel()
    {
        if (HasNextLevel())
        {
            _currentLevel = _levels[_levels.IndexOf(_currentLevel) + 1];
        }
    }

    public bool HasNextLevel()
    {
        return _levels.IndexOf(_currentLevel) < _levels.Count - 1;
    }

    public void AddDestroyedBrick()
    {
        _bricksDestroyed += 1;
        CheckEndGame();
    }

    private void CheckEndGame()
    {
        if (_bricksDestroyed >= _bricksToDestroy)
        {
            GameManager.Instance.ChangeState(GameState.Win);
            Destroy(_ball);
        }
    }

    private IEnumerator ResetLevel()
    {
        Destroy(_ball);
        foreach (Brick brick in _brickList)
        {
            if (brick != null)
            {
                yield return brick.Destroy(destroyBrickDuration, false);
            }
        }
        _brickList.Clear();
        _bricksDestroyed = 0;
        _bricksToDestroy = 0;
    }

}
