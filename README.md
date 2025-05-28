# Blazor 2048 - 数字パズルゲーム

![Blazor 2048](https://github.com/user-attachments/assets/17f12646-06fe-4fe3-be4f-00b3a3ec856d)

Blazor 2048 は、[Blazor WebAssembly](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor) を使用して構築されたクラシックな 2048 パズルゲームです。  
キーボードまたはスワイプ操作でタイルを移動し、同じ数のタイルを結合しながら **2048 のタイルを目指します！**

---

## **🎯 特徴**
✅ **Blazor WebAssembly** を使用したシングルページアプリケーション (SPA)  
✅ **C# 12** の最新機能 (`readonly struct`, `Span<T>`, `primary constructor`) を活用  
✅ **オブジェクト指向設計 (OOP) & SOLID 原則** に基づいた高可読性のコード  
✅ **キーボード & スワイプ対応**（PC & モバイル対応）  
✅ **スコアシステム** を実装  

---

## **📂 プロジェクト構成**
```
Blazor2048/
│── Pages/                # Blazor UI コンポーネント
│   ├── Game.razor        # 2048 の UI & 操作処理
│
│── Core/                 # ゲームのコアロジック
│   ├── Board.cs          # タイルの状態を管理
│   ├── Tile.cs           # タイルの合成処理
│   ├── Direction.cs      # 移動方向の Enum
│
│── GameLogic/            # ゲームの進行管理
│   ├── GameManager.cs    # ゲームのスコア & 進行管理
│
│── wwwroot/              # CSS, JavaScript など
│── Program.cs            # Blazor アプリのエントリポイント
│── App.razor             # Blazor アプリのルートコンポーネント
│── README.md             # このファイル
```

---

## **🛠 セットアップ**

### **🔹 必要条件**
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) 以上
- [Visual Studio Code](https://code.visualstudio.com/) または [Visual Studio](https://visualstudio.microsoft.com/)
- Web ブラウザ (Google Chrome, Edge, Firefox など)

### **🔹 インストール**
1. **リポジトリをクローン**
   ```sh
   git clone https://github.com/your-username/Blazor2048.git
   cd Blazor2048
   ```

2. **依存関係をインストール**
   ```sh
   dotnet restore
   ```

3. **アプリケーションを実行**
   ```sh
   dotnet run
   ```

4. **ブラウザで開く**
   - `http://localhost:5267` にアクセス

---

## **🎮 遊び方**
### **🔹 キーボード操作**
| 操作     | キー              |
| -------- | ----------------- |
| 上へ移動 | `↑ (Arrow Up)`    |
| 下へ移動 | `↓ (Arrow Down)`  |
| 左へ移動 | `← (Arrow Left)`  |
| 右へ移動 | `→ (Arrow Right)` |

### **🔹 モバイル（スワイプ）**
| 操作     | ジェスチャー |
| -------- | ------------ |
| 上へ移動 | 上にスワイプ |
| 下へ移動 | 下にスワイプ |
| 左へ移動 | 左にスワイプ |
| 右へ移動 | 右にスワイプ |

### **🔹 ルール**
1. **同じ数のタイルがぶつかると合成される。**
2. **スライドするたびに新しいタイル（`2` or `4`）が追加される。**
3. **タイルを動かせなくなるとゲームオーバー！**
4. **`2048` のタイルを作れば勝利！**

---

## **📌 設計のポイント**
### **1️⃣ `Board.cs`（ボードの管理）**
- **タイルの状態を保持**（`int[,] Tiles`）
- **タイルの移動処理 (`MoveTiles`)** を `Dictionary<Direction, Func<int, int[]>>` でシンプルに管理
- **新しいタイル (`AddNewTile`) をランダムに追加**
- **`Span<T>` を活用して `AreBoardsEqual()` を最適化**

### **2️⃣ `Tile.cs`（タイルの合成処理）**
- **タイルのマージ (`MergeLine`) を `Board` から分離**
- **合成の際にスコアを計算**
- **C# 12 の `List[^1]`（最後の要素）を活用**

### **3️⃣ `GameManager.cs`（ゲームの進行管理）**
- **スコアの管理 (`Score`)**
- **ゲームオーバーの判定 (`IsGameOver`)**
- **ゲームのリセット (`Restart`)**
- **`Move(string direction)` に `Enum.TryParse` を使い、スワイプ/キー入力を統一**

---

## **🚀 技術スタック**
| 技術                   | 説明                       |
| ---------------------- | -------------------------- |
| **C# 12**              | 最新の C# 機能を活用       |
| **Blazor WebAssembly** | フロントエンドを C# で構築 |
| **.NET 8**             | 最新の .NET を使用         |
| **HTML / CSS**         | UI のデザイン              |
| **JavaScript**         | スワイプ操作を補助         |

---

## **🛠 よくある質問（FAQ）**
### **Q1: `dotnet run` してもブラウザが開かない**
A: `http://localhost:5267` を手動で開いてください。

### **Q2: ゲームオーバーにならない**
A: `GameManager.IsGameOver()` のロジックを確認してください。

### **Q3: モバイルでスワイプ操作が反応しない**
A: `wwwroot/script.js` に `touchstart` & `touchend` のイベントがあるか確認してください。

---

## **📜 ライセンス**
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## **📢 クレジット**
GitHub: [@sano-suguru](https://github.com/sano-suguru)  
---

## **🎉 最後に**
🚀 **Blazor 2048 を楽しんでください！** 🚀  
バグ報告やフィードバックをお待ちしています！ 🎮🎯
```
