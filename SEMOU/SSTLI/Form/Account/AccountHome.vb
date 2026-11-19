Imports System.Data.SqlClient

Public Class AccountHome
    Dim dbcontext As New SemouEntities
    Sub CREATE_NODE()
        Dim TRN As New TreeNode
        Dim dt As New DataTable
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("ShowTreeAccount", connection)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowTreeAccount")
        For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
            If ds.Tables(0).Rows(i)(2).ToString = "00000000-0000-0000-0000-000000000000" Then
                TRN = New TreeNode(ds.Tables(0).Rows(i)(0).ToString() + " " + ds.Tables(0).Rows(i)(1).ToString)
                TRN.Tag = ds.Tables(0).Rows(i)(4).ToString()
                If ds.Tables(0).Rows(i)(3).ToString() <> 0 Then
                    TRN.ImageIndex = 0
                    TRN.SelectedImageIndex = 0
                Else
                    TRN.ImageIndex = 1
                    TRN.SelectedImageIndex = 1
                End If
                TreeItem.Nodes.Add(TRN)
            End If
        Next
        connection.Close()
        For Each node As TreeNode In TreeItem.Nodes
            CHELD(node)
        Next

    End Sub
    Sub CHELD(ByVal NODE As TreeNode)

        Dim TRN As New TreeNode
        Dim dt As New DataTable
        Dim connection = dbcontext.Database.Connection
        Dim fg = New SqlCommand("ShowTreeAccount", connection)
        fg.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()

        End If
        connection.Open()
        fg.ExecuteNonQuery()

        Dim tx As New SqlDataAdapter(fg)
        Dim er As DataSet = New DataSet()
        tx.Fill(er, "ShowTreeAccount")
        For i As Integer = 0 To er.Tables(0).Rows.Count - 1
            Dim GUID__ As String = NODE.Tag
            If er.Tables(0).Rows(i)(2).ToString = GUID__ Then
                TRN = New TreeNode(er.Tables(0).Rows(i)(0).ToString() + " " + er.Tables(0).Rows(i)(1).ToString)
                TRN.Tag = er.Tables(0).Rows(i)(4).ToString()
                If er.Tables(0).Rows(i)(3).ToString() <> 0 Then
                    TRN.ImageIndex = 0
                    TRN.SelectedImageIndex = 0
                Else
                    TRN.ImageIndex = 1
                    TRN.SelectedImageIndex = 1
                End If
                NODE.Nodes.Add(TRN)
                CHELD(TRN)
            End If

        Next
    End Sub
    Private Sub AccountHome_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try

        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            If AllFiles.FORFIND("accountchart") = False Then
                Return
            Else
                TreeItem.ImageList = ImageList1
                TreeItem.Nodes.Clear()
                CREATE_NODE()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdSave.Click
        Try
            If AllFiles.FORFIND("accountchart") = False Then
                Return
            Else
                TreeItem.ImageList = ImageList1
                TreeItem.Nodes.Clear()
                CREATE_NODE()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub
End Class