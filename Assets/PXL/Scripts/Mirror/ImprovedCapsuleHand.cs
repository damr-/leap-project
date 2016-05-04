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

		private const float SphereRadius = 0.008f;
		private const float CylinderRadius = 0.006f;
		private const float PalmRadius = 0.015f;

		private static int _colorIndex;
		private static readonly Color[] ColorList = { Color.blue, Color.green, Color.magenta, Color.cyan, Color.red, Color.yellow };

		public bool ShowArm = true;

		public Material Material;

		private Material jointMat;

		protected Transform[] JointSpheres;
		protected Transform MockThumbJointSphere;
		protected Transform PalmPositionSphere;

		protected Transform WristPositionSphere;

		private List<Renderer> armRenderers;
		private List<Transform> capsuleTransforms;
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
		[SerializeField]
		private Chirality handedness;

		public override Hand GetLeapHand() {
			return Hand;
		}

		public override void SetLeapHand(Hand hand) {
			Hand = hand;
		}

		private void OnValidate() {
			if (armRenderers != null)
				UpdateArmVisibility();
		}
		
		public override void InitHand() {
			if (Material != null) {
				jointMat = new Material(Material) {
					hideFlags = HideFlags.DontSaveInEditor,
					color = ColorList[_colorIndex]
				};
				_colorIndex = (_colorIndex + 1) % ColorList.Length;
			}

			JointSpheres = new Transform[4 * 5];
			armRenderers = new List<Renderer>();
			capsuleTransforms = new List<Transform>();
			sphereATransforms = new List<Transform>();
			sphereBTransforms = new List<Transform>();

			CreateSpheres();
			CreateCapsules();

			UpdateArmVisibility();
		}

		public override void UpdateHand() {
			UpdateSpheres();

			if (ShowArm)
				UpdateArm();

			UpdateCapsules();
		}

		protected virtual void UpdateSpheres() {
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
											Vector3.Reflect(thumbBaseToPalm, Hand.Basis.xBasis.ToVector3().normalized);
		}

		protected virtual void UpdateArm() {
			var arm = Hand.Arm;
			var right = arm.Basis.xBasis.ToVector3().normalized * arm.Width * 0.7f * 0.5f;
			var wrist = arm.WristPosition.ToVector3();
			var elbow = arm.ElbowPosition.ToVector3();

			var armLength = Vector3.Distance(wrist, elbow);
			wrist -= arm.Direction.ToVector3() * armLength * 0.05f;

			ArmFrontRight.position = wrist + right;
			ArmFrontLeft.position = wrist - right;
			ArmBackRight.position = elbow + right;
			ArmBackLeft.position = elbow - right;
		}

		private void UpdateCapsules() {
			for (var i = 0; i < capsuleTransforms.Count; i++) {
				var capsule = capsuleTransforms[i];
				var sphereA = sphereATransforms[i];
				var sphereB = sphereBTransforms[i];

				var delta = sphereA.position - sphereB.position;

				var scale = capsule.localScale;
				scale.x = CylinderRadius * 2;
				scale.y = delta.magnitude * 0.5f / transform.lossyScale.x;
				scale.z = CylinderRadius * 2;

				capsule.localScale = scale;

				capsule.position = (sphereA.position + sphereB.position) / 2;

				if (delta.sqrMagnitude <= Mathf.Epsilon) 
					continue;

				Vector3 perp;
				if (Vector3.Angle(delta, Vector3.up) > 170 || Vector3.Angle(delta, Vector3.up) < 10) {
					perp = Vector3.Cross(delta, Vector3.right);
				}
				else {
					perp = Vector3.Cross(delta, Vector3.up);
				}

				capsule.rotation = Quaternion.LookRotation(perp, delta);
			}
		}

		private void UpdateArmVisibility() {
			foreach (var r in armRenderers)
				r.enabled = ShowArm;
		}

		private void CreateSpheres() {
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

		private void CreateCapsules() {
			for (var i = 0; i < 5; i++) {
				for (var j = 0; j < 3; j++) {
					var keyA = GetFingerJointIndex(i, j);
					var keyB = GetFingerJointIndex(i, j + 1);

					var sphereA = JointSpheres[keyA];
					var sphereB = JointSpheres[keyB];

					CreateCapsule("Finger Joint", sphereA, sphereB);
				}
			}
			
			for (var i = 0; i < 4; i++) {
				var keyA = GetFingerJointIndex(i, 0);
				var keyB = GetFingerJointIndex(i + 1, 0);

				var sphereA = JointSpheres[keyA];
				var sphereB = JointSpheres[keyB];

				CreateCapsule("Hand Joints", sphereA, sphereB);
			}

			var thumbBase = JointSpheres[ThumbBaseIndex];
			var pinkyBase = JointSpheres[PinkyBaseIndex];
			CreateCapsule("Hand Bottom", thumbBase, MockThumbJointSphere);
			CreateCapsule("Hand Side", pinkyBase, MockThumbJointSphere);

			CreateCapsule("ArmFront", ArmFrontLeft, ArmFrontRight, true);
			CreateCapsule("ArmBack", ArmBackLeft, ArmBackRight, true);
			CreateCapsule("ArmLeft", ArmFrontLeft, ArmBackLeft, true);
			CreateCapsule("ArmRight", ArmFrontRight, ArmBackRight, true);
		}

		protected int GetFingerJointIndex(int fingerIndex, int jointIndex) {
			return fingerIndex * 4 + jointIndex;
		}

		private Transform CreateSphere(string title, float radius, bool isPartOfArm = false) {
			var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			DestroyImmediate(sphere.GetComponent<Collider>());
			sphere.transform.parent = transform;
			sphere.transform.localScale = Vector3.one * radius * 2;
			sphere.GetComponent<Renderer>().sharedMaterial = jointMat;

			sphere.name = title;
			sphere.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy | HideFlags.HideInInspector;

			if (isPartOfArm)
				armRenderers.Add(sphere.GetComponent<Renderer>());

			return sphere.transform;
		}

		private void CreateCapsule(string title, Transform jointA, Transform jointB, bool isPartOfArm = false) {
			var capsule = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			DestroyImmediate(capsule.GetComponent<Collider>());
			capsule.name = title;
			capsule.transform.parent = transform;
			capsule.transform.localScale = Vector3.one * CylinderRadius * 2;
			capsule.GetComponent<Renderer>().sharedMaterial = Material;

			capsuleTransforms.Add(capsule.transform);
			sphereATransforms.Add(jointA);
			sphereBTransforms.Add(jointB);

			capsule.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy | HideFlags.HideInInspector;

			if (isPartOfArm) {
				armRenderers.Add(capsule.GetComponent<Renderer>());
			}
		}

	}

}