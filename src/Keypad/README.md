# Keypad - Send keypress via StreamDeck

An alternative to the heavy-weight electron-based desktop app with all the bells and whistles. If all you need is keys - then all you get is keys.

Run `keypad [--debug] <path to config file>` to start listening on all configured devices.
The configuration is a YAML file declaring which keys on the device send which keys on your PC.

```yaml
StreamDeckXL2022:
  - Row: 1
    Column: 1
    Key: "A"
    Icon: "./A.jpg"
```

Consider running it directly on startup to make sure the device is always set-up for listening.

## Configuration reference

There is a top-level property for each device you wish to configure. Supported are, as of now: `StreamDeckXL2022`, `StreamDeckPedal`.
Each top-level property contains a list of key configurations consisting of a `Row`, a `Column`, a `Key` and an `Icon`. *Row* and *Column* are
0-based describing which key should be configured.

### Key

The following keys are supported:
- The letters A-Z as `"A"`, `"B"`, ... etc.
- The digits 0-9 on the digit row as `"0"`, `"1"`, ... etc.
- The digits 0-9 on the numpad as `"Numpad-0"`, `"Numpad-1"`, ... etc.
- The function keys F1-F24 as `"F1"`, `"F2"`, ... etc.
- The _media keys_ Play/Pause, Next, Previous as `"media:play"`, `"media:next"` and `"media:previous"`

### Icon

Icons can be fed from files relative to the configuration or from a set of built-in icons.

The builtin icons are available via `builtin:<name>`.

