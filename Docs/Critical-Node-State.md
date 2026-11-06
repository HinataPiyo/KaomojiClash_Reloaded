# Criticalノード状態

`NodeBase` はノード表示オブジェクトに付け、`Node ID` は全ノードで重複しない固定値にします。同じNodeオブジェクトを複数の記号に使い回すため、解放・活性状態は表示オブジェクトではなく `NodeProgressionDatabase` ScriptableObjectに記号ごとに保存します。

## セットアップ

1. Projectウィンドウで **Create > KaomojiClash_Reloaded > Node Progression Database** を選び、状態SOを作ります。
2. `NodeController` の `Progression Database` にSOを割り当てます。`Nodes` は空なら子階層から取得します。
3. 各 `NodeBase` にButtonと固定の `Node ID` を割り当てます。初期状態を指定する場合は `Unlocked On Start` / `Active On Start` を設定します。活性ノードは解放済みである必要があります。
4. `CriticalUIHandler` と `NodeController` を同じGameObjectに配置します。目の記号を選択するとCriticalUIHandlerがイベントを受け取り、選択記号・Criticalパラメーター・その記号に紐づくノード状態を更新します。初回はNodeBaseの初期状態がその記号の状態として作られ、次回以降はSOから復元されます。

ActivateButtonは、解放済みかつ未活性のノードを選択中の場合のみ押せます。ノードを押すと選択され、ActivateButtonを押すと活性化されます。`CriticalRateNode` はActivateごとにCriticalRateを0.025（2.5%）上げ、`CriticalDamageNode` は0.15（15%）上げます。値は `CriticalStats` の最大値を超えません。活性済みノードをクリックし直すとActivateButtonは無効になります。

活性化時には `NodeController` が `TreeConnectionGraphic` の接続先を一度構築した隣接リストから取得し、直後に接続された全ノードを解放します。接続の追加・削除もグラフィックの変更イベントを通じて隣接リストへ反映されます。接続の `Visible` は描画設定であり、進行接続の有無には影響しません。

状態表示にはDescriptionの後ろに `(Locked)` / `(Unlocked)` / `(Active)` が付きます。活性ノードは同時に1つまでで、ロックすると活性・選択も解除されます。`TryUnlockNode` / `TryLockNode` / `TrySelectNode` / `TryActivateSelectedNode` を使ってController経由で状態を変更してください。Nodeの `StateChanged`、Controllerの状態イベントを購読して追加のViewを更新できます。

このSOの状態変更は実行中メモリ内だけです。ビルド済みゲームの終了後も保持するセーブ機能は含みません。
