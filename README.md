[![Install via Homebrew](https://img.shields.io/badge/homebrew-phkiener%2Fpersonal%2Fkeypad-orange?style=for-the-badge)](https://github.com/phkiener/homebrew-personal/blob/main/Casks/keypad.rb)
&nbsp;
![X-Clacks-Overhead "GNU Terry Pratchett"](https://img.shields.io/badge/this%20is-a%20badge!-blue?style=for-the-badge)

---

# Keypad 

A lightweight app for Elgato StreamDeck (and other Elgato input devices).

## Installation

Either download the latest [GitHub Release](https://github.com/phkiener/Keypad/releases/) and extract the `.app` bundle yourself
... or use homebrew!

```sh
brew install phkiener/personal/keypad
```

## Companion CLI

Next to the main app (running in the status bar), there is a companion-CLI app
to control a few settings on your device(s):

- Setting a screensaver
- Sleep and awaken from sleep
- Setting a fullscreen image

This CLI is (as of now) only available by building the source yourself.

## Configuration

The app will look for its configuration in the following locations, in order:

1. `$KEYPAD_CONFIGPATH`
2. `$XDG_CONFIG_HOME/keypad/config.json`
3. `~/.config/keypad/config.json`

If no file exists at any of these locations, an empty configuration will be
used. So be sure to you create the configuration file yourself!

Note that the app is most likely started via `launchtl`, so if you do want to
set environment variables, make sure they're set for `launchtl` and not just in
your shell profile.

### Configuration format

```json5
{
  "devices": [
    {
      "type": "XL2022", // supported are "XL2022" and "Pedal"
      "serial": "AN...01", // optional; used to specify a specific device instead of all devices of the given type
      "brightness": 0.5, // optional, only on XL2022; brightness of the screen in [0;1]
      "sensitivity": 0.3, // optional, only on Pedal; sensitivity of the buttons in [0;0.7]
      "statusColor": "#FF0000", // optional, only on Pedal; color of the status LED as hex-code
      "keys": [
        {
          "button": "1;1", // button to configure; format is "$row;$column" - 1-based
          "key": "Shift+Ctrl+A", // the key to press when the button is pressed, see 'emulated keys' below
          "image": "color:blue" // optional, only on XL2022: image to set for the key, see 'key image' below
        }
      ]
    }
  ]
}

```

#### Emulated keys

Keys are specified by describing the combination as `"($Modifier+)*$Keycode"`,
e.g. `"Shift+Ctrl+L"` or `"Cmd+Option+S"`. Supported modifiers are:

- Command / Cmd
- Control / Ctrl
- Shift
- Option / Opt

The supported keycodes are:

- letters A to Z
- digits 0 to 9
- numpad keys 0 to 9 (as Num0, Num1, ...)
- F1 to F20

#### Key image

Images can be either full-color images or loaded from a path.

- `color:blue` sets a blue color; supported are all CSS colors
- `file:$path` loads the image from `$path`

File loading is supported for JPEG and PNG. More formats may be supported,
but I've never tested it (it's using skia in the background).

## Supported devices

### StreamDeck XL 2022

```
┌────────────────────────────────────────────┐
│                @ Stream Deck               │
│                                            │
│  ┌───┐┌───┐┌───┐┌───┐┌───┐┌───┐┌───┐┌───┐  │
│  │1;1││1;2││1;3││1;4││1;5││1;6││1;7││1;8│  │
│  └───┘└───┘└───┘└───┘└───┘└───┘└───┘└───┘  │
│  ┌───┐┌───┐┌───┐┌───┐┌───┐┌───┐┌───┐┌───┐  │
│  │2;1││2;2││2;3││2;4││2;5││2;6││2;7││2;8│  │
│  └───┘└───┘└───┘└───┘└───┘└───┘└───┘└───┘  │
│  ┌───┐┌───┐┌───┐┌───┐┌───┐┌───┐┌───┐┌───┐  │
│  │3;1││3;2││3;3││3;4││3;5││3;6││3;7││3;8│  │
│  └───┘└───┘└───┘└───┘└───┘└───┘└───┘└───┘  │
│  ┌───┐┌───┐┌───┐┌───┐┌───┐┌───┐┌───┐┌───┐  │
│  │4;1││4;2││4;3││4;4││4;5││4;6││4;7││4;8│  │
│  └───┘└───┘└───┘└───┘└───┘└───┘└───┘└───┘  │
└────────────────────────────────────────────┘
```


### StreamDeck Pedal

```
┌─────────────────────────────┐
│  ┌─────┐┌─────────┐┌─────┐  │
│  │     ││         ││     │  │
│  │ 1;1 ││   1;2   ││ 1;3 │  │
│  │     ││         ││     │  │
│  └─────┘└─────────┘└─────┘  │
└─────────────────────────────┘
```
