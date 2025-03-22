<div align="center">
  <h1>RadioFrequency</h1>
  <h6>SCP: SL Plugin for creating radio frequencies</h6>
  <img alt="Version" src="https://img.shields.io/github/v/release/BoltonDev/RadioFrequency?style=flat-square&label=Version&color=8dd0ff">
  <img alt="Downloads" src="https://img.shields.io/github/downloads/BoltonDev/RadioFrequency/total?style=flat-square&label=Downloads&color=b0ff8d">
  <img alt="Stars" src="https://img.shields.io/github/stars/BoltonDev/RadioFrequency?style=flat-square&label=Stars&color=f3ff8d">

  <div>
    <img src="https://repobeats.axiom.co/api/embed/e3d273d93ab81fa31dc5d51b1d539095332aa6fa.svg" alt="RadioFrequency statistics">
  </div>
</div>

---

## Installation
This plugin works on [EXILED](https://github.com/ExMod-Team/EXILED/) >= **9.0.0**.  
Download `RadioFrequency.dll` in the [latest](https://github.com/BoltonDev/RadioFrequency/releases/latest) release assets, then put it in your plugins folder `../EXILED/Plugins/`.

## How it works?
Let's take a look at the default configuration before explaining.

#### Default configuration
```yaml
radio_frequency:
# Whether the plugin is enabled or disabled.
  is_enabled: true
  # Whether the debug mode is enabled or disabled.
  debug: false
  # Whether the default radio channel is enabled or disabled.
  use_default_radio: true
  # The default radio frequency name.
  default_radio_name: 'All'
  # All radio frenquency.
  frequencies:
  -
  # Name of the frequency.
    name: '446 MHz'
    # Roles that can access this frequency. If empty, then everyone can access this frequency.
    authorized_roles: []
    # Whenever a radio is dropped, its current frequency is saved.
    # If this config is true, then a player with an unauthorized role will be able to talk on this frequency if he picks up the radio.
    # This config is ignored if there is no authorized roles
    can_be_picked_up: false
  # The centered text (header) of the category.
  setting_header_label: 'RadioFrequency'
  # The unique id of the setting.
  keybind_id: 201
  # The keybind label.
  keybind_label: 'Change radio frequency.'
  # The keybind hint used to provides additional information.
  keybind_hint: 'Allows you to change the frequency of your radio.'
  # Hint displayed when the player has no radio.
  no_radio_hint: 'You need a radio to change its frequency.'
  # Hint displayed when the frequency has been changed.
  changed_frequency_hint: 'You changed the radio frequency to {radio_frequency}.'
```

#### What's a frequency?
A frequency is a specific channel on a radio.  
  
`name`: The name of the frequency that will be shown in hint for example.  
`authorized_roles`: The [RoleTypeId (look at Debug Name)](https://en.scpslgame.com/index.php/Remote_Admin#Forceclass) list of authorized roles. If a player has one of the roles in the list, he will be able to access this frequency. An empty list gives access to all roles.  
`can_be_picked_up`: When a radio is dropped, its current frequency (set by the player) is saved. Whenever a player picks up this radio, he will access the frequency saved if he has an authorized role or if this config is true otherwise, he will be switch to the default frequency he can access.  
  
You can create as many frequencies as you like.

#### How to switch the radio frequency?
You can do that with a keybind which is configurable in the config.  
> [!NOTE]  
> The `keybind_id` is not a keycode, but a unique identifier used in the code to identify the keybind.

#### Support for PlaceholderAPI (Additional)
If you are using [PlaceholderAPI](https://github.com/PlaceholderAPI-SL/PlaceholderAPI), you can download the expansion `RadioFrequency_PAPI.dll`, and then put it in `EXILED/Plugins/Expansions`.  
You can use `%radiofrequency_currentfrequency%` which will return the current frequency of the requesting player.  

:small_red_triangle: **If you have any question, request or other, do not hesitate to dm me on discord `boltonn`! Have fun!**
