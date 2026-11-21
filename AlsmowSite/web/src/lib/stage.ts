/**
 * One show at a time: the butterflies' scenes and the programmes box's envelopes take turns, so they never cover
 * each other. Whoever claims the stage keeps it until it releases it; then a short breathing gap before the next one.
 */
const GAP = 4000
let owner: string | null = null
let freeAt = 0

export const stage = {
  claim(who: string): boolean {
    if (owner && owner !== who) return false
    if (!owner && Date.now() < freeAt) return false
    owner = who
    return true
  },
  release(who: string) {
    if (owner !== who) return
    owner = null
    freeAt = Date.now() + GAP
  },
}
