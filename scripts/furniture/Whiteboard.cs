using Godot;

/// <summary>
/// Represents a Whiteboard in the game, responsible for displaying instructional or narrative text.
/// 
/// Handles the display of a main text area with a typing animation (letter by letter) 
/// and an optional button hint label. The text display speed can be configured, and new text 
/// can be set dynamically at runtime.
/// </summary>
/// <author>Sören Lehmann</author>
public partial class Whiteboard : Node3D
{
    [Export] public RichTextLabel MainText;
    [Export] public Label ButtonHint;

    private string _fullText = "";
    private int _currentIndex;
    private float _timer;
    private float _charInterval = 0.04f; // Time per character in seconds
    private bool _isTyping;
    private AudioStreamPlayer3D _soundTyping;

    /// <summary>
    /// Sets the main text to be displayed on the whiteboard.
    /// The text will appear character by character at the configured interval.
    /// </summary>
    /// <param name="text">The full string to display.</param>
    public void SetMainText(string text)
    {
        _fullText = text;
        _currentIndex = 0;
        _timer = 0f;
        _isTyping = true;
        
        // Clear previous text before starting
        MainText.Text = "";
        
        // Sound player for the typing sound
        _soundTyping = GetNode<AudioStreamPlayer3D>("SoundTextTyping");
    }

    public void SetButtonHint(string text)
    {
        ButtonHint.Text = text;
    }

    /// <summary>
    /// Called every frame. Handles the letter-by-letter typing animation for the main text.
    /// Accumulates delta time and adds characters to the RichTextLabel at fixed intervals.
    /// </summary>
    /// <param name="delta">Time since the last frame.</param>
    public override void _Process(double delta)
    {
        if (!_isTyping) return;

        // Accumulate time since last frame
        _timer += (float)delta;
        
        // Display letters one by one according to the interval
        while (_timer >= _charInterval && _currentIndex < _fullText.Length)
        {
            _timer -= _charInterval;
            MainText.Text += _fullText[_currentIndex];
            _soundTyping.Play();
            _currentIndex++;
        }

        // Stop typing when all characters have been displayed
        if (_currentIndex >= _fullText.Length)
            _isTyping = false;
    }
}
