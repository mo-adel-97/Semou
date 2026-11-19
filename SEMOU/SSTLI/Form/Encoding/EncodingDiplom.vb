Imports System.Data.SqlClient

Public Class EncodingDiplom
    Dim dbcontext As New SemouEntities
    Dim REASON As String = ""
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub
    Public Function GET___NO() As Integer
        Dim MaxId = dbcontext.Diploms.OrderByDescending(Function(p) p.ID).FirstOrDefault()?.ID
        Dim MaxDay = New Diplom
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
    Sub ADDDIPLOM()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDiplom", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Id", GET___NO)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@DiplomName", TxtDiplomName.Text)
        comm.Parameters.AddWithValue("@CountNum", TxtCount.Text)
        comm.Parameters.AddWithValue("@CountText", TxtCountText.Text)
        comm.Parameters.AddWithValue("@DiplomDesc", TxtNotes.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@DiplomType", CboType.SelectedIndex)
        comm.Parameters.AddWithValue("@DiplomUnit", TxtUnit.Text)
        comm.Parameters.AddWithValue("@TaxType", CboTax.SelectedIndex)
        comm.Parameters.AddWithValue("@TaxValue", TxtTaxValue.Text)
        comm.Parameters.AddWithValue("@StudyType", CboStudyType.SelectedIndex)
        comm.Parameters.AddWithValue("@Depart", CboDepart.SelectedIndex)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub UPDATEDIPLOM()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateDiplom", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@DiplomName", TxtDiplomName.Text)
        comm.Parameters.AddWithValue("@CountNum", TxtCount.Text)
        comm.Parameters.AddWithValue("@CountText", TxtCountText.Text)
        comm.Parameters.AddWithValue("@DiplomDesc", TxtNotes.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        comm.Parameters.AddWithValue("@DiplomType", CboType.SelectedIndex)
        comm.Parameters.AddWithValue("@DiplomUnit", TxtUnit.Text)
        comm.Parameters.AddWithValue("@TaxType", CboTax.SelectedIndex)
        comm.Parameters.AddWithValue("@TaxValue", TxtTaxValue.Text)
        comm.Parameters.AddWithValue("@StudyType", CboStudyType.SelectedIndex)
        comm.Parameters.AddWithValue("@Depart", CboDepart.SelectedIndex)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
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
    Sub SHOWDIPLOMINF()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowDiplomInfo", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowDiplomInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtDiplomName.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtCount.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtCountText.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtNotes.Text = ds.Tables(0).Rows(i)(3).ToString()
                ChkStaut.Value = ds.Tables(0).Rows(i)(4)
                TxtGuid.Text = ds.Tables(0).Rows(i)(5).ToString()
                CboType.SelectedIndex = ds.Tables(0).Rows(i)(6)
                TxtUnit.Text = ds.Tables(0).Rows(i)(7).ToString()
                CboTax.SelectedIndex = ds.Tables(0).Rows(i)(8)
                TxtTaxValue.Text = ds.Tables(0).Rows(i)(9).ToString()
                If IsDBNull(ds.Tables(0).Rows(i)(10)) = False Then
                    CboStudyType.SelectedIndex = ds.Tables(0).Rows(i)(10)
                Else
                    CboStudyType.Text = Nothing
                End If
                If IsDBNull(ds.Tables(0).Rows(i)(11)) = False Then
                    CboDepart.SelectedIndex = ds.Tables(0).Rows(i)(11)
                Else
                    CboDepart.Text = Nothing
                End If
            Next
        End If
    End Sub
    Function VALIDATE_DATA() As Boolean
        If TxtDiplomName.Text = Nothing Then
            MessageBox.Show("برجاء ادخال اسم الدبلوم - الدورة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtCount.Text = Nothing Then
            MessageBox.Show("برجاء ادخال مدة الدبلوم - الدورة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf TxtCountText.Text = Nothing Then
            MessageBox.Show("برجاء وصف مدة الدبلوم - الدورة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf CboTax.Text = Nothing Then
            MessageBox.Show("برجاء أختيار نوع الضريبة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Sub FORSAVE()
        If AllFiles.FORSAVE("adddiplom") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDDIPLOM()
            SHOWDIPLOMINF()
            ADD_USERACTION_()
            MessageBox.Show("تم حفظ بيانات الدبلوم - الدورة بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("adddiplom") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل"
            UPDATEDIPLOM()
            ADD_USERACTION_()
            MessageBox.Show("تم تعديل بيانات الدبلوم - الدورة بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Private Sub EncodingDiplom_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RegDocPosted.WHATFORMTYPEGUID("adddiplom")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
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
    Sub NEW__()
        TxtDiplomName.Text = Nothing
        TxtCode.Text = Nothing
        TxtGuid.Text = Nothing
        TxtCount.Text = Nothing
        TxtCountText.Text = Nothing
        TxtNotes.Text = Nothing
        ChkStaut.Value = True
        CboType.Text = Nothing
        TxtUnit.Text = "Pcs"
        CboTax.Text = Nothing
        TxtTaxValue.Text = 0
        CboDepart.Text = Nothing
    End Sub

    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            NEW__()
            EncodingDiplom_Load(e, e)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFind_Click(sender As Object, e As EventArgs) Handles CmdFind.Click
        Try
            If AllFiles.FORFIND("adddiplom") = False Then
                Return
            Else
                Dim w As New DiplomList
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                    TxtGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                End If
                If TxtGuid.Text <> Nothing Then
                    SHOWDIPLOMINF()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class