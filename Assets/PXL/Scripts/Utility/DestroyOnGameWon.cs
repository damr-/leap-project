using System;
using UnityEngine;
using PXL.Gamemodes;
using PXL.Utility;
using UniRx;

public class DestroyOnGameWon : MonoBehaviour {

	/// <summary>
	/// Whether this object shouldn't be destroyed but despawned into the pool.
	/// </summary>
	public bool Despawn;

	/// <summary>
	/// Subscription to the GameWon state in GameMode
	/// </summary>
	private IDisposable gameWinDisposable = Disposable.Empty;

	private void Start() {
		gameWinDisposable = GameMode.GameWon.Subscribe(_ => HandleGameWon());
	}

	/// <summary>
	/// Called when the GameWon state changes
	/// </summary>
	private void HandleGameWon() {
		if(Despawn)
			SimplePool.Despawn(gameObject);
		else
			Destroy(gameObject);
	}

	/// <summary>
	/// Called when this object is disabled
	/// </summary>
	private void OnDisable() {
		gameWinDisposable.Dispose();
    }

}