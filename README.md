# Interfacial Absence

## This is a [NeosModLoader](https://github.com/zkxs/NeosModLoader) mod that removes much of the need for LogiX interfaces.

## Overview
Since LogiX interfaces are aging, clutter up your code, spawn unecessary duplicates, and fly around like spaghetti monsters half the time, this mod gives you a path to avoid them in nearly every scenario where you don't need direct impulses to a component.

One of the main features this adds is an extra couple of extra options to the extraction mode of a given LogiX tip.

### Extra Extract Modes
- Linked register
    This lets you extract a value (or reference) register that is linked to the reference proxy you're holding (such as when you grab a value from an inspector for example) via ValueCopy, and allows you to not just pull from it, but also write to it via write nodes like you would normally.

- Drive **value** node
    Emphasized because this will actually pull out and automatically drive whatever value you're holding (again, such as from an inspector) with the `Drive` node usually found in `Actions` in the LogiX menu.

These options both let you avoid having to use interfaces for nearly any purpose whatsoever, and makes your LogiX truly free from any worldly attachments.
