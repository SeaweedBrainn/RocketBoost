using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.InputSystem;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] float levelLoadDelay = 2;
    [SerializeField] AudioClip successSFX;
    [SerializeField] AudioClip crashSFX;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem crashParticles;
    
    private AudioSource audioSource;
    private Movement movement;

    private bool isControllable = true;
    private bool isCollidable = true;

    private void Start()
    {
        movement = GetComponent<Movement>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        RespondToDebugKeys();
    }
    private void OnCollisionEnter(Collision other)
    {
        if (!isControllable || !isCollidable) return;
        
        switch (other.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Fuel":
                break;
            case "Finish":
                StartSuccessSequence(levelLoadDelay);
                break;
            default:
                StartCrashSequence(levelLoadDelay);
                break;
        }
    }
    
    void ReloadLevel()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene);
    }

    void LoadNextLevel()
    {
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextScene == SceneManager.sceneCountInBuildSettings) SceneManager.LoadScene(0);
        else SceneManager.LoadScene(nextScene);
    }

    void StartCrashSequence(float delay)
    {
        isControllable = false;
        audioSource.Stop(); //stop thrust sound
        audioSource.PlayOneShot(crashSFX);
        crashParticles.Play();
        movement.thrust.Disable();
        movement.rotation.Disable();
        Invoke("ReloadLevel", delay);
    }

    void StartSuccessSequence(float delay)
    {
        isControllable = false;
        audioSource.Stop(); //stop thrust sound
        audioSource.PlayOneShot(successSFX);
        successParticles.Play();
        movement.thrust.Disable();
        movement.rotation.Disable();
        Invoke("LoadNextLevel", delay);
    }
    
    private void RespondToDebugKeys()
    {
        if (Keyboard.current.lKey.wasPressedThisFrame) LoadNextLevel();
        else if (Keyboard.current.cKey.wasPressedThisFrame) isCollidable = !isCollidable;
    }
}
