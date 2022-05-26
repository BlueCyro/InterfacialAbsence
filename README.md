# Interfacial Absence

## This is a [NeosModLoader](https://github.com/zkxs/NeosModLoader) mod that removes much of the need for LogiX interfaces.

## Overview
Since LogiX interfaces are aging, clutter up your code, spawn unecessary duplicates, and fly around like spaghetti monsters half the time, this mod gives you a path to avoid them in nearly every scenario where you don't need direct impulses to a component. This is done through a couple extra extract modes - listed below.

### Extra Extract Modes
- Linked register
    
    This lets you extract a value (or reference) register that is linked to the reference proxy you're holding (such as when you grab a value from an inspector for example) via ValueCopy, and allows you to not just pull from it, but also write to it via write nodes like you would normally.

- Drive **value** node
    
    Emphasized because this will actually pull out and automatically drive whatever value you're holding (again, such as from an inspector) with the `Drive` node usually found in `Actions` in the LogiX menu.

These options both let you avoid having to use interfaces for nearly any purpose whatsoever, and makes your LogiX truly free from any worldly attachments.

# Extra features

- Lets you long click on reference arrows to reveal what they point at
- Pairs nicely with [Nodentify](https://github.com/RileyGuy/Nodentify) for automatic node labelling
- Pairs nicely with [Proxify](https://github.com/RileyGuy/Proxify), allowing you to drive value registers just by extracting them, or create registers for your registers - if you're into that :)
