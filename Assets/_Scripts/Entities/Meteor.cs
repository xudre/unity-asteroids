using UnityEngine;
using Random = System.Random;

namespace Asteroids
{
  public class Meteor : Destructible
  {

    private static Random _random;

    [SerializeField]
    private Sprite[] _variations;
    [SerializeField]
    private Meteor[] _debris;

    private SpriteRenderer _renderer;

    public Meteor[] Debris
    {
      get { return _debris; }
    }

    private void Start()
    {
      _renderer = GetComponent<SpriteRenderer>();

      if (_variations == null)
        return;

      if (_random == null)
        _random = new Random();

      _renderer.sprite = _variations[_random.Next(0, _variations.Length)];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
      if (!gameObject.activeSelf || Dead || !other.tag.Equals("PlayerBullet"))
        return;

      Life--;

      other.gameObject.SetActive(false);

      if (!Dead)
        return;

      LevelManager.Instance.AddMeteorDebris(this, other);

      OnDead();
    }

  }
}
