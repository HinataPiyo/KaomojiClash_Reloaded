# ステータス表示依存関係のリファクタリング記録

## 目的

Home画面のステータス表示で、各Viewが `PlayerStatsCalculator` を直接参照していた依存を解消する。Presenterや追加のサービス層は設けず、表示更新の呼び出し順を保ったまま、計算結果を共有する小さなデータ境界を作る。

## 変更後の依存とデータフロー

```text
CurrentKaomojiHandler
  ├─ PlayerStatsCalculator.GetStatusDisplayData()
  │    └─ StatusDisplayData[]
  └─ OnCurrentKaomojiUpdated(CurrentKaomojiUpdate)
       └─ CurrentKaomojiEventListener
            ├─ StatusRaderChartView
            ├─ BaseStatusView
            ├─ SkillDescriptionHandler
            └─ CurrentEquipedSkillHandler
```

`PlayerStatsCalculator` が表示に必要な値を一度組み立て、読み取り専用のイベントデータ `CurrentKaomojiUpdate` にまとめて購読側へ通知する。`CurrentKaomojiEventListener` が購読・解除のライフサイクルをまとめ、派生クラスは更新処理だけを担当する。

## 変更箇所とコード内注釈

| ファイル | 注釈 | 変更内容 |
|---|---|---|
| `Assets/Scripts/Kaomoji/StatCalculator/StatsCalculator.cs` | `// [設計変更] 計算結果をViewへ渡す共通データ。` | `StatusDisplayData` を追加。種別、基本値、現在値、最小値、最大値だけを持ち、UIやUnityオブジェクトに依存しない値型とした。 |
| `Assets/Scripts/Kaomoji/StatCalculator/PlayerStatsCalculator.cs` | `// [設計変更] 表示に必要な値をここで一括化し、` | `GetStatusDisplayData()` を追加。基本値、レーダー値、範囲を一度に収集する。既存の戦闘用計算APIは変更していない。 |
| `Assets/Scripts/UI/Home/BaseStatusView.cs` | `// [設計変更] イベント受信を表示処理の入口にし、` | 共通イベントリスナーから読み取り専用スナップショットを受け取り、基本値・クリティカル値を描画する。未使用だった表示インターフェースと転送メソッドは追加しない。 |
| `Assets/Scripts/UI/Home/StatusChart/StatusRaderChartView.cs` | `// [設計変更] イベント受信をチャート描画の入口にし、` | 共通イベントリスナーから直接更新を受け、スナップショットをラベルと `RadarStatus[]` に反映する。表示順・パーセント表記・チャート範囲を維持する。 |
| `Assets/Scripts/UI/Home/KaomojiBuild/CurrentKaomojiHandler.cs` | `// [イベント化]` | 表示スナップショットをイベント経由で購読側へ通知する。初期表示と選択後の表示更新を同じ経路にする。 |

## イベント購読への追加変更

`CurrentKaomojiHandler` の通知を `event Action<CurrentKaomojiUpdate>` にまとめた。イベントデータは顔文字データと `IReadOnlyList<StatusDisplayData>` を公開し、将来の通知項目追加でイベント引数を増やさずに済む。`CurrentKaomojiEventListener` が購読登録、null参照の診断、無効化時の解除を一元化し、派生クラスは `OnCurrentKaomojiChanged` の実装だけを持つ。`Start` の初期表示も通知するため、初期状態と選択後の経路が一致する。

| ファイル | 注釈と変更内容 |
|---|---|
| `Assets/Scripts/UI/Home/KaomojiBuild/CurrentKaomojiHandler.cs` | `// [イベント化]` 注釈のとおり、ステータスViewとスキル説明への直接参照を削除。初期化および選択後に顔文字データと表示スナップショットをイベント通知する。誤って自身の更新メソッドを自身のイベントへ登録していた処理も除去。 |
| `Assets/Scripts/UI/Home/KaomojiBuild/CurrentKaomojiHandler.cs` | `// [設計変更] 顔文字更新のイベント値をまとめ、` 注釈のとおり、`CurrentKaomojiUpdate` と共通の `CurrentKaomojiEventListener` を追加。後者が購読寿命を管理する。 |
| `Assets/Scripts/UI/Home/BaseStatusView.cs` | `CurrentKaomojiEventListener` を継承し、通知データでステータスを描画。イベント登録・解除の重複コードを削除。 |
| `Assets/Scripts/UI/Home/StatusChart/StatusRaderChartView.cs` | 共通リスナーを継承し、通知データからチャートを更新。`IReadOnlyList` を使いデータ変更APIを公開しない。 |
| `Assets/Scripts/UI/Home/Skill/SkillDescriptionHandler.cs` | 共通リスナーを継承し、顔文字変更時に説明をクリア。更新処理以外のイベントコードを削除。 |
| `Assets/Scripts/UI/Home/KaomojiBuild/CurrentEquipedSkillHandler.cs` | 共通リスナーを継承し、通知された顔文字データから装備中の口データを読む。ラムダと購読寿命コードをなくし、口データがない場合は一覧を空にする。 |
| `Assets/Scripts/UI/Home/SkillSetting/SkillSlotsController.cs` | `// [イベント化]` 注釈のとおり、顔文字更新イベントで行う説明クリアの重複呼び出しを除去。スロット選択・ロック時の説明クリアは引き続きPresenterが担当する。 |
| `Assets/Prefabs/UI/BaseStatusUI.prefab` / `Assets/Prefabs/UI/StatusRadarChart UI.prefab` / `Assets/Prefabs/UI/CurrentKaomoji UI.prefab` / `Assets/Scenes/HomeScene.unity` | イベント発行元の参照を各ViewのPrefabインスタンスに割り当て、発行側からの旧View参照を削除。HomeScene の2組の表示がそれぞれ対応する発行元を購読する。スキル説明Viewもシーン上で発行元を参照する。 |

## 設計判断

- **最小の境界を追加:** 同じ更新通知を受け取るUI群には専用の共通基底クラスを使う。汎用Presenterやイベント基盤は追加せず、購読処理を一箇所にまとめて実装・Inspector配線・保守コストを抑える。
- **計算機を表示のデータ源とする:** 基本値やステータス上限はすでに計算機が知っているため、Viewに同じ情報取得ロジックを置かない。
- **用途別の値を区別:** `BaseValue` は基本ステータス表示用、`Value` はレーダー表示用。クリティカル値はこれまで通り基本値表示とレーダーの両方に使う。
- **互換性を限定的に維持:** 戦闘ロジックが利用する `GetSpeed()` 等は触れず、Home画面の表示用経路だけを変更する。
- **範囲不一致を見逃さない:** Viewに設定されたラベル数がデータ数と異なる場合は警告し、存在する要素の範囲内で描画する。
- **購読寿命を管理:** Viewは有効化時に購読し、無効化時に解除するため、シーン内オブジェクトの破棄・再有効化で購読が重複しない。
- **変更に強い通知契約:** 1引数の読み取り専用イベントデータに通知情報を集約し、イベントのシグネチャを安定させる。共通基底クラスは同一イベントを購読するUIのみに限定し、汎用イベント基盤や不要なジェネリック抽象化は追加しない。

## 今回の対象外

- `CurrentKaomojiHandler` は顔文字テキスト表示と計算機を保持し、画面固有データを作成する。各表示コンポーネントへの参照はイベント購読へ移した。汎用Presenterは追加していない。
- `EnemyStatsCalculator` への共通表示API導入。敵ステータス表示の利用箇所が見当たらず、現時点で抽象化を広げない。
- キャッシュ、イベント/Rx、追加のフォーマッター、ステータス列挙型の再設計。更新頻度・プロファイル上の必要性が確認されていない。

## 検証

- `dotnet build Assembly-CSharp.csproj --no-restore` でコンパイルを確認する。
- Home画面で、ステータスの基本値、パーセントラベル、レーダーチャート、シンボル選択後の再描画を確認する。Unity Editorでの画面確認はコードビルドだけでは代替できない。
