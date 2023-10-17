Imports System.IO
Imports System.Text.RegularExpressions

Public Class LBInitial
    Public initialControlStates As New Dictionary(Of Control, ControlState)
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Log Book Screen loading time actions
        '

        ' Set the user actions file name
        mainProject.UAFILE()

        ' Store control states
        StoreInitialControlState(Me)

        ' Set form name
        Me.Text = "Log Book (" & mainProject.PROJECTNAME & "*)"

        ' Load the Activity Table
        Try
            Dim success As Boolean = initial.processDefinitionFile(mainProject.ACTIVITYFILE)
            If Not success Then
                MessageQuestion.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error
                MessageQuestion.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK
                MessageQuestion.Show("Activity list could not be loaded. Please load the file from 'File -> Add -> Activity List', or contact the provider", "Error")
                Exit Sub
            End If
        Catch ex As Exception
            MessageQuestion.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error
            MessageQuestion.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK
            MessageQuestion.Show("Activity list could not be loaded. Please load the file from 'File -> Add -> Activity List', or contact the provider", "Error")
            Exit Sub
        End Try

        ' Create the user action table if doesn't exist
        initial.CreateOrReadUA()

        ' Add activities in the "Activities" tabpage
        Dim temp As String() = initial.DSinitial.ActivityTable.Select(Function(x) x.ActivityName).ToArray
        ComboBox2.Items.AddRange(temp)

        ' Debugging actions. Delete later
        DataGridView1.DataSource = initial.DSinitial.ActivityTable
        DataGridView2.DataSource = initial.DSinitial.UserActionTable

    End Sub


    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        ' Shortcut of registering and logging for debugging speed purposes
        ' temporary function

        initial.logUserAction("Ultrasonic Testing", (New Date(2023, 9, 26)).Date, 1, 15, "experience")
        initial.logUserAction("Ultrasonic Testing", (New Date(2023, 9, 26)).Date, 1, 15, "experience")
        initial.logUserAction("Ultrasonic Testing", (New Date(2023, 9, 24)).Date, 1, 15, "experience")
        initial.logUserAction("Ultrasonic Testing", (New Date(2023, 9, 23)).Date, 1, 15, "experience")

        initial.logUserAction("Ultrasonic Testing", (New Date(2023, 9, 22)).Date, 1, 15, "experience")
        initial.logUserAction("Ultrasonic Testing", (New Date(2023, 9, 21)).Date, 1, 15, "experience")
        initial.logUserAction("Ultrasonic Testing", (New Date(2023, 9, 19)).Date, 1, 15, "experience")
        initial.logUserAction("Ultrasonic Testing", (New Date(2023, 9, 18)).Date, 1, 15, "experience")

        initial.logUserAction("Ultrasonic Testing", (New Date(2023, 9, 17)).Date, 1, 15, "training")
        initial.logUserAction("Ultrasonic Testing", (New Date(2023, 9, 17)).Date, 1, 15, "training")
        initial.logUserAction("Ultrasonic Testing", (New Date(2023, 9, 17)).Date, 1, 15, "training")
        initial.logUserAction("Ultrasonic Testing", (New Date(2020, 9, 15)).Date, 1, 15, "training")
        initial.logUserAction("Ultrasonic Testing", (New Date(2011, 9, 16)).Date, 1, 15, "training")

        DataGridView1.DataSource = initial.DSinitial.ActivityTable
        DataGridView2.DataSource = initial.DSinitial.UserActionTable
        'DataGridView3.DataSource = initial.counUltrasonic Testingays()


    End Sub

    Private Sub MonthCalendar1_DateChanged(sender As Object, e As DateRangeEventArgs) Handles MonthCalendar1.DateChanged
        ' Log book screen Calendar change action
        refreshCalendarDGV()
    End Sub

    Private Sub TabPage2_Enter(sender As Object, e As EventArgs) Handles TabPage2.Enter
        ' Log book screen entering actions
        refreshCalendarDGV()
    End Sub


    Private Sub ActivityListToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ActivityListToolStripMenuItem.Click
        ' "Add new activity" File option
        '

        MessageQuestion.Icon = Guna.UI2.WinForms.MessageDialogIcon.Question
        MessageQuestion.Buttons = Guna.UI2.WinForms.MessageDialogButtons.YesNo
        Dim response As DialogResult = MessageQuestion.Show("This action will overwrite all existing record. Do you want to proceed?", "Confirm")
        If response = DialogResult.Yes Then
            Dim newForm As New AddDefinition
            newForm.ShowDialog()
        End If

    End Sub

    Private Sub ClearLogBookToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearLogBookToolStripMenuItem.Click
        ' "Clear Log Book" File option
        ' Okay

        MessageQuestion.Icon = Guna.UI2.WinForms.MessageDialogIcon.Question
        MessageQuestion.Buttons = Guna.UI2.WinForms.MessageDialogButtons.YesNo
        Dim response As DialogResult = MessageQuestion.Show("This action will clear all recorded activities. Do you want to proceed?", "Confirm")
        If response = DialogResult.Yes Then
            initial.DSinitial.UserActionTable.Clear()
            ResetControlsToInitialState(Me)
        End If

    End Sub

    Private Sub SaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveToolStripMenuItem.Click
        ' "Save user actions" File option
        ' Okay here

        MessageQuestion.Icon = Guna.UI2.WinForms.MessageDialogIcon.Question
        MessageQuestion.Buttons = Guna.UI2.WinForms.MessageDialogButtons.YesNo
        Dim response As DialogResult = MessageQuestion.Show("Save progress?", "Confirm")
        If response = DialogResult.Yes Then
            initial.SaveUAFile()
            Me.Text = Me.Text.Replace("*", "")
        End If

    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        ' Check it

        MessageQuestion.Icon = Guna.UI2.WinForms.MessageDialogIcon.Question
        MessageQuestion.Buttons = Guna.UI2.WinForms.MessageDialogButtons.YesNoCancel
        Dim response As DialogResult = MessageQuestion.Show("Save and exit?", "Confirm")


        If response = DialogResult.Yes Then
            initial.SaveUAFile()
            Application.Exit()
        ElseIf response = DialogResult.No Then
            Application.Exit()
        ElseIf response = DialogResult.Cancel Then
            Exit Sub
        End If

    End Sub

    Private Sub AddUA_Click(sender As Object, e As EventArgs) Handles Button6.Click
        ' Add User Action button click
        ' Okay
        Dim newForm As New ActionInitial
        newForm.DateTimePicker1.Value = MonthCalendar1.SelectionEnd.Date
        newForm.DateTimePicker1.Enabled = False

        newForm.MODIFY = False
        newForm.ShowDialog()

        refreshCalendarDGV()

        Me.Text = "Log Book (" & mainProject.PROJECTNAME & "*)"

    End Sub

    Private Sub ModifyUA_Click(sender As Object, e As EventArgs) Handles Button7.Click
        ' Modify User Action button click
        ' Okay?

        Dim newForm As New ActionInitial

        ' Item setting in the modification form
        Dim activityName As String = DataGridView5.SelectedRows(0).Cells("ActivityName_").Value     ' ActivityName_ on DGV
        Dim hour As Double = DataGridView5.SelectedRows(0).Cells("Hour").Value

        newForm.ITEMTEXT = activityName
        newForm.ITEMHOUR = hour

        newForm.ComboBox1.Enabled = False

        ' Level
        Dim level_ As String = DataGridView5.SelectedRows(0).Cells("Level").Value
        If level_ = 1 Then
            newForm.RadioButton1.Checked = True
        ElseIf level_ = 2 Then
            newForm.RadioButton2.Checked = True
        ElseIf level_ = 3 Then
            newForm.RadioButton3.Checked = True
        End If


        ' Date setting in the modification form
        Dim date_ As Date = DataGridView5.SelectedRows(0).Cells("Date_").Value
        newForm.DateTimePicker1.Value = date_.Date
        newForm.DateTimePicker1.Enabled = True

        ' Action ID to modify
        Dim actionID As String = DataGridView5.SelectedRows(0).Cells("ActionID").Value
        newForm.ACTIONID = actionID

        newForm.MODIFY = True
        newForm.ShowDialog()

        Me.Text = "Log Book (" & mainProject.PROJECTNAME & "*)"

    End Sub

    Private Sub Delete_Click(sender As Object, e As EventArgs) Handles Button8.Click
        ' Delete User Action button click
        ' Okay?

        Dim count_ As Integer = DataGridView5.SelectedRows.Count

        If count_ = 0 Then
            MessageQuestion.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error
            MessageQuestion.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK
            MessageQuestion.Show("Please select the row(s) to be deleted", "Error")

            Exit Sub
        Else

            MessageQuestion.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OKCancel
            Dim response As DialogResult = MessageQuestion.Show("Do you want to delete the selected " & count_ & " rows?", "Confirm")

            If response = DialogResult.Yes Then
                Dim finalMessage As String = "Following actions have been deleted:: " & vbCrLf

                For Each row As DataGridViewRow In DataGridView5.SelectedRows
                    Dim actionID As String = row.Cells("ActionID").Value
                    Dim selectedRow As Data.DataRow = initial.DSinitial.UserActionTable.Select("ActionID = '" & actionID & "'")(0)

                    Dim parentRow As Data.DataRow = selectedRow.GetParentRow("ActivityTable_UserActionTable")
                    finalMessage &= parentRow("ActivityName") & " -- " & selectedRow("Hour") & " @@ " & selectedRow("Date") & vbCrLf
                    selectedRow.Delete()
                Next

                initial.DSinitial.UserActionTable.AcceptChanges()

                MessageQuestion.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information
                MessageQuestion.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK
                MessageQuestion.Show(finalMessage, "Success")

                refreshCalendarDGV()
                Me.Text = "Log Book (" & mainProject.PROJECTNAME & "*)"
            End If


        End If



    End Sub

    Public Sub refreshCalendarDGV()
        ' Okay?

        ' Refresh data gridview related to the calendar in the Log Screen tab
        DataGridView5.DataSource = mainProject.FilterByDate(MonthCalendar1.SelectionEnd, initial.DSinitial.UserActionTable)

        ' Modify and delete buttons
        If DataGridView5.DataSource.rows.count = 0 Then
            Button7.Enabled = False
            Button8.Enabled = False
        Else
            Button7.Enabled = True
            Button8.Enabled = True
        End If

        ' Hiding everything but some
        For Each col As DataGridViewColumn In DataGridView5.Columns

            If Regex.IsMatch(col.DataPropertyName, "(ActivityName)|(Level)|(Hour)|(TorE)") Then
                col.Visible = True
            Else
                col.Visible = False
            End If
        Next
    End Sub


    Private Sub SummaryTabScoreCal()
        ' Score calculation trigger on the summary tab
        ' Okay

        ' Dates
        Dim dateTo As Date = DateTimePicker4.Value
        Dim dateFrom As Date = DateTimePicker2.Value

        Dim a As Data.DataTable = initial.calculateScore(dateFrom, dateTo)
        DataGridView6.DataSource = a
    End Sub


    Private Sub TabPage2_Click(sender As Object, e As EventArgs) Handles TabPage2.Enter
        ' Summary tab loading events
        ' okay

        ' Select a combobox item
        Try
            If Not DataGridView5.DataSource = Nothing AndAlso DataGridView5.DataSource.rows.count = 0 Then
                Button7.Enabled = False
                Button8.Enabled = False
            Else
                Button7.Enabled = True
                Button8.Enabled = True
            End If
        Catch ex As Exception

        End Try


    End Sub

    Private Sub TabPage3_Click(sender As Object, e As EventArgs) Handles TabPage3.Enter
        ' Summary tab loading events
        ' Okay

        ' Select a combobox item
        If TabControl1.SelectedTab.Name = "TabPage3" Then
            If initial.DSinitial.UserActionTable.Rows.Count > 0 Then
                DateTimePicker2.Value = initial.DSinitial.UserActionTable.AsEnumerable().Min(Function(row) row.Field(Of DateTime)("Date"))
            End If
        End If

        SummaryTabScoreCal()

    End Sub

    Private Sub TabPage_Click(sender As Object, e As EventArgs) Handles TabControl1.Click, TabPage1.Click, TabPage2.Click, TabPage3.Click, TabPage4.Click
        ' okay
        Me.ActiveControl = Nothing
    End Sub

    Private Sub ComboBox_ActivityTab(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged, ComboBox3.SelectedIndexChanged, DateTimePicker3.Leave
        ' Combobox/date value changing actions of the Activity tab
        ' okay

        If ComboBox2.SelectedIndex <> -1 And ComboBox3.SelectedIndex <> -1 Then
            initial.ActivityTabSummary(ComboBox2.SelectedItem, ComboBox3.SelectedItem, DateTimePicker3.Value)
        End If

    End Sub

    Private Sub StoreInitialControlState(container As Control)
        ' okay

        For Each ctrl As Control In container.Controls
            ' Store the initial state of each control
            initialControlStates.Add(ctrl, New ControlState With {
            .BackColor = ctrl.BackColor,
            .ForeColor = ctrl.ForeColor,
            .Enabled = ctrl.Enabled,
            .Text = ctrl.Text
        })

            ' If the control has child controls (e.g., panels, group boxes), recurse into them
            If ctrl.HasChildren Then
                StoreInitialControlState(ctrl)
            End If
        Next
    End Sub

    Private Sub ResetControlsToInitialState(container As Control)
        ' okay

        For Each ctrl As Control In container.Controls
            ' Retrieve the initial state of the control
            Dim initialState As ControlState = initialControlStates(ctrl)

            ' Restore the control's properties to their initial values
            ctrl.BackColor = initialState.BackColor
            ctrl.ForeColor = initialState.ForeColor
            ctrl.Enabled = initialState.Enabled
            ctrl.Text = initialState.Text

            If Regex.IsMatch(ctrl.Name.ToLower, "listbox") Then
                Dim lb As ListBox = DirectCast(ctrl, ListBox)
                lb.Items.Clear()
            ElseIf Regex.IsMatch(ctrl.Name.ToLower, "datagridview") Then
                Dim dgv As DataGridView = DirectCast(ctrl, DataGridView)
                dgv.DataSource = Nothing
            End If

            ' If the control has child controls (e.g., panels, group boxes), recurse into them
            If ctrl.HasChildren Then
                ResetControlsToInitialState(ctrl)
            End If
        Next
    End Sub

    Public Class ControlState
        Public Property BackColor As Color
        Public Property ForeColor As Color
        Public Property Enabled As Boolean
        Public Property Text As String
    End Class

    Private Sub Form_Close(sender As Object, e As EventArgs) Handles MyBase.FormClosed
        ' 

        Application.Exit()
    End Sub

    Private Sub SummaryTabDateTimePicker_ValueChanged(sender As Object, e As EventArgs) Handles DateTimePicker2.ValueChanged, DateTimePicker4.ValueChanged
        ' 

        SummaryTabScoreCal()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click

    End Sub

    Private Sub NewLogBookMenuItem_Click(sender As Object, e As EventArgs) Handles LogbookToolStripMenuItem.Click
        ' New Logbook

    End Sub
End Class