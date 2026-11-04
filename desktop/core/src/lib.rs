//! Talks to the public Statlocker site from the end user's machine.
//!
//! Verified against https://statlocker.gg on 2026-11-04:
//! - `GET /api/profile/steam-profile/{id}` resolves a Steam friend code (account id)
//!   and a SteamID64. 404 or an empty body means that player does not exist.
//!   A numeric `search-profiles` query is a name search, so friend codes must not use it.
//! - `GET /api/profile/data/matches/{accountId}/true` is the profile page's
//!   "all games" payload. Each row includes `accuracy` and `headshotAccuracy`
//!   as fractions from 0 to 1 (null when Statlocker has no figure).
//! - Match pages are `https://statlocker.gg/match/{match_id}`.
//!
//! Requests without `Origin: https://statlocker.gg` get `401` and
//! `{"error":"Missing API key"}`. The website itself sends that origin.
//! There is no private key.

mod code;
mod http;

pub use code::{normalize_friend_code, CodeError};
pub use http::{load_matches, lookup_player, LookupError, MatchRow, Player};

use serde::Serialize;

#[derive(Debug, Clone, PartialEq, Eq, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct PlayerView {
    pub account_id: String,
    pub name: String,
    pub avatar_url: Option<String>,
}

#[derive(Debug, Clone, PartialEq, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct MatchView {
    pub match_id: String,
    pub accuracy: Option<f64>,
    pub headshot_accuracy: Option<f64>,
    pub start_time_ms: Option<i64>,
    pub kills: Option<i64>,
    pub deaths: Option<i64>,
    pub assists: Option<i64>,
}

impl From<Player> for PlayerView {
    fn from(player: Player) -> Self {
        Self {
            account_id: player.account_id,
            name: player.name,
            avatar_url: player.avatar_url,
        }
    }
}

impl From<MatchRow> for MatchView {
    fn from(row: MatchRow) -> Self {
        Self {
            match_id: row.match_id,
            accuracy: row.accuracy,
            headshot_accuracy: row.headshot_accuracy,
            start_time_ms: row.start_time_ms,
            kills: row.kills,
            deaths: row.deaths,
            assists: row.assists,
        }
    }
}

pub fn explain_code(err: CodeError) -> &'static str {
    match err {
        CodeError::Empty => "请输入 Steam 好友码。",
        CodeError::Invalid => "好友码格式不正确。请输入数字好友码（Steam 账户 ID），也可以粘贴 Steam64、数字个人资料链接或 [U:1:数字]。",
        CodeError::VanityUrl => "这是自定义主页链接，不是好友码。请改用数字好友码。",
    }
}

pub fn explain_lookup(err: LookupError) -> &'static str {
    match err {
        LookupError::NotFound => "没有找到这个好友码对应的玩家。",
        LookupError::Rejected => "Statlocker 拒绝了这次查询，请稍后重试。",
        LookupError::Unavailable => "Statlocker 暂时无法响应，请稍后重试。",
        LookupError::BadResponse => "Statlocker 返回了无法识别的内容。",
        LookupError::Timeout => "连接 Statlocker 超时，请检查网络后重试。",
        LookupError::Offline => "无法连接 Statlocker，请检查网络后重试。",
    }
}
