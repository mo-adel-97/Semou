Imports System.Data.SqlClient

Public Class WhatRegTypeNeed
    Dim dbcontext As New SemouEntities
    Public IS_Yes As Boolean = False
    Private Sub CmdClose_Click(sender As Object, e As EventArgs) Handles CmdClose.Click
        Me.Close()
    End Sub

    Private Sub WhatRegTypeNeed_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            RegDocPosted.WHATFORMTYPEGUID("addregdoc")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub GETAUTODOCINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("AutoGetDocType", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@DocTypeGuid", TxtTypeGuid.Text)
        comm.Parameters.AddWithValue("@BranchGuid", TxtBranchGuid.Text)
        comm.Parameters.AddWithValue("@RegType", CboType.SelectedIndex)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "AutoGetDocType")
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtDocCode.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtDocName.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtDocGuid.Text = ds.Tables(0).Rows(i)(2).ToString()
            Next
        Else
            CboType.Text = Nothing
            MessageBox.Show("برجاء مراجعه مدير النظام لا يتوفر دفتر للتسجيل", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If
    End Sub

    Private Sub CboType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CboType.SelectedIndexChanged

    End Sub

    Private Sub CboType_SelectedValueChanged(sender As Object, e As EventArgs) Handles CboType.SelectedValueChanged

    End Sub

    Private Sub CmdOk_Click(sender As Object, e As EventArgs) Handles CmdOk.Click
        Try
            If TxtBranchGuid.Text = Nothing Then
                MessageBox.Show("برجاء اختيار فرع الدراسة اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                If TxtDocGuid.Text = Nothing Then
                    MessageBox.Show("برجاء اختيار فرع الدراسة ونوع التسجيل اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                Else
                    If CboType.Text <> Nothing Then
                        If CboType.SelectedIndex = 0 Then
                            If AllFiles.ASKSTUDENTDIPLOMINFO(TxtAccountGuid.Text) = True Then
                                MessageBox.Show("لا يمكن تسجيل الطالب فى اكثر من دبلوم فى نفس الوقت" & vbCrLf &
                                                "الطالب مسجل فى" & " " & AllFiles.DIPLOMNAME, "تنبيه",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                                Return
                            Else
                                ''''''''''''''''''''''''''''''''''''   الجزء الخاص باتفاقية التدريب 
                                Dim result As DialogResult = MessageBox.Show("هل وقع المتدرب اتفاقية التدريب؟", "اتفاقية التدريب", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                                If result = DialogResult.Yes Then
                                    ChkContract.Checked = True
                                Else
                                    ChkContract.Checked = False
                                End If
                                IS_Yes = True
                                Me.Close()
                            End If
                        Else
                            IS_Yes = True
                            Me.Close()
                        End If
                    Else
                        MessageBox.Show("برجاء تحديد نوع التسجيل اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try

    End Sub

    Private Sub CmdFindDepart_Click(sender As Object, e As EventArgs) Handles CmdFindDepart.Click
        Try
            Dim w As New NewBranchList
            w.TxtType.Text = 0
            w.TxtIsUse.Text = True
            w.TxtIsRegOn.Text = True
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

    Private Sub CboType_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles CboType.SelectionChangeCommitted
        Try
            If TxtBranchGuid.Text <> Nothing Then
                GETAUTODOCINFO()
            Else
                MessageBox.Show("برجاء اختيار فرع الدراسة اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                CboType.Text = Nothing
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CboType_KeyPress(sender As Object, e As KeyPressEventArgs) Handles CboType.KeyPress
        e.Handled = True
    End Sub
End Class