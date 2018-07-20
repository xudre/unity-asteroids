using System;
using UnityEngine;
using UnityEngine.UI;

namespace Asteroids
{
  public class UIManager : MonoBehaviour
  {

    [SerializeField]
    private Text _lives;
    [SerializeField]
    private Text _level;
    [SerializeField]
    private Text _score;
    [SerializeField]
    private RectTransform _warpBar;

    private float _updateCountdown;
    private float _updateInterval = .5f;

    private void LateUpdate () {
		  UpdateUserInterface();
    }

    private void UpdateUserInterface()
    {
      _updateCountdown -= Time.deltaTime;

      if (_warpBar != null)
        _warpBar.sizeDelta = new Vector2(100 * LevelManager.Instance.WarpReady, _warpBar.sizeDelta.y);

      if (_updateCountdown > 0)
        return;
      
      if (_lives != null)
        _lives.text = String.Format("{0:N0}", LevelManager.Instance.Player.Life);

      if (_level != null)
        _level.text = String.Format("Level {0:N0}", LevelManager.Instance.Level);

      if (_score != null)
        _score.text = String.Format("{0:N0}", GameManager.Instance.Score);

      _updateCountdown = _updateInterval;
    }

  }
}
