use crate::code::{normalize_friend_code, CodeError};
use reqwest::{Client, StatusCode};
use serde_json::Value;
use std::time::Duration;

const ORIGIN: &str = "https://statlocker.gg";
const USER_AGENT: &str = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36";

#[derive(Debug, Clone, PartialEq, Eq)]
pub struct Player {
    pub account_id: String,
    pub name: String,
    pub avatar_url: Option<String>,
}

#[derive(Debug, Clone, PartialEq)]
pub struct MatchRow {
    pub match_id: String,
    pub accuracy: Option<f64>,
    pub headshot_accuracy: Option<f64>,
    pub start_time_ms: Option<i64>,
    pub kills: Option<i64>,
    pub deaths: Option<i64>,
    pub assists: Option<i64>,
}

#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum LookupError {
    NotFound,
    Rejected,
    Unavailable,
    BadResponse,
    Timeout,
    Offline,
}

pub async fn lookup_player(input: &str) -> Result<Player, LookupError> {
    let code = normalize_friend_code(input).map_err(code_to_lookup)?;
    let url = format!("{ORIGIN}/api/profile/steam-profile/{code}");
    let response = send(&url, Duration::from_secs(20)).await?;
    parse_player(response.status, &response.body)
}

pub async fn load_matches(account_id: &str) -> Result<Vec<MatchRow>, LookupError> {
    let account_id = normalize_friend_code(account_id).map_err(code_to_lookup)?;
    // Friend codes are account ids. A SteamID64 here would be the wrong path
    // segment once we already resolved `accountId`.
    if account_id.len() > 10 {
        return Err(LookupError::NotFound);
    }
    let url = format!("{ORIGIN}/api/profile/data/matches/{account_id}/true");
    let response = send(&url, Duration::from_secs(60)).await?;
    parse_matches(response.status, &response.body)
}

fn code_to_lookup(err: CodeError) -> LookupError {
    match err {
        CodeError::Empty | CodeError::Invalid | CodeError::VanityUrl => LookupError::NotFound,
    }
}

struct RawResponse {
    status: StatusCode,
    body: String,
}

async fn send(url: &str, timeout: Duration) -> Result<RawResponse, LookupError> {
    let client = Client::builder()
        .timeout(timeout)
        .gzip(true)
        .build()
        .map_err(|_| LookupError::Offline)?;
    let result = client
        .get(url)
        .header("Accept", "application/json")
        .header("Origin", ORIGIN)
        .header("Referer", format!("{ORIGIN}/"))
        .header("User-Agent", USER_AGENT)
        .send()
        .await;

    let response = match result {
        Ok(response) => response,
        Err(err) if err.is_timeout() => return Err(LookupError::Timeout),
        Err(err) if err.is_connect() || err.is_request() => return Err(LookupError::Offline),
        Err(_) => return Err(LookupError::Offline),
    };
    let status = response.status();
    let body = response
        .text()
        .await
        .map_err(|_| LookupError::BadResponse)?;
    Ok(RawResponse { status, body })
}

pub(crate) fn parse_player(status: StatusCode, body: &str) -> Result<Player, LookupError> {
    if status == StatusCode::NOT_FOUND {
        return Err(LookupError::NotFound);
    }
    if status == StatusCode::UNAUTHORIZED || status == StatusCode::FORBIDDEN {
        return Err(LookupError::Rejected);
    }
    if status.is_server_error() {
        return Err(LookupError::Unavailable);
    }
    if !status.is_success() {
        return Err(LookupError::BadResponse);
    }
    let body = body.trim();
    if body.is_empty() {
        return Err(LookupError::NotFound);
    }
    let value: Value = serde_json::from_str(body).map_err(|_| LookupError::BadResponse)?;
    if value.get("error").is_some() {
        return Err(LookupError::NotFound);
    }
    let account_id = value
        .get("accountId")
        .and_then(json_digits)
        .ok_or(LookupError::NotFound)?;
    if account_id.len() > 10 {
        return Err(LookupError::BadResponse);
    }
    let name = value
        .get("name")
        .and_then(Value::as_str)
        .map(str::trim)
        .filter(|name| !name.is_empty())
        .unwrap_or("未命名玩家")
        .to_string();
    let avatar_url = value
        .get("avatarUrl")
        .and_then(Value::as_str)
        .map(str::trim)
        .filter(|url| {
            url.starts_with("https://avatars.steamstatic.com/")
                || url.starts_with("https://avatars.akamai.steamstatic.com/")
                || url.starts_with("https://avatars.cloudflare.steamstatic.com/")
        })
        .map(str::to_string);
    Ok(Player {
        account_id,
        name,
        avatar_url,
    })
}

pub(crate) fn parse_matches(status: StatusCode, body: &str) -> Result<Vec<MatchRow>, LookupError> {
    if status == StatusCode::NOT_FOUND {
        return Ok(Vec::new());
    }
    if status == StatusCode::UNAUTHORIZED || status == StatusCode::FORBIDDEN {
        return Err(LookupError::Rejected);
    }
    if status.is_server_error() {
        return Err(LookupError::Unavailable);
    }
    if !status.is_success() {
        return Err(LookupError::BadResponse);
    }
    let body = body.trim();
    if body.is_empty() {
        return Err(LookupError::BadResponse);
    }
    let value: Value = serde_json::from_str(body).map_err(|_| LookupError::BadResponse)?;
    if value.get("error").is_some() {
        return Err(LookupError::BadResponse);
    }
    let history = match value.get("matchHistory") {
        None | Some(Value::Null) => return Ok(Vec::new()),
        Some(Value::Array(items)) => items,
        Some(_) => return Err(LookupError::BadResponse),
    };
    let mut rows = Vec::with_capacity(history.len());
    for item in history {
        let Some(match_id) = item.get("match_id").and_then(json_digits) else {
            continue;
        };
        rows.push(MatchRow {
            match_id,
            accuracy: item.get("accuracy").and_then(json_finite),
            headshot_accuracy: item.get("headshotAccuracy").and_then(json_finite),
            start_time_ms: item.get("start_time").and_then(json_i64),
            kills: item.get("player_kills").and_then(json_i64),
            deaths: item.get("player_deaths").and_then(json_i64),
            assists: item.get("player_assists").and_then(json_i64),
        });
    }
    Ok(rows)
}

fn json_digits(value: &Value) -> Option<String> {
    match value {
        Value::Number(number) => number.as_u64().map(|n| n.to_string()),
        Value::String(text) => {
            let text = text.trim();
            if text.is_empty() || !text.chars().all(|c| c.is_ascii_digit()) {
                None
            } else {
                Some(text.trim_start_matches('0').to_string()).filter(|s| !s.is_empty())
            }
        }
        _ => None,
    }
}

fn json_finite(value: &Value) -> Option<f64> {
    match value {
        Value::Number(number) => number.as_f64().filter(|n| n.is_finite()),
        _ => None,
    }
}

fn json_i64(value: &Value) -> Option<i64> {
    match value {
        Value::Number(number) => number
            .as_i64()
            .or_else(|| number.as_u64().and_then(|n| i64::try_from(n).ok()))
            .or_else(|| number.as_f64().map(|n| n as i64)),
        _ => None,
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    use crate::{MatchView, PlayerView};

    #[test]
    fn parses_player_and_hides_non_steam_avatars() {
        let body = r#"{
            "accountId": 9560260,
            "name": "TWITCH LOMEINE",
            "avatarUrl": "https://avatars.steamstatic.com/abc_full.jpg"
        }"#;
        let player = parse_player(StatusCode::OK, body).unwrap();
        assert_eq!(player.account_id, "9560260");
        assert_eq!(player.name, "TWITCH LOMEINE");
        assert!(player.avatar_url.unwrap().contains("steamstatic"));

        let bad = r#"{"accountId":1,"name":"x","avatarUrl":"https://evil.example/a.png"}"#;
        let player = parse_player(StatusCode::OK, bad).unwrap();
        assert_eq!(player.avatar_url, None);
    }

    #[test]
    fn missing_player_shapes() {
        assert_eq!(
            parse_player(StatusCode::NOT_FOUND, "").unwrap_err(),
            LookupError::NotFound
        );
        assert_eq!(
            parse_player(StatusCode::OK, "").unwrap_err(),
            LookupError::NotFound
        );
        assert_eq!(
            parse_player(
                StatusCode::OK,
                r#"{"error":"Not Found","message":"Invalid account id"}"#
            )
            .unwrap_err(),
            LookupError::NotFound
        );
        assert_eq!(
            parse_player(StatusCode::UNAUTHORIZED, r#"{"error":"Missing API key"}"#).unwrap_err(),
            LookupError::Rejected
        );
    }

    #[test]
    fn parses_match_fractions_and_nulls() {
        let body = r#"{
            "matchHistory": [
                {
                    "match_id": 32580148,
                    "accuracy": 0.598052,
                    "headshotAccuracy": 0.272152,
                    "start_time": 1738622539000,
                    "player_kills": 11,
                    "player_deaths": 6,
                    "player_assists": 14
                },
                {
                    "match_id": 28855301,
                    "accuracy": null,
                    "headshotAccuracy": null,
                    "start_time": 1730000000000,
                    "player_kills": 10,
                    "player_deaths": 1,
                    "player_assists": 2
                }
            ]
        }"#;
        let rows = parse_matches(StatusCode::OK, body).unwrap();
        assert_eq!(rows.len(), 2);
        assert_eq!(rows[0].match_id, "32580148");
        assert_eq!(rows[0].accuracy, Some(0.598052));
        assert_eq!(rows[0].headshot_accuracy, Some(0.272152));
        assert_eq!(rows[0].kills, Some(11));
        assert_eq!(rows[1].accuracy, None);
        assert_eq!(rows[1].headshot_accuracy, None);

        let empty = parse_matches(
            StatusCode::OK,
            r#"{"matchHistory":[],"playerPerformance":{"totalMatches":0}}"#,
        )
        .unwrap();
        assert!(empty.is_empty());
    }

    #[tokio::test]
    #[ignore = "calls the live Statlocker site"]
    async fn live_friend_code_resolves_and_has_rates() {
        let player = lookup_player("9560260").await.unwrap();
        assert_eq!(player.account_id, "9560260");
        assert!(!player.name.is_empty());
        let rows = load_matches(&player.account_id).await.unwrap();
        assert!(rows.len() > 10, "expected a real match history");
        assert!(rows
            .iter()
            .any(|row| row.accuracy.is_some() && row.headshot_accuracy.is_some()));
        let missing = lookup_player("4000000001").await.unwrap_err();
        assert_eq!(missing, LookupError::NotFound);
    }

    #[test]
    fn frontend_contract_uses_camel_case() {
        let view = MatchView::from(MatchRow {
            match_id: "32580148".into(),
            accuracy: Some(0.5),
            headshot_accuracy: Some(0.2),
            start_time_ms: Some(1738622539000),
            kills: Some(1),
            deaths: Some(2),
            assists: Some(3),
        });
        let json = serde_json::to_value(view).unwrap();
        assert!(json.get("matchId").is_some());
        assert!(json.get("headshotAccuracy").is_some());
        assert!(json.get("startTimeMs").is_some());

        let player = PlayerView::from(Player {
            account_id: "9560260".into(),
            name: "A".into(),
            avatar_url: None,
        });
        let json = serde_json::to_value(player).unwrap();
        assert_eq!(json["accountId"], "9560260");
    }
}
