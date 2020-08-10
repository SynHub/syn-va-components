# Syn VA Components

This repository may contain source code of components like _plugins_ and _libraries_ used by **Syn VA Framework 10.0.0**.

>We've moved the older project files of version 8.5.6 to the `OLD` folder.

## Libraries

`Libraries` provide set of APIs and useful functions that can be used by `Plugins`.

At the moment only the `Syn.VA.Libraries.Language` library is available in this repository.

All `Plugins` must reference the `Syn.VA.Libraries.Language` for language based string resources. We plan to rely on this library for multi-language feature support.

We encourage adding _Keys_ and _Values_ to the `Syn.VA.Libraries.Language` project whenever necessary.

## Plugins

The `Plugins` folder at the moment contains a sample `Hello Plugin` project to help developers learn how to build custom plugins.

## Workspace

The `Workspace Projects` folder contains example Workspace project files for you to download and see how you can extend the VA using OSCOVA and VA Framework nodes.

## More Coming Up

A few more **Plugins** or **Libraries**  may get added to this repository once we've fully tested their reliability and have confirmed the necessity of open-sourcing them for improvement by developers.

## License

![LGPL-3 Logo](https://www.gnu.org/graphics/lgplv3-147x51.png)

All `Syn.VA.Plugins`, `Syn.VA.Libraries` and _Projects_  included this repository are licensed under [GNU Lesser General Public License version 3 or Later](https://www.gnu.org/licenses/lgpl-3.0.html)
