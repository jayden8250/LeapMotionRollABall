using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using Leap;


public class PlayerController : MonoBehaviour {

	public float speed;
	public Text countText;
	public Text winText;
	public Text LeapControllerText;
	public Text SceneText;
	private Rigidbody rb;
	public static int count;
	Controller LEAPcontroller;
	public bool rightHandControl = true;

	void Start ()
	{
		rb = GetComponent<Rigidbody> ();
		count = 0;
		SetCountText ();
		winText.text = "";

		Scene scene = SceneManager.GetActiveScene (); //Get the name of the active scene and display in UI
		SceneText.text = scene.name;

		//Is the leap motion controller connected?
		LEAPcontroller = new Controller ();

		if (LEAPcontroller.IsConnected) {
			LeapControllerText.text = "Leap Connected";
		} else {
			LeapControllerText.text = "Leap Not Connected";
		}
	}

	void FixedUpdate ()
	{
		//Restarts level by pressing r, loads next levels by pressing n and b
		if (Input.GetKeyDown ("r")) {
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
		}
		if (Input.GetKeyDown ("n")) {
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);
		}
		if (Input.GetKeyDown ("b")) {
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex - 1);
		}


		//If the leap controller is connected, utilize different horizontal & vertical floats
		if (LEAPcontroller.IsConnected) {
			Frame frame = LEAPcontroller.Frame ();

			//Find first hand of control type
			Hand controlHand = Hand.Invalid;
			foreach (Hand hand in frame.Hands) {
				if ((hand.IsRight && rightHandControl) || (hand.IsLeft && !rightHandControl)) {
					controlHand = hand;
					break;
				}
			}

			//Normalize Leap coordinate to range of -1 to 1 using the InteractionBox
			Vector normalized = frame.InteractionBox.NormalizePoint (controlHand.PalmPosition, true) * 2;
			float horizontal = normalized.x - 1;
			float vertical = normalized.z - 1;

			Vector3 movement = new Vector3 (horizontal, 0.0f, -vertical); // movement in (x,y,z) -z accounts for inverted axis
			rb.AddForce (movement * speed);
		}
			
		else { //If the leap controller is NOT connected, utilize different horizontal and vertical floats for keyboards
			float moveHorizontal = Input.GetAxis ("Horizontal");
			float moveVertical = Input.GetAxis ("Vertical");

			Vector3 keysMovement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
			rb.AddForce (keysMovement * (speed - 10));
		}
	}

	void OnTriggerEnter(Collider other) 
	{
		if (other.gameObject.CompareTag ("Pick Up"))
		{
			other.gameObject.SetActive (false);
			count = count + 1;
			SetCountText ();
		}
	}

	void SetCountText ()
	{
		Scene scene = SceneManager.GetActiveScene();
		countText.text = "Count: " + count.ToString ();

		if (scene.name == "minigame") {
			if (count >= 8) {
				winText.text = "You Win!\nLoading Next Level...";
				StartCoroutine ("LoadNextLevel");
			}
		}
		if (scene.name == "minigame2") {
			if (count >= 1) {
				winText.text = "You Win!\nLoading Next Level...";
				StartCoroutine ("LoadNextLevel");
			}
		}
		if (scene.name == "minigame3") {
			if (count >= 39) {
				winText.text = "You Win!";
				StartCoroutine ("LoadNextLevel");
			}
		}
	}
			
	IEnumerator LoadNextLevel() { //Coroutine for loading the next scene after 5 seconds
		yield return new WaitForSeconds (5); 
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}
}