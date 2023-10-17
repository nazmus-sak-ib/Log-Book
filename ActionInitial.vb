Imports System.Text.RegularExpressions

Public Class ActionInitial
    Public MODIFY As Boolean   ' If modifying existing data, it's true. Else false
    Public ITEMTEXT As String = ""  ' Activity Combobox value During modification
    Public ITEMHOUR As String = ""  ' Activity Hours textbox value During modification
    Public ACTIONID As Integer = -1  ' Action ID During modification
    Private Sub Action_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Form loading actions
        '

        ' Load the combobox
        For Each row As Data.DataRow In initial.DSinitial.ActivityTable
            ComboBox1.Items.Add(row("ActivityName"))

            ' For modification of an action
            If ITEMTEXT.Trim = row("ActivityName").Trim Then
                ComboBox1.SelectedIndex = ComboBox1.Items.Count - 1
            End If
        Next

    End Sub

    Private Sub MeClick(sender As Object, e As EventArgs) Handles Me.Click
        ' Form loading actions
        '

        Me.ActiveControl = Nothing

    End Sub

    Private Sub SaveAction_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' Save action button

        Dim activityName As String = ComboBox1.SelectedItem

        ' Errors
        If activityName = "" Then
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

        If Not (RadioButton4.Checked Or RadioButton5.Checked) Then
            MessageQuestion.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error
            MessageQuestion.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK
            MessageQuestion.Show("Please select experience/training", "Error")

            Me.ActiveControl = RadioButton4
            Exit Sub
        End If

        If TextBox1.Text = "" Then
            MessageQuestion.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error
            MessageQuestion.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK
            MessageQuestion.Show("Please enter the number of hours", "Error")

            Me.ActiveControl = TextBox1
            Exit Sub
        End If

        MessageQuestion.Icon = Guna.UI2.WinForms.MessageDialogIcon.Question
        MessageQuestion.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OKCancel
        Dim response As DialogResult = MessageQuestion.Show("Save the following action?", "Confirm")


        If response = DialogResult.Yes Then

            ' Level
            Dim level_ As Integer

            If RadioButton1.Checked Then
                level_ = 1
            ElseIf RadioButton2.Checked Then
                level_ = 2
            ElseIf RadioButton3.Checked Then
                level_ = 3
            End If

            ' TorE (training/experience)
            Dim TorE As String

            If RadioButton5.Checked Then
                TorE = "Training"
            ElseIf RadioButton4.Checked Then
                TorE = "Experience"
            End If

            ' Date
            Dim date_ As Date = DateTimePicker1.Value.Date
            Dim hours As Double = TextBox1.Text

            initial.logUserAction(activityName, date_, level_, hours, TorE, ACTIONID)

            ' Refresh
            LBInitial.refreshCalendarDGV()

            Me.Close()
        End If
    End Sub



    Private Sub ExitClick(sender As Object, e As EventArgs) Handles Button2.Click
        MessageQuestion.Icon = Guna.UI2.WinForms.MessageDialogIcon.Question
        MessageQuestion.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OKCancel
        Dim response As DialogResult = MessageQuestion.Show("Exit without saving?", "Confirm")

        If response = DialogResult.Yes Then Me.Close()

    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.Leave
        Try
            Dim a As Double = Double.Parse(TextBox1.Text)
        Catch ex As Exception
            TextBox1.Clear()
        End Try
    End Sub


End Class