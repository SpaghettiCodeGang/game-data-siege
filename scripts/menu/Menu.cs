using System;
using System.Threading.Tasks;
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

	private AudioStreamPlayer2D _soundClicked;

	/// <summary>
	/// Connects button signals to their respective handler methods.
	/// Loads the audio player
	/// </summary>
	public override void _Ready()
	{
		_btnPlay.Connect("pressed", new Callable(this, nameof(OnPlayPressed)));
		_btnTutorial.Connect("pressed", new Callable(this, nameof(OnTutorialPressed)));
		_btnExit.Connect("pressed", new Callable(this, nameof(OnExitPressed)));
		
		// Sound player for the button clicked sound
		_soundClicked = GetNode<AudioStreamPlayer2D>("ButtonClick");
	}

	private async void OnTutorialPressed()
	{
		try
		{
			await LoadStage(TutorialStage);
		}
		catch (Exception e)
		{
			GD.Print("Loading Tutorial Failed");
		}
	}

	private void OnPlayPressed()
	{
		// TODO: GameManager.Instance.LoadStage(GameStage);
	}

	private void OnExitPressed()
	{
		GetTree().Quit();
	}
	
	/// <summary>
	/// Starts the sound player and waits before loading the stage.
	/// </summary>
	/// <param name="stage">Name of the stage to be loaded.</param>
	private async Task LoadStage(PackedScene stage)
	{
		if (stage == null)
		{
			return;
		}
		if (_soundClicked != null)
		{
			_soundClicked.Play();
			await ToSignal(_soundClicked, AudioStreamPlayer2D.SignalName.Finished);
		}
		
		GameManager.Instance.LoadStage(stage);
	}
}