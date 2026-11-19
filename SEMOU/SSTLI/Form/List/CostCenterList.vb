Imports System.Data.SqlClient

Public Class CostCenterList
    Dim dbcontext As New SemouEntities
    Public IS_Yes As Boolean = False
    Sub LOADCOSTCENTER()
        BranchGrid.Rows.Clear()
        Dim NAME As String = Nothing
        Dim CODE As String = Nothing
        Dim ISUSE As String = Nothing
        Dim ACCOUNTTYPE As String = Nothing
        Dim ACCOUNTTYPE2 As String = Nothing
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("CostCenterList", connection)
        If TxtFind.Text <> Nothing Then
            If FindByName.Checked = True Then
                NAME = TxtFind.Text
                comm.Parameters.AddWithValue("@CenterName", NAME)
                comm.Parameters.AddWithValue("@Code", CODE)
            ElseIf FindByCode.Checked = True Then
                CODE = TxtFind.Text
                comm.Parameters.AddWithValue("@CenterName", NAME)
                comm.Parameters.AddWithValue("@Code", CODE)
            End If
        End If
        If TxtIsUse.Text <> Nothing Then
            ISUSE = TxtIsUse.Text
            comm.Parameters.AddWithValue("@IsUse", ISUSE)
        Else
            comm.Parameters.AddWithValue("@IsUse", ISUSE)
        End If
        If TxtAccountType.Text <> Nothing Then
            ACCOUNTTYPE = TxtAccountType.Text
            comm.Parameters.AddWithValue("@AccountType", ACCOUNTTYPE)
        Else
            comm.Parameters.AddWithValue("@AccountType", ACCOUNTTYPE)
        End If
        If TxtAccountType2.Text <> Nothing Then
            ACCOUNTTYPE2 = TxtAccountType2.Text
            comm.Parameters.AddWithValue("@AccountType2", ACCOUNTTYPE2)
        Else
            comm.Parameters.AddWithValue("@AccountType2", ACCOUNTTYPE2)
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "CostCenterList")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            BranchGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                BranchGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                BranchGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                BranchGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                BranchGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                BranchGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Private Sub CostCenterList_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            LOADCOSTCENTER()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TxtFind_TextChanged(sender As Object, e As EventArgs) Handles TxtFind.TextChanged
        Try
            LOADCOSTCENTER()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub BranchGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles BranchGrid.CellContentClick

    End Sub

    Private Sub BranchGrid_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles BranchGrid.CellDoubleClick
        IS_Yes = True
        Me.Close()
    End Sub
End Class