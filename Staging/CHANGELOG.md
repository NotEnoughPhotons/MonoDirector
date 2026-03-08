# Changelog
This is the changelog for MonoDirector! This changelog only features public release versions.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html)

## [1.2.6] - 3/7/2026

### Fixed
- Fixed a problem with the mod incorrectly reporting a pallet needed updating even though the most recent version was installed

## [1.2.5] - 3/7/2026

### Added
- Added laser pointers to both the Actor Pen and Propifier
- Added new notifications that will tell you if you don't have dependencies installed
- Added a notification to let you know when there are new pallet updates

### Updated
- Moved UI sounds into their own bundles so that it doesn't break the mod if someone didn't have AudioImportLib installed
- Actor management panel now faces the player
- Custom SFX is now loaded from **"MonoDirector/SFX"** instead of **"MonoDirector/SFX/Sounds"** for simplicity

### Fixed
- Fixed issues with the mod coming to a screeching halt if AudioImportLib wasn't installed

## [1.2.4] - 3/4/2026

### Removed
- Removed the option to bring up the caster menu from BoneMenu due to it not being implemented properly

## [1.2.3] - 3/3/2026 (Hotfix)

### Added
- Added back the Quest MonoDirector embedded bundle in the DLL

### Fixed
- Changed file compression algorithm to something other than LZMA4 because it couldn't be opened with anything

## [1.2.2] - 3/3/2026 (Hotfix)

### Fixed
- Fixed a backwards compatibility problem with the Quest version because of an incompatible console logger
- Reverted back to using MelonLoader 0.7.0 for compatibility

## [1.2.1] - 3/3/2026

### Removed
- Removed external pallet downloading code

## [1.2.0] - 3/3/2026

### Added
- Added spot lights
- Added omni lights
- Added sound source tethering to actors
- Added light intensity, radius, angle, and color gizmos for light tools
- Added a volume gizmo for sound sources
- Added visual effect (VFX) charges and volumes, activated by a gizmo on the top
- Added an "actor pen" tool that allows for easy actor management
- Added a frame object around selected actors
- Added back the prop icons
- Added a feature to check to see if you have the MonoDirector content pallet installed, and notifies you if you don't have it

### Updated
- Sound sources, among other tools, have been given a makeover with new sprites and visuals
- Updated the sprite for props
- Brought MonoDirector to Patch 6
- Vehicle functionality (vehicles are still kinematic, but most things are tracked now)

### Fixed
- Fixed a bug that, when firing a weapon, would freeze the actor, breaking the mod
- Fixed an issue with the info interface being backwards
- Fixed a problem with prop markers not showing up
- Fixed an issue with removed actors not having their props cleaned up

### Removed
- Removed most camera options to make them better in the future

## [1.1.1] - 4/26/2024

### Added
- Added back the working handheld camera
- A lens to the handheld camera

### Updated
- Cleaned up the hierarchy of the handheld camera
- VFX changes in BoneMenu now get reflected across both cameras
- Kinematic on release for the handheld camera now triggers on both grips

## [1.1.0] - 4/24/2024

### Added
- Added sound sources (2D and 3D)
- Added sound holders and the ability to put music in **UserData/Not Enough Photons/MonoDirector/SFX/Sounds**
- Added a "Caster Menu" for managing actors you recently created
- Added the ability to recast an actor, or redo the take of an actor

### Updated
- Weapon tracking, including casing scale, slide animations, and more (Rexmeck)
- Actor jaw movement

### Fixed
- Fixed an issue with horrible random crashes

### Removed
- Removed blinding effects that would play when actors got casted
- Removed debug text that would sometimes appear in the console
- Pausing was removed, as it currently does nothing


## [1.0.0] - 4/10/2024

Initial release!