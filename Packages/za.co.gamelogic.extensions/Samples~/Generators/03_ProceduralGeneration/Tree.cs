using UnityEngine;

namespace Gamelogic.Extensions
{
	[RequireComponent(typeof(SpriteRenderer))]
	[RequireComponent(typeof(MovingSceneObject))]
	public class Tree : GLMonoBehaviour
	{
		private SpriteRenderer spriteRenderer;
		private float baseHeight;
	
		public MovingSceneObject MovingSceneObject => GetRequiredComponent<MovingSceneObject>();

		public void Awake()
		{
			spriteRenderer = GetRequiredComponent<SpriteRenderer>();
			baseHeight = spriteRenderer.transform.localScale.y;
		}

		public void SetProperties(Color color, float width, float height, float yBasePosition)
		{
			spriteRenderer.color = color;
			transform.SetScaleXY(width, height);
			transform.SetLocalY(yBasePosition - (baseHeight - height)/2 * 5);
			Debug.Log(transform.position.y);
			
		}
	}
}
