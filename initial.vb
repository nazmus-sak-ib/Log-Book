Imports System.Drawing.Text
Imports System.IO
Imports System.Text.RegularExpressions

Namespace initial
    Module initial
        ' All codes pertaining to an initial type of project
        Public DSinitial As New DSInitial                                                     ' Main dataset (from Dataset1.xsd)

        Public Function processDefinitionFile(ByVal path As String) As Boolean
            ' ReaDSinitial a definition file, unscrambles, and finally stores in a datatable
            ' @param path File location
            ' returns if function successful or not
            '

            Dim tempDS As New DSInitial                           ' Temporary DSinitial

            Using reader As New StreamReader(path)
                Dim line As String = reader.ReadLine()     ' Skip the header
                line = reader.ReadLine()
                While line IsNot Nothing
                    ' Process the current line
                    Dim tempArray As String() = Split(line, ",")

                    Dim acName As String = tempArray(0)

                    ' Adding to the dataset
                    If Not tempDS.ActivityTable.Rows.Contains(acName) Then

                        Dim newRow As Data.DataRow = tempDS.ActivityTable.NewRow()
                        newRow("ActivityName") = acName
                        newRow("L1TrainingMin") = tempArray(1)
                        newRow("L1ExperienceMin") = tempArray(2)
                        newRow("L2TrainingMin") = tempArray(3)
                        newRow("L2ExperienceMin") = tempArray(4)
                        newRow("L3TrainingMin") = tempArray(5)
                        newRow("L3ExperienceMin") = tempArray(6)

                        tempDS.ActivityTable.Rows.Add(newRow)
                    Else
                        MsgBox("Activity ID " & acName & " exists more than once in the excel file! Please try again", vbOK, "Abort")
                        Return False
                    End If

                    ' Read the next line
                    line = reader.ReadLine()
                End While
            End Using

            DSinitial.Clear()
            DSinitial = tempDS

            Return True
        End Function
        Public Sub logUserAction(ByVal activityName As String, ByVal date_ As Date, ByVal level_ As Integer, ByVal hours As Double, ByVal TorE As String, Optional ByVal actionID As Integer = -1)
            ' @acID ActivityID
            ' @date_ Date of a user action
            ' @parm actionID if = -1, logs a new row, else modifies that row corr to that action ID
            ' Logs a SINGLE user action of an activity
            ' Okay

            If actionID = -1 Then
                Dim newRow As Data.DataRow = DSinitial.UserActionTable.NewRow()


                ' Auto update Action ID
                If DSinitial.UserActionTable.Rows.Count = 0 Then
                    newRow("ActionID") = 0
                Else
                    newRow("ActionID") = DSinitial.UserActionTable.AsEnumerable().Max(Function(row) row("ActionID")) + 1    ' Auto updates
                End If

                newRow("ActivityName") = activityName
                newRow("Date") = date_.Date
                newRow("Level") = level_
                newRow("isL" & level_) = 1
                newRow("Hour") = hours
                newRow("TorE") = TorE

                DSinitial.UserActionTable.Rows.Add(newRow)
            Else
                Dim selectedRow As Data.DataRow = DSinitial.UserActionTable.Select("ActionID = '" & actionID & "'")(0)
                selectedRow("ActivityName") = activityName
                selectedRow("Date") = date_.Date
                selectedRow("Level") = level_
                selectedRow("isL" & level_) = 1
                selectedRow("Hour") = hours
                selectedRow("TorE") = TorE
            End If

        End Sub
        Public Sub CreateOrReadUA()
            ' Creates User Action file if it does not exist. ReaDSinitial it if it does
            '

            Try
                Using fs As New FileStream(mainProject.UAFILE, FileMode.OpenOrCreate)
                End Using
            Catch
            End Try

            ' load it
            Dim success As Boolean = initial.processUAFile(mainProject.UAFILE)
        End Sub

        Public Function processUAFile(ByVal path As String) As Boolean
            ' ReaDSinitial a User Actions file, unscrambles????, and finally stores in a datatable
            ' @param path File location
            ' returns if function successful or not
            '

            Dim tempDS As New DSInitial                           ' Temporary DSinitial

            Using reader As New StreamReader(path)
                Dim line As String = reader.ReadLine()     ' Skip the header

                While line IsNot Nothing
                    ' Process the current line
                    Dim tempArray As String() = Split(line, "<>")

                    If tempArray.Length = 4 Then

                        Dim actionID As Double = tempArray(0)

                        If Not tempDS.UserActionTable.Rows.Contains(actionID) Then
                            Dim newRow As Data.DataRow = tempDS.UserActionTable.NewRow()
                            newRow("ActionID") = actionID
                            newRow("ActivityName") = tempArray(1)
                            newRow("Date") = tempArray(2)
                            newRow("Level") = tempArray(3)
                            newRow("Hour") = tempArray(4)
                            newRow("TorE") = tempArray(5)

                            newRow("isL" & tempArray(3).ToString) = 1

                            tempDS.UserActionTable.Rows.Add(newRow)
                        Else
                            Return False
                        End If
                    End If

                    ' Read the next line
                    line = reader.ReadLine()
                End While
            End Using

            DSinitial.UserActionTable.Clear()
            DSinitial.UserActionTable.Load(tempDS.UserActionTable.CreateDataReader)

            Return True
        End Function

        Public Function calculateScore(ByVal dateFrom As Date, ByVal dateTo As Date) As Data.DataTable
            ' Calculate scores (sums up the hours) for all initial activities 
            ' okay?

            Dim temp = DSinitial.UserActionTable.Where(Function(x) x("Date") >= dateFrom.Date And x("Date") <= dateTo.Date)

            Dim query1 As IEnumerable(Of Object) =
                From parentRow In temp
                Group parentRow By ID = parentRow("ActivityName"), Level = parentRow("Level"), TorE = parentRow("TorE") Into def = Group
                Select New With {
                    .ActivityName = def.FirstOrDefault()("ActivityName"),
                    .TorE = def.FirstOrDefault()("TorE"),
                    .Level = def.FirstOrDefault()("Level"),
                    .Achieved = def.Sum(Function(x) x.Hour)
                    }



            ' Getting level wise aggregates of activities

            Dim query As IEnumerable(Of Object) =
                From parentRow In DSinitial.ActivityTable
                Group Join childRow In query1 On parentRow("ActivityName") Equals childRow.ActivityName Into abc = Group
                Select New With {
                    .ActivityName = parentRow("ActivityName"),
                     .L1TrainingAchieved = abc.Where(Function(x) x.Level = 1 And x.TorE = "training").Sum(Function(x) x.Achieved),
                     .L1TrainingMin = parentRow.L1TrainingMin,
                   .L1ExperienceAchieved = abc.Where(Function(x) x.Level = 1 And x.TorE = "experience").Sum(Function(x) x.Achieved),
                    .L1ExperienceMin = parentRow.L1ExperienceMin,
                     .L2TrainingAchieved = abc.Where(Function(x) x.Level = 2 And x.TorE = "training").Sum(Function(x) x.Achieved),
                     .L2TrainingMin = parentRow.L2TrainingMin,
                     .L2ExperienceAchieved = abc.Where(Function(x) x.Level = 2 And x.TorE = "experience").Sum(Function(x) x.Achieved),
                     .L2ExperienceMin = parentRow.L2ExperienceMin,
                    .L3TrainingAchieved = abc.Where(Function(x) x.Level = 3 And x.TorE = "training").Sum(Function(x) x.Achieved),
                    .L3TrainingMin = parentRow.L3TrainingMin,
                  .L3ExperienceAchieved = abc.Where(Function(x) x.Level = 3 And x.TorE = "experience").Sum(Function(x) x.Achieved),
                     .L3ExperienceMin = parentRow.L3ExperienceMin
                }


            Dim resultTable As DataTable = DSinitial.ActivityTable.Clone()
            resultTable = mainProject.queryToTable(query, resultTable)

            calculateScore = resultTable

        End Function

        Public Sub ActivityTabSummary(ByVal activityName As String, ByVal level As String, ByVal dateTo As Date)
            ' Activity tab summaries
            ' okay?

            Dim temp = From parentRow In DSinitial.UserActionTable
                       Where parentRow("ActivityName") = activityName And parentRow("Date") <= dateTo.Date
                       Select New With {
                           .ActivityName = parentRow("ActivityName"),
                           .TorE = parentRow("TorE"),
                           .Level = parentRow("Level"),
                           .Date = parentRow("Date"),
                           .Hour = parentRow("Hour"),
                           .ActionID = parentRow("ActionID")
                           }


            If level.ToLower <> "any" Then
                temp = temp.Where(Function(x) x.Level = Int(level))
            End If

            '' Show on the DGV
            Dim tbl As New Data.DataTable
            tbl = DSinitial.UserActionTable.Clone

            tbl = mainProject.queryToTable(temp, tbl)
            LBInitial.DataGridView7.DataSource = tbl

            '' Since diagrams are level specific 
            If level.ToLower = "any" Then
                LBInitial.ProgressBar1.Text = "Training hours: "
                LBInitial.ProgressBar1.Value = 0

                LBInitial.ProgressBar2.Text = "Experience hours: "
                LBInitial.ProgressBar2.Value = 0
                Exit Sub
            End If


            '' Training Hours
            Dim tempUAtable1 = tbl.AsEnumerable.Where(Function(x) x("TorE").tolower = "training")
            Dim tempTotal1 As Double = tempUAtable1.Sum(Function(x) x("Hour"))
            Dim tempPossible1 As Double = DSinitial.ActivityTable.Select("ActivityName = '" & activityName & "'")(0)("L" & level & "TrainingMin")

            LBInitial.Label11.Text = "Training hours: " & tempTotal1 & "/" & tempPossible1

            If tempTotal1 > tempPossible1 Then
                tempTotal1 = tempPossible1
                LBInitial.ProgressBar1.ProgressColor = Color.FromArgb(182, 83, 50)
                LBInitial.ProgressBar1.ProgressColor2 = Color.FromArgb(167, 83, 50)
            Else
                LBInitial.ProgressBar1.ProgressColor = Color.FromArgb(139, 201, 77)
                LBInitial.ProgressBar1.ProgressColor2 = Color.FromArgb(192, 255, 192)
            End If

            LBInitial.ProgressBar1.Maximum = tempPossible1
            LBInitial.ProgressBar1.Value = tempTotal1

            '' Experience Hours
            Dim tempUAtable2 = tbl.AsEnumerable.Where(Function(x) x("TorE").tolower = "experience")
            Dim tempTotal2 As Double = tempUAtable2.Sum(Function(x) x("Hour"))
            Dim tempPossible2 As Double = DSinitial.ActivityTable.Select("ActivityName = '" & activityName & "'")(0)("L" & level & "ExperienceMin")

            LBInitial.Label9.Text = "Experience hours: " & tempTotal2 & "/" & tempPossible2

            If tempTotal2 > tempPossible2 Then
                tempTotal2 = tempPossible2
                LBInitial.ProgressBar2.ProgressColor = Color.FromArgb(182, 83, 50)
                LBInitial.ProgressBar2.ProgressColor2 = Color.FromArgb(167, 83, 50)
            Else
                LBInitial.ProgressBar2.ProgressColor = Color.FromArgb(139, 201, 77)
                LBInitial.ProgressBar2.ProgressColor2 = Color.FromArgb(192, 255, 192)
            End If

            LBInitial.ProgressBar2.Maximum = tempPossible2
            LBInitial.ProgressBar2.Value = tempTotal2



        End Sub

        Public Sub SaveUAFile()
            ' Okay

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
                    txt &= row("ActionID") & "<>" & row("ActivityName") & "<>" & row("Date") & "<>" & row("Level") & row("Hour") & row("TorE") & vbCrLf
                    writer.WriteLine(txt)
                Next
            End Using
        End Sub

    End Module
End Namespace