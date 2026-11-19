Imports System.Data.SqlClient

Public Class ChooseData
    Dim dbcontext As New SemouEntities
    Public IS_Yes As Boolean = False
    Private Sub CmdFind_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdSave.Click
        If TxtRoomGuid.Text <> Nothing Then
            IS_Yes = True
            Me.Close()
        Else
            MessageBox.Show("برجاء اختيار القاعه اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If
    End Sub

    Private Sub CmdClose_Click(sender As Object, e As EventArgs) Handles CmdClose.Click
        Me.Close()
    End Sub
    Sub SHOWROOM()
        RoomGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowEmptyRoomInDay", connection)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@DayGuid", TxtDayGuid.Text)
        comm.Parameters.AddWithValue("@TimeGuid", TxttimeGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowEmptyRoomInDay")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            RoomGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                RoomGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(1).ToString()
                RoomGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(0).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Private Sub ChooseData_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            SHOWROOM()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub RoomGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles RoomGrid.CellContentClick

    End Sub

    Private Sub RoomGrid_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles RoomGrid.CellDoubleClick
        Try
            Dim i As Integer = RoomGrid.CurrentCell.RowIndex
            Dim j As Integer = RoomGrid.CurrentCell.ColumnIndex
            TxtRoomName.Text = RoomGrid.Rows(i).Cells(0).Value.ToString()
            TxtRoomGuid.Text = RoomGrid.Rows(i).Cells(1).Value.ToString()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class