Imports System.IO
Imports System.Reflection.Emit
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar

Public Class NewProject
    Private Sub NewProject2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        '
        Me.BackColor = Color.FromArgb(49, 51, 53)
        Button1.BackColor = Color.FromArgb(76, 80, 82)

        Button1.FlatStyle = FlatStyle.Popup

        Button1.FlatAppearance.BorderColor = Color.GhostWhite

        Button1.ForeColor = Color.GhostWhite

        Label1.ForeColor = Color.GhostWhite

        RadioButton1.ForeColor = Color.GhostWhite
        RadioButton2.ForeColor = Color.GhostWhite

        RadioButton1.FlatStyle = FlatStyle.Popup
        RadioButton2.FlatStyle = FlatStyle.Popup

        RadioButton1.FlatAppearance.CheckedBackColor = Color.GhostWhite
        RadioButton2.FlatAppearance.CheckedBackColor = Color.GhostWhite

    End Sub

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged
        If RadioButton1.Checked Then
            RadioButton2.Checked = False
        Else
            RadioButton2.Checked = True
        End If

        If MaskedTextBox1.Text.ToString <> "" And (RadioButton1.Checked Or RadioButton2.Checked) Then
            Button1.Enabled = True
        Else
            Button1.Enabled = False
        End If
    End Sub


    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton2.CheckedChanged
        If RadioButton2.Checked Then
            RadioButton1.Checked = False
        Else
            RadioButton1.Checked = True
        End If

        If MaskedTextBox1.Text.ToString <> "" And (RadioButton1.Checked Or RadioButton2.Checked) Then
            Button1.Enabled = True
        Else
            Button1.Enabled = False
        End If
    End Sub

    Private Sub createNewProject_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' Create New Project click

        Dim projectName As String = MaskedTextBox1.Text.Trim
        If WelcomeScreen.allProjects.Contains(projectName) Then
            MessageQuestion.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK
            MessageQuestion.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error
            MessageQuestion.Show("Project name already exists", "Error")
        Else
            ' Add project name and type

            Dim projectType As String

            If RadioButton1.Checked Then
                projectType = "initial"
            ElseIf RadioButton2.Checked Then
                projectType = "renewal"
            End If

            Using fs As New FileStream(mainProject.PROJECTSLIST, FileMode.Append), writer As New StreamWriter(fs)
                writer.WriteLine(projectName & "<>" & projectType)
            End Using

            mainProject.PROJECTNAME = projectName
            mainProject.PROJECTTYPE = projectType

            Me.Hide()

            If RadioButton1.Checked Then
                LBInitial.Show()
            ElseIf RadioButton2.Checked Then
                LBRenewal.Show()
            End If

        End If

    End Sub

    Private Sub Closed_Form(sender As Object, e As EventArgs) Handles Me.FormClosed
        WelcomeScreen.Show()
    End Sub

    Private Sub body_Click(sender As Object, e As EventArgs) Handles Me.Click
        Me.ActiveControl = Nothing
    End Sub

    Private Sub MaskedTextBox1_MaskśInputRejected(sender As Object, e As EventArgs) Handles MaskedTextBox1.TextChanged
        If MaskedTextBox1.Text.ToString <> "" And (RadioButton1.Checked Or RadioButton2.Checked) Then
            Button1.Enabled = True
        Else
            Button1.Enabled = False
        End If
    End Sub


End Class