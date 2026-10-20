<div align="center">

<img src="https://i.pinimg.com/736x/6c/f0/21/6cf021b41941e1e25bf48ea548be27cf.jpg" alt="IsisFuscator Banner" width="650"/>

# IsisFuscator

### 

</div>

---

## Overview

**IsisFuscator** is an advanced, high-performance .NET obfuscation and protection tool built with C# Windows Forms and powered by **Mono.Cecil** and **dnlib**. Designed for developers and reverse-engineering defense, it secures .NET assemblies (`.exe` / `.dll`) against unauthorized decompilation, debugging, and tampering.

---

<div align="center">
<img src="https://imgur.com/So9FJy6.png" alt="Screenshot 1" width="700"/>
</div>

---

## Features

### 1.  Renamer
- **Smart & Safe Renaming:** Obfuscates namespaces, non-public types, methods, and fields while fully preserving Windows Forms UI classes, entry points, and public APIs to ensure crash-free execution.
- **Custom Prefixes & Lengths:** Configure custom identifier prefixes, character sets, and name lengths.

---

<div align="center">
<img src="https://imgur.com/BQYuY0r.png" alt="Screenshot 2" width="700"/>
</div>

---

### 2. Essential Protections
- **Anti-Debug:** Injects runtime checks that terminate the process if a debugger is attached.
- **Anti-ILDasm:** Adds `SuppressIldasmAttribute` to block IL disassemblers.
- **Anti-Tamper & Anti-Dump:** Injects metadata decoys and integrity markers against memory dumping and patching.
- **DnSpy Crasher:** Injects malformed types with null bytes (`\0`) and recursive branching that instantly crashes decompiler tree views.

---

<div align="center">
<img src="https://imgur.com/Yp43EhL.png" alt="Screenshot 3" width="700"/>
</div>

---

## Installation & Usage

1. **Prerequisites:** Ensure .NET 8.0 SDK is installed.
2. **Clone & Build:**
   ```bash
   git clone https://github.com/yourusername/IsisFuscator.git
   cd IsisFuscator
   dotnet build
   dotnet run
   ```

---

<div align="center">
<img src="https://imgur.com/1dqdkUl.png" alt="Screenshot 4" width="700"/>
</div>

---

## Credits & Team

- **Designer:** Nmk (Dis: .5.1.1._.)
- **Developer:** Mkaf7h (Snapchat: c7k.mkaf7h)
- **Partner:** Mugy
- **partner:** Rakan (Dis: tktkz)
- **Group:** 2026 Nubersec Group. All Rights Reserved.

---

## Disclaimer

**.** IsisFuscator is provided for educational and defensive purposes to protect intellectual property. The authors assume no responsibility for any misuse or illegal activities.
