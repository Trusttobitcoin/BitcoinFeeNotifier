Imports System.Net.Http
Imports Newtonsoft.Json.Linq
Imports Microsoft.Win32

Public Class Form1
    ' Function to check if the system is in dark mode
    Private Function IsDarkMode() As Boolean
        Dim registryKey As RegistryKey = Registry.CurrentUser.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Themes\Personalize")
        If registryKey IsNot Nothing Then
            Dim value As Object = registryKey.GetValue("AppsUseLightTheme")
            If value IsNot Nothing Then
                Return Not CBool(value)
            End If
        End If
        Return False ' Default to light mode if registry value is not found
    End Function

    Private Async Function GetCurrentBitcoinFee() As Task(Of String)
        Using client As New HttpClient()
            Dim response As String = Await client.GetStringAsync("https://mempool.space/api/v1/fees/recommended")
            Dim json As JObject = JObject.Parse(response)
            Return json("fastestFee").ToString()
        End Using
    End Function

    ' Function to create icon with appropriate colors based on system theme
    Private Function CreateTextIcon(text As String, darkMode As Boolean) As Icon
        Dim bitmap As New Bitmap(16, 16)
        Dim g As Graphics = Graphics.FromImage(bitmap)

        ' Set background color based on theme
        Dim backgroundColor As Color = If(darkMode, Color.Black, Color.White)
        g.Clear(backgroundColor)

        ' Set text color based on theme
        Dim textColor As Color = If(darkMode, Color.White, Color.Black)

        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit

        ' Adjust font size based on the length of the text
        Dim fontSize As Integer = 9
        If text.Length > 2 Then
            fontSize = 7 ' Decrease font size for longer text
        End If

        Dim font As New Font("Microsoft Sans Serif", fontSize, FontStyle.Bold)

        Dim stringSize As SizeF = g.MeasureString(text, font)
        Dim x As Single = (16 - stringSize.Width) / 2
        Dim y As Single = (16 - stringSize.Height) / 2

        g.DrawString(text, font, New SolidBrush(textColor), x, y)
        g.Dispose()

        Return Icon.FromHandle(bitmap.GetHicon())
    End Function


    Private Async Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Try
            Dim fee As String = Await GetCurrentBitcoinFee()
            Dim darkMode As Boolean = IsDarkMode()
            Dim icon As Icon = CreateTextIcon(fee, darkMode)
            NotifyIcon1.Icon = icon

            ' Set tooltip text
            NotifyIcon1.Text = "High Priority" & vbCrLf & fee & " sat/vB"
        Catch ex As Exception
            ' Error handling: Display error message if fee retrieval fails
            NotifyIcon1.Text = "Error: Unable to fetch fee data"
        End Try
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.WindowState = FormWindowState.Minimized
        Me.ShowInTaskbar = False

        ' Show tooltip when program is minimized to system tray
        NotifyIcon1.Icon = SystemIcons.Information
        NotifyIcon1.Visible = True
        NotifyIcon1.BalloonTipTitle = "Bitcoin Fee Tracker"
        NotifyIcon1.BalloonTipText = "The program is now running in the system tray."
        NotifyIcon1.ShowBalloonTip(2000) ' Display for 2 seconds
    End Sub


    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        e.Cancel = True
        Me.Hide()
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Application.Exit()
    End Sub
End Class
