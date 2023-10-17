Imports System.IO
Imports System.Text.RegularExpressions

Public Class LoadProject
    Private Sub LoadProject_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Aesthetics
        Me.BackColor = Color.FromArgb(49, 51, 53)
        ListBox1.BackColor = Color.FromArgb(60, 63, 65)
        ListBox1.ForeColor = Color.GhostWhite

        Button1.BackColor = Color.FromArgb(76, 80, 82)
        Button1.ForeColor = Color.GhostWhite
        Button1.FlatStyle = FlatStyle.Popup

        Button2.BackColor = Color.FromArgb(76, 80, 82)
        Button2.ForeColor = Color.GhostWhite
        Button2.FlatStyle = FlatStyle.Popup

        ' Functions
        If ListBox1.Items.Count = 0 Then
            Button1.Enabled = False
            Button2.Enabled = False

        End If

        ListBox1.DisplayMember = "Name"
    End Sub

    Private Sub LoadProject_close(sender As Object, e As EventArgs) Handles MyBase.FormClosed
        WelcomeScreen.Show()
    End Sub


    Private Sub LoadButton_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' Load Button
        '

        If ListBox1.SelectedIndex <> -1 Then
            Dim projectName As String = ListBox1.SelectedItem.Name
            Dim projectType As String = ListBox1.SelectedItem.Type_

            mainProject.PROJECTNAME = projectName
            mainProject.PROJECTTYPE = projectType
            Me.Hide()

            If projectType = "initial" Then
                LBInitial.Show()
            ElseIf projectType = "renewal" Then
                LBRenewal.Show()
            End If

        End If
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.DoubleClick
        ' Double click loads 

        Button1.PerformClick()

    End Sub

    Private Sub DeleteProject_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ' Delete project permanently

        If ListBox1.SelectedIndex <> -1 Then
            MessageQuestion.Icon = Guna.UI2.WinForms.MessageDialogIcon.Question
            Dim response As DialogResult = MessageQuestion.Show("Are you sure you want to delete this project permanently?", "Confirm")

            If response = DialogResult.Yes Then
                Dim projectName As String = ListBox1.SelectedItem.Name
                Dim projectType As String = ListBox1.SelectedItem.Type_

                mainProject.PROJECTNAME = projectName

                Try
                    IO.File.Delete(mainProject.UAFILE())

                Catch ex As Exception
                End Try

                ' Edit the projects list file

                Dim projects As New List(Of String)

                '' Reading the lines first
                Using reader As New StreamReader(mainProject.PROJECTSLIST)
                    Dim line As String = reader.ReadLine()

                    While line IsNot Nothing
                        Dim tempProjectName As String = line.Split("<>")(0).ToLower
                        If tempProjectName <> projectName.ToLower Then
                            projects.Add(line)
                        End If

                        line = reader.ReadLine()
                    End While
                End Using

                '' Delete the file
                Try
                    IO.File.Delete(mainProject.PROJECTSLIST)
                Catch ex As Exception
                    Exit Sub
                End Try

                '' Clear listbox
                ListBox1.Items.Clear()

                '' Rewrite
                Using fs As New FileStream(mainProject.PROJECTSLIST, FileMode.Create), writer As New StreamWriter(fs)
                    For Each line In projects
                        writer.WriteLine(line)


                        '' ' adding to listbox again
                        Dim item = New With {
                            .Name = line.Split("<>")(0),
                            .Type_ = line.Split("<>")(1)
                            }

                        ListBox1.Items.Add(item)
                    Next
                End Using
            End If

        End If


    End Sub
End Class