using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

using Gamelogic.Extensions.Algorithms;

namespace Gamelogic.Extensions.Samples
{
	public class ResponseCurveMain : GLMonoBehaviour
	{
		[Header("Response Curve Points")]
		[SerializeField] private float[] inputs = null;
		[SerializeField] private float[] floatOutputs = null;
		[SerializeField] private Vector3[] vector3Outputs = null;
		[SerializeField] private Color[] colorOutputs = null;

		[Header("UI")]
		[SerializeField] private Text floatText = null;
		[SerializeField] private Image colorSwatch = null;
		[SerializeField] private GameObject objectToMove = null;

		private IResponseCurve<string> floatTextCurve;
		private IResponseCurve<Vector3> vector3Curve;
		private IResponseCurve<Color> colorCurve;

		public void Start()
		{
			Assert.AreEqual(inputs.Length, floatOutputs.Length);
			Assert.AreEqual(inputs.Length, vector3Outputs.Length);
			Assert.AreEqual(inputs.Length, colorOutputs.Length);

			floatTextCurve = new ResponseCurveFloat(inputs, floatOutputs).Select(x => x.ToString(CultureInfo.InvariantCulture));
			colorCurve = new ResponseCurveColor(inputs, colorOutputs);
			vector3Curve = new ResponseCurveVector3(inputs, vector3Outputs);

			OnSliderChange(0);
		}

		public void OnSliderChange(float value)
		{
			floatText.text = floatTextCurve[value];
			colorSwatch.color = colorCurve[value];
			objectToMove.transform.position = vector3Curve[value];
		}
	}
}
