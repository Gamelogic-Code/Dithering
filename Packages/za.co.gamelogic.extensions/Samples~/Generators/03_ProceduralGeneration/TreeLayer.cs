using System.Linq;
using Gamelogic.Extensions;
using Gamelogic.Extensions.Algorithms;
using Gamelogic.Extensions.Support;
using UnityEngine;
using UnityEngine.Serialization;
using Tree = Gamelogic.Extensions.Tree;

public class TreeLayer : MonoBehaviour
{
	[SerializeField, ValidateNotNull]
	private Tree treePrefab = null;
	
	[SerializeField] private float screenEdgeFromCenter = 1200;
	[FormerlySerializedAs("treeLevel")] [SerializeField] private float treePositionY = -76;
	
	[MinMaxRange(0, 100)]
	[SerializeField] private MinMaxFloat treeWidthRange = new MinMaxFloat(16, 64);
	
	[MinMaxRange(0, 300)]
	[SerializeField] private MinMaxFloat treeHeightRange = new MinMaxFloat(60, 240);
	
	[SerializeField] private int treesInSceneCount = 100;
	[SerializeField] private int treesInPoolCount = 200;
	[SerializeField] private float speed = 50f;
	
	private IPool<Tree> trees;
	private IGenerator<float> treeWidthGenerator;
	private IGenerator<float> treeHeightGenerator;
	
	private static readonly IGenerator<float> Random = Generator.UniformRandomFloat();

	private IGenerator<Color>[] colorGenerators;
	private float season;

	readonly Color[] summerColors = new[]
	{
		Branding.Apple,
		Branding.Lemon,
		Branding.Azure,
		Branding.Indigo,
		Branding.AppleDark,
		Branding.AppleLight,
	};

	readonly Color[] autumnColors = new[]
	{
		Branding.Coral,
		Branding.Carrot,
		Branding.Lemon,
		Branding.CoralDark,
		Branding.CarrotDark,
		Branding.LemonDark,
	};

	readonly Color[] winterColors = new[]
	{
		Branding.Azure,
		Branding.Indigo,
		Branding.Coral,
		Color.white,
		Branding.Aqua,
		Branding.AquaLight,
		Branding.AzureLight,
		Branding.IndigoLight,
	};

	readonly Color[] springColors = new[]
	{
		Branding.Apple,
		Branding.AppleDark,
		Branding.Coral,
		Branding.Lemon,
		Branding.CoralLight,
		Branding.CarrotLight,
		Branding.LemonLight,
	};
	
	private Color[][] colors;

	public void Start()
	{
		colors = new[] {summerColors, autumnColors, winterColors, springColors};
		InitializeGenerators();
		
		trees = new HashPool<Tree>(
			treesInPoolCount,
			CreateTree,
			null,
			ActivateTree,
			DeactivateTree);
		
		InitScene();

		

	}
	
	public IGenerator<Color> GetColorGenerator(Color[] colorSet)
	{
		var indexGenerator = Generator.UniformRandomInt(colorSet.Length);
		var mixGenerator = Generator.UniformRandomFloat();
		var colorsGenerator = Generator
			.Choose(colorSet, indexGenerator)
			.Group(2);
		
		return Generator.Combine(
			colorsGenerator,
			mixGenerator,
			(group, mix) => Color.Lerp(group[0], group[1], mix));
	}
	

	public void InitializeGenerators()
	{
		colorGenerators = colors
			.Select(GetColorGenerator)
			.ToArray();
		
		treeWidthGenerator = Generator
			.UniformRandomFloat()
			.Select(t => treeWidthRange.Lerp(t));
		
		treeHeightGenerator = Generator
			.UniformRandomFloat()
			.Select(t => treeHeightRange.Lerp(t));
	}

	public void InitScene()
	{
		float interval = screenEdgeFromCenter * 2 / treesInSceneCount;
		
		for(int i = 0; i < treesInSceneCount; i++)
		{
			var tree = trees.Get();
			tree.transform.SetX(-screenEdgeFromCenter + interval * i);
		}
	}
	
	private Tree CreateTree()
	{
		var tree = Instantiate(treePrefab, transform);
		tree.transform.SetLocalZ(Random.Next());
		tree.MovingSceneObject.speed = speed;
		var movingObject = tree.MovingSceneObject;
		movingObject.LeftEdge = -screenEdgeFromCenter;
		movingObject.OnOutOfBounds += () =>
		{
			trees.Release(tree);
			trees.Get().transform.SetX(screenEdgeFromCenter);
		};
		
		return tree;
	}

	private void ActivateTree(Tree tree)
	{
		int index = GetIndex(season);
		tree.SetProperties(colorGenerators[index].Next(), treeWidthGenerator.Next(), treeHeightGenerator.Next(), treePositionY);
		tree.transform.SetLocalX(screenEdgeFromCenter);
		tree.gameObject.SetActive(true);
		
		Debug.Log("ActivateTree");
	}
	
	private void DeactivateTree(Tree tree)
	{
		tree.gameObject.SetActive(false);
	}

	public void SetSeason(float newSeason)
	{
		season = newSeason;
	}

	private static int GetIndex(float season)
	{
		float frac = GLMathf.Frac(season);
		int index = Mathf.FloorToInt(season);

		return Random.Next() > frac ? index : (index + 1) % 4;
	}
}
