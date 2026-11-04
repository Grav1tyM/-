// Statlocker stores per-match accuracy and headshotAccuracy as fractions
// (0–1). The site renders them with (100 * value).toFixed(1) + "%".
// Comparison stays on the unrounded percent so "高于" is strict.

export function toPercent(fraction) {
  if (typeof fraction !== "number" || !Number.isFinite(fraction)) return null;
  return fraction * 100;
}

export function evaluateMatch(headshotFraction, accuracyFraction, headshotAbove, accuracyAbove) {
  const headshot = toPercent(headshotFraction);
  const accuracy = toPercent(accuracyFraction);
  if (headshot === null || accuracy === null) {
    return { include: false, missing: true, headshot, accuracy };
  }
  return {
    include: headshot > headshotAbove && accuracy > accuracyAbove,
    missing: false,
    headshot,
    accuracy,
  };
}

export function filterMatches(matches, headshotAbove, accuracyAbove) {
  const included = [];
  let missingAim = 0;
  for (const match of matches) {
    const result = evaluateMatch(match.headshotAccuracy, match.accuracy, headshotAbove, accuracyAbove);
    if (result.missing) {
      missingAim += 1;
      continue;
    }
    if (result.include) {
      included.push({
        matchId: match.match_id,
        startTimeMs: match.start_time,
        heroId: match.hero_id,
        headshot: result.headshot,
        accuracy: result.accuracy,
      });
    }
  }
  included.sort((a, b) => (b.startTimeMs || 0) - (a.startTimeMs || 0));
  return { included, missingAim };
}
