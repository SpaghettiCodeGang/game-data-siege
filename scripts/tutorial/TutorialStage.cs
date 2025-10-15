using Godot;
using System.Collections.Generic;

/// <summary>
/// Stage for the tutorial sequence.
/// 
/// Manages a linear sequence of <see cref="TutorialStep"/> objects,
/// displays their text and button hints on the <see cref="Whiteboard"/>,
/// and reacts to player inputs passed from the <see cref="Player"/> to move
/// forward or backward through the sequence.
/// 
/// Each step may also define an optional start action (e.g. spawning items).
/// </summary>
/// <author>Sören Lehmann</author>
/// <coauthor>Elias Kugel</coauthor>
public partial class TutorialStage : BaseStage
{
	[Export] public PackedScene EnemyScene;
	[Export] public Marker3D EnemyPositionMarker;
	[Export] public Whiteboard Whiteboard;
	
	private Enemy _enemy;
	private int _currentStepIndex;
	private List<TutorialStep> _tutorialSteps;
	
	public override void OnEnter()
	{
		if (Player == null) return;
		InitTutorialSteps();
		_currentStepIndex = 0;
		_enemy = SpawnEnemy(EnemyPositionMarker);
		StartCurrentStep();
	}
	
	/// <summary>
	/// Spawns an enemy instance at the given spawn point.
	/// </summary>
	/// <param name="enemyPositionMarker">The Marker3D defining where the enemy should spawn.</param>
	/// <returns>The instantiated <see cref="Enemy"/> instance, or null if spawning failed.</returns>
	private Enemy SpawnEnemy(Marker3D enemyPositionMarker)
	{
		if (enemyPositionMarker == null || EnemyScene == null) 
			return null;
	
		var enemy = EnemyScene.Instantiate<Enemy>();
		enemy.GlobalTransform = enemyPositionMarker.GlobalTransform;
		enemy.Player = Player;
		AddChild(enemy);
	
		return enemy;
	}

	/// <summary>
	/// Initializes the full sequence of <see cref="TutorialStep"/> objects
	/// that define the tutorial flow for this stage.
	/// </summary>
	private void InitTutorialSteps()
	{
		_tutorialSteps =
		[
			new TutorialStep(
				"Willkommen im virtuellen Wissenshub der DHSN. " +
				"Ich bin Aurora, deine persönliche Lernhilfe. Zusammen mit meinen Helfer-Bots stehe ich dir bei allen Fragen zur Seite. " +
				"Hier hast du Zugriff auf das gesamte Wissen der Menschheit.",
				"🅐 - Weiter",
				["A"]
			),

			new TutorialStep(
				"EINIGE STUNDEN SPÄTER ...",
				"🅐 - Weiter | 🅧 - Zurück",
				["A"],
				["X"]
			),

			new TutorialStep(
				"Ah, du bist ja noch da! Alle anderen sind schon gegangen. " +
				"Ich sehe, du beschäftigst dich schon eine ganze Weile mit der Fourier-Transformation. Hast du Fra... ",
				"🅐 - Weiter | 🅧 - Zurück",
				["A"],
				["X"]
			),

			new TutorialStep(
				"SYSTEMALARM ... BOTS VON KI INFIZIERT ... KEINE KONTROLLE ... AKTIVIERE NOTFALLPROTOKOLL!",
				"🅐 - Weiter | 🅧 - Zurück.",
				["A"],
				["X"]
			),

			new TutorialStep(
				"Ich habe die Kontrolle über meine Bots verloren! Sie erkennen nun alle Personen in der DHSN als Eindringlinge. " +
				"Ihre Laserpointer wurden umprogrammiert, ich befürchte, sie können jetzt als Waffen eingesetzt werden ...",
				"🅐 - Weiter | 🅧 - Zurück",
				["A"],
				["X"],
				() =>
				{
					Player.PlayerInventory.RemoveGun();
				}
			),

			new TutorialStep(
				"Aber ich habe eine Idee. Mit meinen Replikatoren kann ich dich ebenfalls bewaffnen. " +
				"Ich habe dir eine Elektro-Pistole erstellt, du solltest sie jetzt an deinem Holster finden. " +
				"Nimm die Waffe in die Hand.",
				"Ziehe die Waffe mit 🅡❸ | 🅧 - Zurück",
				["Picked"],
				["X"],
				() =>
				{
					Player.PlayerInventory.SpawnGun();
					Player.PlayerInventory.RemoveMagazine();
				}
			),

			new TutorialStep(
				"Deine Pistole ist noch nicht geladen. Das musst du selbst übernehmen, dabei kann ich leider nicht helfen. " +
				"Nimm ein Batterie-Magazin mit 🅡❸ von deinem Gürtel und lade deine Elektro-Pistole. " + 
				"Mit 🅑 kannst du das Magazin wieder auswerfen.",
				"Lade die Waffe | 🅧 - Zurück",
				["Loaded"],
				["X"],
				() =>
				{
					Player.PlayerInventory.SpawnMagazine();
					_enemy.CurrentState = Enemy.EnemyState.Passive;
				}
			),

			new TutorialStep(
				"Bitte hilf mir, die infizierten Bots auszuschalten, bevor sie die gesamte DHSN übernehmen!",
				"Menü-Taste - Hauptmenü | 🅧 - Zurück",
				[],
				["X"],
				() =>
				{
					_enemy.CurrentState = Enemy.EnemyState.Aggressive;
				}
			)
		];
	}
	
	/// <summary>
	/// Starts (or restarts) the currently selected tutorial step.
	/// Displays the step text and button hint on the <see cref="Whiteboard"/>
	/// and executes the step’s optional start action if defined.
	/// </summary>
	private void StartCurrentStep()
	{
		var tutorialStep = _tutorialSteps[_currentStepIndex];
		
		if (Whiteboard == null) return;
		Whiteboard.SetMainText(tutorialStep.MainText);
		Whiteboard.SetButtonHint(tutorialStep.ButtonHint);
		
		tutorialStep.OnStepStart?.Invoke();
	}
	
	public override void OnPlayerButtonPressed(string buttonName)
	{
		if (_tutorialSteps == null || _currentStepIndex >= _tutorialSteps.Count) return;

		var tutorialStep = _tutorialSteps[_currentStepIndex];

		// Backward navigation
		if (tutorialStep.BackInputs.Contains(buttonName) && _currentStepIndex > 0)
		{
			_currentStepIndex--;
			StartCurrentStep();
			return;
		}

		// Forward navigation
		if (tutorialStep.NextInputs.Contains(buttonName) && _currentStepIndex < _tutorialSteps.Count - 1)
		{
			_currentStepIndex++;
			StartCurrentStep();
		}
	}

	public override void OnExit()
	{
		if (Player == null) return;
		Player.PlayerInventory.RemoveGun();
		Player.PlayerInventory.RemoveMagazine();
	}
}
