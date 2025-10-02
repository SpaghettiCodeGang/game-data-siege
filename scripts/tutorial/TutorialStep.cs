using System;
using System.Collections.Generic;

/// <summary>
/// Represents a single step in a tutorial.
/// Contains the text, button hints, accepted inputs, and optional actions.
/// </summary>
/// <author>Sören Lehmann</author>
public class TutorialStep
{
    public readonly string MainText;
    public readonly string ButtonHint;
    public readonly HashSet<string> NextInputs;  // The set of input identifiers that allow advancing to the next step.
    public readonly HashSet<string> BackInputs;  // The set of input identifiers that allow returning to the previous step.
    public readonly Action OnStepStart;          // Optional action executed when this step begins.

    /// <summary>
    /// Creates a new tutorial step with the given configuration.
    /// </summary>
    /// <param name="mainText">The main text to display for this step.</param>
    /// <param name="buttonHint">A short button hint text to display.</param>
    /// <param name="nextInputs">The collection of valid inputs to progress to the next step.</param>
    /// <param name="backInputs">Optional collection of valid inputs to go back to the previous step. Defaults to empty.</param>
    /// <param name="onStart">Optional action to execute when the step starts.</param>
    public TutorialStep(
        string mainText,
        string buttonHint,
        IEnumerable<string> nextInputs = null,
        IEnumerable<string> backInputs = null,
        Action onStart = null)
    {
        MainText = mainText;
        ButtonHint = buttonHint;
        NextInputs = nextInputs != null ? [..nextInputs] : [];
        BackInputs = backInputs != null ? [..backInputs] : [];
        OnStepStart = onStart;
    }
}