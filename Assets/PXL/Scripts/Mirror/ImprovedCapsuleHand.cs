using System.Collections.Generic;
using Leap;
using Leap.Unity;
using UnityEngine;

namespace PXL.Mirror {

	/// <summary>
	/// Blank copy of <see cref="CapsuleHand"/> but with proper formatting, naming and 
	/// adjusted access modifiers for inheritance in <see cref="MirroredHand"/>
	/// </summary>
	public class ImprovedCapsuleHand : IHandModel {

		protected const int ThumbBaseIndex = (int)Finger.FingerType.TYPE_THUMB * 4;
		private const int PinkyBaseIndex = (int)Finger.FingerType.TYPE_PINKY * 4;

		public float SphereRadius = 0.009f;
		public float CylinderRadius = 0.021f;
		public float CylinderLengthFactor = 4f;

		private const float PalmRadius = 0.015f;

		private static int _leftColorIndex;
		private static int _rightColorIndex;

		private static readonly Color[] LeftColorList = {
			new Color(0.0f, 0.0f, 1.0f), new Color(0.2f, 0.0f, 0.4f),
			new Color(0.0f, 0.2f, 0.2f)
		};

		private static readonly Color[] RightColorList = {
			new Color(1.0f, 0.0f, 0.0f), new Color(1.0f, 1.0f, 0.0f),
			new Color(1.0f, 0.5f, 0.0f)
		};

		[SerializeField]
		private Chirality handedness;

		[SerializeField]
		private bool showArm = true;

		[SerializeField]
		private Material material;

		[SerializeField]
		private Mesh sphereMesh;

		[SerializeField]
		private int cylinderResolution = 12;

		private bool hasGeneratedMeshes;
		private Material jointMat;

		[SerializeField, HideInInspector]
		private List<Transform> serializedTransforms;

		protected Transform[] JointSpheres;
		protected Transform MockThumbJointSphere;
		protected Transform PalmPositionSphere;
		protected Transform WristPositionSphere;

		private List<Renderer> armRenderers;
		private List<Transform> cylinderTransforms;
		private List<Transform> sphereATransforms;
		private List<Transform> sphereBTransforms;

		protected Transform ArmFrontLeft;
		protected Transform ArmFrontRight;
		protected Transform ArmBackLeft;
		protected Transform ArmBackRight;
		protected Hand Hand;

		public override ModelType HandModelType {
			get { return ModelType.Graphics; }
		}

		public override Chirality Handedness {
			get { return handedness; }
		}

		public override bool SupportsEditorPersistence() {
			return true;
		}

		public override Hand GetLeapHand() {
			return Hand;
		}

		public override void SetLeapHand(Hand hand) {
			Hand = hand;
		}

		private void OnValidate() {
			//Resolution must be at least 3!
			cylinderResolution = Mathf.Max(3, cylinderResolution);

			//Update visibility on validate so that the user can toggle in real-time
			if (armRenderers != null) {
				UpdateArmVisibility();
			}
		}

		public override void InitHand() {
			if (material != null) {
				jointMat = new Material(material) { hideFlags = HideFlags.DontSaveInEditor };
			}

			if (serializedTransforms != null) {
				foreach (var obj in serializedTransforms) {
					if (obj != null)
						DestroyImmediate(obj.gameObject);
				}
				serializedTransforms.Clear();
			}
			else {
				serializedTransforms = new List<Transform>();
			}

			JointSpheres = new Transform[4 * 5];
			armRenderers = new List<Renderer>();
			cylinderTransforms = new List<Transform>();
			sphereATransforms = new List<Transform>();
			sphereBTransforms = new List<Transform>();

			CreateSpheres();
			CreateCylinders();

			UpdateArmVisibility();

			hasGeneratedMeshes = false;
		}

		public override void BeginHand() {
			base.BeginHand();

			if (Hand.IsLeft) {
				jointMat.color = LeftColorList[_leftColorIndex];
				_leftColorIndex = (_leftColorIndex + 1) % LeftColorList.Length;
			}
			else {
				jointMat.color = RightColorList[_rightColorIndex];
				_rightColorIndex = (_rightColorIndex + 1) % RightColorList.Length;
			}
		}

		public override void UpdateHand() {
			//Update the spheres first
			UpdateSpheres();

			//Update Arm only if we need to
			if (showArm) {
				UpdateArm();
			}

			//The cylinder transforms are deterimined by the spheres they are connected to
			UpdateCylinders();
		}

		protected virtual void UpdateSpheres() {
			//Update all spheres
			var fingers = Hand.Fingers;
			foreach (var finger in fingers) {
				for (var j = 0; j < 4; j++) {
					var key = GetFingerJointIndex((int)finger.Type, j);
					var sphere = JointSpheres[key];
					sphere.position = finger.Bone((Bone.BoneType)j).NextJoint.ToVector3();
				}
			}

			PalmPositionSphere.position = Hand.PalmPosition.ToVector3();

			var wristPos = Hand.PalmPosition.ToVector3();
			WristPositionSphere.position = wristPos;

			var thumbBase = JointSpheres[ThumbBaseIndex];

			var thumbBaseToPalm = thumbBase.position - Hand.PalmPosition.ToVector3();
			MockThumbJointSphere.position = Hand.PalmPosition.ToVector3() +
											Vector3.Reflect(thumbBaseToPalm, Hand.Basis.xBasis.ToVector3());
		}

		protected virtual void UpdateArm() {
			var arm = Hand.Arm;
			var right = arm.Basis.xBasis.ToVector3() * arm.Width * 0.7f * 0.5f;
			var wrist = arm.WristPosition.ToVector3();
			var elbow = arm.ElbowPosition.ToVector3();

			var armLength = Vector3.Distance(wrist, elbow);
			wrist -= arm.Direction.ToVector3() * armLength * 0.05f;

			ArmFrontRight.position = wrist + right;
			ArmFrontLeft.position = wrist - right;
			ArmBackRight.position = elbow + right;
			ArmBackLeft.position = elbow - right;
		}

		private void UpdateCylinders() {
			for (var i = 0; i < cylinderTransforms.Count; i++) {
				var cylinder = cylinderTransforms[i];
				var sphereA = sphereATransforms[i];
				var sphereB = sphereBTransforms[i];

				var delta = sphereA.position - sphereB.position;

				if (!hasGeneratedMeshes) {
					var filter = cylinder.GetComponent<MeshFilter>();
					filter.sharedMesh = GenerateCylinderMesh(delta.magnitude / transform.lossyScale.x);
				}

				cylinder.position = sphereA.position;

				if (delta.sqrMagnitude <= Mathf.Epsilon) {
					//Two spheres are at the same location, no rotation will be found
					continue;
				}

				cylinder.LookAt(sphereB);
			}

			hasGeneratedMeshes = true;
		}

		private void UpdateArmVisibility() {
			foreach (var r in armRenderers) {
				r.enabled = showArm;
			}
		}

		private void CreateSpheres() {
			//Create spheres for finger joints
			var fingers = Hand.Fingers;
			foreach (var finger in fingers) {
				for (var j = 0; j < 4; j++) {
					var key = GetFingerJointIndex((int)finger.Type, j);
					JointSpheres[key] = CreateSphere("Joint", SphereRadius);
				}
			}

			MockThumbJointSphere = CreateSphere("MockJoint", SphereRadius);
			PalmPositionSphere = CreateSphere("PalmPosition", PalmRadius);
			WristPositionSphere = CreateSphere("WristPosition", SphereRadius);

			ArmFrontLeft = CreateSphere("ArmFrontLeft", SphereRadius, true);
			ArmFrontRight = CreateSphere("ArmFrontRight", SphereRadius, true);
			ArmBackLeft = CreateSphere("ArmBackLeft", SphereRadius, true);
			ArmBackRight = CreateSphere("ArmBackRight", SphereRadius, true);
		}

		private void CreateCylinders() {
			//Create cylinders between finger joints
			for (var i = 0; i < 5; i++) {
				for (var j = 0; j < 3; j++) {
					var keyA = GetFingerJointIndex(i, j);
					var keyB = GetFingerJointIndex(i, j + 1);

					var sphereA = JointSpheres[keyA];
					var sphereB = JointSpheres[keyB];

					CreateCylinder("Finger Joint", sphereA, sphereB);
				}
			}

			//Create cylinders between finger knuckles
			for (var i = 0; i < 4; i++) {
				var keyA = GetFingerJointIndex(i, 0);
				var keyB = GetFingerJointIndex(i + 1, 0);

				var sphereA = JointSpheres[keyA];
				var sphereB = JointSpheres[keyB];

				CreateCylinder("Hand Joints", sphereA, sphereB);
			}

			//Create the rest of the hand
			var thumbBase = JointSpheres[ThumbBaseIndex];
			var pinkyBase = JointSpheres[PinkyBaseIndex];
			CreateCylinder("Hand Bottom", thumbBase, MockThumbJointSphere);
			CreateCylinder("Hand Side", pinkyBase, MockThumbJointSphere);

			CreateCylinder("ArmFront", ArmFrontLeft, ArmFrontRight, true);
			CreateCylinder("ArmBack", ArmBackLeft, ArmBackRight, true);
			CreateCylinder("ArmLeft", ArmFrontLeft, ArmBackLeft, true);
			CreateCylinder("ArmRight", ArmFrontRight, ArmBackRight, true);
		}

		protected int GetFingerJointIndex(int fingerIndex, int jointIndex) {
			return fingerIndex * 4 + jointIndex;
		}

		private Transform CreateSphere(string sphereName, float radius, bool isPartOfArm = false) {
			var sphere = new GameObject(sphereName);
			serializedTransforms.Add(sphere.transform);

			sphere.AddComponent<MeshFilter>().mesh = sphereMesh;
			sphere.AddComponent<MeshRenderer>().sharedMaterial = jointMat;
			sphere.transform.parent = transform;
			sphere.transform.localScale = Vector3.one * radius * 2;

			sphere.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy | HideFlags.HideInInspector;
			sphere.layer = gameObject.layer;

			if (isPartOfArm) {
				armRenderers.Add(sphere.GetComponent<Renderer>());
			}

			return sphere.transform;
		}

		private void CreateCylinder(string cylinderName, Transform jointA, Transform jointB, bool isPartOfArm = false) {
			var cylinder = new GameObject(cylinderName);
			serializedTransforms.Add(cylinder.transform);

			cylinder.AddComponent<MeshFilter>();
			cylinder.AddComponent<MeshRenderer>().sharedMaterial = material;
			cylinder.transform.parent = transform;

			cylinderTransforms.Add(cylinder.transform);
			sphereATransforms.Add(jointA);
			sphereBTransforms.Add(jointB);

			cylinder.gameObject.layer = gameObject.layer;
			cylinder.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy | HideFlags.HideInInspector;

			if (isPartOfArm) {
				armRenderers.Add(cylinder.GetComponent<Renderer>());
			}
		}

		private Mesh GenerateCylinderMesh(float length) {
			var mesh = new Mesh {
				name = "GeneratedCylinder",
				hideFlags = HideFlags.DontSave
			};

			var verts = new List<Vector3>();
			var colors = new List<Color>();
			var tris = new List<int>();

			var p0 = Vector3.zero;
			var p1 = Vector3.forward * length * CylinderLengthFactor;
			for (var i = 0; i < cylinderResolution; i++) {
				var angle = (Mathf.PI * 2.0f * i) / cylinderResolution;
				var dx = CylinderRadius * Mathf.Cos(angle);
				var dy = CylinderRadius * Mathf.Sin(angle);

				var spoke = new Vector3(dx, dy, 0);

				verts.Add(p0 + spoke);
				verts.Add(p1 + spoke);

				colors.Add(Color.white);
				colors.Add(Color.white);

				var triStart = verts.Count;
				var triCap = cylinderResolution * 2;

				tris.Add((triStart + 0) % triCap);
				tris.Add((triStart + 2) % triCap);
				tris.Add((triStart + 1) % triCap);

				tris.Add((triStart + 2) % triCap);
				tris.Add((triStart + 3) % triCap);
				tris.Add((triStart + 1) % triCap);
			}

			mesh.SetVertices(verts);
			mesh.SetIndices(tris.ToArray(), MeshTopology.Triangles, 0);
			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
			mesh.Optimize();
			mesh.UploadMeshData(true);

			return mesh;
		}

	}

}