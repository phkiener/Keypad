# Keypad 

A lightweight app for Elgato StreamDeck (and other Elgato input devices).

## Configuration

The app will look for its configuration in the following locations, in order:

1. `$KEYPAD_CONFIGPATH`
2. `$XDG_CONFIG_HOME/keypad/config.json`
3. `~/.config/keypad/config.json`

If no file exists at any of these locations, an empty configuration will be
used. So be sure to you create the configuration file yourself!

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
