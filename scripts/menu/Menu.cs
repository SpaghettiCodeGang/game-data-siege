using Godot;

/// <summary>
/// Represents the main menu UI.
/// Provides buttons to start the game, launch the tutorial, or exit the application.
/// </summary>
/// <author>Sören Lehmann</author>
public partial class Menu : VBoxContainer
{
	[Export] public PackedScene GameStage;
	[Export] public PackedScene TutorialStage;

	[Export] private Button _btnPlay;
	[Export] private Button _btnTutorial;
	[Export] private Button _btnExit;

	[Export] private AudioStreamPlayer _soundClick;

	/// <summary>
	/// Connects button signals to their respective handler methods.
	/// </summary>
	public override void _Ready()
	{
		_btnPlay.Connect("pressed", new Callable(this, nameof(OnPlayPressed)));
		_btnTutorial.Connect("pressed", new Callable(this, nameof(OnTutorialPressed)));
		_btnExit.Connect("pressed", new Callable(this, nameof(OnExitPressed)));
		
		_soundClick = GetNode<AudioStreamPlayer>("SoundClick");
	}

	/// <summary>
	/// Handles the event triggered when the Tutorial is pressed.
	/// If a click sound is defined, it is played, and a timer is started to wait 
	/// for the sound's duration before loading the stage. Otherwise, the stage 
	/// is loaded immediately.
	/// </summary>
	private void OnTutorialPressed()
	{
		if (_soundClick == null)
		{
			GameManager.Instance.LoadStage(TutorialStage);
			return;
		}
		
		_soundClick.Play();

		var timer = new Timer
		{
			WaitTime = _soundClick.Stream.GetLength(),
			OneShot = true
		};
		AddChild(timer);
		timer.Timeout += () =>
		{
			GameManager.Instance.LoadStage(TutorialStage);
			timer.QueueFree();
		};
		timer.Start();

	}

	private void OnPlayPressed()
	{
		// TODO: GameManager.Instance.LoadStage(GameStage);
	}

	private void OnExitPressed()
	{
		GetTree().Quit();
	}
}