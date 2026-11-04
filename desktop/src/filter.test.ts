import { describe, expect, it } from "vitest";
import { filterMatches, parseThreshold, type MatchRow } from "./filter";

function row(partial: Partial<MatchRow> & Pick<MatchRow, "matchId">): MatchRow {
  return {
    accuracy: 0.6,
    headshotAccuracy: 0.3,
    startTimeMs: 1_700_000_000_000,
    kills: 1,
    deaths: 1,
    assists: 1,
    ...partial,
  };
}

describe("filterMatches", () => {
  it("keeps matches strictly above both thresholds", () => {
    const result = filterMatches(
      [
        row({ matchId: "1", headshotAccuracy: 0.2, accuracy: 0.5 }),
        row({ matchId: "2", headshotAccuracy: 0.2000001, accuracy: 0.5000001 }),
        row({ matchId: "3", headshotAccuracy: 0.25, accuracy: 0.49 }),
        row({ matchId: "4", headshotAccuracy: 0.19, accuracy: 0.8 }),
        row({ matchId: "5", headshotAccuracy: 0.3, accuracy: 0.7 }),
      ],
      20,
      50,
      "headshot",
    );
    expect(result.rows.map((match) => match.matchId)).toEqual(["5", "2"]);
  });

  it("sorts by the priority metric, then the other metric, then time", () => {
    const matches = [
      row({
        matchId: "old",
        headshotAccuracy: 0.3,
        accuracy: 0.6,
        startTimeMs: 1,
      }),
      row({
        matchId: "new",
        headshotAccuracy: 0.3,
        accuracy: 0.6,
        startTimeMs: 2,
      }),
      row({
        matchId: "accurate",
        headshotAccuracy: 0.22,
        accuracy: 0.9,
        startTimeMs: 9,
      }),
      row({
        matchId: "headshots",
        headshotAccuracy: 0.4,
        accuracy: 0.55,
        startTimeMs: 3,
      }),
    ];

    expect(
      filterMatches(matches, 20, 50, "headshot").rows.map((match) => match.matchId),
    ).toEqual(["headshots", "new", "old", "accurate"]);

    expect(
      filterMatches(matches, 20, 50, "accuracy").rows.map((match) => match.matchId),
    ).toEqual(["accurate", "new", "old", "headshots"]);
  });

  it("counts matches that are missing either rate and leaves them out", () => {
    const result = filterMatches(
      [
        row({ matchId: "ok", headshotAccuracy: 0.3, accuracy: 0.6 }),
        row({ matchId: "no-hs", headshotAccuracy: null, accuracy: 0.9 }),
        row({ matchId: "no-acc", headshotAccuracy: 0.4, accuracy: null }),
      ],
      10,
      10,
      "accuracy",
    );
    expect(result.rows.map((match) => match.matchId)).toEqual(["ok"]);
    expect(result.missingStats).toBe(2);
  });
});

describe("parseThreshold", () => {
  it("accepts 0 to 100 and rejects anything else", () => {
    expect(parseThreshold(" 20.5 ")).toBe(20.5);
    expect(parseThreshold("0")).toBe(0);
    expect(parseThreshold("100")).toBe(100);
    expect(parseThreshold("")).toBeNull();
    expect(parseThreshold("-1")).toBeNull();
    expect(parseThreshold("100.1")).toBeNull();
    expect(parseThreshold("abc")).toBeNull();
  });
});
