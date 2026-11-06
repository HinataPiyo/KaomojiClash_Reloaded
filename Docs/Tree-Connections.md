# UGUIツリー接続線

`UI.TreeConnectionGraphic` は `MaskableGraphic` として複数の線を1つのメッシュにまとめます。`UI.TreeConnection` が接続元・接続先、表示、個別色を保持し、ゲームの解放状態そのものは保持しません。線は各Buttonの `RectTransform.rect.center` 同士を結びます。

## 変更内容と理由

| 対象 | 変更内容 | 理由・注釈 |
| --- | --- | --- |
| [TreeConnectionGraphic.cs](../Assets/Scripts/UI/Home/TreeConnections/TreeConnectionGraphic.cs) | `TreeConnection` と `TreeConnectionGraphic` を追加 | Inspectorで複数のButton間接続を設定し、1つのUGUIメッシュとして描画するため。実装上の重要な箇所には意図をコメントで記載しています。 |
| [UI.TreeConnections.asmdef](../Assets/Scripts/UI/Home/TreeConnections/UI.TreeConnections.asmdef) | 描画コンポーネントのアセンブリ定義を追加 | 他のUIから利用できるようにしつつ、テストアセンブリから参照できるようにするため。 |
| [TreeConnectionGraphicTests.cs](../Assets/Tests/Editor/TreeConnections/TreeConnectionGraphicTests.cs) | EditModeテストを追加 | 描画位置の追従、分岐、接続の操作、レイアウト変更、不正な配置などを検証するため。 |
| [UI.TreeConnections.EditorTests.asmdef](../Assets/Tests/Editor/TreeConnections/UI.TreeConnections.EditorTests.asmdef) | Editor専用テストアセンブリを追加 | Unity Test Runnerから対象のテストを実行するため。 |
| シーン・既存スクリプト | この機能のための変更なし | 既存の強化用Canvasは準備段階であり、接続対象や配置を決め打ちしないため。Editorでの設定が必要です。 |

上記の追加アセットにはUnity用の `.meta` ファイルも付属します。

## Editorでの設定

既存の強化用Canvasは準備段階のため、シーンやPrefabには自動配置していません。

1. 対象のCanvas内にツリーの親 `TreeRoot` を用意します。Button群と線は同じCanvasの同一平面上に置きます。
2. `TreeRoot` の子として空のUIオブジェクト `ConnectionLines` を作り、RectTransformのアンカーを上下左右にストレッチし、余白を0にします。ツリー全体を覆う大きさにしてください。
3. `ConnectionLines` をButton群よりHierarchyの前に配置します。例えば `TreeRoot` の最初の子を `ConnectionLines`、次の子をButton群の親 `Nodes` にします。GraphicやImageを持つ不透明な背景は線より前に配置してください。別Canvasのソート順やWorld Spaceの奥行きによってはHierarchyだけで前後関係を保証できません。
4. `ConnectionLines` に **Tree Connection Graphic** を追加し、`Line Width` とGraphicの `Color` を設定します。太さはこのRectTransformのローカル単位です。線はRaycastを受け取りません。
5. `Connections` のサイズを増やし、各要素の `Source` と `Destination` にButtonコンポーネントを割り当てます。`Visible` と個別の `Color` も設定できます。最終色はGraphicの `Color` と個別色の乗算です。1つのButtonを複数の要素の `Source` に指定できます。

例:

| 要素 | Source | Destination | Visible | Color |
| --- | --- | --- | --- | --- |
| 0 | RootButton | AttackButton | true | 白 |
| 1 | RootButton | DefenseButton | true | 白 |
| 2 | AttackButton | CriticalButton | true | 白 |

これで Root→Attack、Root→Defense、Attack→Critical の3本が描画されます。線の端はButtonの中心なので、Buttonの背景が不透明ならButtonの下に隠れます。

## 実行時の操作例

```csharp
using UI;
using UnityEngine;
using UnityEngine.UI;

public class TreeConnectionExample : MonoBehaviour
{
    [SerializeField] TreeConnectionGraphic lines;
    [SerializeField] Button rootButton;
    [SerializeField] Button attackButton;
    [SerializeField] Button defenseButton;

    void Start()
    {
        TreeConnection attack = lines.AddConnection(rootButton, attackButton);
        lines.AddConnection(rootButton, defenseButton);
        lines.SetConnectionColor(attack, Color.green);
        lines.SetConnectionVisible(attack, true);
    }
}
```

Inspector設定済みの接続は `GetConnection(index)` で取得し、`SetConnectionColor` / `SetConnectionVisible` で変更できます。動的に追加した接続は戻り値を保持し、不要になったら `RemoveConnection` で削除できます。存在しない接続に設定しようとした場合などは例外で通知します。Inspector設定とコード追加を併用する場合は同じ接続を重複登録しないようにしてください。

## 更新と制約

- `Canvas.willRenderCanvases` でレイアウト・通常のGraphic再構築の後に接続先の中心位置を比較し、変化があった場合のみその描画前にメッシュを再生成します。`Update` は使用しませんが、Canvas描画時の位置チェックは接続数に比例します。Canvasが再描画されるたびにチェックが行われます。
- Overlay、Camera、World Spaceともに座標はスクリーン投影ではなく共通Canvasのローカル平面で変換します。CameraモードではCanvasのRender Cameraを、World Spaceでは適切な描画・イベント用カメラを設定してください。異なるCanvas間、別平面や奥行きの異なるButton間の接続は対象外で、Consoleに警告を出します。マスクを使う場合は線とButtonに同じマスク範囲が適用される階層に置いてください。
- Buttonが非アクティブまたは無効な間は接続線を隠します。`interactable` の変更だけでは隠しません。参照切れは警告し、他の有効な接続は描画します。同一位置のButton間は長さ0のため描画しません。
- 1つのGraphicは最大16000接続です。超過時は警告し、先頭16000件だけ描画します。さらに必要な場合はGraphicを分割してください。
- RectTransformが描画対象を覆わない場合、`RectMask2D` によるカリングなどで線が期待通りに見えないことがあります。線の幅はCanvas Scalerや親のスケールの影響を受けます。

## 拡張

解放状態・選択状態と接続の対応は別のコントローラーに持たせ、状態が変わったタイミングで表示や個別色を更新します。共通色の乗算に注意してください。アニメーションはそのコントローラーから必要な期間だけ色や太さを変えるか、描画クラスを拡張します。接続線をゲームデータの正本にせず、状態を復元できるようにしてください。曲線や端点をButtonの外周に合わせる場合はメッシュ生成と位置キャッシュを合わせて変更します。

## テスト

Unity Test RunnerのEditModeで `UI.TreeConnections.EditorTests` を実行します。3種類のRender Mode、位置変更、分岐、色・表示・削除、Buttonの無効化・破棄、レイアウト更新を検証します。

前回実装時はUnity 6000.3.20f1の分離プロジェクトで8件すべて成功しました。これはコンポーネントの挙動を検証した結果であり、実際のシーンのカメラ配置、Canvasソート、マスクや見た目を確認したものではありません。対象シーンへの配置後にも確認してください。
