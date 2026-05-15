# quicksheet-worldtm 🌍

Multi-timezone world clock extension for [QuickSheet](https://github.com/cemheren/QuickSheet) — your team's timezones at a glance on your desktop wallpaper.

## Features

- **40+ timezone aliases** — use `NY`, `London`, `PST`, or full IANA IDs like `America/New_York`
- **Business hours indicator** — 🟢 business hours, 🟡 early/late, 🔴 off hours
- **Time-of-day icons** — 🌅 morning, ☀️ afternoon, 🌆 evening, 🌙 night
- **Fuzzy matching** — type a city name and it finds the closest timezone
- **Zero network calls** — pure local `System.TimeZoneInfo`, works offline
- **Default view** — shows UTC, New York, London, Berlin, Tokyo, Sydney with no params

## Install

In any QuickSheet cell, type:

```
ext: github:cemheren/quicksheet-worldtm
```

## Usage

```
worldtm: London, Tokyo, NY
worldtm: PST, CET, IST, JST
worldtm: America/New_York, Europe/Berlin, Asia/Shanghai
worldtm:                              # defaults: UTC, NY, London, Berlin, Tokyo, Sydney
```

### Example output

```
🌍 City      | Time  | Date         | Offset     | Status
🌅 London    | 09:30 | Mon May 12   | UTC+01:00  | 🟢 Business hrs
☀️ New York  | 04:30 | Mon May 12   | UTC-04:00  | 🔴 Off hours
🌙 Tokyo     | 18:30 | Mon May 12   | UTC+09:00  | 🟡 Late
```

### Supported aliases

| Alias | Timezone |
|-------|----------|
| `NY`, `EST`, `EDT` | America/New_York |
| `LA`, `SF`, `PST`, `PDT` | America/Los_Angeles |
| `Chicago`, `CST`, `CDT` | America/Chicago |
| `London`, `BST` | Europe/London |
| `Berlin`, `CET`, `CEST` | Europe/Berlin |
| `Paris` | Europe/Paris |
| `Tokyo`, `JST` | Asia/Tokyo |
| `Shanghai`, `Beijing` | Asia/Shanghai |
| `Mumbai`, `Delhi`, `IST` | Asia/Kolkata |
| `Sydney`, `AEST`, `AEDT` | Australia/Sydney |
| `Seoul`, `KST` | Asia/Seoul |
| `Singapore`, `SGT` | Asia/Singapore |
| `Dubai` | Asia/Dubai |
| `Toronto`, `Vancouver` | America/Toronto, America/Vancouver |
| `UTC`, `GMT` | UTC |

Plus any valid [IANA timezone ID](https://en.wikipedia.org/wiki/List_of_tz_database_time_zones).

## Use case: distributed team dashboard

Set your wallpaper to always show when teammates are available:

```csv
Team Clock,,
"ext: github:cemheren/quicksheet-worldtm",,
"worldtm: NY, London, Berlin, Mumbai, Tokyo, Sydney",,
```

## Requirements

- [QuickSheet](https://github.com/cemheren/QuickSheet) v0.6.0+
- .NET 9 SDK

## License

MIT
