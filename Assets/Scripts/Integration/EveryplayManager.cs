using UnityEngine;
using System.Collections; 

public class EveryplayManager : MonoBehaviour {

	public void OnReadyForRecording(bool enabled) {
		if(enabled) {
			// The recording is supported
		//	 Everyplay.SetUpRecording();
		} 
	}

	void Start() {
		// Other init code
		// ...

		// Register for the Everyplay ReadyForRecording event
	//	Everyplay.ReadyForRecording += OnReadyForRecording;
	}

}
