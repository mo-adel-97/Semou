Namespace My
    Partial Friend Class MyApplication
        ' تسجيل اتصال السيرفر (المشفر جوه البرنامج) قبل ما أي شاشة تفتح
        Private Sub MyApplication_Startup(sender As Object, e As ApplicationServices.StartupEventArgs) Handles Me.Startup
            Try
                ServerConfig.APPLY()
            Catch ex As Exception
                MessageBox.Show("تعذر تجهيز الاتصال بالسيرفر" & vbCrLf & ex.Message, "SEMOU", MessageBoxButtons.OK, MessageBoxIcon.Error)
                e.Cancel = True
            End Try
        End Sub
    End Class
End Namespace
