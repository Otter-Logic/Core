# OtterLogic.Core

[![build](https://github.com/Otter-Logic/Core/actions/workflows/build.yml/badge.svg)](https://github.com/Otter-Logic/Core/actions/workflows/build.yml)

The shared layer of [OtterLogic](https://github.com/Otter-Logic/Rhino3D) — deliberately small.

Core holds what **more than one domain** needs, and nothing else: the section
vocabulary both front-ends read, shared geometry helpers, and tolerance
conventions. One test keeps it honest — *would a second domain plausibly need
this?* If no, it belongs in that domain.

| | |
|---|---|
| `Sections` | tool group names, read by the Grasshopper ribbon and the Rhino toolbar |
| `Naming` | `Humanise` turns a PascalCase name into readable text — "WarrenWithVerticals" into "Warren with verticals" |

`Naming` passes the test by a clear margin: every domain grows option enums, and
both front-ends have to show them — Grasshopper in a right-click menu, Rhino as
a command-line prompt. Two copies drift into two spellings of the same option,
which is the exact confusion a shared vocabulary exists to prevent.

The failure mode guarded against is not drift. It is Core becoming a grab-bag,
or a bottleneck where every domain change needs a Core release first.

## Rules

- References **RhinoCommon and nothing else**. No Grasshopper, no `Rhino.UI`,
  no Eto, no `System.Drawing`.
- Never references a domain or an adaptor. Dependencies run one way:
  **adaptor → domain → core**.

## Consumers

| | |
|---|---|
| [StructuralForm](https://github.com/Otter-Logic/StructuralForm) | trusses and structural layouts |
| [Rhino3D](https://github.com/Otter-Logic/Rhino3D) | the Rhino and Grasshopper adaptor |

## Build

```
dotnet build OtterLogic.slnx
```

Targets `net7.0-windows` against the Rhino 8.0 API baseline, so anything built
on it runs on any Rhino 8. See `Directory.Build.props` for why that pin is
deliberate.

## License

[MIT](LICENSE).
