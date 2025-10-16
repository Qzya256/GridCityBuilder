# GridCityBuilder
> 2D grid-based city building system for Unity

## Description  
A simple 2D grid-based city builder where players can place, remove, and create buildings using a user-friendly interface.  
All building data is saved between sessions.

![](https://github.com/Qzya256/Image/blob/78171104e8de3c232cc1dc2582bb16f947bb86b9/Image%20Sequence_001_0000.jpg)

## Technical Details  
- All core mechanics are separated into independent modules using **Assembly Definitions**.  
- The project entry point is implemented through **Bootstrap** using **Zenject**.  
- User input is handled via the **New Input System**.  
- Building data persistence is implemented through files or a database (not PlayerPrefs).  
- Special attention was paid to the convenience of editing static (configuration) data.

> 💡 **Note:**  
> To add or edit building configurations, open the editor from the top menu:  
> **Tools → Building Config Editor**
