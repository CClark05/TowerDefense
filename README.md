# Tower Defense Roguelike (working title)

A 2D roguelike tower defense game where towers are built from cards. Inspired by Slay the Spire and Bloons Tower Defense, it replaces fixed tower types with a modular card system: pick cards, slot them onto towers, and combine them into synergies to survive increasingly dangerous waves.

<img width="624" height="351" alt="Wave Preparation" src="https://github.com/user-attachments/assets/53f15b73-f2f4-4a35-84e1-f72b7844c997" />
<img width="624" height="351" alt="Gameplay" src="https://github.com/user-attachments/assets/03f074f4-8319-4d8e-8585-c4b8db43ad45" />

## Features
- **Modular card system:** towers have no fixed identity. Cards placed on them define what they do, so the same tower can become something completely different run to run
- **50+ cards with synergies:** combine cards to create unique tower builds and effects
- **Card upgrades:** upgrade cards to make them more powerful
- **Procedural wave generation:** waves are generated rather than hand-authored, so every run plays differently
- **Boss fights:** currently 2 bosses with behavior driven by finite state machines
- **Custom A\* pathfinding:** enemies navigate the map with a pathfinding implementation written from scratch
- **Level editor:** a custom editor for building and testing maps
- **Original pixel art:** all sprites, visuals, animation, and visual effects are made using Aseprite


## How to Play
Enemies follow a path toward your base. Between waves, you choose cards and place them onto towers to shape their abilities. Look for combinations that work together, upgrade your best cards, and prepare for boss waves. Survive for as long as possible. 

## Run
Demo build coming soon

## Tech
- Unity (6000.0.25f1), C#
- JetBrains Rider for code editing


## Status
Work in progress, started September 2025.

-**Implemented:** card system (50+ cards, upgrades), procedural waves, 2 bosses, level editor
-**Planned:** more bosses, enemy types, cards, save system, menu, settings, sound effects, music
