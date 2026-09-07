// Fleccy — hardware monitoring with Slint + Fluent UI
// Entry point. Application logic lives in the modules below.

mod monitor;
mod ui;

slint::include_modules!();

fn main() -> Result<(), slint::PlatformError> {
    let window = AppWindow::new()?;
    window.run()
}
