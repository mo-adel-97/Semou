/**
 * Real 3D shapes in pure CSS (no WebGL, no library, nothing to download):
 * a glossy sphere, a tilted golden torus that spins, a glass cube turning in space (6 faces, preserve-3d),
 * and a capsule. They take the site's brand colour, float on their own, and follow the depth parallax
 * (mouse / phone tilt) through the .depth-* classes. Everything animates transform only → GPU, no layout work.
 */
export function Sphere({ className = '', gold }: { className?: string; gold?: boolean }) {
  return <span aria-hidden className={`s3d s3d-sphere ${gold ? 'gold' : ''} ${className}`} />
}

export function Torus({ className = '' }: { className?: string }) {
  return (
    <span aria-hidden className={`s3d s3d-torus-wrap ${className}`}>
      <span className="s3d-torus" />
    </span>
  )
}

export function Cube({ className = '' }: { className?: string }) {
  return (
    <span aria-hidden className={`s3d s3d-cube-wrap ${className}`}>
      <span className="s3d-cube">
        {['front', 'back', 'right', 'left', 'top', 'bottom'].map((f) => <i key={f} className={`f ${f}`} />)}
      </span>
    </span>
  )
}

export function Capsule({ className = '' }: { className?: string }) {
  return <span aria-hidden className={`s3d s3d-capsule ${className}`} />
}
