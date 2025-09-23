using Godot;

namespace gamedatasiege.scripts.menu;

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

	/// <summary>
	/// Connects button signals to their respective handler methods.
	/// </summary>
	public override void _Ready()
	{
		_btnPlay.Connect("pressed", new Callable(this, nameof(OnPlayPressed)));
		_btnTutorial.Connect("pressed", new Callable(this, nameof(OnTutorialPressed)));
		_btnExit.Connect("pressed", new Callable(this, nameof(OnExitPressed)));
	}

	private void OnTutorialPressed()
	{
		core.GameManager.Instance.LoadStage(TutorialStage);
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