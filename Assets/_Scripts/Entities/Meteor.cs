using UnityEngine;
using Random = System.Random;

namespace Asteroids
{
  public class Meteor : Destructible
  {

    private static Random _random;

    [SerializeField]
    private Sprite[] _variations;

    private SpriteRenderer _renderer;

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

      OnDead();
    }

  }
}
