using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource audioSource;

    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip dying;
    [SerializeField] AudioClip nextLevel;

    [SerializeField] ParticleSystem engineParticles;
    [SerializeField] ParticleSystem dyingParticles;
    [SerializeField] ParticleSystem finishParticles;

    enum State {Alive, Dying, Transending}
    State state = State.Alive;

    bool disableCollisions = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            Thrust();
            Rotate();
        }
        if (Debug.isDebugBuild)
        {
            DebugMode();
        }
    }

    private void DebugMode()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            disableCollisions = !disableCollisions;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || disableCollisions) { return; }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                StartFinishSequence();
                break;
            default:
                StartDyingSequence();
                break;
        }
    }

    private void StartFinishSequence()
    {
        state = State.Transending;
        Invoke("LoadNextLevel", levelLoadDelay);
        finishParticles.Play();
        audioSource.PlayOneShot(nextLevel);
        
    }
    private void StartDyingSequence()
    {
        state = State.Dying;     
        print("Hit something deadly");
        Invoke("LoadFirstLevel", levelLoadDelay);
        audioSource.Stop();
        dyingParticles.Play();
        audioSource.PlayOneShot(dying);
    }

    private void LoadNextLevel() //Loading next scene when finished
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        int nextScene = currentScene + 1;
        if (nextScene == SceneManager.sceneCountInBuildSettings)
        {
            nextScene = 0;
        }
            SceneManager.LoadScene(nextScene);
    }

    private void LoadFirstLevel() //Loading first scene when dying
    {
        SceneManager.LoadScene(0);
    }

    private void Thrust() //Thrust mechanics
    {
        float mainThrustThisFrame = mainThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.Space))
        {
            rigidBody.AddRelativeForce(Vector3.up * mainThrustThisFrame * Time.deltaTime);
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(mainEngine);
            }
            engineParticles.Play();
        }
        else
        {
            audioSource.Stop();
            engineParticles.Stop();
        }
    }
    private void Rotate() //Rotate mechanics
    {
        rigidBody.angularVelocity = Vector3.zero;
        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
    }
}
