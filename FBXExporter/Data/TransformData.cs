using Newtonsoft.Json;

namespace ANYTY.FBXExporter.Data
{
    [System.Serializable]
    public sealed class TransformData
    {
		[JsonProperty("position")]
		public Vector3 Position { get; set; } = new();
		[JsonProperty("rotation")]
		public Quaternion Rotation { get; set; } = new();
		[JsonProperty("scale")]
		public Vector3 Scale { get; set; } = new();

		public TransformData() { }

		public TransformData(Vector3 position, Quaternion rotation, Vector3 scale)
		{
			Position = position;
			Rotation = rotation;
			Scale = scale;
		}

		public TransformData(UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, UnityEngine.Vector3 scale)
		{
			Position = new Vector3(position.x, position.y, position.z);
			Rotation = new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w);
			Scale = new Vector3(scale.x, scale.y, scale.z);
		}

		[System.Serializable]
		public class Vector3
		{
			[JsonProperty("x")]
			public float X { get; set; } = 0.0f;
			[JsonProperty("y")]
			public float Y { get; set; } = 0.0f;
			[JsonProperty("z")]
			public float Z { get; set; } = 0.0f;

			[JsonIgnore]
			public UnityEngine.Vector3 UnityVector =>
				new UnityEngine.Vector3(X, Y, Z);

			public Vector3() { }

			public Vector3(float x, float y, float z)
			{
				X = x;
				Y = y;
				Z = z;
			}
		}

		[System.Serializable]
		public class Quaternion
		{
			[JsonProperty("x")]
			public float X { get; set; } = 0.0f;
			[JsonProperty("y")]
			public float Y { get; set; } = 0.0f;
			[JsonProperty("z")]
			public float Z { get; set; } = 0.0f;
			[JsonProperty("w")]
			public float W { get; set; } = 1.0f;

			[JsonIgnore]
			public UnityEngine.Quaternion UnityQuaternion =>
				new UnityEngine.Quaternion(X, Y, Z, W);

			public Quaternion() { }

			public Quaternion(float x, float y, float z, float w)
			{
				X = x;
				Y = y;
				Z = z;
				W = w;
			}
		}
	}
}