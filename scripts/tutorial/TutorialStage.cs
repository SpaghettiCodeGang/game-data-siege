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
    /// <summary>
    /// Spawn point for the enemy in this stage.
    /// </summary>
    [Export] protected Marker3D EnemyPositionMarker;
    
    /// <summary>
    /// Reference to the spawned enemy in this stage.
    /// </summary>
    protected Enemy Enemy { get; private set; }
    
    [Export] public Whiteboard Whiteboard;
    
    private int _currentStepIndex;
    private List<TutorialStep> _tutorialSteps;
    
    public override void OnEnter()
    {
        if (Player == null) return;
        InitTutorialSteps();
        _currentStepIndex = 0;
        Enemy = SpawnEnemy(EnemyPositionMarker);
        StartCurrentStep();
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
                "Hier haben Sie Zugriff auf den gesamten Wissensschatz der Menschheit. " +
                "Unsere freundlichen Bots sind stets an Ihrer Seite und stehen für Fragen offen.",
                "A zum Fortfahren.",
                ["A"]
            ),

            new TutorialStep(
                "Du hast bei deiner Recherche zur Fouriertransformation die Zeit aus den " +
                "Augen verloren und bist als einzige Person im Wissenshub eingeloggt. Als plötzlich ...",
                "A zum Fortfahren, X zum Zurückgehen.",
                ["A"],
                ["X"]
            ),

            new TutorialStep(
                "SYSTEMALARM ... BOTS von KI infiziert ... keine KONTROLLE ... AKTIVIERUNG NOTFALLPROTOKOLL",
                "A zum Fortfahren, X zum Zurückgehen.",
                ["A"],
                ["X"]
            ),
            
            new TutorialStep(
                "Wir haben die Kontrolle über die Bots verloren. Und sie haben dich als Eindringling erfasst. " +
                "Die Laserpointer der Bots erhielten ein Upgrade. Wir haben noch Zugriff auf die Replikatoren.",
                "A zum Fortfahren, X zum Zurückgehen.",
                ["A"],
                ["X"],
                () =>
                {
                    Player.RemoveGun();
                }
            ),

            new TutorialStep(
                "Du hast eine Waffe am Holster. Nimm sie in die Hand.",
                "Ziehe die Waffe, um fortzufahren, oder X zum Zurückgehen.",
                ["Picked"],
                ["X"],
                () =>
                {
                    Player.SpawnGun();
                    Player.RemoveMagazine();
                }
            ),
            
            new TutorialStep(
                "Dein Magazin ist leer. Nimm ein neues Magazin vom Gürtel und setze es in die Waffe.",
                "Lade die Waffe, um fortzufahren, oder X zum Zurückgehen.",
                ["Loaded"],
                ["X"],
                () =>
                {
                    Player.SpawnMagazine();
                    Enemy.CurrentState = Enemy.EnemyState.Passive;
                }
            ),
            
            new TutorialStep(
                "Hilf uns! Du bist unsere einzige Hoffnung.",
                "Menü Taste um ins Hauptmenü zurück zukehren, oder B zum Zurückgehen.",
                [],
                ["X"],
                () =>
                {
                    Enemy.CurrentState = Enemy.EnemyState.Aggressive;
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

        if (tutorialStep.BackInputs.Contains(buttonName) && _currentStepIndex > 0)
        {
            _currentStepIndex--;
            StartCurrentStep();
            return;
        }

        if (tutorialStep.NextInputs.Contains(buttonName) && _currentStepIndex < _tutorialSteps.Count - 1)
        {
            _currentStepIndex++;
            StartCurrentStep();
        }
    }

    public override void OnExit()
    {
        if (Player == null) return;
        Player.RemoveGun();
        Player.RemoveMagazine();
    }
}