using UnityEngine;
using System.Collections;

public class NavigationVR_touch : MonoBehaviour
{
		public GameObject head;
		public GameObject pointer;
		// bookkeep x rotation because Unity uses quaternions and behavior is not adapted to my algorithm
		// no gimbal lock here because body and torso rotations are separated
		Vector3 headEulerAngles;


		Vector3 secondTouchFirstPosition;
		Vector3 cameraPoint;
		//Vector3 targetPoint;
		Ray targetRay;
		bool secondTouchPressed = false;

		float rayCastMinLimit = 10f;
		float rayCastMaxLimit = 80f;

		float pointerRange;
		float pointerMinRange;
		float pointerMaxRange;



		// Use this for initialization
		void Start ()
		{
				resetPointerRange ();
				headEulerAngles = head.transform.rotation.eulerAngles;
				pointer.SetActive (false);
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (secondTouchPressed && Input.touchCount != 2) {
						this.transform.position = new Vector3(pointer.transform.position.x, this.transform.position.y, pointer.transform.position.z);
				}
				if (Input.touchCount >= 1) {
						UpdatePointer ();
						if (Input.touchCount == 1) {
							RotateTorso ();
						}
						if (Input.touchCount == 2) {
								UpdatePointerRange ();
						} else {
								secondTouchPressed = false;
						}
				} else {
						pointer.SetActive (false);
						resetPointerRange ();
						secondTouchPressed = false;
				}


				head.transform.eulerAngles = headEulerAngles;
		}

		void UpdatePointer() {
				Vector2 firstTouchOnScreen = Input.GetTouch (0).position;
				targetRay = Camera.main.ScreenPointToRay (firstTouchOnScreen);

				RaycastHit hit;
				if (Physics.Raycast (targetRay, out hit)) {
						pointerMaxRange = Mathf.Min (hit.distance, rayCastMaxLimit);
				} else {
						pointerMaxRange = rayCastMaxLimit;
				}
				Debug.Log (pointerRange + "," + pointerMinRange + "," + pointerMaxRange);
				pointer.transform.position = targetRay.GetPoint (Mathf.Clamp(pointerRange, pointerMinRange, pointerMaxRange));

				pointer.SetActive (true);
		}

		void RotateTorso() {
				Vector2 firstTouchOnScreen = Input.GetTouch (0).position;
				float seuil = 0.5f;
				Vector2 screen_center = new Vector2 (Screen.width, Screen.height) / 2f;

				//left-right			
				float lr_rotation = (firstTouchOnScreen.x - screen_center.x) / screen_center.x;
				lr_rotation = Mathf.Sign (lr_rotation) * ((Mathf.Max (seuil, Mathf.Abs (lr_rotation))) - seuil) * (1f / (1f - seuil));
				lr_rotation = Mathf.Sign (lr_rotation) * Mathf.Pow (lr_rotation, 2f);
				headEulerAngles.y = headEulerAngles.y + lr_rotation;
		}


		void UpdatePointerRange() {
				secondTouchPressed = true;
				//pointerRange = Mathf.Clamp (pointerRange + 5, rayCastMinLimit, rayCastMaxLimit);
		}

		void resetPointerRange() {
				pointerRange = rayCastMaxLimit;
				pointerMaxRange = rayCastMaxLimit;
				pointerMinRange = rayCastMinLimit;
		}

		public static float computeAngle (Vector3 v1, Vector3 v2, Vector3 n)
		{
				return Mathf.Atan2 (
						Vector3.Dot (n, Vector3.Cross (v1, v2)),
						Vector3.Dot (v1, v2)) * Mathf.Rad2Deg;
		}

}
