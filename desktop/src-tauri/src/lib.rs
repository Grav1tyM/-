use deadlock_core::{explain_code, explain_lookup, normalize_friend_code, MatchView, PlayerView};

#[tauri::command]
async fn lookup_player(code: String) -> Result<PlayerView, String> {
    normalize_friend_code(&code).map_err(|err| explain_code(err).to_string())?;
    let player = deadlock_core::lookup_player(&code)
        .await
        .map_err(|err| explain_lookup(err).to_string())?;
    Ok(player.into())
}

#[tauri::command]
async fn load_matches(account_id: String) -> Result<Vec<MatchView>, String> {
    normalize_friend_code(&account_id).map_err(|err| explain_code(err).to_string())?;
    let rows = deadlock_core::load_matches(&account_id)
        .await
        .map_err(|err| explain_lookup(err).to_string())?;
    Ok(rows.into_iter().map(MatchView::from).collect())
}

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run() {
    tauri::Builder::default()
        .plugin(tauri_plugin_opener::init())
        .invoke_handler(tauri::generate_handler![lookup_player, load_matches])
        .run(tauri::generate_context!())
        .expect("启动窗口失败");
}
