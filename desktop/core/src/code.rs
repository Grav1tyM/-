//! Steam friend-code shapes that Statlocker already accepts as an account id.

#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum CodeError {
    Empty,
    Invalid,
    VanityUrl,
}

/// Returns the id to pass to `/api/profile/steam-profile/{id}`.
///
/// Account ids stay as decimal account ids. 17-digit SteamID64 values are
/// passed through because that endpoint resolves them.
pub fn normalize_friend_code(input: &str) -> Result<String, CodeError> {
    let trimmed = input.trim();
    let trimmed = trimmed
        .strip_prefix('"')
        .and_then(|s| s.strip_suffix('"'))
        .or_else(|| {
            trimmed
                .strip_prefix('\'')
                .and_then(|s| s.strip_suffix('\''))
        })
        .unwrap_or(trimmed)
        .trim();

    if trimmed.is_empty() {
        return Err(CodeError::Empty);
    }

    let lower = trimmed.to_ascii_lowercase();
    if lower.contains("steamcommunity.com/id/") && !lower.contains("steamcommunity.com/profiles/") {
        return Err(CodeError::VanityUrl);
    }
    if lower.contains("steamcommunity.com/profiles/") {
        let digits =
            extract_after(&trimmed, "steamcommunity.com/profiles/").ok_or(CodeError::Invalid)?;
        return accept_digits(&digits);
    }
    if lower.contains("steamcommunity.com") {
        return Err(CodeError::Invalid);
    }

    if let Some(id) = parse_steam3(trimmed) {
        return accept_account_id(&id);
    }
    if let Some(id) = parse_steam2(trimmed) {
        return Ok(id);
    }

    let mut digits = String::new();
    for ch in trimmed.chars() {
        if ch.is_whitespace() || ch == '\u{3000}' {
            continue;
        }
        match to_ascii_digit(ch) {
            Some(d) => digits.push(d),
            None => return Err(CodeError::Invalid),
        }
    }
    accept_digits(&digits)
}

fn extract_after(input: &str, marker: &str) -> Option<String> {
    let lower = input.to_ascii_lowercase();
    let marker = marker.to_ascii_lowercase();
    let idx = lower.find(&marker)?;
    let rest = &input[idx + marker.len()..];
    let mut digits = String::new();
    for ch in rest.chars() {
        match to_ascii_digit(ch) {
            Some(d) => digits.push(d),
            None => break,
        }
    }
    if digits.is_empty() {
        None
    } else {
        Some(digits)
    }
}

fn parse_steam3(input: &str) -> Option<String> {
    let mut text = input.trim();
    if let Some(inner) = text.strip_prefix('[').and_then(|s| s.strip_suffix(']')) {
        text = inner.trim();
    }
    let lower = text.to_ascii_lowercase();
    let rest = lower.strip_prefix("u:1:")?.trim();
    if rest.is_empty() || !rest.chars().all(|c| c.is_ascii_digit()) {
        return None;
    }
    Some(rest.to_string())
}

fn parse_steam2(input: &str) -> Option<String> {
    let lower = input.trim().to_ascii_lowercase();
    let rest = lower.strip_prefix("steam_")?;
    let mut parts = rest.split(':');
    let universe = parts.next()?;
    let y = parts.next()?;
    let z = parts.next()?;
    if parts.next().is_some() {
        return None;
    }
    if universe.len() != 1 || !universe.chars().all(|c| c.is_ascii_digit()) {
        return None;
    }
    if y != "0" && y != "1" {
        return None;
    }
    if z.is_empty() || !z.chars().all(|c| c.is_ascii_digit()) {
        return None;
    }
    let y: u64 = y.parse().ok()?;
    let z: u64 = z.parse().ok()?;
    let id = z.checked_mul(2)?.checked_add(y)?;
    if id == 0 || id > u32::MAX as u64 {
        return None;
    }
    Some(id.to_string())
}

fn to_ascii_digit(ch: char) -> Option<char> {
    if ch.is_ascii_digit() {
        return Some(ch);
    }
    let value = ch as u32;
    let start = '０' as u32;
    if (start..=start + 9).contains(&value) {
        return char::from_u32('0' as u32 + (value - start));
    }
    None
}

fn accept_digits(raw: &str) -> Result<String, CodeError> {
    if raw.is_empty() || !raw.chars().all(|c| c.is_ascii_digit()) {
        return Err(CodeError::Invalid);
    }
    let trimmed = raw.trim_start_matches('0');
    if trimmed.is_empty() {
        return Err(CodeError::Invalid);
    }
    if trimmed.len() > 17 {
        return Err(CodeError::Invalid);
    }
    if trimmed.len() != 17 && trimmed.len() > 10 {
        return Err(CodeError::Invalid);
    }
    if trimmed.len() <= 10 {
        return accept_account_id(trimmed);
    }
    Ok(trimmed.to_string())
}

fn accept_account_id(digits: &str) -> Result<String, CodeError> {
    let trimmed = digits.trim_start_matches('0');
    if trimmed.is_empty() || trimmed.len() > 10 {
        return Err(CodeError::Invalid);
    }
    let value: u64 = trimmed.parse().map_err(|_| CodeError::Invalid)?;
    if value == 0 || value > u32::MAX as u64 {
        return Err(CodeError::Invalid);
    }
    Ok(trimmed.to_string())
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn accepts_plain_friend_code() {
        assert_eq!(normalize_friend_code("  9560260 ").unwrap(), "9560260");
        assert_eq!(normalize_friend_code("\"9560260\"").unwrap(), "9560260");
        assert_eq!(normalize_friend_code("９５６０２６０").unwrap(), "9560260");
    }

    #[test]
    fn accepts_steam64_and_profile_url() {
        let steam64 = "76561197969825988";
        assert_eq!(normalize_friend_code(steam64).unwrap(), steam64);
        assert_eq!(
            normalize_friend_code("https://steamcommunity.com/profiles/76561197969825988/")
                .unwrap(),
            steam64
        );
    }

    #[test]
    fn accepts_steam2_and_steam3() {
        assert_eq!(
            normalize_friend_code("STEAM_0:0:4780130").unwrap(),
            "9560260"
        );
        assert_eq!(normalize_friend_code("[U:1:9560260]").unwrap(), "9560260");
        assert_eq!(normalize_friend_code("u:1:9560260").unwrap(), "9560260");
    }

    #[test]
    fn rejects_bad_shapes() {
        assert_eq!(normalize_friend_code("   ").unwrap_err(), CodeError::Empty);
        assert_eq!(normalize_friend_code("0").unwrap_err(), CodeError::Invalid);
        assert_eq!(
            normalize_friend_code("12ab").unwrap_err(),
            CodeError::Invalid
        );
        assert_eq!(
            normalize_friend_code("123456789012").unwrap_err(),
            CodeError::Invalid
        );
        assert_eq!(
            normalize_friend_code("9999999999").unwrap_err(),
            CodeError::Invalid
        );
        assert_eq!(
            normalize_friend_code("https://steamcommunity.com/id/some-name").unwrap_err(),
            CodeError::VanityUrl
        );
    }
}
