/**
 * Turns the dashboard logo into a tab icon that reads at 16px: only the emblem (the wordmark under it is cut off
 * when there is a clear gap), trimmed and centred on a white rounded tile, so it shows on dark and light tabs.
 */
export async function tabIcon(src: string, size = 64, rounded = true): Promise<string> {
  const img = new Image()
  img.crossOrigin = 'anonymous'
  img.src = src
  await img.decode()
  const W = img.naturalWidth, H = img.naturalHeight
  const c = document.createElement('canvas'); c.width = W; c.height = H
  const x = c.getContext('2d', { willReadFrequently: true })!
  x.drawImage(img, 0, 0)
  const d = x.getImageData(0, 0, W, H).data
  const ink = (i: number) => d[i + 3] > 40 && d[i] + d[i + 1] + d[i + 2] < 600
  const rows: number[] = []
  for (let y = 0; y < H; y++) { let k = 0; for (let i = y * W * 4, e = i + W * 4; i < e; i += 4) if (ink(i)) k++; rows.push(k) }
  const top = rows.findIndex((k) => k > 1)
  if (top < 0) throw new Error('empty logo')
  let last = H - 1; while (last > top && rows[last] <= 1) last--
  // the first clear band (≥ 1.5% of the height) after the first 45% of the drawing separates the emblem from the name
  const gap = Math.max(3, Math.round(H * 0.015))
  let bot = last + 1
  for (let y = top + Math.round((last - top) * 0.45); y < last - gap; y++) {
    if (rows.slice(y, y + gap).every((k) => k <= 1)) { bot = y; break }
  }
  let l = W, r = 0
  for (let y = top; y < bot; y++) for (let xx = 0; xx < W; xx++) if (ink((y * W + xx) * 4)) { if (xx < l) l = xx; if (xx > r) r = xx }
  const o = document.createElement('canvas'); o.width = o.height = size
  const g = o.getContext('2d')!
  g.fillStyle = '#fff'; g.beginPath(); g.roundRect(0, 0, size, size, rounded ? size * 0.22 : 0); g.fill()
  const bw = r - l + 1, bh = bot - top, inner = size * (rounded ? 0.84 : 0.76), sc = Math.min(inner / bw, inner / bh)
  g.imageSmoothingQuality = 'high'
  g.drawImage(img, l, top, bw, bh, (size - bw * sc) / 2, (size - bh * sc) / 2, bw * sc, bh * sc)
  return o.toDataURL('image/png')
}
