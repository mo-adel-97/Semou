Imports System.Data.SqlClient

Public Class DailyHome
    Dim dbcontext As New SemouEntities
    Sub LOADDAILY()
        DiplomGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowDaily", connection)
        comm.Parameters.AddWithValue("@FDate", Convert.ToDateTime(TxtFromDate.Value))
        comm.Parameters.AddWithValue("@TDate", Convert.ToDateTime(TxtToDate.Value))
        If TxtBranchGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        End If
        If TxtCostCenterGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@CostGuid", TxtCostCenterGuid.Text)
        End If
        If TxtDocTypeGuid.Text <> Nothing Then
            comm.Parameters.AddWithValue("@TypeGuid", TxtDocTypeGuid.Text)
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowDaily")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            DiplomGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                DiplomGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                DiplomGrid.Rows(COUNT__).Cells(1).Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(1)).ToShortDateString()
                DiplomGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                DiplomGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                DiplomGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                DiplomGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5).ToString()
                DiplomGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(6).ToString()
                DiplomGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(7).ToString()
                DiplomGrid.Rows(COUNT__).Cells(8).Value = ds.Tables(0).Rows(i)(8).ToString()
                'DiplomGrid.Rows(COUNT__).Cells(9).Value = ds.Tables(0).Rows(i)(9).ToString()
                COUNT__ = COUNT__ + 1
            Next
            DAILYTOTAL()
        End If
    End Sub
    Sub DAILYTOTAL()
        Dim DIPLOMTAX As Double = 0
        Dim DIPLOMTOTAL As Double = 0
        For i As Integer = 0 To DiplomGrid.Rows.Count - 1
            If DiplomGrid.Rows(i).Cells(2).Value <> Nothing Then
                DIPLOMTOTAL = Val(DIPLOMTOTAL) + (Val(DiplomGrid.Rows(i).Cells(5).Value))
                DIPLOMTAX = Val(DIPLOMTAX) + (Val(DiplomGrid.Rows(i).Cells(6).Value))
            End If
        Next
        TxtTotalMaden.Text = Math.Round(Val(DIPLOMTOTAL), 2)
        TxtTotalDaen.Text = Math.Round(Val(DIPLOMTAX), 2)
        TxtDef.Text = Math.Round(Val(TxtTotalMaden.Text) - Val(TxtTotalDaen.Text), 2)
    End Sub
    Private Sub DailyHome_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            LOADDAILY()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindDepart_Click(sender As Object, e As EventArgs) Handles CmdFindDepart.Click
        Try
            Dim w As New NewBranchList
            w.TxtType.Text = 0
            w.TxtIsUse.Text = True
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtBranchName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtBranchGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
            If TxtBranchGuid.Text <> Nothing Then
                LOADDAILY()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdDelBranch_Click(sender As Object, e As EventArgs) Handles CmdDelBranch.Click
        TxtBranchName.Text = Nothing
        TxtBranchGuid.Text = Nothing
        LOADDAILY()
    End Sub

    Private Sub CmdFindCostCenter_Click(sender As Object, e As EventArgs) Handles CmdFindCostCenter.Click
        Try
            Dim w As New CostCenterList
            w.TxtAccountType.Text = 1
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtCostCenterName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtCostCenterGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
            If TxtCostCenterGuid.Text <> Nothing Then
                LOADDAILY()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdDelCostCenter_Click(sender As Object, e As EventArgs) Handles CmdDelCostCenter.Click
        TxtCostCenterName.Text = Nothing
        TxtCostCenterGuid.Text = Nothing
        LOADDAILY()
    End Sub

    Private Sub CmdFindDocType_Click(sender As Object, e As EventArgs) Handles CmdFindDocType.Click
        Try
            Dim w As New DocTypeList
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtDocTypeName.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                TxtDocTypeGuid.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
            End If
            If TxtDocTypeGuid.Text <> Nothing Then
                LOADDAILY()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdDelDoc_Click(sender As Object, e As EventArgs) Handles CmdDelDoc.Click
        TxtDocTypeName.Text = Nothing
        TxtDocTypeGuid.Text = Nothing
        LOADDAILY()
    End Sub
End Class