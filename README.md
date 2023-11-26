
# BitcoinFeeNotifier

BitcoinFeeNotifier is a VB.NET script designed to display the current Bitcoin transaction fee in the system tray, updating every 5 seconds. The script fetches this data from the Mempool Space API and is intended to run unobtrusively in the system tray, providing real-time updates with minimal user interaction.

## Features

- Fetches real-time Bitcoin transaction fee from Mempool Space API.
- Displays the fee in the system tray as an icon.
- Updates the display every 5 seconds.
- Minimizes to the system tray on startup.
- Provides a right-click context menu with an 'Exit' option.

## Prerequisites

- Windows Operating System
- .NET Framework 
- Visual Studio 

## Setup

1. **Create a New Project**:
   - Open Visual Studio.
   - Create a new Windows Forms App (.NET Framework) project in VB.NET.

2. **Add the Provided Script**:
   - Replace the content of the auto-generated `Form1.vb` with the provided `BitcoinFeeNotifier.vb` script.

3. **Install Required NuGet Package**:
   - Right-click on your project in the Solution Explorer.
   - Manage NuGet Packages > Browse.
   - Search for `Newtonsoft.Json` and install it.

4. **Add Necessary Components to the Form**:
   - Drag a `Timer`, `NotifyIcon`, and `ContextMenuStrip` from the toolbox onto your form.
   - Set the `Interval` property of the `Timer` to `5000`.
   - Set the `Enabled` property of the `Timer` to `True`.
   - Associate the `ContextMenuStrip` with the `NotifyIcon`.

5. **Build and Run**:
   - Build the project and run the application through Visual Studio.

## Application Code

### Fetching the Bitcoin Fee

```vb.net
Private Async Function GetCurrentBitcoinFee() As Task(Of String)
    Using client As New HttpClient()
        Dim response As String = Await client.GetStringAsync("https://mempool.space/api/v1/fees/recommended")
        Dim json As JObject = JObject.Parse(response)
        Return json("fastestFee").ToString()
    End Using
End Function
```

### Creating the System Tray Icon

```vb.net
Private Function CreateTextIcon(text As String) As Icon
    Dim bitmap As New Bitmap(16, 16)
    Using g As Graphics = Graphics.FromImage(bitmap)
        g.Clear(Color.Transparent)
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit
        g.DrawString(text, New Font("Microsoft Sans Serif", 9, FontStyle.Bold), Brushes.White, 0, 0)
    End Using
    Return Icon.FromHandle(bitmap.GetHicon())
End Function
```

### Timer Event for Periodic Updates

```vb.net
Private Async Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
    Dim fee As String = Await GetCurrentBitcoinFee()
    NotifyIcon1.Icon = CreateTextIcon(fee)
End Sub
```

### Form Load and Closing Events

```vb.net
Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
    Me.WindowState = FormWindowState.Minimized
    Me.ShowInTaskbar = False
End Sub

Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
    e.Cancel = True
    Me.Hide()
End Sub
```

### Context Menu Exit Functionality

```vb.net
Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
    Application.Exit()
End Sub
```

## Usage

After running, the application resides in the system tray. Hover over the icon to view the current Bitcoin transaction fee. Right-click on the icon to access and select the 'Exit' option to close the application.


## License

This project is open source and available under the MIT License.
