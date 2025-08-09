using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Movement : MonoBehaviour
{
    [SerializeField] public InputAction thrust;
    [SerializeField] float thrustStrength = 100f;
    [SerializeField] public InputAction rotation;
    [SerializeField] float rotationStrength= 100f;
    [SerializeField] AudioClip mainEngineSFX;
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem leftThrustParticles;
    [SerializeField] ParticleSystem rightThrustParticles;
    
    private Rigidbody rb;
    private AudioSource audioSource;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        thrust.Enable();
        rotation.Enable();
    }

    private void FixedUpdate()
    {
        ProcessThrust();
        ProcessRotation();
    }

    private void ProcessThrust()
    {
        if (thrust.IsPressed())
        {
            StartThrusting();
        }
        else if (thrust.enabled == true)
        {
            StopThrusting();
        }
    }

    private void StopThrusting()
    {
        audioSource.Stop();
        mainEngineParticles.Stop();
    }

    private void StartThrusting()
    {
        rb.AddRelativeForce(Vector3.up * thrustStrength * Time.fixedDeltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngineSFX);
        }
        if (!mainEngineParticles.isPlaying) mainEngineParticles.Play();
    }

    private void ProcessRotation()
    {
        float rotationInput = rotation.ReadValue<float>();

        ManageConstraintsWhileRotation(rotationInput);
        ManageParticlesWhileRotation(rotationInput);
        
        transform.Rotate(-rotationInput * Vector3.forward * rotationStrength * Time.fixedDeltaTime);
        // Right rotation is negative Z-axis in Unity
        // Left rotation is positive Z-axis in Unity
        
        
    }

    private void ManageParticlesWhileRotation(float rotationInput)
    {
        if (rotationInput > 0)
        {
            if (!leftThrustParticles.isPlaying)
            {
                rightThrustParticles.Stop();
                leftThrustParticles.Play();
            }
        }
        else if (rotationInput < 0)
        {
            if (!leftThrustParticles.isPlaying)
            {
                leftThrustParticles.Stop();
                rightThrustParticles.Play();
            }
        }
        else
        {
            leftThrustParticles.Stop();
            rightThrustParticles.Stop();
        }
    }

    private void ManageConstraintsWhileRotation(float rotationInput)
    {
        if (rotationInput != 0)
        {
            rb.freezeRotation = true; // Prevent physics from interfering with rotation Y (specifically)
        }
        else
        {
            rb.constraints = RigidbodyConstraints.None; // Re-enable physics rotation
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY; // Freeze rotation on X and Y axes back
            rb.constraints |= RigidbodyConstraints.FreezePositionZ; // Freeze position on Z axis
            
            //rb.freezeRotation = false; 
        }
    }
}
