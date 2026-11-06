# スキル設定画面リファクタリング記録

## 目的と設計

スキル設定の入力処理、配置ルール、画面更新が複数の Handler に分散していたため、画面の調整役を `SkillSlotsController` に集約しました。同クラスは HomeScene 上のスキル設定画面に限る Presenter として扱い、Singleton にはしません。

```text
SymbolListHandler
  └─ OnSelectedSymbolChanged
       ├─ CurrentKaomojiHandler (顔文字・ステータス表示)
       └─ SkillSlotsController (スキル設定Presenter)
            ├─ SkillLoadoutService (配置・重複・コスト・解放ルール)
            ├─ CurrentSymbolHandler (現在のシンボル表示)
            ├─ SkillNameListHandler (一覧表示・選択入力)
            ├─ SkillSlotHandler[] (各スロット表示・入力)
            └─ SkillDescriptionHandler (説明表示)
```

シンボル選択後、Presenter が現在データを決め、スロット・一覧・説明の各 View を更新します。スキル一覧のボタン押下は `SkillNameListHandler.OnSkillSelected` 経由で Presenter に届き、選択中スロットがあれば `SkillLoadoutService` が妥当性を検証して `SymbolMouthData` を更新します。ロック中スロットの入力も Presenter に集約し、次に解放するスロットと所持金を確認してから確認パネルを表示します。

`CurrentKaomojiHandler` は同じシンボル選択イベントを引き続き購読しますが、これは顔文字とステータスを更新する別の画面責務です。スキル設定用 View 同士のイベント購読や直接参照はなくしました。

## AIによる変更箇所

| ファイル | 注釈と変更内容 |
|---|---|
| `Assets/Scripts/UI/Home/SkillSetting/SkillSlotsController.cs` | クラス注釈のとおり、選択イベントを一元処理する Presenter に整理。スキル一覧・スロット・説明・シンボル表示の調整、確認後の解放処理を担当。 |
| `Assets/Scripts/UI/Home/SkillSetting/SkillLoadoutService.cs` | クラス注釈のとおり新規作成。スキル配置可能判定、重複判定、占有スロット算出、次スロット解放とモデル更新を担当。 |
| `Assets/Scripts/UI/Home/Skill/SkillNameListHandler.cs` | クラス注釈のとおり一覧 View に限定。データを受けてボタンを生成し、選択をイベントで通知、Presenter から受けた判定結果で操作可否を反映。 |
| `Assets/Scripts/UI/Home/SkillSetting/CurrentSymbolHandler.cs` | クラス注釈のとおり表示専用に変更。選択イベントの購読やモデル更新は Presenter に移動。 |
| `Assets/Scripts/UI/Home/Skill/SkillDescriptionHandler.cs` | クラス注釈のとおり表示 API に限定。外部から Action を呼び出す通知方式を廃止し、Presenter が明示的に `SetSkillDescription` / `Clear` を呼ぶ。 |
| `Assets/Scripts/UI/Home/SkillSetting/SkillSlotHandler.cs` | クラス注釈で表示・入力 View と明示。所持金判定を Presenter / Service 側へ移し、View はロックボタンの有効状態を描画するだけに変更。初期化順も修正。 |
| `Assets/Scripts/Kaomoji/SymbolMouthData.cs` | `AddSkillData` の AI リファクタリング注釈のとおり、指定スロット番号まで空枠を補ってから代入し、リスト位置とスロット番号を一致させる。 |
| `Assets/Scripts/UI/Home/KaomojiBuild/CurrentKaomojiHandler.cs` | 説明表示をクリアする箇所に注釈。外部 Action 呼び出しを明示的な `Clear()` 呼び出しへ変更。 |
| `Assets/Scenes/HomeScene.unity` | Presenter 注釈のとおり依存参照を `SkillSlotsController` に集約。旧 Handler 間参照を除き、一覧 View とテスト用スキルを Presenter に配線。 |

## 設計理由とメリット

- **依存の方向を揃える:** シンボル一覧から Presenter に入力が届き、Presenter が View とサービスを呼び出します。スキル一覧・現在シンボル表示が相互に別 Handler を操作しないため、変更時に追う経路が明確になります。
- **ルールを一箇所に置く:** 配置可能範囲、重複、コストによる占有、次のスロット解放を `SkillLoadoutService` にまとめ、UI 表示ロジックとの重複を避けます。
- **モデルを正として画面を再描画する:** スキル割り当て後にモデルを更新し、Presenter がスロットとボタン可否を再計算します。UIだけ先に更新される経路をなくします。
- **スロット番号のずれを防ぐ:** 空のリストへ index 2 のデータを入れる場合にも index 0・1 の空枠を用意します。保存位置と表示スロットが一致します。
- **テストしやすくする:** 配置・解放のルールは Unity UI に依存しない C# サービスに分離され、Presenter/View の操作と分けて検証できます。
- **Inspector 配線を減らす:** 画面内の Handler が互いに依存するのではなく、Scene の Presenter から各 View へ参照を集約します。どの画面に属する依存かも見やすくなります。

## ルール上の明確化

- スキルの占有コストは配置判定と表示で共通して `1` 以上として扱います。従来は配置時と占有表示時でコスト 0 の扱いが異なっていました。
- スロット解放は現在の解放数と一致する「次のスロット」のみに許可します。別のロック枠を押して、解放数と表示状態が食い違うことを防ぎます。
- 装備状態の保存先は今回変更していません。`SymbolMouthData` の既存データをサービス経由で更新します。永続化方式を独立ランタイムモデルへ移す作業は、セーブデータや戦闘側の読み取りを含む別の変更として扱います。
- HomeScene の `WallReflection` 登録は既存のテスト用設定を維持し、Presenter に移しました。利用可能スキル一覧の本番カタログ化は今回の範囲外です。
