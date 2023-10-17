Imports System.Data.Entity.Core
Imports System.IO
Imports System.Net.Security
Imports System.Reflection
Imports System.Security
Imports System.Text.RegularExpressions
Imports System.Windows.Forms.LinkLabel
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports Microsoft.VisualBasic.Logging

Namespace mainProject
    Public Module Module1
        ' This module contains codes related to the main Log Book form
        '

        Public SOURCEFOLDERPATH As String = Path.Combine(Directory.GetCurrentDirectory(), "source") ' Source files stored in this folder
        Public PROJECTSLIST As String = Path.Combine(SOURCEFOLDERPATH, "projects list.txt")
        Public PROJECTNAME As String
        Public PROJECTTYPE As String

        Public Function UAFILE() As String                     ' User actions stored in this file
            Return Path.Combine(SOURCEFOLDERPATH, PROJECTNAME & " user actions.csv")
        End Function
        Public Function ACTIVITYFILE() As String               ' Activity definitions stored in this fine
            Return Path.Combine(SOURCEFOLDERPATH, PROJECTTYPE & " activity definition.csv")
        End Function

        Public Function countOfDays() As Data.DataTable
            ' Activity frequency table
            ' @DGV Datagridview to show summary on

            Dim dv As New DataView(renewal.DSrenewal.ActivityTable)

            Dim newCol As New DataColumn("Total Days", GetType(Integer)) With {
                .Expression = "count(Child.Date)"
                }

            dv.Table.Columns.Add(newCol)

            countOfDays = dv.ToTable

        End Function


        Public Function FilterByDate(ByVal date_ As Date, ByRef dt As DataTable) As Data.DataTable
            ' Shows all activities on a day
            ' @date_ Read from Datetimepicker
            ' @returns filtered table
            '

            Dim dv As New DataView(dt) With {
                .RowFilter = "Date = #" & date_.Date & "#"
            }

            FilterByDate = dv.ToTable

        End Function







    End Module
End Namespace