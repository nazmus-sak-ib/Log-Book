Imports System.IO
Imports System.Net.WebRequestMethods
Imports System.Text.RegularExpressions

Namespace renewal
    ' All codes pertaining to a renewal type of project
    Module renewal
        Public DSrenewal As New DSRenewal                                                     ' Main dataset (from Dataset1.xsd)

        Public Function processDefinitionFile(ByVal path As String) As Boolean
            ' ReaDSrenewal a definition file, unscrambles, and finally stores in a datatable
            ' @param path File location
            ' returns if function successful or not
            '

            Dim tempDSrenewal As New DSRenewal                          ' Temporary DSrenewal

            Using reader As New StreamReader(path)
                Dim line As String = reader.ReadLine()     ' Skip the header
                line = reader.ReadLine()
                While line IsNot Nothing
                    ' Process the current line
                    Dim tempArray As String() = Split(line, ",")

                    Dim tempActivity As New RenewalActivity With {
                        .ActivityID = tempArray(0),
                        .AcitvityName = tempArray(1)
                        }

                    tempActivity.PPD.AddRange({tempArray(2), tempArray(5), tempArray(8)})
                    tempActivity.MaxPointPerYear.AddRange({tempArray(3), tempArray(6), tempArray(9)})
                    tempActivity.MaxPointFiveYears.AddRange({tempArray(4), tempArray(7), tempArray(10)})

                    Dim success As Boolean = renewal.parseActivity(tempActivity, tempDSrenewal)

                    If Not success Then Return False

                    ' Read the next line
                    line = reader.ReadLine()
                End While
            End Using

            DSrenewal.Clear()
            DSrenewal = tempDSrenewal

            Return True
        End Function
        Public Function parseActivity(ByRef activity As RenewalActivity, ByRef tempDSrenewal As DSRenewal) As Boolean
            ' AdDSrenewal a renewal activity object to the temporary Activity Table
            ' @param activity Activity to add
            ' @param tempDSrenewal Temporary Dataset 
            ' returns if function successful or not
            ' here

            If Not tempDSrenewal.ActivityTable.Rows.Contains(activity.ActivityID) Then
                Dim newRow As Data.DataRow = tempDSrenewal.ActivityTable.NewRow()
                newRow("ActivityID") = activity.ActivityID
                newRow("ActivityName") = activity.AcitvityName
                newRow("ActivityString") = activity.ActivityID & " - " & activity.AcitvityName

                newRow("L1PerYear") = activity.MaxPointPerYear(0)
                newRow("L2PerYear") = activity.MaxPointPerYear(1)
                newRow("L3PerYear") = activity.MaxPointPerYear(2)

                newRow("L1PerFiveYear") = activity.MaxPointFiveYears(0)
                newRow("L2PerFiveYear") = activity.MaxPointFiveYears(1)
                newRow("L3PerFiveYear") = activity.MaxPointFiveYears(2)

                newRow("L1PointDefinition") = activity.PPD(0)
                newRow("L2PointDefinition") = activity.PPD(1)
                newRow("L3PointDefinition") = activity.PPD(2)

                Dim pattern As String = "(\d+)\/?(.+)?"

                Dim match As Match = Regex.Match(activity.PPD(0), pattern)
                newRow("L1Point") = Double.Parse(match.Groups(1).Value)

                If match.Groups.Count = 1 Then
                    newRow("L1Unit") = ""
                Else
                    newRow("L1Unit") = match.Groups(2).Value.ToLower
                End If


                match = Regex.Match(activity.PPD(1), pattern)
                newRow("L2Point") = Double.Parse(match.Groups(1).Value)

                If match.Groups.Count = 1 Then
                    newRow("L2Unit") = ""
                Else
                    newRow("L2Unit") = match.Groups(2).Value.ToLower
                End If

                match = Regex.Match(activity.PPD(2), pattern)
                newRow("L3Point") = Double.Parse(match.Groups(1).Value)

                If match.Groups.Count = 1 Then
                    newRow("L3Unit") = ""
                Else
                    newRow("L3Unit") = match.Groups(2).Value.ToLower
                End If


                tempDSrenewal.ActivityTable.Rows.Add(newRow)
            Else
                MsgBox("Activity ID " & activity.ActivityID & " exists more than once in the excel file! Please try again", vbOK, "Abort")
                Return False
            End If

            Return True
        End Function
        Public Sub CreateOrReadUA()
            ' Creates User Action file if it does not exist. ReaDSrenewal it if it does
            '

            Try
                Using fs As New FileStream(mainProject.UAFILE, FileMode.OpenOrCreate)
                End Using
            Catch

            End Try

            ' load it
            Dim success As Boolean = renewal.processUAFile(mainProject.UAFILE)
        End Sub

        Public Sub logUserAction(ByVal acID As String, ByVal date_ As Date, ByVal level_ As Integer, Optional ByVal actionID As Integer = -1)
            ' @acID ActivityID
            ' @date_ Date of a user action
            ' @parm actionID if = -1, logs a new row, else modifies that row corr to that action ID
            ' Logs a SINGLE user action of an activity
            '

            If actionID = -1 Then
                Dim newRow As Data.DataRow = DSrenewal.UserActionTable.NewRow()
                newRow("ActivityID") = acID

                ' Auto update Action ID
                If DSrenewal.UserActionTable.Rows.Count = 0 Then
                    newRow("ActionID") = 0
                Else
                    newRow("ActionID") = DSrenewal.UserActionTable.AsEnumerable().Max(Function(row) row.Field(Of Integer)("ActionID")) + 1    ' Auto updates

                End If
                newRow("Date") = date_.Date
                newRow("Level") = level_
                newRow("isL" & level_) = 1
                DSrenewal.UserActionTable.Rows.Add(newRow)
            Else
                Dim selectedRow As Data.DataRow = DSrenewal.UserActionTable.Select("ActionID = '" & actionID & "'")(0)
                selectedRow("ActivityID") = acID
                selectedRow("Date") = date_.Date
                selectedRow("Level") = level_
                selectedRow("isL" & level_) = 1
            End If

        End Sub
        Public Function processUAFile(ByVal path As String) As Boolean
            ' ReaDSrenewal a User Actions file, unscrambles????, and finally stores in a datatable
            ' @param path File location
            ' returns if function successful or not
            '

            Dim tempDSrenewal As New DSRenewal                           ' Temporary DSrenewal

            Using reader As New StreamReader(path)
                Dim line As String = reader.ReadLine()     ' Skip the header

                While line IsNot Nothing
                    ' Process the current line
                    Dim tempArray As String() = Split(line, "<>")

                    If tempArray.Length = 4 Then
                        Dim tempAction As New UserAction With {
                        .ActionID = tempArray(0),
                        .ActivityDate = tempArray(1),
                        .ActivityID = tempArray(2),
                        .Level = tempArray(3)
                        }

                        Dim success As Boolean = parseUserAction(tempAction, tempDSrenewal)

                        If Not success Then Return False

                    End If

                    ' Read the next line
                    line = reader.ReadLine()
                End While
            End Using

            DSrenewal.UserActionTable.Clear()
            DSrenewal.UserActionTable.Load(tempDSrenewal.UserActionTable.CreateDataReader)

            Return True
        End Function

        Public Function parseUserAction(ByRef action As UserAction, ByRef tempDSrenewal As DSRenewal) As Boolean
            ' AdDSrenewal a renewal activity object to the temporary Activity Table
            ' @param activity Activity to add
            ' @param tempDSrenewal Temporary Dataset 
            ' returns if function successful or not

            If Not tempDSrenewal.UserActionTable.Rows.Contains(action.ActionID) Then
                Dim newRow As Data.DataRow = tempDSrenewal.UserActionTable.NewRow()
                newRow("ActivityID") = action.ActivityID
                newRow("ActionID") = action.ActionID
                newRow("Date") = action.ActivityDate
                newRow("Level") = action.Level

                newRow("isL" & action.Level.ToString) = 1

                tempDSrenewal.UserActionTable.Rows.Add(newRow)
            Else
                Return False
            End If

            Return True
        End Function

        Public Function calculateScore(ByVal dateFrom As Date, ByVal dateTo As Date, ByVal scoreString As String) As Data.DataTable
            ' Calculate scores for all renewal activities 
            '

            Dim temp = DSrenewal.UserActionTable.Where(Function(x) x("Date") >= dateFrom.Date And x("Date") < dateTo.Date)

            ' Getting level wise aggregates of activities
            Dim query As IEnumerable(Of Object) =
                From parentRow In DSrenewal.ActivityTable
                Group Join childRow In temp On parentRow("ActivityID") Equals childRow("ActivityID") Into abc = Group
                Select New With {
                    .ActivityRow = parentRow,
                    .totalL1 = abc.Sum(Function(c) c("isL1")),
                    .totalL2 = abc.Sum(Function(c) c("isL2")),
                    .totalL3 = abc.Sum(Function(c) c("isL3"))
                }


            Dim resultTable As DataTable = DSrenewal.ActivityTable.Clone()
            resultTable = mainProject.queryToTable(query, resultTable)


            '' Imputing 0 for Null values
            For Each row As DataRow In resultTable.Rows
                For Each L As String In {"L1", "L2", "L3"}
                    If IsDBNull(row("total" & L)) Then row("total" & L) = 0
                Next

            Next

            '' Renewal algorithm
            For Each row As Data.DataRow In resultTable.Rows
                Dim L1Achieved, L2Achieved, L3Achieved As Double

                For Each L As String In {"L1", "L2", "L3"}
                    If Regex.IsMatch(row(L & "Unit"), "(day)|(presentation)|(activity)|(membership)|(mentee)") Then
                        row(L & "Achieved") = {row("total" & L) * row(L & "Point"), row(L & scoreString)}.Min

                        ' week is assumed to be of 5 days
                    ElseIf Regex.IsMatch(row(L & "Unit"), "(week)") Then
                        row(L & "Achieved") = {Math.Floor(row("total" & L) / 5) * row(L & "Point"), row(L & scoreString)}.Min
                    End If
                Next


            Next

            ' Cleaning
            For Each L As String In {"L1", "L2", "L3"}
                If resultTable.Columns.Contains("total" & L) Then
                    resultTable.Columns.Remove("total" & L)
                End If
            Next

            calculateScore = resultTable

        End Function

        Public Sub ActivityTabSummary(ByVal activityString As String, ByVal level As String, ByVal dateTo As Date)
            ' Activity tab summaries
            '


            ' Left part of the tab (DatagridviewDatagridview)
            Dim pattern As String = "(^\w\d+) - (.+)"
            Dim match As Match = Regex.Match(activityString, pattern)
            Dim acID As String = match.Groups(1).Value

            Dim temp = DSrenewal.UserActionTable.Select().Where(Function(x) x("ActivityID") = acID And x("Date") <= dateTo.Date)

            If level.ToLower <> "any" Then
                temp = temp.Where(Function(x) x("Level") = Int(level))
            End If


            Dim temp2 As IEnumerable(Of Object) = From t In temp
                                                  Group t By ID = t("Date"), L = t("Level") Into Group
                                                  Select New With {
                         .Date = Group.FirstOrDefault()("Date"),
                         .Level = Group.FirstOrDefault()("Level"),
                        .ActionCount = Group.Count(Function(x) x("ActivityID") = acID)
                        }

            '' Show on the DGV
            Dim tbl As New Data.DataTable
            tbl.Columns.Add("Date", GetType(Date))
            tbl.Columns.Add("Level")
            tbl.Columns.Add("ActionCount")


            tbl = mainProject.queryToTable(temp2, tbl)
            LBRenewal.DataGridView7.DataSource = tbl

            '' 'Column hide
            ''hereee
            For Each dgc As DataGridViewColumn In LBRenewal.DataGridView7.Columns
                If Regex.IsMatch(dgc.DataPropertyName.ToLower, "(date)|(level)|(actioncount)") Then
                    ' dgc.Visible = True
                Else
                    'dgc.Visible = False
                End If
            Next

            '' Since diagrams are level specific 
            If level.ToLower = "any" Then
                LBRenewal.Label11.Text = "One year score:"
                LBRenewal.Label9.Text = "Five years score:"
                Exit Sub
            End If


            ' Now calculate scores for this Activity
            Dim scoreString As String
            Dim dateFrom As Date
            Dim tempActable = DSrenewal.ActivityTable.Where(Function(x) x("ActivityID") = acID)(0)
            Dim unitPoint As Double = tempActable("L" & level & "Point")
            Dim achieved1 As Double         ' One year score
            Dim possible1 As Double
            Dim achieved5 As Double         ' Five years score
            Dim possible5 As Double


            '' For one year
            dateFrom = DateAdd(DateInterval.Year, -1, dateTo)        ' 1 year back date

            Dim tempUAtable1 = DSrenewal.UserActionTable.Where(Function(x) x("Level") = level And x("ActivityID") = acID And x("Date") >= dateFrom.Date And x("Date") < dateTo.Date)
            Dim tempTotal1 As Double = tempUAtable1.Sum(Function(x) x("isL" & level))
            possible1 = tempActable("L" & level & "PerYear")

            '' For five years
            dateFrom = DateAdd(DateInterval.Year, -5, dateTo)        ' 5 year back date
            possible5 = tempActable("L" & level & "PerFiveYear")

            Dim tempUAtable5 = DSrenewal.UserActionTable.Where(Function(x) x("Level") = level And x("ActivityID") = acID And x("Date") >= dateFrom.Date And x("Date") < dateTo.Date)
            Dim tempTotal5 As Double = tempUAtable5.Sum(Function(x) x("isL" & level))

            '' Renewal algorithm
            If Regex.IsMatch(tempActable("L" & level & "Unit"), "(day)|(presentation)|(activity)|(membership)|(mentee)") Then
                achieved1 = {tempTotal1 * unitPoint, possible1}.Min
                achieved5 = {tempTotal5 * unitPoint, possible5}.Min

                ' week is assumed to be of 5 days
            ElseIf Regex.IsMatch(tempActable("L" & level & "Unit"), "(week)") Then
                achieved1 = {Math.Floor(tempTotal1 / 5) * unitPoint, possible1}.Min
                achieved5 = {Math.Floor(tempTotal5 / 5) * unitPoint, possible5}.Min
            End If



            '' For one year
            LBRenewal.Label11.Text = "One year score: achieved " & achieved1 & " out of a total possible of " & possible1
            LBRenewal.ProgressBar1.Maximum = possible1
            LBRenewal.ProgressBar1.Value = achieved1

            If achieved1 > possible1 Then
                LBRenewal.ProgressBar1.ProgressColor = Color.FromArgb(182, 83, 50)
                LBRenewal.ProgressBar1.ProgressColor2 = Color.FromArgb(167, 83, 50)
            Else
                LBRenewal.ProgressBar1.ProgressColor = Color.FromArgb(139, 201, 77)
                LBRenewal.ProgressBar1.ProgressColor2 = Color.FromArgb(192, 255, 192)
            End If


            '' For five years
            LBRenewal.Label9.Text = "Five years score: achieved " & achieved5 & " out of a total possible of " & possible5
            LBRenewal.ProgressBar2.Maximum = possible5
            LBRenewal.ProgressBar2.Value = achieved5

            If achieved5 > possible5 Then
                LBRenewal.ProgressBar2.ProgressColor = Color.FromArgb(182, 83, 50)
                LBRenewal.ProgressBar2.ProgressColor2 = Color.FromArgb(167, 83, 50)
            Else
                LBRenewal.ProgressBar2.ProgressColor = Color.FromArgb(139, 201, 77)
                LBRenewal.ProgressBar2.ProgressColor2 = Color.FromArgb(192, 255, 192)
            End If


        End Sub

        Public Sub SaveUAFile()
            '
            ' Delete first
            Try
                IO.File.Delete(mainProject.UAFILE)
            Catch ex As Exception
            End Try

            ' Create from scratch
            Using fs As New FileStream(mainProject.UAFILE, FileMode.OpenOrCreate), writer As New StreamWriter(fs)

                ' Loop through the lines and write each line to the file
                For Each row As Data.DataRow In renewal.DSrenewal.UserActionTable.Rows
                    Dim txt As String = ""
                    txt &= row("ActionID") & "<>" & row("Date") & "<>" & row("ActivityID") & "<>" & row("Level") & vbCrLf
                    writer.WriteLine(txt)
                Next
            End Using
        End Sub



    End Module
End Namespace
