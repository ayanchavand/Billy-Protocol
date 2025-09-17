using UnityEngine;
using System.Collections;

/// <summary>
/// Animates a character’s facial expressions by swapping material textures at runtime.
/// Supports a default expression, random blinking, and temporary overrides (e.g. when zooming
/// upward in flight). Uses a <see cref="CharacterController"/> to detect grounded state and
/// vertical velocity for personality-driven expression changes. Designed to give characters
/// more life without relying on traditional blendshape animation.
/// </summary>

[System.Serializable]
public struct FaceExpression
{
    public Texture albedo;
    public Texture emission;
}

[RequireComponent(typeof(CharacterController))]
public class FaceAnimator : MonoBehaviour
{
    [Header("Expressions")]
    [SerializeField] FaceExpression[] expressions;

    [Header("Materials")]
    [SerializeField] Material faceMat;

    [Header("Blink Settings")]
    [SerializeField] int blinkIndex = 1;
    [SerializeField] float minBlinkInterval = 3f;
    [SerializeField] float maxBlinkInterval = 6f;
    [SerializeField] float blinkDuration = 0.1f;

    [Header("Flight Detection")]
    [SerializeField] float upwardVelocityThreshold = 3f;
    [SerializeField] float groundCheckDistance = 0.1f;

    private int currentExpressionIndex = 0;
    private Coroutine blinkRoutine;

    private CharacterController controller;
    private Vector3 lastPosition;
    private bool isOverriding = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        lastPosition = transform.position;
        ApplyExpression(currentExpressionIndex);
        blinkRoutine = StartCoroutine(BlinkRoutine());
    }

    //Checking if the player is flying upwards and override the expression to blinking for added personality
    void Update()
    {   
        Vector3 velocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;

        bool isGrounded = controller.isGrounded;
        bool isZoomingUp = velocity.y > upwardVelocityThreshold;

        if (isZoomingUp && !isOverriding)
        {
            isOverriding = true;
            ApplyExpression(blinkIndex);
        }
        else if (isOverriding && isGrounded)
        {
            isOverriding = false;
            ApplyExpression(currentExpressionIndex);
        }
    }

    // Handles blinking my changing texture of the material, procedurally animating the face
    private IEnumerator BlinkRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minBlinkInterval, maxBlinkInterval);
            yield return new WaitForSeconds(waitTime);

            if (isOverriding) continue;

            int previous = currentExpressionIndex;
            ApplyExpression(blinkIndex);
            yield return new WaitForSeconds(blinkDuration);
            ApplyExpression(previous);
        }
    }
    // Aplies the expression by changing the material textures form the array
    private void ApplyExpression(int index)
    {
        faceMat.mainTexture = expressions[index].albedo;
        faceMat.SetTexture("_EmissionMap", expressions[index].emission);
    }

}
