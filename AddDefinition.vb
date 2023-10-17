Imports System.IO
Imports System.Windows.Forms.ToolTip

Public Class AddDefinition
    Private Sub Help_Click(sender As Object, e As EventArgs) Handles Button2.MouseHover
        ' Show a help form/tooltip/dialog box

        Dim tt As New ToolTip

        tt.Show("Select a CSV file of the proper format. Please check with the website to check the format or get the file", Me, 2000)


    End Sub

    Private Sub Browse_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' Browse for definition file
        '

        Dim filter_ As String = "CSV files (*.csv)|*.csv"
        Dim multiselect_ As Boolean = False
        Dim filePath As String = mainProject.fileBrowseAndShow(ListBox1, filter_, multiselect_)

        If filePath <> "" Then
            Button3.Enabled = True
        End If

    End Sub

    Private Sub Save_Click(sender As Object, e As EventArgs) Handles Button3.Click
        ' Save the progress, and clear the DB
        ' Okay

        If Not (RadioButton1.Checked Or RadioButton2.Checked) Then
            MessageQuestion.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error
            MessageQuestion.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK
            MessageQuestion.Show("Please check the activity type", "Warning")
            Exit Sub
        End If

        MessageQuestion.Icon = Guna.UI2.WinForms.MessageDialogIcon.Warning
        MessageQuestion.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OKCancel
        Dim response As DialogResult = MessageQuestion.Show("This action will overwrite all existing record. Do you want to proceed?", "Confirm")


        If response = DialogResult.Yes Then
            Dim success As Boolean = renewal.processDefinitionFile(ListBox1.Items(0))

            If mainProject.PROJECTTYPE.ToLower = "renewal" Then
                renewal.CreateOrReadUA()
            ElseIf mainProject.PROJECTTYPE.ToLower = "initial" Then
                initial.CreateOrReadUA()
            End If


            Button3.Enabled = False

            Try
                Dim tempName As String = IO.Path.GetFileName(mainProject.ACTIVITYFILE)
                IO.File.Copy(ListBox1.Items(0), mainProject.ACTIVITYFILE, True)
            Catch
            End Try
        End If

    End Sub

    Private Sub AddDefinition_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    'Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged
    '    If RadioButton1.Checked Then
    '        RadioButton2.Checked = False
    '    End If
    'End Sub

    'Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton2.CheckedChanged
    '    If RadioButton2.Checked Then
    '        RadioButton1.Checked = False
    '    End If
    'End Sub


End Class