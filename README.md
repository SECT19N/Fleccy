# Fleccy

A Speccy-like hardware monitor for the desktop, built with [Slint](https://slint.dev)
and Fluent UI design principles.

## Status

Scaffold only — no functionality yet.

## Layout

```
build.rs              compiles the Slint UI
src/
  main.rs             app entry point
  monitor/            hardware probing (cpu, memory, gpu, storage, os, network)
  ui/                 Rust <-> Slint glue (callbacks, models, timers)
ui/
  app-window.slint    root window
  theme/              Fluent design tokens
  components/          reusable widgets
  views/              per-category pages
assets/
  icons/  fonts/
```

## Build

```
cargo run
```

## License

GPL-3.0-or-later. See [LICENSE](LICENSE).
