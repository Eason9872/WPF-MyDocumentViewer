# WPF小作家

2026/11/11

## 本次修改

1. 新增 `MyDocumentViewer` 視窗，用來顯示及編輯文件內容。
2. 在主視窗 `MainWindow` 加上「開啟新文件」按鈕，點擊後開啟 `MyDocumentViewer`。
3. 視窗上方加入 `ToolBar`，下方加入 `StatusBar`，中間加入 `RichTextBox`。
4. 使用 `CommandBinding` 處理新增、開啟及儲存，剪下、複製、貼上則使用 `RichTextBox` 內建命令。
5. 在 `EditingToolbar` 加入粗體、斜體及底線按鈕，並將命令指定到 `MainRichTextBox`。
6. 使用 `Extended.Wpf.Toolkit` 的 `ColorPicker` 選擇文字顏色。
7. 加入字型 `ComboBox`，可選擇電腦中已安裝的字型。
8. 加入字體大小 `ComboBox`，可修改選取文字的大小。
9. 修正圖示路徑為 `img/`，並在專案檔設定為 `Resource`。
10. 加入 RTF 與 TXT 檔案讀寫、清空內容，以及未儲存時的提醒。狀態列顯示檔名和儲存狀態。

## 執行方式

1. 在 Windows 使用 Visual Studio 2022（支援 .NET 8 的版本），安裝「.NET 桌面開發」工作負載與 .NET 8 SDK。
2. **先將 ZIP 完整解壓縮**，再開啟 `2026_WpfApp4.sln`。
3. 等待 NuGet 還原 `Extended.Wpf.Toolkit 5.1.2`，按 F5 執行。
4. 點擊「開啟新文件」，確認出現 `MyDocumentViewer`。
5. 輸入文字，選取後試用粗體、斜體、底線、顏色、字型及大小。
6. 儲存為 `.rtf`，關閉後重新開啟，確認文字與格式保留。`.txt` 只保留純文字。

字體大小採用 WPF 的裝置獨立單位。新增按鈕會另開一個文件視窗。

## 作業截圖

請使用自己實際執行的畫面。老師提供的圖片是參考範例。

| 截圖 | 要呈現的內容 | 建議檔名 |
| --- | --- | --- |
| GitHub profile | 自己的 GitHub 個人主頁，包含帳號與個人頁資訊 | `screenshots/github-profile.png` |
| 執行畫面 | 主視窗及 MyDocumentViewer，清楚顯示 ToolBar、RichTextBox、StatusBar | `screenshots/mydocumentviewer.png` |
| README | GitHub 儲存庫中呈現本次修改的 README | `screenshots/readme.png` |

在 Windows 按 `Win + Shift + S` 截圖，將前兩張存入 `screenshots` 資料夾。
再取消下面兩行的 HTML 註解，讓 README 顯示圖片：

<!-- ![我的 GitHub profile](screenshots/github-profile.png) -->
<!-- ![MyDocumentViewer 實際執行畫面](screenshots/mydocumentviewer.png) -->

將專案與 README 上傳至自己的 GitHub 儲存庫後，拍下 README 頁面，依老師指定的位置繳交三張截圖。

## 驗證狀態

已檢查 XAML/XML 格式、事件名稱與程式處理方法、圖片資源路徑，以及壓縮檔內容。
製作環境為 Linux，未進行 Windows WPF 編譯或執行測試；請依上方步驟在 Windows 驗證並補上實際截圖。
