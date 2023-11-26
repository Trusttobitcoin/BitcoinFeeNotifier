Imports System.Net.Http
Imports Newtonsoft.Json.Linq
Public Class Form1
    Private Async Function GetCurrentBitcoinFee() As Task(Of String)
        Using client As New HttpClient()
            Dim response As String = Await client.GetStringAsync("https://mempool.space/api/v1/fees/recommended")
            Dim json As JObject = JObject.Parse(response)
            Return json("fastestFee").ToString()
        End Using
    End Function
    Private Function CreateTextIcon(text As String) As Icon
        Dim bitmap As New Bitmap(16, 16)
        Dim g As Graphics = Graphics.FromImage(bitmap)

        g.Clear(Color.Transparent) ' Set the background to transparent
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit

        ' Adjust the font size as much as possible
        Dim fontSize As Integer = 9 ' Experiment with this value
        Dim font As New Font("Microsoft Sans Serif", fontSize, FontStyle.Bold)

        ' Measure the string to center it in the bitmap
        Dim stringSize As SizeF = g.MeasureString(text, font)
        Dim x As Single = (16 - stringSize.Width) / 2
        Dim y As Single = (16 - stringSize.Height) / 2

        ' Draw the string on the bitmap with white color
        g.DrawString(text, font, Brushes.White, x, y)
        g.Dispose()

        ' Create an icon from the bitmap
        Return Icon.FromHandle(bitmap.GetHicon())
    End Function


    Private Async Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Dim fee As String = Await GetCurrentBitcoinFee()
        NotifyIcon1.Icon = CreateTextIcon(fee)
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.WindowState = FormWindowState.Minimized
        Me.ShowInTaskbar = False
    End Sub
    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        e.Cancel = True
        Me.Hide()
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Application.Exit()
    End Sub
End Class
