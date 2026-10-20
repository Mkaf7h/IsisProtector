# 🛡️ IsisFuscator

<div align="center">

### Professional .NET Protection & Obfuscation Suite

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20.NET%208.0-blue.svg)](https://dotnet.microsoft.com/)
[![Status](https://img.shields.io/badge/Status-Active%20%7C%20Production-success.svg)]()

</div>

---

## 🚀 Overview

**IsisFuscator** is an advanced, high-performance .NET obfuscation and protection tool built with C# Windows Forms and powered by **Mono.Cecil** and **dnlib**. Designed for developers and reverse-engineering defense, it secures .NET assemblies (`.exe` / `.dll`) against unauthorized decompilation, debugging, and tampering.

---

## ✨ Key Features

### 🔐 1. Advanced Renamer
- **Smart & Safe Renaming:** Obfuscates namespaces, non-public types, methods, and fields while fully preserving Windows Forms UI classes, entry points, and public APIs to ensure 100% crash-free execution.
- **Customizable Prefixes & Lengths:** Configure custom name prefixes, character sets, and identifier lengths.

### 🛡️ 2. Anti-Analysis & Antis Protections
- **Anti-Debug:** Injects runtime checks that terminate the process if a debugger (like dnSpy or VS) is attached.
- **Anti-ILDasm:** Adds `SuppressIldasmAttribute` to block IL disassemblers.
- **Anti-Tamper & Anti-Dump:** Injects metadata decoys and integrity markers against memory dumping and patching.
- **Anti-de4dot:** Creates decoy types to corrupt de4dot renaming heuristics.

### 🔒 3. String & Resource Encryption
- **String Obfuscation:** Encrypts and hides string literals.
- **Resource Protection:** Scrambles embedded .resources streams (forms, images, icons, texts) so they cannot be extracted or viewed.
- **Code Mutation:** Inserts dead code and mutation instructions.

### 👻 4. Stealth Virtualization
- **DnSpy Crasher:** Injects malformed types with null bytes (`\0`) and recursive branching that instantly crashes decompiler tree views.
- **Fake Obfuscator Attributes:** Injects decoy attributes (ConfuserEx, Babel, Xenocode, Yano, SmartAssembly) to mislead reverse engineers.
- **Type Scrambling & Method Hiding:** Obfuscates metadata tables and method signatures.

---

## 🎛️ UI & Theme
- **Pure Black Glassmorphism:** Sleek dark-mode aesthetic with custom PNG icons and smooth window dragging.
- **Interactive Navigation:** Dedicated tabs for Build, Protections, Encryption, Renamer, and Stealth Virtualization.

---

## 🛠️ Installation & Usage

1. **Prerequisites:** Ensure [.NET 8.0 SDK](https://dotnet.microsoft.com/) is installed.
2. **Clone the Repository:**
   ```bash
   git clone https://github.com/yourusername/IsisFuscator.git
   cd IsisFuscator
   ```
3. **Build & Run:**
   ```bash
   dotnet build
   dotnet run
   ```

---

## 👥 Credits & Team

- **Designer:** Rakan (`Dis: tktkz`)
- **Developer:** Mkaf7h (`Snapchat: c7k.mkaf7h`)
- **Partner:** Mugy (`Instagram: mugy.2011`)
- **Group:** © 2026 Nubersec Group. All Rights Reserved.

---

## ⚠️ Disclaimer

**Authorized use only.** IsisFuscator is provided for educational and defensive purposes to protect intellectual property. The authors assume no responsibility for any misuse or illegal activities.
