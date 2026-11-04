#![cfg_attr(not(debug_assertions), windows_subsystem = "windows")]

fn main() {
    deadlock_match_filter_lib::run();
}
