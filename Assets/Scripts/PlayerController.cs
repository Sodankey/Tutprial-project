using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private int count = 0;
    private float movementX;
    private float movementY;
    public float speed = 0;

    public TextMeshProUGUI countText;
    public GameObject winTextObject;

    public AudioClip winningSound;
    public AudioClip pickupSound;
    private AudioSource playerAudio;

    public ParticleSystem collectibleParticle;
    public ParticleSystem winParticle;
    public ParticleSystem bloodParticle;

    private Vector3 targetPos;
    [SerializeField] private bool isMoving = false;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        SetCountText();
        winTextObject.SetActive(false);
        playerAudio = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (Pointer.current.press.isPressed)
        {
            Vector2 aimPosition = Pointer.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(aimPosition);
            Debug.DrawRay(ray.origin, ray.direction * 50, Color.green);

            RaycastHit hit; // Define variable to hold ray cast hit information
            // Check if ray cast hits an object
            if (Physics.Raycast(ray, out hit))
            {
                targetPos = hit.point; // Set target position
                isMoving = true; // Start player movement
            }
            else
            {
                isMoving = false; // Stop player movement
            }
        }
    }
    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;
    }
    void SetCountText()
    { 
        countText.text = "Count: " + count.ToString();
        if (count >= 3)
        { 
            winTextObject.SetActive(true);
            Destroy(GameObject.FindGameObjectWithTag("Enemy"));
            playerAudio.PlayOneShot(winningSound, 1f);
            winParticle.Play();
        }
    }
    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);

        if (isMoving)
        {
            // Move the player towards the target position
            Vector3 direction = targetPos - rb.position;
            direction.Normalize();
            rb.AddForce(direction * speed);
        }
        if (Vector3.Distance(rb.position, targetPos) < 0.5f)
        {
            isMoving = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            playerAudio.PlayOneShot(pickupSound, 1f);
            collectibleParticle.Play();
            count = count + 1;
            SetCountText();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponentInChildren<Animator>().SetTrigger("PlayerProximity");
            Destroy(gameObject);
            collision.gameObject.GetComponent<AudioSource>().Play();
            collision.gameObject.GetComponentInChildren<Animator>().SetFloat("speed_f", 0);
            winTextObject.gameObject.SetActive(true);
            winTextObject.GetComponent<TextMeshProUGUI>().text = "You lose!";
        }
    }
 
}

