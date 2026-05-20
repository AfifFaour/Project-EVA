# Project Eva

![Unity Version](https://img.shields.io/badge/Unity-6000.3.9f1-lightgrey?logo=unity)
![URP](https://img.shields.io/badge/Render%20Pipeline-URP-blue)
![Status](https://img.shields.io/badge/Status-Complete-brightgreen)

A mini open‑world RPG with turn‑based, parry‑driven combat.  
Explore a hand‑crafted 3D world, gather materials, upgrade weapons, and master the art of timed defence.  
Built entirely from scratch as a university game‑development project.

---

## 📖 Table of Contents

- [About the Game](#about-the-game)
- [Features](#features)
- [Technical Architecture](#technical-architecture)
- [Controls](#controls)
- [Screenshots](#screenshots)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [Report & Documentation](#report--documentation)
- [License](#license)

---

## 🎮 About the Game

**Project Eva** is a 3D exploration RPG inspired by classic turn‑based systems with modern active‑defence mechanics.  
The player roams a stylised environment, interacts with NPCs and merchants, discovers hidden chests, and fights enemies in cinematic, instanced battles.

Every system—from the combat loop to the inventory UI—has been coded from the ground up, with a focus on modularity, clean architecture, and event‑driven communication.

---

## ✨ Features

### 🗺️ Exploration
- Free third‑person movement with mouse‑controlled camera.
- Interactable objects (NPCs, merchants, puzzles, loot chests) via a generic `IInteractable` interface.
- World pickups for instant health, materials, gold, and consumables.

### ⚔️ Turn‑Based Combat
- Dedicated battle arena with dynamic camera framing.
- Player actions: **basic attack**, **two weapon skills**, and **item use** (opening a selection panel).
- **Active parry** – a timed window (based on Agility) to negate enemy damage and trigger a counter‑attack.
- Screen shake and particle effects (hit sparks, parry flash, death effect) for juicy feedback.
- Victory rewards: experience points and material drops; defeat returns the player to exploration.

### 📈 Progression Systems
- **Character stats** (Strength, Agility, Intelligence) that affect damage and parry window.
- Level up via XP; earn 3 stat points per level to allocate freely.
- **Weapon upgrades** using materials dropped by enemies; each weapon has 10 upgrade tiers.
- **World level** auto‑scales based on total XP, increasing enemy HP/damage and drop quality.

### 🛒 Economy & Inventory
- **Merchant** with a fixed‑slot shop UI; trade gold for weapons and consumables.
- **Loot boxes** scattered around the world with customisable rewards.
- **Inventory panel** (Tab) listing all owned materials, consumables, and weapons.

### 🎛️ UI & Feedback
- Combat HUD with colour‑changing HP bars, turn indicators, and action prompts.
- Post‑battle summary screen showing damage dealt, taken, healing, and loot obtained (auto‑fades after 5 seconds).
- World‑level HUD, character stats panel, weapon upgrade panel – all managed by a central `UIManager`.
- Cursor lock/unlock automatically when any panel opens/closes.

---

## 🧱 Technical Architecture

Project Eva is built with **Unity (URP)** and a fully modular C# codebase.

| Layer         | Key Scripts |
|---------------|-------------|
| **Managers**  | `GameManager` (state machine), `WorldManager` (scaling), `CombatManager` (turn loop), `UIManager` (panel toggles) |
| **Player**    | `PlayerController` (movement + camera), `PlayerStats` (HP, XP, stats), `Inventory`, `WeaponHandler` |
| **Combat**    | `EnemyUnit`, `EncounterTrigger`, `BattleArenaSetup` (optional), `CameraShake`, `CombatUI`, `CombatSummaryUI`, `ItemSelectUI` |
| **Data**      | All ScriptableObjects live in a single `GameData.cs`: materials, weapons, consumables, skills, enemies, encounters |
| **Interaction** | `IInteractable`, `NPC`, `Merchant`, `LootBox`, `WorldPickup`, `ShopUI` |
| **UI**        | `CharacterStatsUI`, `WeaponUpgradeUI`, `InventoryUI`, `WorldLevelUI` |

**Design principles:**
- **Singletons** for core managers, with robust null‑checking.
- **Events** (`PlayerStats.OnHealthChanged`, `OnXPChanged`, etc.) to update UI without polling.
- **ScriptableObjects** for data‑driven design; designers can tweak balance without touching code.
- **Direct Inspector references** over runtime `Find` calls to prevent broken references.

All code is original; no third‑party gameplay logic was used.

---
