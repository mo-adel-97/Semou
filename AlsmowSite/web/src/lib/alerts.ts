import Swal from 'sweetalert2'

const base = Swal.mixin({
  customClass: { popup: 'font-sans' },
  buttonsStyling: true,
  confirmButtonText: 'حسنًا',
  cancelButtonText: 'إلغاء',
  reverseButtons: false,
})

const toastMixin = Swal.mixin({
  toast: true,
  position: 'bottom-start',
  showConfirmButton: false,
  timer: 2600,
  timerProgressBar: true,
})

export const toast = {
  success: (title: string) => toastMixin.fire({ icon: 'success', title }),
  error: (title: string) => toastMixin.fire({ icon: 'error', title, timer: 4500 }),
  info: (title: string) => toastMixin.fire({ icon: 'info', title }),
}

export async function confirmDanger(title: string, text?: string, confirmText = 'نعم، احذف') {
  const r = await base.fire({
    icon: 'warning',
    title,
    text,
    showCancelButton: true,
    confirmButtonText: confirmText,
    confirmButtonColor: '#c0392b',
    focusCancel: true,
  })
  return r.isConfirmed
}

export function successDialog(title: string, text?: string) {
  return base.fire({ icon: 'success', title, text })
}

export function errorDialog(title: string, text?: string) {
  return base.fire({ icon: 'error', title, text })
}
