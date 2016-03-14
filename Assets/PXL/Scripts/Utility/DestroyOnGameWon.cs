using UnityEngine;
using PXL.Gamemodes;
using PXL.Utility;
using UniRx;

public class DestroyOnGameWon : MonoBehaviour {

	/// <summary>
	/// Whether this object shouldn't be destroyed but despawned into the pool.
	/// </summary>
	public bool Despawn;

	private void Start() {
		GameMode.GameWon.Subscribe(HandleGameWon);
	}

	private void HandleGameWon(bool won) {
		if(!won || this == null || gameObject == null)
			return;

		if(Despawn)
			SimplePool.Despawn(gameObject);
		else
			Destroy(gameObject);
	}

}