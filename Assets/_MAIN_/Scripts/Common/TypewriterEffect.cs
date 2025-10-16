using UnityEngine;
using TMPro;
using DG.Tweening;

public class TypewriterEffect : MonoBehaviour
{
    /// <summary>
    /// Animates text character by character using DoTween
    /// </summary>
    /// <param name="textComponent">The TMP_Text component to animate</param>
    /// <param name="fullText">The complete text to display</param>
    /// <param name="duration">Total duration for all characters to appear (in seconds)</param>
    /// <param name="onComplete">Optional callback when animation completes</param>
    public static Tween AnimateText(TMP_Text textComponent, string fullText, float duration, TweenCallback onComplete = null)
    {
        // Safety checks
        if (textComponent == null)
        {
            Debug.LogError("TMP_Text component is null!");
            return null;
        }

        if (string.IsNullOrEmpty(fullText))
        {
            Debug.LogWarning("Text string is empty!");
            textComponent.text = "";
            return null;
        }

        if (duration <= 0)
        {
            Debug.LogWarning("Duration must be greater than 0. Setting to 1 second.");
            duration = 1f;
        }

        // Kill any existing tweens on this text component
        DOTween.Kill(textComponent);

        // Set initial state
        textComponent.text = fullText;
        textComponent.maxVisibleCharacters = 0;

        // Calculate character reveal speed
        int totalCharacters = fullText.Length;
        float delayPerCharacter = duration / totalCharacters;

        // Create tween that reveals characters one by one
        Tween tween = DOTween.To(
            () => 0,
            (value) => textComponent.maxVisibleCharacters = value,
            totalCharacters,
            duration
        )
        .SetEase(Ease.Linear)
        .SetTarget(textComponent);

        // Add completion callback if provided
        if (onComplete != null)
        {
            tween.OnComplete(onComplete);
        }

        return tween;
    }

    /// <summary>
    /// Alternative version with more control - reveals characters with delay between each
    /// </summary>
    public static Tween AnimateTextWithDelay(TMP_Text textComponent, string fullText, float delayPerCharacter, TweenCallback onComplete = null)
    {
        if (textComponent == null)
        {
            Debug.LogError("TMP_Text component is null!");
            return null;
        }

        if (string.IsNullOrEmpty(fullText))
        {
            Debug.LogWarning("Text string is empty!");
            textComponent.text = "";
            return null;
        }

        if (delayPerCharacter <= 0)
        {
            Debug.LogWarning("Delay must be greater than 0. Setting to 0.05 seconds.");
            delayPerCharacter = 0.05f;
        }

        DOTween.Kill(textComponent);

        textComponent.text = fullText;
        textComponent.maxVisibleCharacters = 0;

        int totalCharacters = fullText.Length;
        float totalDuration = delayPerCharacter * totalCharacters;

        Tween tween = DOTween.To(
            () => 0,
            (value) => textComponent.maxVisibleCharacters = value,
            totalCharacters,
            totalDuration
        )
        .SetEase(Ease.Linear)
        .SetTarget(textComponent);

        if (onComplete != null)
        {
            tween.OnComplete(onComplete);
        }

        return tween;
    }

    /// <summary>
    /// Skip the animation and show all text immediately
    /// </summary>
    public static void SkipAnimation(TMP_Text textComponent)
    {
        if (textComponent == null) return;

        DOTween.Kill(textComponent);
        textComponent.maxVisibleCharacters = textComponent.text.Length;
    }

    /// <summary>
    /// Pause the typewriter animation
    /// </summary>
    public static void PauseAnimation(TMP_Text textComponent)
    {
        if (textComponent == null) return;

        Tween tween = DOTween.TweensById(textComponent).Find(t => t.target == textComponent);
        if (tween != null && tween.IsPlaying())
        {
            tween.Pause();
        }
    }

    /// <summary>
    /// Resume the typewriter animation
    /// </summary>
    public static void ResumeAnimation(TMP_Text textComponent)
    {
        if (textComponent == null) return;

        Tween tween = DOTween.TweensById(textComponent).Find(t => t.target == textComponent);
        if (tween != null && tween.IsPlaying() == false)
        {
            tween.Play();
        }
    }
}