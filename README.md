# Syn VA Components

This repository contains source code of components like _plugins_ and _libraries_ used by **Syn Virtual Assistant Prototype 8.5**.

## Directory Structure

|Directory|Description|
|:----|:----|
|Plugins|Contains open-sourced `Syn.VA.Plugins`|
|Libraries|Contains open-sourced `Syn.VA.Libraries`|
|Dependencies|Contains all Class libraries required by the plugins|

## Plugins

`Plugins` add or extend features of the Syn Virtual Assistant

* Adapters created by these plugins use the `xmlns:o` namespace to denote official adapters.
* The string resources are retrieved from `Syn.VA.Libraries.Language` library.
* Many plugins may depend on internal libraries specially designed for the **VA Framework**.

>Adapters created by developers must use the namespace `xmlns:x` to differentiate them from _official_ adapters and to prevent them from being overridden.

## Libraries

`Libraries` provide set of APIs and useful functions that can be used by `Plugins`.

At the moment only the `Syn.VA.Libraries.Language` library is available in this repository. However, the `Dependencies` folder contains all the compiled version of other required libraries.

All `Plugins` must reference the `Syn.VA.Libraries.Language` for language based string resources. We plan to rely on this library for multi-language feature support.

We encourage adding _Keys_ and _Values_ to the `Syn.VA.Libraries.Language` project whenever necessary.

## More Projects
A few more **Plugins** or **Libraries**  may get added to this repository once we've fully tested their reliability and have confirmed the necessity of open-sourcing them for improvment by developers.

We've currently removed projects that were reported to have bugs in them. Once the bugs have been fixed and an update is released those projects will be added to this repository as well.

## License

![LGPL-3 Logo](https://www.gnu.org/graphics/lgplv3-147x51.png)

All `Syn.VA.Plugins`, `Syn.VA.Libraries` and _Projects_  included this repository are licensed under [GNU Lesser General Public License version 3 or Later](https://www.gnu.org/licenses/lgpl-3.0.html)
