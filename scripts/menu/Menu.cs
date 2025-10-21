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

	[Export] private AudioStreamPlayer _soundClick;

	/// <summary>
	/// Connects button signals to their respective handler methods.
	/// </summary>
	public override void _Ready()
	{
		_btnPlay.Pressed += async () => await HandleButtonPress(() =>
		{
			// TODO: GameManager.Instance.LoadStage(GameStage);
		});

		_btnTutorial.Pressed += async () => await HandleButtonPress(() => GameManager.Instance.LoadStage(TutorialStage));
		_btnExit.Pressed += async () => await HandleButtonPress(() => GetTree().Quit());
	}

	/// <summary>
	/// Plays the click sound (if available), waits for it to finish, and executes the given action.
	/// </summary>
	private async Task HandleButtonPress(Action action)
	{
		if (_soundClick != null)
		{
			_soundClick.Play();
			await ToSignal(_soundClick, "finished");
		}

		action?.Invoke();
	}
	
}