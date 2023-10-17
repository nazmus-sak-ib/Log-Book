Imports System.IO

Public Class WelcomeScreen
    Public allProjects As New List(Of Object)      ' All projects
    Private Sub LoadingScreen_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Form loading actions
        '

        ' Aesthetics
        Me.BackColor = Color.FromArgb(49, 51, 53)
        Button1.BackColor = Color.FromArgb(76, 80, 82)
        Button2.BackColor = Color.FromArgb(76, 80, 82)

        Button1.FlatStyle = FlatStyle.Popup
        Button2.FlatStyle = FlatStyle.Popup

        Button1.FlatAppearance.BorderColor = Color.GhostWhite
        Button2.FlatAppearance.BorderColor = Color.GhostWhite

        Button1.ForeColor = Color.GhostWhite
        Button2.ForeColor = Color.GhostWhite

        Label1.ForeColor = Color.GhostWhite
        Label2.ForeColor = Color.GhostWhite
        Label3.ForeColor = Color.GhostWhite
        Label1.FlatStyle = FlatStyle.Popup
        Label2.FlatStyle = FlatStyle.Popup

        SplitContainer1.Panel1.BackColor = Color.FromArgb(60, 63, 65)
        SplitContainer1.Panel2.BackColor = Color.FromArgb(49, 51, 53)

        ' Actions

        '' Open or create "projects list" file
        Try
            Using fs As New FileStream(mainProject.PROJECTSLIST, FileMode.OpenOrCreate)
            End Using
        Catch
        Finally
            readAllprojects()
        End Try

        If allProjects.Count = 0 Then
            Button2.Enabled = False
        Else
            Button2.Enabled = True
        End If




    End Sub



    Private Sub NewProject_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' New project
        '
        Me.Hide()
        Dim form As New NewProject
        form.ShowDialog()


    End Sub

    Private Sub LoadProject_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ' Load project
        '

        ' Create new form and show
        Dim form As New LoadProject

        Me.Hide()
        form.ListBox1.Items.AddRange(allProjects.ToArray)
        form.ShowDialog()

    End Sub

    Private Sub readAllprojects()
        ' Reads the PROJECTLIST file to read all the projects name
        ' Projects list file is assumed to contain lines like "My project name<>project type"
        ' Where project type could be "initial" OR "renewal"
        '

        Using reader As New StreamReader(mainProject.PROJECTSLIST)
            Dim line As String = reader.ReadLine()

            While line IsNot Nothing
                Dim tempArray As String() = Split(line, "<>")

                Dim item = New With {
                    .Name = tempArray(0),
                    .Type_ = tempArray(1)
                    }

                allProjects.Add(item)

                line = reader.ReadLine()
            End While
        End Using
    End Sub

End Class
