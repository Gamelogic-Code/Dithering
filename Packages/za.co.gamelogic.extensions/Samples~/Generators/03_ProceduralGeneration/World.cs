using System;
using System.Collections;
using Gamelogic.Extensions;
using Gamelogic.Extensions.Algorithms;
using Gamelogic.Extensions.Support;
using UnityEngine;

public class World : GLMonoBehaviour
{
	[Serializable]
	public class Season
	{
		public Color sky;
		public Color sun;
		public Color river;
	}
	
	[SerializeField, ValidateNotNull] private Camera mainCamera = null;
	[SerializeField, ValidateNotNull] private SpriteRenderer sun = null;
	[SerializeField, ValidateNotNull] private SpriteRenderer river = null;
	[SerializeField, ValidateNotNull] private SpriteRenderer[] atmosphereLayer = null;
	[SerializeField, ValidateNotNull] private TreeLayer[] treeLayers = null;

	[Header("Seasons")]
	[SerializeField] private Season summer = new Season
	{
		sky = Branding.Azure,
		sun = Branding.Lemon,
		river = Branding.Azure
	};
	
	[SerializeField] private Season autumn = new Season
	{
		sky = Branding.Coral,
		sun = Branding.Carrot,
		river = Branding.Coral
	};
	
	[SerializeField] private Season winter = new Season
	{
		sky = Branding.Indigo,
		sun = Color.white,
		river = Branding.Aqua
	};
	
	[SerializeField] private Season spring = new Season
	{
		sky = Branding.Azure,
		sun = Branding.Lemon,
		river = Branding.Azure
	};
	
	[Space]
	[SerializeField] private int sampleCount = 100;
	
	private Season[] seasons;
	private IGenerator<float> seasonGenerator;

	public void Awake()
	{
		seasons = new[] {summer, autumn, winter, spring};
		seasonGenerator = Generator.OpenSawTooth(sampleCount);
	}
	
	public void Start() => StartCoroutine(SlowFixedUpdate());
	
	/*	Generators are not sampled based on time, so we use a coroutine to update the season
		in consistent time intervals, to make it more or less framerate independent.
	*/
	private IEnumerator SlowFixedUpdate()
	{
		while (Application.isPlaying)
		{
			float seasonValue = seasonGenerator.Next() * 4;
			UpdateSeason(seasonValue);

			yield return new WaitForSeconds(0.1f);
		}
	}

	public void UpdateSeason(float seasonValue)
	{
		float frac = GLMathf.Frac(seasonValue);
		int seasonIndex = Mathf.FloorToInt(seasonValue);
		int nextSeasonIndex = (seasonIndex + 1) % 4;
		var season0 = seasons[seasonIndex];
		var season1 = seasons[nextSeasonIndex];
		var backgroundColor = Color.Lerp(season0.sky, season1.sky, frac);
		mainCamera.backgroundColor = backgroundColor;

		foreach (var layer in atmosphereLayer)
		{
			layer.color = backgroundColor.WithAlpha(0.7f);
		}
		
		sun.color = Color.Lerp(season0.sun, season1.sun, frac);
		river.color = Color.Lerp(season0.river, season1.river, frac);

		foreach (var treeLayer in treeLayers)
		{
			treeLayer.SetSeason(seasonValue);
		}
	}
}
