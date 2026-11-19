Imports System.Data.SqlClient

Public Class EncodingCalssRoom
    Dim dbcontext As New SemouEntities
    Dim REASON As String = ""
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub

    Private Sub Label4_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub TextBox4_TextChanged(sender As Object, e As EventArgs)

    End Sub

    Private Sub CmdFindBranch_Click(sender As Object, e As EventArgs) Handles CmdFindBranch.Click
        Try
            AllFiles.CHEKDEFBRANCHINFO(AllFiles.USER_GUID____.ToString)
            Dim w As New NewBranchList
            If AllFiles.DEFBRANCH = True Then
                w.TxtType.Text = 1
                w.TxtUserGuid.Text = AllFiles.USER_GUID____.ToString
                w.TxtFind.ReadOnly = True
            Else
                w.TxtType.Text = 0
                w.TxtIsUse.Text = True
                w.TxtFind.ReadOnly = False
            End If
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtBranchName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtBranchGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub NEW___()
        TxtBranchGuid.Text = Nothing
        TxtBranchName.Text = Nothing
        TxtH.Text = 0
        TxtW.Text = 0
        TxtRoomCount.Text = 0
        CboType.Text = Nothing
        TxtCode.Text = Nothing
        TxtGuid.Text = Nothing
        TxtRoomName.Text = Nothing
        TxtNotes.Text = Nothing
    End Sub
    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            NEW___()
            EncodingCalssRoom_Load(e, e)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Private Sub EncodingCalssRoom_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RegDocPosted.WHATFORMTYPEGUID("classroom")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CboType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CboType.SelectedIndexChanged

    End Sub

    Private Sub CboType_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles CboType.SelectionChangeCommitted
        Try
            If TxtH.Text <> 0 And TxtW.Text <> 0 And TxtH.Text <> Nothing And TxtW.Text <> Nothing Then
                Dim MAINSPEACE As Double = Math.Round(Val(TxtH.Text) * Val(TxtW.Text), 2)
                Dim COUNT As Double = 0
                Select Case CboType.SelectedIndex
                    Case 0
                        COUNT = Math.Round((Val(MAINSPEACE) - 4) / 1.5, 0)
                    Case 1
                        COUNT = Math.Round((Val(MAINSPEACE) - 4) / 2, 0)
                End Select
                TxtRoomCount.Text = COUNT.ToString()
            Else
                CboType.Text = Nothing
                MessageBox.Show("برجاء ادخال مساحة القاعة الطول والعرض اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Public Function GET___NO() As Integer
        Dim BRANCHGUID As Guid = Guid.Parse(TxtBranchGuid.Text)
        Dim MaxId = dbcontext.RoomClasses.Where(Function(m) m.BranchGuid = BRANCHGUID).OrderByDescending(Function(p) p.Id).FirstOrDefault()?.Id
        Dim MaxDay = New RoomClass
        If (IsNothing(MaxId)) Then
            MaxId = 1
            TxtCode.Text = MaxId
        Else
            MaxId += 1
            MaxDay.Id = MaxId
            TxtCode.Text = MaxId
        End If
        Return MaxId
    End Function
    Sub ADDROOMCLASS()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddRoomClass", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@ID", GET___NO)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@RoomName", TxtRoomName.Text)
        comm.Parameters.AddWithValue("@RoomH", TxtH.Text)
        comm.Parameters.AddWithValue("@RoomW", TxtW.Text)
        comm.Parameters.AddWithValue("@RoomType", CboType.SelectedIndex)
        comm.Parameters.AddWithValue("@RoomCount", TxtRoomCount.Text)
        comm.Parameters.AddWithValue("@RoomNotes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub UPDATEROOMCLASS()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateRoomClass", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@RoomName", TxtRoomName.Text)
        comm.Parameters.AddWithValue("@RoomH", TxtH.Text)
        comm.Parameters.AddWithValue("@RoomW", TxtW.Text)
        comm.Parameters.AddWithValue("@RoomType", CboType.SelectedIndex)
        comm.Parameters.AddWithValue("@RoomCount", TxtRoomCount.Text)
        comm.Parameters.AddWithValue("@RoomNotes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWROOMCLASSINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowRoomClassInfo", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowRoomClassInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtRoomName.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtH.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtW.Text = ds.Tables(0).Rows(i)(3).ToString()
                CboType.SelectedIndex = ds.Tables(0).Rows(i)(4)
                TxtRoomCount.Text = ds.Tables(0).Rows(i)(5).ToString()
                TxtNotes.Text = ds.Tables(0).Rows(i)(6).ToString()
                TxtGuid.Text = ds.Tables(0).Rows(i)(7).ToString()
            Next
        End If
    End Sub
    Sub ADD_USERACTION_()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("Add_UserAction", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@ActionGuid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@UserGuid", AllFiles.USER_GUID____)
        comm.Parameters.AddWithValue("@Date_", Convert.ToDateTime(DateTime.Now))
        comm.Parameters.AddWithValue("@Time_", Convert.ToDateTime(DateTime.Now.ToShortTimeString))
        comm.Parameters.AddWithValue("@Notes_", AllFiles.ActionType___)
        comm.Parameters.AddWithValue("@ActionTypeGuid", TxtTypeGuid.Text)
        comm.Parameters.AddWithValue("@ActionReason", REASON)
        comm.Parameters.AddWithValue("@ActionCode", TxtCode.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Function VALIDATE_DATA() As Boolean
        If TxtBranchGuid.Text = Nothing Then
            MessageBox.Show("برجاء ادخال اسم الطالب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtRoomName.Text = Nothing Then
            MessageBox.Show("برجاء ادخال رقم الجوال", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtH.Text = Nothing Or TxtH.Text = 0 Then
            MessageBox.Show("برجاء ادخال طول القاعه", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtW.Text = Nothing Or TxtW.Text = 0 Then
            MessageBox.Show("برجاء ادخال عرض القاعه", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf CboType.Text = Nothing Then
            MessageBox.Show("برجاء تحديد نوع القاعه", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Sub FORSAVE()
        If AllFiles.FORSAVE("classroom") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDROOMCLASS()
            SHOWROOMCLASSINFO()
            ADD_USERACTION_()
            MessageBox.Show("تم حفظ القاعه بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("classroom") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل"
            UPDATEROOMCLASS()
            ADD_USERACTION_()
            MessageBox.Show("تم تعديل القاعه بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Private Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdSave.Click
        Try
            If VALIDATE_DATA() = False Then
                Return
            Else
                If TxtGuid.Text = Nothing Then
                    FORSAVE()
                Else
                    Dim w As New UpdateResaon
                    w.ShowDialog()
                    If w.IS_Yes = True Then
                        REASON = w.TxtNotes.Text
                        FORUPDATE()
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindRoom_Click(sender As Object, e As EventArgs) Handles CmdFindRoom.Click
        Try
            If TxtBranchGuid.Text <> Nothing Then
                Dim w As New RoomClassList
                w.TxtBranchGuid.Text = TxtBranchGuid.Text
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                End If
                If TxtCode.Text <> Nothing Then
                    SHOWROOMCLASSINFO()
                End If
            Else
                MessageBox.Show("برجاء اختيار الفرع اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class