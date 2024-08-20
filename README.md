## ScpSwap

##### This was originally [BuildBoy12](https://github.com/BuildBoy12-SL/ScpSwap) version
##### I may maintain this plugin if I get his permission


### How do I download this?
  - Go here and download the latest release, [https://github.com/SnivyFilms/ScpSwap/releases](https://github.com/SnivyFilms/ScpSwap/releases)

### Default Config
```yml
ScpSwap:
  is_enabled: true
  # Indicates whether debug messages should be shown.
  debug: false
  # The duration, in seconds, before a swap request gets automatically deleted.
  request_timeout: 20
  # The duration, in seconds, after the round starts that swap requests can be sent.
  swap_timeout: 60
  # Indicates whether a player can switch to a class if there is nobody playing as it.
  allow_new_scps: true
  # A collection of roles blacklisted from being swapped to.
  blacklisted_scps:
  - Scp0492
  # A collection of the names of custom scps blacklisted from being swapped to. This must match the name the developer integrated the SCP into this plugin's API with.
  blacklisted_names: []
```
