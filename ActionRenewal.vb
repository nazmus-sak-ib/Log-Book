Imports System.Text.RegularExpressions

Public Class ActionRenewal
    Public MODIFY As Boolean   ' If modifying existing data, it's true. Else false
    Public ITEMTEXT As String = ""  ' Activity Combobox value During modification
    Public ACTIONID As Integer = -1  ' Action ID During modification

    Private Sub Action_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Form loading actions
        '

        ' Load the combobox
        For Each row As Data.DataRow In renewal.DSrenewal.ActivityTable
            Dim txt As String = row("ActivityID") & " - " & row("ActivityName")
            ComboBox1.Items.Add(txt)
            If ITEMTEXT.Trim = txt.Trim Then ComboBox1.SelectedIndex = ComboBox1.Items.Count - 1
        Next

    End Sub

    'Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged
    '    If RadioButton1.Checked Then
    '        RadioButton2.Checked = False
    '        RadioButton3.Checked = False
    '    End If
    'End Sub

    'Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton2.CheckedChanged
    '    If RadioButton2.Checked Then
    '        RadioButton1.Checked = False
    '        RadioButton3.Checked = False
    '    End If
    'End Sub

    'Private Sub RadioButton3_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton3.CheckedChanged
    '    If RadioButton3.Checked Then
    '        RadioButton1.Checked = False
    '        RadioButton2.Checked = False
    '    End If
    'End Sub

    Private Sub SaveAction_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' Save action button

        ' Errors
        Dim a = ComboBox1.SelectedItem
        Dim v As String = ComboBox1.SelectedText
        If ComboBox1.SelectedItem = "" Then
            MessageQuestion.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error
            MessageQuestion.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK
            MessageQuestion.Show("Please select an activity from the dropdown", "Error")

            Me.ActiveControl = ComboBox1

            Exit Sub
        End If

        If Not (RadioButton1.Checked Or RadioButton2.Checked Or RadioButton3.Checked) Then
            MessageQuestion.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error
            MessageQuestion.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK
            MessageQuestion.Show("Please select a level for the activity", "Error")

            Me.ActiveControl = RadioButton1

            Exit Sub
        End If

        MessageQuestion.Icon = Guna.UI2.WinForms.MessageDialogIcon.Question
        MessageQuestion.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OKCancel
        Dim response As DialogResult = MessageQuestion.Show("Save the following action?", "Confirm")


        If response = DialogResult.Yes Then
            ' Activity ID
            Dim pattern As String = "(^\w\d+) - (.+)"
            Dim match As Match = Regex.Match(ComboBox1.SelectedItem.trim, pattern)
            Dim acID As String = match.Groups(1).Value


            ' Level
            Dim level_ As Integer

            If RadioButton1.Checked Then
                level_ = 1
            ElseIf RadioButton2.Checked Then
                level_ = 2
            ElseIf RadioButton3.Checked Then
                level_ = 3
            End If

            ' Date
            Dim date_ As Date = DateTimePicker1.Value.Date

            renewal.logUserAction(acID, date_, level_, ACTIONID)

            ' Clear
            ComboBox1.SelectedIndex = -1
            ComboBox1.Enabled = True
            RadioButton1.Checked = False
            RadioButton2.Checked = False
            RadioButton3.Checked = False

            DateTimePicker1.Value = Date.Today.Date

            ' refresh
            LBRenewal.refreshCalendarDGV()

            Me.Close()
        End If
    End Sub

    Private Sub ExitClick(sender As Object, e As EventArgs) Handles Button2.Click
        MessageQuestion.Icon = Guna.UI2.WinForms.MessageDialogIcon.Question
        MessageQuestion.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OKCancel
        Dim response As DialogResult = MessageQuestion.Show("Exit without saving?", "Confirm")

        If response = DialogResult.Yes Then Me.Close()

    End Sub
End Class