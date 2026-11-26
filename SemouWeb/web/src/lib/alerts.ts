import Swal, { type SweetAlertIcon } from 'sweetalert2'

/**
 * Every message in the app goes through SweetAlert2 (replacing the desktop MessageBox):
 *  - alerts.*  : centered dialogs (success / error / warning / confirm / reason / loading)
 *  - notify.*  : small toasts in the corner for quick feedback
 * The look is themed in index.css (.swal2-*): brand green, Thmanyah Sans font, RTL.
 */
const base = Swal.mixin({
  customClass: {
    popup: 'sm-popup',
    title: 'sm-title',
    htmlContainer: 'sm-text',
    confirmButton: 'btn',
    cancelButton: 'btn ghost',
    denyButton: 'btn danger',
    actions: 'sm-actions',
    input: 'input',
    validationMessage: 'sm-validation',
  },
  buttonsStyling: false,
  reverseButtons: false,
  showClass: { popup: 'sm-in' },
  hideClass: { popup: 'sm-out' },
  confirmButtonText: 'حسناً',
  cancelButtonText: 'إلغاء',
})

const toast = Swal.mixin({
  toast: true,
  position: 'bottom-start',
  showConfirmButton: false,
  timer: 3200,
  timerProgressBar: true,
  customClass: { popup: 'sm-toast' },
  didOpen: el => {
    el.addEventListener('mouseenter', Swal.stopTimer)
    el.addEventListener('mouseleave', Swal.resumeTimer)
  },
})

export const notify = {
  success: (title: string) => toast.fire({ icon: 'success', title }),
  error: (title: string) => toast.fire({ icon: 'error', title, timer: 5000 }),
  info: (title: string) => toast.fire({ icon: 'info', title }),
}

export const alerts = {
  success: (title: string, text?: string) => base.fire({ icon: 'success', title, text, timer: text ? 4500 : 1800, showConfirmButton: false }),
  error: (text: string, title = 'تنبيه') => base.fire({ icon: 'error', title, text }),
  warning: (text: string, title = 'تنبيه') => base.fire({ icon: 'warning', title, text }),
  info: (text: string, title = 'معلومة') => base.fire({ icon: 'info', title, text }),

  /** Yes/No question. Resolves true when confirmed. */
  async confirm(opts: { title: string; text?: string; confirmText?: string; cancelText?: string; danger?: boolean; icon?: SweetAlertIcon }) {
    const r = await base.fire({
      icon: opts.icon ?? (opts.danger ? 'warning' : 'question'),
      title: opts.title,
      text: opts.text,
      showCancelButton: true,
      focusCancel: !!opts.danger,
      confirmButtonText: opts.confirmText ?? 'تأكيد',
      cancelButtonText: opts.cancelText ?? 'إلغاء',
      customClass: { confirmButton: opts.danger ? 'btn danger' : 'btn', cancelButton: 'btn ghost', popup: 'sm-popup', title: 'sm-title', htmlContainer: 'sm-text', actions: 'sm-actions' },
    })
    return r.isConfirmed
  },

  /** Desktop UpdateResaon: asks for the reason of an edit/delete; resolves null when cancelled. */
  async reason(title = 'سبب التعديل') {
    const r = await base.fire<string>({
      title,
      text: 'يتم تسجيل السبب في سجل حركات المستخدمين',
      input: 'textarea',
      inputPlaceholder: 'اكتب سبب الإجراء...',
      inputAttributes: { maxlength: '250', dir: 'rtl' },
      showCancelButton: true,
      confirmButtonText: 'تأكيد',
      inputValidator: v => (!v?.trim() ? 'يرجى إدخال سبب الإجراء أولاً' : undefined),
    })
    return r.isConfirmed ? (r.value ?? '').trim() : null
  },

  /** Small form inside a SweetAlert. `read` gets the popup and returns the value, or a string = validation error. */
  async form<T>(opts: { title: string; html: string; confirmText?: string; icon?: SweetAlertIcon; read: (popup: HTMLElement) => T | string; didOpen?: (popup: HTMLElement) => void; wide?: boolean }) {
    const r = await base.fire<T>({
      icon: opts.icon, title: opts.title, html: opts.html, showCancelButton: true, focusConfirm: false, didOpen: opts.didOpen, width: opts.wide ? 620 : undefined,
      confirmButtonText: opts.confirmText ?? 'تأكيد',
      preConfirm: () => {
        const v = opts.read(Swal.getPopup()!)
        if (typeof v === 'string') { Swal.showValidationMessage(v); return false }
        return v
      },
    })
    return r.isConfirmed ? (r.value as T) : null
  },

  /** Blocking spinner for long operations. Close it with alerts.close(). */
  loading(title = 'جاري الحفظ...') {
    base.fire({ title, allowOutsideClick: false, allowEscapeKey: false, showConfirmButton: false, didOpen: () => Swal.showLoading() })
  },
  close: () => Swal.close(),
}
