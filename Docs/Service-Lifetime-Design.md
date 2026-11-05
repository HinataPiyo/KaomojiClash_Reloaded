# Singleton による依存先管理

## 方針

`ApiProvider` を廃止し、シーンやゲーム全体で一意に扱うサービスの取得方法を `SceneSingleton<T>.Instance` に統一します。呼び出し側からは、各具象クラスの静的な `Instance` を取得します。

例:

```csharp
IStage stage = StageController.Instance;
IAudioManager audio = AudioManager.Instance;
```

ローカル変数やメソッド引数では既存のインターフェース型を維持し、具体実装への依存を呼び出し境界に限定します。同一のクラスが複数インターフェースを実装する場合（例: `WaveController` は `IWave` と `IBattleState`）も、Singleton の入口はクラスごとに1つです。

## Singleton を採用する理由

- `ApiProvider` の登録漏れと、登録されるまで `Get` できない順序依存をなくします。インスタンスの所有者と取得口がクラス上で明らかになります。
- 各サービスはプロジェクト上でシーンまたはゲーム全体に1つだけ必要なため、`Instance` による一意性と重複検出が実態に合います。
- Unity の Inspector で全参照を配線しなくても取得でき、既存の呼び出しパターンを大きく変えずに単一の仕組みにできます。

一方で Singleton は呼び出し側を具象クラスに結びつけ、テスト用実装の差し替えを難しくします。局所的なオブジェクトや複数存在するコンポーネントには適用せず、引数や Inspector 参照を使います。インターフェース型で依存を保持することで利用側の変更範囲は抑えますが、Singleton 自体が依存関係を隠す性質は残るため、全クラスを Singleton 化しません。

## 適用範囲

### シーン内 Singleton

シーン内に1つだけ配置し、シーン遷移時に破棄するサービスは `SceneSingleton<T>` を継承します。

- 戦闘制御: `WaveController`、`StageController`、`EnemySpawn`、`PlayerSpawn`、`EnemyExpFactory`、`HitStop`、`CameraController`
- 戦闘状態・選択: `Combo`、`ExpHandler`、`ArenaObjectController`
- シーンに属する UI: `LetterBoxAnimation`、`ResultUIHandler`、`WorldCanvasController`、`ComboUI`、`RemainingTimeUI`、`StartWaveAnimationUI`、`ArenaObjectSelectUI`、`ArenaObjectEditModeUI`、`PlayerEXPUI`、`PlayerStaminaUI`

これらの初期化と破棄時の処理は `OnSingletonAwake` / `OnSingletonDestroy` に実装します。Unity の `Awake` / `OnDestroy` を派生クラス側で隠さないでください。

### ゲーム全体の永続 Singleton

`AudioManager` と `SceneChange` はシーン遷移後も必要なので、同じ `SceneSingleton<T>` を使いながら `PersistAcrossScenes` を `true` にし、`OnSingletonAwake` で `DontDestroyOnLoad` を呼びます。シーン内 Singleton と同じアクセス方法・重複管理を使い、寿命だけを明示的に延長します。

## ライフサイクルと制約

- `SceneSingleton<T>` は同じシーン内の重複を警告して破棄します。永続 Singleton はシーンを越えても重複しません。
- 非同期シーン遷移中に旧・新のシーンインスタンスが一時的に共存すると、新しいシーンの Singleton が `Instance` となります。旧インスタンスの破棄時に、新しい `Instance` は消去されません。
- この設計は同じサービスを複数の同時アクティブシーンから個別に取得する用途には向きません。加算ロードした各シーンに独立した同名サービスが必要になった場合は、Singleton ではなくシーン単位の依存コンテキストを使います。
- `Instance` は `Awake` 実行後に利用できます。依存先を `Awake` 内で取得する場合、Unity の実行順に依存しない初期化が必要です。既存コードでは依存の取得は主に `Start` で行っています。

## 新しいクラスを追加する基準

1. シーン内またはゲーム全体で一意なサービスであれば `SceneSingleton<T>` を継承し、`T` は自分のクラスにします。
2. シーン間で保持する必要があるときだけ `PersistAcrossScenes` をオーバーライドし、`OnSingletonAwake` から `DontDestroyOnLoad` を呼びます。
3. 依存する側は `SomeService.Instance` から取得します。アクセスをローカル変数や引数でインターフェース型に保持すると、処理内の結合を抑えられます。
4. シーン内の通常のオブジェクトや複数存在し得るコンポーネントは Singleton 化せず、Inspector 参照または引数で渡します。
