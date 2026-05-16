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

QuickSheet clones the repo, reads the manifest, and registers the `worldtm` prefix.

## Usage

```
worldtm: London, Tokyo, NY,5,8
worldtm: PST, CET, IST, JST,5,6
worldtm: America/New_York, Europe/Berlin, Asia/Shanghai,5,5
```

> **Important:** The last two comma-separated numbers are always the output grid dimensions (columns, rows). Without them, QuickSheet may not allocate enough space for the output. Always append `,<cols>,<rows>` after your timezone list.

### Minimal examples

| Cell content | Description |
|---|---|
| `worldtm: London, Tokyo, NY,5,8` | 3 timezones, 5 columns × 8 rows |
| `worldtm: PST, CET, IST, JST,5,6` | 4 timezones, 5 columns × 6 rows |
| `worldtm: ,5,8` | Default timezones (UTC, NY, London, Berlin, Tokyo, Sydney) |

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
"worldtm: NY, London, Berlin, Mumbai, Tokyo, Sydney,5,8",,
```

## Requirements

- [QuickSheet](https://github.com/cemheren/QuickSheet) v0.6.0+
- .NET 9 SDK

## License

MIT
