export type Priority = "headshot" | "accuracy";

export type MatchRow = {
  matchId: string;
  /** Fraction from 0 to 1, same as Statlocker's `accuracy` field. */
  accuracy: number | null;
  /** Fraction from 0 to 1, same as Statlocker's `headshotAccuracy` field. */
  headshotAccuracy: number | null;
  startTimeMs: number | null;
  kills: number | null;
  deaths: number | null;
  assists: number | null;
};

export type FilterResult = {
  rows: MatchRow[];
  missingStats: number;
};

/**
 * A match is listed only when both rates are strictly above their thresholds.
 * Priority decides the order: the chosen rate first, then the other rate,
 * then newer matches first. Equality is not "above".
 */
export function filterMatches(
  matches: readonly MatchRow[],
  headshotThresholdPercent: number,
  accuracyThresholdPercent: number,
  priority: Priority,
): FilterResult {
  let missingStats = 0;
  const rows: MatchRow[] = [];
  for (const match of matches) {
    if (!hasRate(match.headshotAccuracy) || !hasRate(match.accuracy)) {
      missingStats += 1;
      continue;
    }
    if (
      above(match.headshotAccuracy, headshotThresholdPercent) &&
      above(match.accuracy, accuracyThresholdPercent)
    ) {
      rows.push(match);
    }
  }

  rows.sort((left, right) => {
    const primary = priority === "headshot" ? "headshotAccuracy" : "accuracy";
    const secondary = priority === "headshot" ? "accuracy" : "headshotAccuracy";
    const byPrimary = compareDesc(left[primary], right[primary]);
    if (byPrimary !== 0) {
      return byPrimary;
    }
    const bySecondary = compareDesc(left[secondary], right[secondary]);
    if (bySecondary !== 0) {
      return bySecondary;
    }
    const byTime = compareDesc(left.startTimeMs ?? 0, right.startTimeMs ?? 0);
    if (byTime !== 0) {
      return byTime;
    }
    return compareDesc(Number(left.matchId), Number(right.matchId));
  });

  return { rows, missingStats };
}

export function parseThreshold(raw: string): number | null {
  const text = raw.trim();
  if (!text) {
    return null;
  }
  const value = Number(text);
  if (!Number.isFinite(value) || value < 0 || value > 100) {
    return null;
  }
  return value;
}

function hasRate(value: number | null): value is number {
  return value != null && Number.isFinite(value);
}

function above(fraction: number, thresholdPercent: number): boolean {
  return fraction * 100 > thresholdPercent + 1e-6;
}

function compareDesc(left: number | null, right: number | null): number {
  const a = left ?? Number.NEGATIVE_INFINITY;
  const b = right ?? Number.NEGATIVE_INFINITY;
  if (a === b) {
    return 0;
  }
  return a > b ? -1 : 1;
}
