import assert from "node:assert/strict";
import test from "node:test";
import { evaluateMatch, filterMatches, toPercent } from "./filter.mjs";

test("fractions become percents", () => {
  assert.equal(toPercent(0.272152), 27.2152);
  assert.equal(toPercent(null), null);
  assert.equal(toPercent(Number.NaN), null);
});

test("both stats must be strictly above the thresholds", () => {
  const hit = evaluateMatch(0.272152, 0.598052, 27, 59);
  assert.equal(hit.include, true);

  const equalHeadshot = evaluateMatch(0.27, 0.6, 27, 50);
  assert.equal(equalHeadshot.include, false);

  const equalAccuracy = evaluateMatch(0.5, 0.59, 20, 59);
  assert.equal(equalAccuracy.include, false);

  const onlyOne = evaluateMatch(0.4, 0.5, 30, 60);
  assert.equal(onlyOne.include, false);
});

test("missing aim stats are skipped, not invented", () => {
  const result = filterMatches(
    [
      { match_id: 1, start_time: 10, hero_id: 1, headshotAccuracy: 0.4, accuracy: 0.7 },
      { match_id: 2, start_time: 20, hero_id: 2, headshotAccuracy: null, accuracy: 0.9 },
      { match_id: 3, start_time: 30, hero_id: 3, headshotAccuracy: 0.3, accuracy: 0.8 },
    ],
    25,
    60,
  );
  assert.equal(result.missingAim, 1);
  assert.deepEqual(
    result.included.map((match) => match.matchId),
    [3, 1],
  );
});
