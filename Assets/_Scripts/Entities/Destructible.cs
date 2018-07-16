using UnityEngine;

namespace Asteroids
{
  [RequireComponent(typeof(PolygonCollider2D))]
  [RequireComponent(typeof(Rigidbody2D))]
  public class Destructible: MonoBehaviour
  {

    [SerializeField]
    private int _life;
    [SerializeField]
    private int _points;
    [SerializeField]
    private Vector2Int _screenSize = new Vector2Int(0, 0);

    private PolygonCollider2D _collider2D;
    private Rigidbody2D _rigidbody2D;

    public int Life
    {
      get { return _life; }
      set { _life = value; }
    }

    public int Points
    {
      get { return _points; }
      private set { _points = value; }
    }

    public Vector2Int ScreenSize
    {
      get { return _screenSize; }
    }

    public bool Dead
    {
      get { return _life <= 0; }
    }

    public PolygonCollider2D Collider
    {
      get { return _collider2D; }
    }

    public Rigidbody2D Rigidbody
    {
      get { return _rigidbody2D; }
    }

    private void Awake()
    {
      _collider2D = GetComponent<PolygonCollider2D>();
      _rigidbody2D = GetComponent<Rigidbody2D>();
    }

  }
}