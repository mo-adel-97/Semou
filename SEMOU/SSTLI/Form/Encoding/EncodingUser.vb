Imports System.Data.SqlClient

Public Class EncodingUser
    Dim dbcontext As New SemouEntities
    Dim REASON As String = ""
    Sub LOADMAINMENU()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("LoadMainMenu", connection)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "LoadMainMenu")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            MenuGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                MenuGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                MenuGrid.Rows(COUNT__).Cells(1).Value = False
                MenuGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(1).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub LOADMAINFORM()
        BasicFormGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("LoadMainForm", connection)
        If TxtFind.Text <> Nothing Then
            comm.Parameters.AddWithValue("@Name", TxtFind.Text)
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "LoadMainForm")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            BasicFormGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                BasicFormGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                BasicFormGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub LOADBRANCH()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("LoadBranch", connection)
        comm.Parameters.AddWithValue("@IsUse", True)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "LoadBranch")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            MainBranchGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                MainBranchGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                MainBranchGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                MainBranchGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(3).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Private Sub EncodingUser_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            LOADMAINMENU()
            LOADMAINFORM()
            LOADBRANCH()
            RegDocPosted.WHATFORMTYPEGUID("adduser")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
            Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub

    Private Sub TxtFind_TextChanged(sender As Object, e As EventArgs) Handles TxtFind.TextChanged
        Try
            LOADMAINFORM()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub BasicFormGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles BasicFormGrid.CellContentClick

    End Sub
    Sub FINDITEMEDIT_EDIT()
        Dim rowindex As String
        Dim found As Boolean = False
        For Each row As DataGridViewRow In FormGrid.Rows
            If row.Cells.Item("ItemNewCode").Value.ToString() = TxtFormCode.Text Then
                rowindex = row.Index.ToString()
                found = True
                FormGrid.FirstDisplayedScrollingRowIndex = row.Index
                FormGrid.Rows(row.Index).Selected = True
                Exit For
            Else
                found = False
            End If
        Next
    End Sub
    Private Sub BasicFormGrid_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles BasicFormGrid.CellDoubleClick
        Try
            Dim row As DataGridViewRow
            Dim found As Boolean = False
            Dim ii As Integer = BasicFormGrid.CurrentRow.Index
            For Each row In FormGrid.Rows
                For i As Integer = 0 To FormGrid.Rows.Count - 1
                    If row.Cells(7).Value = BasicFormGrid.Rows(ii).Cells(1).Value.ToString Then
                        found = True
                        TxtFormCode.Text = BasicFormGrid.Rows(ii).Cells(1).Value.ToString()
                        FINDITEMEDIT_EDIT()
                        MessageBox.Show("تم اختيار الشاشة مسبقا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Exit For
                    End If
                Next
            Next
            If Not found Then
                For Each row In BasicFormGrid.SelectedRows
                    FormGrid.Rows.Add()
                    Dim i As Integer = FormGrid.Rows.Count() - 1
                    FormGrid.Rows(i).Cells(0).Value = BasicFormGrid.Rows(ii).Cells(0).Value.ToString()
                    FormGrid.Rows(i).Cells(1).Value = False
                    FormGrid.Rows(i).Cells(2).Value = False
                    FormGrid.Rows(i).Cells(3).Value = False
                    FormGrid.Rows(i).Cells(4).Value = False
                    FormGrid.Rows(i).Cells(5).Value = False
                    FormGrid.Rows(i).Cells(6).Value = False
                    FormGrid.Rows(i).Cells(7).Value = BasicFormGrid.Rows(ii).Cells(1).Value.ToString()
                Next
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub ADDALLFORM()
        FormGrid.Rows.Clear()
        Dim NAME As String = Nothing
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("LoadMainForm", connection)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "LoadMainForm")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            FormGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                FormGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                FormGrid.Rows(COUNT__).Cells(1).Value = True
                FormGrid.Rows(COUNT__).Cells(2).Value = True
                FormGrid.Rows(COUNT__).Cells(3).Value = True
                FormGrid.Rows(COUNT__).Cells(4).Value = True
                FormGrid.Rows(COUNT__).Cells(5).Value = True
                FormGrid.Rows(COUNT__).Cells(6).Value = True
                FormGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(1).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub ADDALLMAINMENU()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("LoadMainMenu", connection)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "LoadMainMenu")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            MenuGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                MenuGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                MenuGrid.Rows(COUNT__).Cells(1).Value = True
                MenuGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(1).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Private Sub CmdDelAll_Click(sender As Object, e As EventArgs) Handles CmdDelAll.Click
        Try
            FormGrid.Rows.Clear()
            LOADMAINMENU()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFullControl_Click(sender As Object, e As EventArgs) Handles CmdFullControl.Click
        Try
            ADDALLFORM()
            ADDALLMAINMENU()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Public Function GET___NO() As Integer
        Dim MaxId = dbcontext.User_Info.OrderByDescending(Function(p) p.Id).FirstOrDefault()?.Id
        Dim MaxDay = New User_Info
        If (IsNothing(MaxId)) Then
            MaxId = 1
            TxtCode.Text = MaxId
        Else
            MaxId += 1
            MaxDay.ID = MaxId
            TxtCode.Text = MaxId
        End If
        Return MaxId
    End Function
    Function VALIDATE_DATA() As Boolean
        If TxtFullName.Text = Nothing Then
            MessageBox.Show("برجاء ادخال اسم الموظف", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtUserName.Text = Nothing Then
            MessageBox.Show("برجاء ادخال اسم المستخدم", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtPassWord.Text = Nothing Then
            MessageBox.Show("برجاء ادخال كلمة المرور", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Public Function UsernameExists(username As String, usercode As Double) As Boolean
        Dim query = dbcontext.User_Info.FirstOrDefault(Function(m) m.UserName = username And m.Code <> usercode)
        If (IsNothing(query)) Then
            Return False
        Else
            Return True
        End If
    End Function
    Sub ADDUSER()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddUser", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Id", GET___NO)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@UserName", TxtUserName.Text)
        comm.Parameters.AddWithValue("@Password", TxtPassWord.Text)
        comm.Parameters.AddWithValue("@FullName", TxtFullName.Text)
        comm.Parameters.AddWithValue("@Staut_", ChkStaut.Value)
        comm.Parameters.AddWithValue("@ChkBranch", ChkBranch.Checked)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@ChkAmount", ChkAmount.Checked)
        comm.Parameters.AddWithValue("@ChkOtherFess", ChkAddFess.Checked)
        comm.Parameters.AddWithValue("@SellerGuid", TxtManGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWUSERINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowUserInfo", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowUserInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtUserName.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtPassWord.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtFullName.Text = ds.Tables(0).Rows(i)(2).ToString()
                ChkStaut.Value = ds.Tables(0).Rows(i)(3)
                TxtGuid.Text = ds.Tables(0).Rows(i)(4).ToString()
                ChkBranch.Checked = ds.Tables(0).Rows(i)(5)
                TxtBranchName.Text = ds.Tables(0).Rows(i)(6).ToString()
                TxtBranchGuid.Text = ds.Tables(0).Rows(i)(7).ToString()
                ChkAmount.Checked = ds.Tables(0).Rows(i)(8)
                ChkAddFess.Checked = ds.Tables(0).Rows(i)(9)
                TxtManName.Text = ds.Tables(0).Rows(i)(10).ToString()
                TxtManGuid.Text = ds.Tables(0).Rows(i)(11).ToString()
            Next
        End If
    End Sub
    Sub ADDUSERMENU()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddUserMenu", connection)
        comm.CommandType = CommandType.StoredProcedure
        For i As Integer = 0 To MenuGrid.Rows.Count - 1
            If MenuGrid.Rows(i).Cells(2).Value = Nothing Then
                Exit For
            Else
                comm.Parameters.AddWithValue("@UserGuid", TxtGuid.Text)
                comm.Parameters.AddWithValue("@MenuGuid", MenuGrid.Rows(i).Cells(2).Value.ToString())
                comm.Parameters.AddWithValue("@View_", MenuGrid.Rows(i).Cells(1).Value)
            End If
            If connection.State = ConnectionState.Open Then
                connection.Close()
            End If
            connection.Open()
            Dim rs As Integer = comm.ExecuteNonQuery()
            If rs > 0 Then
                comm.Parameters.Clear()
            End If
        Next
    End Sub
    Sub ADDUSERPRIMISION()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddUserPremision", connection)
        comm.CommandType = CommandType.StoredProcedure
        For i As Integer = 0 To FormGrid.Rows.Count - 1
            If FormGrid.Rows(i).Cells(7).Value = Nothing Then
                Exit For
            Else
                comm.Parameters.AddWithValue("@UserGuid", TxtGuid.Text)
                comm.Parameters.AddWithValue("@FormGuid", FormGrid.Rows(i).Cells(7).Value.ToString())
                comm.Parameters.AddWithValue("@Add_", FormGrid.Rows(i).Cells(1).Value)
                comm.Parameters.AddWithValue("@Edit", FormGrid.Rows(i).Cells(2).Value)
                comm.Parameters.AddWithValue("@Del", FormGrid.Rows(i).Cells(3).Value)
                comm.Parameters.AddWithValue("@Print_", FormGrid.Rows(i).Cells(4).Value)
                comm.Parameters.AddWithValue("@Find_", FormGrid.Rows(i).Cells(5).Value)
                comm.Parameters.AddWithValue("@View_", FormGrid.Rows(i).Cells(6).Value)
            End If
            If connection.State = ConnectionState.Open Then
                connection.Close()
            End If
            connection.Open()
            Dim rs As Integer = comm.ExecuteNonQuery()
            If rs > 0 Then
                comm.Parameters.Clear()
            End If
        Next
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
    Sub SHOWUSERFORM()
        FormGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowUserPremision", connection)
        comm.Parameters.AddWithValue("@UserGuid", TxtGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowUserPremision")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            FormGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                FormGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                FormGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1)
                FormGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2)
                FormGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3)
                FormGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4)
                FormGrid.Rows(COUNT__).Cells(5).Value = ds.Tables(0).Rows(i)(5)
                FormGrid.Rows(COUNT__).Cells(6).Value = ds.Tables(0).Rows(i)(6)
                FormGrid.Rows(COUNT__).Cells(7).Value = ds.Tables(0).Rows(i)(7).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub SHOWUSERMAINMENU()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowUserMenu", connection)
        comm.Parameters.AddWithValue("@UserGuid", TxtGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowUserMenu")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            MenuGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                MenuGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                MenuGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1)
                MenuGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub SHOWUSERBRANCH()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowUserBranch", connection)
        comm.Parameters.AddWithValue("@UserGuid", TxtGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowUserBranch")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            BranchGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                BranchGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                BranchGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1)
                BranchGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Sub DELETEUSERFORM()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteUserPrimision", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@UserGuid", TxtGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        Dim rs As Integer = comm.ExecuteNonQuery()
        If rs > 0 Then
            comm.Parameters.Clear()
        End If
    End Sub
    Sub DELETEUSERMENU()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteUserMenu", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@UserGuid", TxtGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        Dim rs As Integer = comm.ExecuteNonQuery()
        If rs > 0 Then
            comm.Parameters.Clear()
        End If
    End Sub
    Sub UPDATEUSER()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateUser", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@UserName", TxtUserName.Text)
        comm.Parameters.AddWithValue("@Password", TxtPassWord.Text)
        comm.Parameters.AddWithValue("@FullName", TxtFullName.Text)
        comm.Parameters.AddWithValue("@Staut_", ChkStaut.Value)
        comm.Parameters.AddWithValue("@ChkBranch", ChkBranch.Checked)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@ChkAmount", ChkAmount.Checked)
        comm.Parameters.AddWithValue("@ChkOtherFess", ChkAddFess.Checked)
        comm.Parameters.AddWithValue("@SellerGuid", TxtManGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADDUSERBRANCH()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddUserBranch", connection)
        comm.CommandType = CommandType.StoredProcedure
        For i As Integer = 0 To BranchGrid.Rows.Count - 1
            If BranchGrid.Rows(i).Cells(2).Value = Nothing Then
                Exit For
            Else
                comm.Parameters.AddWithValue("@UserGuid", TxtGuid.Text)
                comm.Parameters.AddWithValue("@BranchGuid", BranchGrid.Rows(i).Cells(2).Value.ToString())
            End If
            If connection.State = ConnectionState.Open Then
                connection.Close()
            End If
            connection.Open()
            Dim rs As Integer = comm.ExecuteNonQuery()
            If rs > 0 Then
                comm.Parameters.Clear()
            End If
        Next
    End Sub
    Sub DELETEUSERBRANCH()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("DeleteUserBranch", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@UserGuid", TxtGuid.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        Dim rs As Integer = comm.ExecuteNonQuery()
        If rs > 0 Then
            comm.Parameters.Clear()
        End If
    End Sub
    Sub FORSAVE()
        If AllFiles.FORSAVE("adduser") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDUSER()
            SHOWUSERINFO()
            ADDUSERMENU()
            ADDUSERPRIMISION()
            ADDUSERBRANCH()
            ADD_USERACTION_()
            MessageBox.Show("تم حفظ بيانات المستخدم بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("adduser") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل"
            UPDATEUSER()
            DELETEUSERMENU()
            ADDUSERMENU()
            DELETEUSERFORM()
            ADDUSERPRIMISION()
            DELETEUSERBRANCH()
            ADDUSERBRANCH()
            ADD_USERACTION_()
            MessageBox.Show("تم تعديل بيانات المستخدم بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdSave.Click
        Try
            If VALIDATE_DATA() = False Then
                Return
            Else
                If UsernameExists(TxtUserName.Text, Val(TxtCode.Text)) = True Then
                    MessageBox.Show("اسم المستخدم الذي تم ادخاله مستخدم مسبقا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
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
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub Cmd_Find_Click(sender As Object, e As EventArgs) Handles Cmd_Find.Click
        Try
            If AllFiles.FORFIND("adduser") = False Then
                Return
            Else
                Dim w As New UserList
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                    TxtGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                End If
                If TxtGuid.Text <> Nothing Then
                    SHOWUSERINFO()
                    SHOWUSERMAINMENU()
                    SHOWUSERFORM()
                    SHOWUSERBRANCH()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub NEW___()
        TxtUserName.Text = Nothing
        TxtPassWord.Text = Nothing
        TxtFullName.Text = Nothing
        TxtCode.Text = Nothing
        TxtGuid.Text = Nothing
        ChkStaut.Value = True
        TxtFind.Text = Nothing
        FormGrid.Rows.Clear()
        ChkBranch.Checked = False
        ChkAmount.Checked = False
        BranchGrid.Rows.Clear()
        ChkAddFess.Checked = False
        TxtManGuid.Text = Nothing
        TxtManName.Text = Nothing
    End Sub

    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            NEW___()
            EncodingUser_Load(e, e)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub ChkBranch_CheckedChanged(sender As Object, e As EventArgs) Handles ChkBranch.CheckedChanged
        If ChkBranch.Checked = True Then
            CmdFindBranch.Enabled = True
            'TxtBranchGuid.Text = Nothing
            MainBranchGrid.Enabled = True
        Else
            CmdFindBranch.Enabled = False
            TxtBranchName.Text = Nothing
            MainBranchGrid.Enabled = False
            TxtBranchGuid.Text = "00000000-0000-0000-0000-000000000000"
        End If
    End Sub

    Private Sub CmdFindBranch_Click(sender As Object, e As EventArgs) Handles CmdFindBranch.Click
        Try
            Dim w As New NewBranchList
            w.TxtIsUse.Text = True
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

    Private Sub MainBranchGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles MainBranchGrid.CellContentClick

    End Sub

    Private Sub MainBranchGrid_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles MainBranchGrid.CellDoubleClick
        Try
            Dim row As DataGridViewRow
            Dim found As Boolean = False
            Dim ii As Integer = MainBranchGrid.CurrentRow.Index
            For Each row In BranchGrid.Rows
                For i As Integer = 0 To BranchGrid.Rows.Count - 1
                    If row.Cells(2).Value = MainBranchGrid.Rows(ii).Cells(2).Value.ToString Then
                        found = True
                        MessageBox.Show("تم اختيار الفرع مسبقا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Exit For
                    End If
                Next
            Next
            If Not found Then
                For Each row In MainBranchGrid.SelectedRows
                    BranchGrid.Rows.Add()
                    Dim i As Integer = BranchGrid.Rows.Count() - 1
                    BranchGrid.Rows(i).Cells(0).Value = MainBranchGrid.Rows(ii).Cells(0).Value.ToString()
                    BranchGrid.Rows(i).Cells(1).Value = MainBranchGrid.Rows(ii).Cells(1).Value.ToString()
                    BranchGrid.Rows(i).Cells(2).Value = MainBranchGrid.Rows(ii).Cells(2).Value.ToString()
                Next
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFindMan_Click(sender As Object, e As EventArgs) Handles CmdFindMan.Click
        Try
            Dim w As New SalesManList
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtManName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtManGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class