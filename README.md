
# 🎮 Data Siege

**Data Siege** ist ein 3D VR-Game, entwickelt mit **Godot 4** und **C#**.

---

## 🔗 Project-Links
- **[Project-Board (Jira)](https://intermedien.atlassian.net/jira/software/projects/INTMED/list)**
- **[Project-Repository (GitHub)](https://github.com/SpaghettiCodeGang/game-data-siege)**

---

## 🧰 Voraussetzungen

- **Git** (mit Git LFS aktiviert)
> [Installationsanleitung Git](https://git-scm.com/downloads)  
> [Installationsanleitung Git LFS](https://git-lfs.com/)

- **Godot 4.x (Mono-Version)** → für C# benötigt ihr **Godot mit .NET-Unterstützung**
> [Download Godot .NET](https://godotengine.org/download)

> 💡 Hinweis: Für C# wird zusätzlich das **.NET SDK benötigt**.
> - Wenn Godot über den **offiziellen Download** bezogen wird, muss das SDK separat installiert werden: [Download .NET SDK](https://dotnet.microsoft.com/en-us/download).
> - Manche Linux-Pakete installieren das SDK automatisch als Abhängigkeit (z. B. `godot-mono` über Paketmanager).

- **IDE** (empfohlen: [JetBrains Rider](https://www.jetbrains.com/rider/) oder [Visual Studio Code](https://code.visualstudio.com/) mit C#-Plugin)

---

## ⚙️ Setup

### 1. Projekt clonen
```bash
  git clone git@github.com:SpaghettiCodeGang/game-data-siege.git
  cd game-data-siege
```

### 2. Git LFS einrichten (nur beim ersten Mal)
```bash
  git lfs install
  git lfs pull
```

### 3. Projekt in Godot öffnen
- Godot starten → **Project → Import** → Projekt-Ordner auswählen
- Editor lädt alle Szenen und Assets automatisch

### 4. C# Build vorbereiten
Beim ersten Öffnen kompiliert Godot die C#-Assemblies. Falls Fehler auftreten:
```bash
  dotnet build
```

---

## 🛠️ Entwicklung

- Den aktuellen `main`-Branch pullen
- Einen neuen `feature`-Branch aus `main` erstellen
- Implementieren, testen, committen
- Regelmäßig pushen
- ✅ Pull Request stellen, wenn fertig
- 🧃 Spaß haben (stay hydrated 😉)

---

## 📦 Addons & Plugins
Alle benötigten **Godot-Addons** liegen im Repo unter `res://addons/` und sind bereits versioniert.  

---

## 🧩 Assets
- Alle großen Dateien (Bilder, Sounds, Modelle) laufen über **Git LFS**.
- Beim ersten Pull werden die Assets automatisch heruntergeladen.
- Wenn neue Dateitypen auftauchen (z. B. ein neues Audioformat), muss **einmalig** `git lfs track` dafür ausgeführt werden und die `.gitattributes` angepasst werden.

---

## 🌿 Git-Konventionen

### Branches
- Neue Branches werden immer vom `main`-Branch erstellt.
- Vor dem Abzweigen sollte ein aktueller Pull durchgeführt werden:
```bash
  git pull origin main
```
- Branch-Namen folgen dem Schema:
```
Projektname-Ticketnummer (Jira)
```
> Beispiel: `INTMED-30`

### Commits
- Commit-Messages bestehen aus einem **Prefix**, das den Typ der Änderung angibt, gefolgt von einer kurzen Beschreibung.

| Typ         | Beschreibung                                                                 |
|-------------|------------------------------------------------------------------------------|
| `feat:`     | Neue Features oder Funktionalität, die dem Nutzer zur Verfügung stehen       |
| `fix:`      | Bugfixes – behebt Fehler im bestehenden Verhalten                            |
| `docs:`     | Änderungen an Dokumentation (README, Wiki, Kommentare, etc.)                 |
| `style:`    | Änderungen an Formatierung, Whitespaces, Linting – **ohne** Code-Logik       |
| `refactor:` | Code-Umstrukturierungen **ohne Änderung** des Verhaltens                     |
| `test:`     | Neue oder angepasste Tests (Unit, Integration, etc.)                         |
| `chore:`    | Wartung, Build-Tools, CI/CD, Dependency-Updates – alles „Drumherum“          |

### Beispiele:
```
feat: add user registration endpoint
fix: correct email validation regex
docs: update JavaDoc in UserService
chore: add Dockerfile for production
```

---

## 📐 Code-Konventionen (C#)

Damit der Code konsistent bleibt, halten wir uns an die folgenden Regeln:

### Sprache
- Alle **Klassen-, Methoden- und Variablennamen** sind auf **Englisch**.
- Namen sind **aussagekräftig** und beschreiben ihre Funktion oder Bedeutung.

### Formatierung
- **Klammern** von Funktions- und Kontrollblöcken beginnen **auf der nächsten Zeile** (Allman-Style).
```csharp
public void DoSomething()
{
	if (condition)
	{
		// code
	}
}
```

- **Einrückung:** 1 Tab = 4 Leerzeichen, Rider/VS Code automatisch konfigurieren.
- Maximal 120 Zeichen pro Zeile.

### Benennungen
- **Klassen, Methoden, Enums:** PascalCase
```csharp
public class PlayerController { }
public void MoveForward() { }
public enum GameState
{
	MainMenu,
	Playing,
	Paused
}
```

- **public und protected Variablen & Felder:** PascalCase
```csharp
public string UserName;
[Export] public Node3D LeftMagBox;
[Export] public PackedScene MagazineScene;
```

- **private Variablen & Felder:** camelCase beginnend mit `_`:
```csharp
private int _currentScore;
```

- **Konstanten:** UPPER_CASE mit `_` als Trenner
```csharp
public const int MAX_HEALTH = 100;
```

### Struktur
- Eine Klasse pro Datei, Dateiname = Klassenname.
- Methoden möglichst **kurz und klar**, Single Responsibility.
- Kommentare, wenn nötig – der Code soll sich überwiegend selbst erklären.

### Godot-spezifisch
- **Node-Referenzen** als Felder deklarieren, wenn sie mehrfach genutzt werden.
```csharp
private Sprite3D playerSprite;

public override void _Ready()
{
	playerSprite = GetNode<Sprite3D>("PlayerSprite");
}
```

- **Signale** klar benennen, z. B. `OnPlayerDied`.
- Kein „magischer Code“ in `_Process` – stattdessen Methoden extrahieren.

---
