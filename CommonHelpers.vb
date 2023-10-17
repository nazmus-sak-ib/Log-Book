Imports System.Reflection
Imports System.Text.RegularExpressions

Namespace mainProject
    Module CommonHelpers
        Public Function fileBrowseAndShow(ByRef listBox As ListBox, ByVal filter_ As String, ByVal Optional multiselect_ As Boolean = False) As String
            ' Browses for a file and adds it to a listbox
            ' @param listBox Listbox to add to
            ' @param filter_ File dialogue filter
            ' @param multiselect_ File dialogue multiselect
            ' @returns Filepath

            Dim openFileDialog As New OpenFileDialog() With {
            .Filter = filter_,
            .Multiselect = False,
            .FilterIndex = 1
            }


            If openFileDialog.ShowDialog() = DialogResult.OK Then
                ' Get the selected file path
                Dim filePath As String = openFileDialog.FileName

                ' Remove if listbox already contains, then add
                If listBox.Items.Count = 1 Then listBox.Items.RemoveAt(0)
                listBox.Items.Add(filePath)

                fileBrowseAndShow = filePath
            Else
                fileBrowseAndShow = ""
            End If
        End Function

        Public Function queryToTable(ByRef query As IEnumerable(Of Object), ByRef resultTable As Data.DataTable) As Data.DataTable
            ' Converts a query to a datatable
            ' Query can contain all Simple Properties (basic variable types) through the SELECT command
            ' It can also contain data rows (obtained from the JOIN command)
            ' In that case, that property is assumed to contain the keyword "row"
            '
            ' Args:
            '       query: query object
            '       resultTable: A blank table with existing column structures (typically cloned from an existing table)
            ' Returns:
            '       Filled result datatable
            '

            ' Extracting the query properties
            Dim querySimpleProperties As New List(Of Boolean)       ' If property at each index is simple (string/int/double) or not (another datarow)
            Dim queryProperties As New List(Of PropertyInfo)        ' The properties
            Dim existingColumns As List(Of String) = resultTable.Columns.Cast(Of Data.DataColumn).Select(Function(x) x.ColumnName).ToList

            Dim firstItem = query.FirstOrDefault()

            If firstItem IsNot Nothing Then
                queryProperties = firstItem.GetType().GetProperties().ToList

                '' Add the property names to the list
                For Each prop In queryProperties
                    If Not Regex.IsMatch(prop.Name.ToLower, "row") Then
                        querySimpleProperties.Add(True)
                    Else
                        querySimpleProperties.Add(False)
                    End If

                Next
            End If

            '' Add columns from the simple properties from before
            For i = 0 To queryProperties.Count - 1
                If querySimpleProperties(i) And Not existingColumns.Contains(queryProperties(i).Name) Then
                    resultTable.Columns.Add(queryProperties(i).Name, queryProperties(i).PropertyType)

                End If
            Next

            ' Loop through each query item to extract the simple and row properties

            For Each item In query
                Dim newRow = resultTable.NewRow()

                For i = 0 To queryProperties.Count - 1

                    If querySimpleProperties(i) Then    ' Simple properties
                        newRow(queryProperties(i).Name) = queryProperties(i).GetValue(item)
                    Else
                        Dim nestedRow = queryProperties(i).GetValue(item)   ' Row properties
                        Dim nestedProperties As List(Of PropertyInfo) = nestedRow.GetType.GetProperties.ToList

                        For Each prop As PropertyInfo In nestedProperties
                            Try
                                newRow(prop.Name) = prop.GetValue(nestedRow)
                            Catch ex As Exception
                            End Try

                        Next
                    End If
                Next

                resultTable.Rows.Add(newRow)
            Next

            Return resultTable

        End Function
    End Module
End Namespace