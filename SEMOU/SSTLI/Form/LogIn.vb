Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System.Data.Entity
Imports System.Threading.Tasks

Public Class LogIn
    Dim dbcontext As New SemouEntities
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub

    Private Sub TxtUserName_TextChanged(sender As Object, e As EventArgs) Handles TxtUserName.TextChanged

    End Sub

    Private Sub TxtUserName_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TxtUserName.KeyPress
        Try
            If e.KeyChar = Chr(13) Then
                TxtPassword.Focus()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TxtPassword_TextChanged(sender As Object, e As EventArgs) Handles TxtPassword.TextChanged

    End Sub

    Private Sub TxtPassword_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TxtPassword.KeyPress
        Try
            If e.KeyChar = Chr(13) Then
                CmdOk.Select()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdOk_Click(sender As Object, e As EventArgs) Handles CmdOk.Click
        Try
            Dim result = dbcontext.User_Info.FirstOrDefault(Function(m) m.UserName = TxtUserName.Text And m.Password = TxtPassword.Text)
            If (IsNothing(result)) Then
                MessageBox.Show("برجاء التأكد من أسم المستخدم أو كلمة المرور وإعادة المحاولة", "مدير النظام", MessageBoxButtons.OK, MessageBoxIcon.Hand)
                TxtUserName.SelectAll()
            Else
                If result.Staut_ = False Then
                    MessageBox.Show("المستخدم موقوف برجاء مراجعة الإدارة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                Else
                    Dim h As New HomePage
                    h.TxtFullName.Caption = result.FullName
                    h.TxtVer.Caption = TxtGuid.Text
                    AllFiles.USER_GUID____ = Guid.Parse(result.Guid.ToString)
                    AllFiles.SELLERMAN_GUID____ = Guid.Parse(result.SellerGuid.ToString)
                    AllFiles.ONLINE_USER_NAME____ = result.FullName
                    h.Show()
                    Me.Close()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub TxtGuid_Click(sender As Object, e As EventArgs) Handles TxtGuid.Click

    End Sub
    Private Sub LogIn_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            Database.SetInitializer(Of SemouEntities)(Nothing)
            ' تسخين Entity Framework في الخلفية وقت ما المستخدم بيكتب الباسورد - عشان زرار الدخول يبقى سريع
            Task.Run(Sub()
                         Try
                             Using db As New SemouEntities
                                 Dim u As String = ""
                                 db.User_Info.FirstOrDefault(Function(m) m.UserName = u And m.Password = u)
                             End Using
                         Catch
                         End Try
                     End Sub)
            AllFiles.SHOWSYSTEMINFO()
            TxtSystemVirsion.Text = AllFiles.SYSTEMVER
            If TxtSystemVirsion.Text = TxtGuid.Text Then
                CmdOk.Enabled = True
                CmdUpdate.Visible = False
            Else
                MessageBox.Show("يتوفر تحديث جديد من البرنامج" & vbCrLf &
                                "لكي تتمكن من استخدام البرنامج يجب الضغط على تحديث", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                CmdOk.Enabled = False
                CmdUpdate.Visible = True
            End If
        Catch ex As SqlException
            MessageBox.Show("مش قادر يتصل بالسيرفر" & vbCrLf & "اتأكد إن الجهاز متصل بالشبكة / الإنترنت وحاول تاني", "الاتصال بالسيرفر", MessageBoxButtons.OK, MessageBoxIcon.Error)
            CmdOk.Enabled = False
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Private Sub PATHPRO(PATH As String)
        Dim file As System.IO.StreamWriter
        AllFiles.SHOWSYSTEMINFO()
        file = My.Computer.FileSystem.OpenTextFileWriter(AllFiles.SETTINGPASS, False)
        file.WriteLine(PATH.ToString())
        file.Close()
    End Sub
    Private Sub CmdUpdate_Click(sender As Object, e As EventArgs) Handles CmdUpdate.Click
        Try
            Dim startInfo = New ProcessStartInfo()
            AllFiles.SHOWSYSTEMINFO()
            startInfo.FileName = AllFiles.UPDATEEXE
            startInfo.Arguments = "value_to_pass"
            PATHPRO(Application.StartupPath & "\" & AllFiles.EXENAME)
            Process.Start(startInfo)
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class