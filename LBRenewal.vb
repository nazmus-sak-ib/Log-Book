Imports System.IO
Imports System.Text.RegularExpressions

Public Class LBRenewal

    Public initialControlStates As New Dictionary(Of Control, ControlState)
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Log Book Screen loading time actions
        '

        mainProject.UAFILE()
        ' Store control states
        StoreInitialControlState(Me)

        ' Set form name
        Me.Text = "Log Book (" & mainProject.PROJECTNAME & "*)"

        ' Load the Activity Table
        Try
            Dim success As Boolean = renewal.processDefinitionFile(mainProject.ACTIVITYFILE)
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
        renewal.CreateOrReadUA()

        ' Add activities in the "Activities" tabpage
        Dim temp As String() = renewal.DSrenewal.ActivityTable.Select(Function(x) x.ActivityString).ToArray
        ComboBox2.Items.AddRange(temp)
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        ' Shortcut of registering and logging for debugging speed purposes
        ' temporary function, DEBUG ONLY

        'renewal.registerActivity("A1", "Initiation")
        'renewal.registerActivity("A2", "Second task")
        'renewal.registerActivity("B1", "Wrap up")

        renewal.logUserAction("B2", (New Date(2023, 9, 26)).Date, 1)
        renewal.logUserAction("B2", (New Date(2023, 9, 26)).Date, 1)
        renewal.logUserAction("B2", (New Date(2023, 9, 24)).Date, 1)
        renewal.logUserAction("B2", (New Date(2023, 9, 23)).Date, 1)
        renewal.logUserAction("B2", (New Date(2023, 9, 22)).Date, 1)
        renewal.logUserAction("B2", (New Date(2023, 9, 21)).Date, 1)
        renewal.logUserAction("B2", (New Date(2023, 9, 19)).Date, 1)
        renewal.logUserAction("B2", (New Date(2023, 9, 18)).Date, 1)
        renewal.logUserAction("B2", (New Date(2023, 9, 17)).Date, 1)
        renewal.logUserAction("B2", (New Date(2023, 9, 17)).Date, 1)
        renewal.logUserAction("B2", (New Date(2023, 9, 17)).Date, 3)
        renewal.logUserAction("B2", (New Date(2020, 9, 15)).Date, 1)
        renewal.logUserAction("B2", (New Date(2011, 9, 16)).Date, 3)

        'DataGridView1.DataSource = renewal.DSrenewal.ActivityTable
        'DataGridView2.DataSource = renewal.DSrenewal.UserActionTable
        'DataGridView3.DataSource = renewal.countOfDays()


    End Sub

    Private Sub MonthCalendar1_DateChanged(sender As Object, e As DateRangeEventArgs) Handles MonthCalendar1.DateChanged
        ' Log book screen Calendar change action
        '
        refreshCalendarDGV()
    End Sub


    Private Sub ActivityListToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ActivityListToolStripMenuItem.Click
        ' "Add new activity" File option
        '
        MessageQuestion.Icon = Guna.UI2.WinForms.MessageDialogIcon.Question
        MessageQuestion.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OKCancel

        Dim response As DialogResult = MessageQuestion.Show("This action will overwrite all existing record. Do you want to proceed?", "Confirm")

        If response = DialogResult.Yes Then
            Dim newForm As New AddDefinition
            newForm.ShowDialog()
        End If

    End Sub


    Private Sub ClearLogBookToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearLogBookToolStripMenuItem.Click
        ' "Clear Log Book" File option
        '

        MessageQuestion.Icon = Guna.UI2.WinForms.MessageDialogIcon.Question
        MessageQuestion.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OKCancel

        Dim response As DialogResult = MessageQuestion.Show("This action will clear all recorded activities. Do you want to proceed?", "Confirm")

        If response = DialogResult.Yes Then
            renewal.DSrenewal.UserActionTable.Clear()
            ResetControlsToInitialState(Me)
        End If

    End Sub

    Private Sub SaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveToolStripMenuItem.Click
        ' "Save user actions" File option

        MessageQuestion.Icon = Guna.UI2.WinForms.MessageDialogIcon.Question
        MessageQuestion.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OKCancel

        Dim response As DialogResult = MessageQuestion.Show("Save progress?", "Confirm")

        If response = DialogResult.Yes Then
            renewal.SaveUAFile()
            Me.Text = Me.Text.Replace("*", "")
        End If


    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click

        MessageQuestion.Icon = Guna.UI2.WinForms.MessageDialogIcon.Question
        MessageQuestion.Buttons = Guna.UI2.WinForms.MessageDialogButtons.YesNoCancel

        Dim response As DialogResult = MessageQuestion.Show("Save and exit?", "Confirm")


        If response = DialogResult.Yes Then
            renewal.SaveUAFile()
            Application.Exit()
        ElseIf response = DialogResult.No Then
            Application.Exit()
        ElseIf response = DialogResult.Cancel Then
            Exit Sub
        End If

    End Sub

    Private Sub AddUA_Click(sender As Object, e As EventArgs) Handles Button6.Click
        ' Add User Action button click
        '
        Dim newForm As New ActionRenewal
        newForm.DateTimePicker1.Value = MonthCalendar1.SelectionEnd.Date
        newForm.DateTimePicker1.Enabled = False

        newForm.MODIFY = False
        newForm.ShowDialog()

        refreshCalendarDGV()

        Me.Text = "Log Book (" & mainProject.PROJECTNAME & "*)"

    End Sub

    Private Sub ModifyUA_Click(sender As Object, e As EventArgs) Handles Button7.Click
        ' Modify User Action button click
        '

        Dim newForm As New ActionRenewal

        ' Item setting in the modification form
        Dim activityID As String = DataGridView5.SelectedRows(0).Cells("ActivityID").Value
        Dim acName As String = renewal.DSrenewal.ActivityTable.Select("ActivityID = '" & activityID & "'")(0)("ActivityName")

        newForm.ITEMTEXT = activityID & " - " & acName

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
        '

        Dim count_ As Integer = DataGridView5.SelectedRows.Count

        If count_ = 0 Then
            MessageQuestion.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error
            MessageQuestion.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK
            MessageQuestion.Show("Please select the row(s) to be deleted", "Error")

            Exit Sub
        Else

            MessageQuestion.Icon = Guna.UI2.WinForms.MessageDialogIcon.Question

            MessageQuestion.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OKCancel
            Dim response As DialogResult = MessageQuestion.Show("Do you want to delete the selected " & count_ & " rows?", "Confirm")

            If response = DialogResult.Yes Then
                Dim finalMessage As String = "Following actions have been deleted:: " & vbCrLf

                For Each row As DataGridViewRow In DataGridView5.SelectedRows
                    Dim actionID As String = row.Cells("ActionID").Value
                    Dim selectedRow As Data.DataRow = renewal.DSrenewal.UserActionTable.Select("ActionID = '" & actionID & "'")(0)

                    Dim parentRow As Data.DataRow = selectedRow.GetParentRow("ActivityTable_UserActionTable")
                    finalMessage &= parentRow("ActivityString") & " @@ " & selectedRow("Date") & vbCrLf
                    selectedRow.Delete()
                Next

                renewal.DSrenewal.UserActionTable.AcceptChanges()

                MessageQuestion.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information
                MessageQuestion.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK
                MessageQuestion.Show(finalMessage, "Success")

                refreshCalendarDGV()
                Me.Text = "Log Book (" & mainProject.PROJECTNAME & "*)"

            End If


        End If



    End Sub

    Public Sub refreshCalendarDGV()
        ' Refresh data gridview related to the calendar in the Log Book tab

        Dim tbl As Data.DataTable = mainProject.FilterByDate(MonthCalendar1.SelectionEnd, renewal.DSrenewal.UserActionTable)

        Dim query As IEnumerable(Of Object) =
                From parentRow In renewal.DSrenewal.ActivityTable
                Join childRow In tbl On parentRow("ActivityID") Equals childRow("ActivityID")
                Select New With {
                    .Date_ = childRow("Date"),
                    .ActionID = childRow("ActionID"),
                    .ActivityID = parentRow("ActivityID"),
                     .ActivityString = parentRow.ActivityString,
                    .Level = childRow("Level")
                }

        Dim tbl2 As New Data.DataTable
        tbl2.Columns.Add("ActivityID")
        tbl2.Columns.Add("ActivityString")
        tbl2.Columns.Add("Level")
        tbl2 = mainProject.queryToTable(query, tbl2)

        DataGridView5.DataSource = tbl2

        ' Modify and delete buttons
        If tbl2.Rows.Count = 0 Then
            Button7.Enabled = False
            Button8.Enabled = False
        Else
            Button7.Enabled = True
            Button8.Enabled = True
        End If

        ' Hiding everything but some
        For Each col As DataGridViewColumn In DataGridView5.Columns
            If Regex.IsMatch(col.Name, "(ActivityString)|(Level)") Then
                col.Visible = True
            Else
                col.Visible = False
            End If
        Next
    End Sub


    Private Sub SummaryTabScoreCal()
        ' Score calculation trigger on the summary tab
        '
        Dim scoreString As String

        ' Dates
        Dim dateTo As Date = DateTimePicker2.Value
        Dim tempDate As Date = DateAdd(DateInterval.Day, 1, dateTo)
        Dim dateFrom As Date

        If ComboBox1.SelectedItem = "one year" Then
            dateFrom = DateAdd(DateInterval.Year, -1, tempDate)        ' 1 year back date
            scoreString = "PerYear"

        ElseIf ComboBox1.SelectedItem = "five years" Then
            dateFrom = DateAdd(DateInterval.Year, -5, tempDate)        ' 5 years back date
            scoreString = "PerFiveYear"
        End If


        Dim a As Data.DataTable = renewal.calculateScore(dateFrom, dateTo, scoreString)
        DataGridView6.DataSource = a

        For Each dgc As DataGridViewColumn In DataGridView6.Columns
            If Regex.IsMatch(dgc.Name, scoreString) Or Regex.IsMatch(dgc.Name, "(ActivityString)|(Achieved)") Then
                dgc.Visible = True
            Else
                dgc.Visible = False
            End If
        Next

    End Sub


    Private Sub TabPage2_Click(sender As Object, e As EventArgs) Handles TabPage2.Enter
        ' Log Book tab loading events
        '

        ' Refresh
        refreshCalendarDGV()

        ' Button enabling/disabling
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

        ' Select a combobox item
        If TabControl1.SelectedTab.Name = "TabPage3" Then
            ComboBox1.SelectedIndex = 0
        End If

    End Sub

    Private Sub TabPage4_Click(sender As Object, e As EventArgs) Handles TabPage2.Click, TabPage3.Click, TabPage4.Click
        Me.ActiveControl = Nothing
    End Sub

    Private Sub ComboBox_ActivityTab(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged, ComboBox3.SelectedIndexChanged, DateTimePicker3.Leave
        ' Combobox/date value changing actions of the Activity tab
        '

        If ComboBox2.SelectedIndex <> -1 And ComboBox3.SelectedIndex <> -1 Then
            renewal.ActivityTabSummary(ComboBox2.SelectedItem, ComboBox3.SelectedItem, DateTimePicker3.Value)
        End If

    End Sub

    Private Sub StoreInitialControlState(container As Control)
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

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        ' Summary tab 
        '

        If ComboBox1.SelectedIndex <> -1 Then
            SummaryTabScoreCal()
        End If

    End Sub

    Private Sub Form1_Close(sender As Object, e As EventArgs) Handles MyBase.FormClosed
        Application.Exit()
    End Sub

End Class

